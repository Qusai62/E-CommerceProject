using ECommerce.Application.Common.Security;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Identity
{
    public static class IdentityRoleSeeder
    {
        public static async Task SeedAsync(
            RoleManager<IdentityRole<Guid>> roleManager)
        {
            string[] roles =
            {
            ApplicationRoles.Customer,
            ApplicationRoles.StoreOwner
        };

            foreach (var roleName in roles)
            {
                bool roleExists =
                    await roleManager.RoleExistsAsync(roleName);

                if (roleExists)
                {
                    continue;
                }

                var result = await roleManager.CreateAsync(
                    new IdentityRole<Guid>(roleName));

                if (!result.Succeeded)
                {
                    var errors = string.Join(
                        "; ",
                        result.Errors.Select(e => e.Description));

                    throw new InvalidOperationException(
                        $"Failed to create role '{roleName}': {errors}");
                }
            }
        }
    }
}
