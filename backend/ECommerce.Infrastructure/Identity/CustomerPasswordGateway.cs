using ECommerce.Application.Common.Results;
using ECommerce.Application.Customers.CustomerAccountManag.Abstractions;
using ECommerce.Application.Customers.CustomerAccountManag.ChangeCustomerPassword;
using ECommerce.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Identity
{
    public class CustomerPasswordGateway: ICustomerPasswordGateway
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public CustomerPasswordGateway(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public async Task<Result<bool>> ChangeAsync(
            Guid applicationUserId,
            ChangeCustomerPasswordCommand command,
            CancellationToken cancellationToken = default)
        {
            // 1. Ensure this Identity user belongs to a Customer.
            bool customerExists = await _context.Customers
                .AnyAsync(
                    c => c.ApplicationUserId == applicationUserId,
                    cancellationToken);

            if (!customerExists)
            {
                return Failure(
                    "Customer.ProfileNotFound",
                    "Customer profile was not found.",
                    string.Empty);
            }

            // 2. Retrieve the Identity user.
            cancellationToken.ThrowIfCancellationRequested();

            var user = await _userManager.FindByIdAsync(
                applicationUserId.ToString());

            if (user is null)
            {
                return Failure(
                    "Customer.ProfileNotFound",
                    "Customer account was not found.",
                    string.Empty);
            }

            // 3. BR-020: Verify current password.
            bool currentPasswordCorrect =
                await _userManager.CheckPasswordAsync(
                    user,
                    command.CurrentPassword);

            if (!currentPasswordCorrect)
            {
                return Failure(
                    "Customer.InvalidCurrentPassword",
                    "Current password is incorrect.",
                    nameof(command.CurrentPassword));
            }

            // 4. BR-022: New password must be different.
            if (string.Equals(
                command.CurrentPassword,
                command.NewPassword,
                StringComparison.Ordinal))
            {
                return Failure(
                    "Customer.SamePassword",
                    "New password must be different from the current password.",
                    nameof(command.NewPassword));
            }

            // 5. Change password using ASP.NET Core Identity.
            cancellationToken.ThrowIfCancellationRequested();

            var identityResult =
                await _userManager.ChangePasswordAsync(
                    user,
                    command.CurrentPassword,
                    command.NewPassword);

            // 6. Handle Identity validation failures.
            if (!identityResult.Succeeded)
            {
                var errors = identityResult.Errors
                    .Select(error => new Error(
                        $"Customer.{error.Code}",
                        error.Description,
                        GetErrorField(error.Code)))
                    .ToList();

                return Result<bool>.Failure(errors);
            }

            // 7. Refresh the current authentication cookie.
            await _signInManager.RefreshSignInAsync(user);

            // 8. Return success.
            return Result<bool>.Success(true);
        }

        private static Result<bool> Failure(
            string code,
            string message,
            string field)
        {
            return Result<bool>.Failure(
                new List<Error>
                {
                new Error(code, message, field)
                });
        }

        private static string GetErrorField(string code)
        {
            if (code == "PasswordMismatch")
            {
                return nameof(
                    ChangeCustomerPasswordCommand.CurrentPassword);
            }

            if (code.StartsWith(
                "Password",
                StringComparison.Ordinal))
            {
                return nameof(
                    ChangeCustomerPasswordCommand.NewPassword);
            }

            return string.Empty;
        }
    }
}

