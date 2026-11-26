using ECommerce.Application.Common.Security;
using ECommerce.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Identity
{
    public static class StoreOwnerSeeder
    {
        public static async Task SeedAsync(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        ApplicationDbContext context,
        IConfiguration configuration,
        CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // 1. Check whether a Store Owner already exists.
            var existingOwners = await userManager
                .GetUsersInRoleAsync(ApplicationRoles.StoreOwner);

            // Detect an invalid V1 state.
            if (existingOwners.Count > 1)
            {
                throw new InvalidOperationException(
                    "Multiple Store Owner accounts were found.");
            }

            // Do not recreate or modify an existing owner.
            if (existingOwners.Count == 1)
            {
                return;
            }

            // 2. Read bootstrap credentials from configuration.
            var email = configuration["BootstrapOwner:Email"];
            var password = configuration["BootstrapOwner:Password"];

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException(
                    "Store Owner bootstrap credentials are missing.");
            }

            email = email.Trim();

            // 3. Check that the required role exists.
            var roleExists = await roleManager.RoleExistsAsync(
                ApplicationRoles.StoreOwner);

            if (!roleExists)
            {
                throw new InvalidOperationException(
                    "The StoreOwner role does not exist.");
            }

            // 4. Prevent promotion of an existing account.
            var existingUser = await userManager.FindByEmailAsync(email);

            if (existingUser is not null)
            {
                throw new InvalidOperationException(
                    "The bootstrap email already belongs to an account. " +
                    "Use a dedicated Store Owner email.");
            }

            // 5. Start one database transaction.
            await using var transaction =
                await context.Database.BeginTransactionAsync(
                    cancellationToken);

            // 6. Prepare the Store Owner Identity account.
            var owner = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = false
            };

            // 7. Create the user with a hashed password.
            var createResult = await userManager.CreateAsync(
                owner,
                password);

            if (!createResult.Succeeded)
            {
                ThrowIdentityErrors(
                    "Store Owner creation",
                    createResult);
            }

            cancellationToken.ThrowIfCancellationRequested();

            // 8. Assign the StoreOwner role.
            var roleResult = await userManager.AddToRoleAsync(
                owner,
                ApplicationRoles.StoreOwner);

            if (!roleResult.Succeeded)
            {
                ThrowIdentityErrors(
                    "Store Owner role assignment",
                    roleResult);
            }

            cancellationToken.ThrowIfCancellationRequested();

            // 9. Commit both operations together.
            await transaction.CommitAsync(cancellationToken);
        }

        private static void ThrowIdentityErrors(
            string operation,
            IdentityResult result)
        {
            var errors = string.Join(
                "; ",
                result.Errors.Select(error =>
                    $"{error.Code}: {error.Description}"));

            throw new InvalidOperationException(
                $"{operation} failed: {errors}");
        }
    }
}
