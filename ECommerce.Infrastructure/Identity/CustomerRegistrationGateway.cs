using ECommerce.Application.Common.Results;
using ECommerce.Application.Common.Security;
using ECommerce.Application.Customers.CustomerAccountManag.Abstractions;
using ECommerce.Application.Customers.CustomerAccountManag.RegisterCustomer;
using ECommerce.Domain.Cart;
using ECommerce.Domain.CustomerAccount;
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
    public class CustomerRegistrationGateway
            : ICustomerRegistrationGateway
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public CustomerRegistrationGateway(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<Result<RegisterCustomerResponse>> RegisterAsync(
            RegisterCustomerCommand command,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var now = DateTime.UtcNow;

            // All registration operations must succeed together.
            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    cancellationToken);

            try
            {
                // STEP 1: Prepare the Identity user.
                var user = new ApplicationUser
                {
                    UserName = command.Email,
                    Email = command.Email,
                    PhoneNumber = command.PhoneNumber,
                    EmailConfirmed = false
                };

                // STEP 2: Create the Identity user and hash the password.
                var identityResult = await _userManager.CreateAsync(
                    user,
                    command.Password);

                // STEP 3: Handle Identity validation failures.
                if (!identityResult.Succeeded)
                {
                    await transaction.RollbackAsync(
                        CancellationToken.None);

                    var errors = identityResult.Errors
                        .Select(error => new Error(
                            $"Customer.{error.Code}",
                            error.Description,
                            GetErrorField(error.Code)))
                        .ToList();

                    return Result<RegisterCustomerResponse>
                        .Failure(errors);
                }

                cancellationToken.ThrowIfCancellationRequested();

                // STEP 4: Assign the Customer role.
                // Public registration must never assign StoreOwner.
                var roleResult = await _userManager.AddToRoleAsync(
                    user,
                    ApplicationRoles.Customer);

                // STEP 5: Handle role-assignment failures.
                if (!roleResult.Succeeded)
                {
                    await transaction.RollbackAsync(
                        CancellationToken.None);

                    var errorDetails = string.Join(
                        "; ",
                        roleResult.Errors.Select(error =>
                            $"{error.Code}: {error.Description}"));

                    throw new InvalidOperationException(
                        "Customer registration failed because the " +
                        "required Customer role could not be assigned. " +
                        errorDetails);
                }

                cancellationToken.ThrowIfCancellationRequested();

                // STEP 6: Create the business Customer record.
                var customer = new Customer
                {
                    ApplicationUserId = user.Id,
                    FirstName = command.FirstName,
                    LastName = command.LastName,
                    CreatedAtUtc = now,
                    UpdatedAtUtc = now,

                    // STEP 7: Create an empty ShoppingCart.
                    ShoppingCart = new ShoppingCart
                    {
                        CreatedAtUtc = now,
                        UpdatedAtUtc = now
                    }
                };

                // STEP 8: Track the Customer and ShoppingCart.
                _context.Customers.Add(customer);

                // STEP 9: Save both business entities.
                await _context.SaveChangesAsync(cancellationToken);

                // STEP 10: Commit the entire registration transaction.
                await transaction.CommitAsync(cancellationToken);

                // STEP 11: Return successful registration.
                return Result<RegisterCustomerResponse>.Success(
                    new RegisterCustomerResponse
                    {
                        CustomerId = customer.CustomerId,
                        Email = user.Email!,
                        IsEmailVerified = user.EmailConfirmed,
                        CreatedAtUtc = customer.CreatedAtUtc
                    });
            }
            catch (DbUpdateException ex)
                when (IsDuplicateEmail(ex))
            {
                await transaction.RollbackAsync(
                    CancellationToken.None);

                return Result<RegisterCustomerResponse>.Failure(
                    new List<Error>
                    {
                        new Error(
                            "Customer.EmailAlreadyExists",
                            "This email address is already registered.",
                            nameof(command.Email))
                    });
            }
        }

        // Map Identity validation errors to registration fields.
        private static string GetErrorField(string code)
        {
            if (code.StartsWith(
                "Password",
                StringComparison.Ordinal))
            {
                return nameof(RegisterCustomerCommand.Password);
            }

            if (code.Contains(
                    "Email",
                    StringComparison.Ordinal)
                ||
                code.Contains(
                    "UserName",
                    StringComparison.Ordinal))
            {
                return nameof(RegisterCustomerCommand.Email);
            }

            return string.Empty;
        }

        // Identify duplicate-email or username database violations.
        private static bool IsDuplicateEmail(DbUpdateException ex)
        {
            if (ex.InnerException is not SqlException sqlEx)
            {
                return false;
            }

            // SQL Server:
            // 2601 = Duplicate unique index key.
            // 2627 = Unique constraint violation.
            if (sqlEx.Number is not (2601 or 2627))
            {
                return false;
            }

            return sqlEx.Message.Contains(
                       "UQ_AspNetUsers_NormalizedEmail",
                       StringComparison.OrdinalIgnoreCase)
                   ||
                   sqlEx.Message.Contains(
                       "UserNameIndex",
                       StringComparison.OrdinalIgnoreCase);
        }
    }
}

