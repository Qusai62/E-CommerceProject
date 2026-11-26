using ECommerce.Application.Common.Results;
using ECommerce.Application.Customers.CustomerAccountManag.Abstractions;
using ECommerce.Application.Customers.CustomerAccountManag.UpdateCustomerProfile;
using ECommerce.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Identity
{
    public class CustomerProfileUpdateGateway : ICustomerProfileUpdateGateway
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public CustomerProfileUpdateGateway(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public async Task<Result<UpdateCustomerProfileResponse>> UpdateAsync(
            Guid applicationUserId,
            UpdateCustomerProfileCommand command,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // 1. Load Identity user
            var user = await _userManager.FindByIdAsync(
                applicationUserId.ToString());

            if (user is null)
            {
                return NotFound();
            }

            // 2. Load Customer as a tracked entity
            var customer = await _context.Customers
                .SingleOrDefaultAsync(
                    c => c.ApplicationUserId == applicationUserId,
                    cancellationToken);

            if (customer is null)
            {
                return NotFound();
            }

            // 3. Determine what changed
            bool emailChanged = !string.Equals(
                user.Email,
                command.Email,
                StringComparison.OrdinalIgnoreCase);

            bool phoneChanged = !string.Equals(
                user.PhoneNumber,
                command.PhoneNumber,
                StringComparison.Ordinal);

            // 4. Reauthenticate sensitive email changes
            if (emailChanged)
            {
                if (string.IsNullOrEmpty(command.CurrentPassword))
                {
                    return Failure(
                        "Customer.CurrentPasswordRequired",
                        "Current password is required to change email.",
                        nameof(command.CurrentPassword));
                }

                bool correctPassword =
                    await _userManager.CheckPasswordAsync(
                        user,
                        command.CurrentPassword);

                if (!correctPassword)
                {
                    return Failure(
                        "Customer.InvalidCurrentPassword",
                        "Current password is incorrect.",
                        nameof(command.CurrentPassword));
                }
            }

            var now = DateTime.UtcNow;

            // 5. Begin transaction
            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    cancellationToken);

            try
            {
                // 6. Update Identity information
                if (emailChanged)
                {
                    user.Email = command.Email;
                    user.UserName = command.Email;
                    user.EmailConfirmed = false;
                }

                if (phoneChanged)
                {
                    user.PhoneNumber = command.PhoneNumber;
                    user.PhoneNumberConfirmed = false;
                }

                if (emailChanged || phoneChanged)
                {
                    var identityResult =
                        await _userManager.UpdateAsync(user);

                    if (!identityResult.Succeeded)
                    {
                        await transaction.RollbackAsync(
                            CancellationToken.None);

                        return IdentityFailure(identityResult);
                    }
                }

                // 7. Update Customer information
                customer.FirstName = command.FirstName;
                customer.LastName = command.LastName;
                customer.UpdatedAtUtc = now;

                await _context.SaveChangesAsync(cancellationToken);

                // 8. Invalidate older authentication tickets
                if (emailChanged || phoneChanged)
                {
                    var stampResult =
                        await _userManager.UpdateSecurityStampAsync(user);

                    if (!stampResult.Succeeded)
                    {
                        await transaction.RollbackAsync(
                            CancellationToken.None);

                        return IdentityFailure(stampResult);
                    }
                }

                // 9. Commit all database changes
                await transaction.CommitAsync(cancellationToken);
            }
            catch (DbUpdateException ex) when (IsDuplicateEmail(ex))
            {
                await transaction.RollbackAsync(CancellationToken.None);

                return Failure(
                    "Customer.EmailAlreadyExists",
                    "This email address is already registered.",
                    nameof(command.Email));
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync(CancellationToken.None);

                return Failure(
                    "Customer.ConcurrentUpdate",
                    "Your account was updated concurrently. Please retry.",
                    string.Empty);
            }

            // 10. Refresh the current browser authentication cookie
            if (emailChanged || phoneChanged)
            {
                await _signInManager.RefreshSignInAsync(user);
            }

            // 11. Return updated profile
            return Result<UpdateCustomerProfileResponse>.Success(
                new UpdateCustomerProfileResponse
                {
                    CustomerId = customer.CustomerId,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = user.Email!,
                    PhoneNumber = user.PhoneNumber!,
                    IsEmailVerified = user.EmailConfirmed,
                    UpdatedAtUtc = customer.UpdatedAtUtc
                });
        }

        private static Result<UpdateCustomerProfileResponse> NotFound()
        {
            return Failure(
                "Customer.ProfileNotFound",
                "Customer profile was not found.",
                string.Empty);
        }

        private static Result<UpdateCustomerProfileResponse> Failure(
            string code,
            string message,
            string field)
        {
            return Result<UpdateCustomerProfileResponse>.Failure(
                new List<Error>
                {
                new Error(code, message, field)
                });
        }

        private static Result<UpdateCustomerProfileResponse> IdentityFailure(
            IdentityResult result)
        {
            var errors = result.Errors
                .Select(error => new Error(
                    $"Customer.{error.Code}",
                    error.Description,
                    error.Code.Contains("Email", StringComparison.Ordinal) ||
                    error.Code.Contains("UserName", StringComparison.Ordinal)
                        ? nameof(UpdateCustomerProfileCommand.Email)
                        : string.Empty))
                .ToList();

            return Result<UpdateCustomerProfileResponse>.Failure(errors);
        }

        private static bool IsDuplicateEmail(DbUpdateException ex)
        {
            return ex.InnerException is SqlException sqlEx
                && sqlEx.Number is 2601 or 2627
                && (
                    sqlEx.Message.Contains(
                        "UQ_AspNetUsers_NormalizedEmail",
                        StringComparison.OrdinalIgnoreCase)
                    ||
                    sqlEx.Message.Contains(
                        "UserNameIndex",
                        StringComparison.OrdinalIgnoreCase)
                );
        }
    }
}
