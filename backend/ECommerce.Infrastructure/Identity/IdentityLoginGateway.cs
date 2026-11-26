using ECommerce.Application.Authentication.Abstractions;
using ECommerce.Application.Authentication.Login;
using ECommerce.Application.Common.Results;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Identity
{
    public class IdentityLoginGateway: ILoginGateway
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IdentityLoginGateway(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<Result<LoginResponse>> LoginAsync(
            LoginCommand command,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // STEP 1: Find the Identity account by email.
            var user = await _userManager.FindByEmailAsync(
                command.Email);

            if (user is null)
            {
                return InvalidCredentials();
            }

            cancellationToken.ThrowIfCancellationRequested();

            // STEP 2: Verify the password and sign-in eligibility.
            // If successful, Identity creates the authentication cookie.
            var signInResult =
                await _signInManager.PasswordSignInAsync(
                    user,
                    command.Password,
                    isPersistent: false,
                    lockoutOnFailure: true);

            // STEP 3: Handle account lockout.
            if (signInResult.IsLockedOut)
            {
                return Result<LoginResponse>.Failure(
                    new[]
                    {
                    new Error(
                        "Authentication.AccountLocked",
                        "This account is temporarily locked. Try again later.",
                        string.Empty)
                    });
            }

            // STEP 4: Handle accounts requiring two-factor authentication.
            if (signInResult.RequiresTwoFactor)
            {
                return Result<LoginResponse>.Failure(
                    new[]
                    {
                    new Error(
                        "Authentication.TwoFactorRequired",
                        "Additional authentication is required.",
                        string.Empty)
                    });
            }

            // STEP 5: Handle invalid credentials or other failed sign-ins.
            if (!signInResult.Succeeded)
            {
                return InvalidCredentials();
            }

            // STEP 6: Retrieve the user's Identity roles.
            var roles = await _userManager.GetRolesAsync(user);

            // STEP 7: Return account information.
            return Result<LoginResponse>.Success(
                new LoginResponse
                {
                    UserId = user.Id,
                    Email = user.Email!,
                    Roles = roles.ToArray(),
                    IsEmailVerified = user.EmailConfirmed
                });
        }

        private static Result<LoginResponse> InvalidCredentials()
        {
            return Result<LoginResponse>.Failure(
                new[]
                {
                new Error(
                    "Authentication.InvalidCredentials",
                    "Invalid email or password.",
                    string.Empty)
                });
        }
    }
}
