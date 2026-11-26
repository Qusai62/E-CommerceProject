using ECommerce.Application.Common.Results;
using ECommerce.Application.Customers.CustomerAccountManag.Abstractions;
using ECommerce.Application.Customers.CustomerAccountManag.ResetCustomerPassword;
using ECommerce.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Identity
{
    public class CustomerPasswordResetGateway: ICustomerPasswordResetGateway
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IPasswordResetEmailSender _emailSender;
        private readonly string _frontendBaseUrl;

        public CustomerPasswordResetGateway(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            IPasswordResetEmailSender emailSender,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _context = context;
            _emailSender = emailSender;

            _frontendBaseUrl =
                configuration["Frontend:BaseUrl"]
                ?? throw new InvalidOperationException(
                    "Frontend:BaseUrl is not configured.");
        }

        public async Task RequestResetAsync(
            string email,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // 1. Find the Identity user.
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
                return;

            // 2. Only customer accounts can use this endpoint.
            bool customerExists = await _context.Customers.AnyAsync(
                c => c.ApplicationUserId == user.Id,
                cancellationToken);

            if (!customerExists)
                return;

            // 3. Require verified ownership of the email address.
            if (!await _userManager.IsEmailConfirmedAsync(user))
                return;

            // 4. Generate an Identity password reset token.
            var token = await _userManager
                .GeneratePasswordResetTokenAsync(user);

            cancellationToken.ThrowIfCancellationRequested();

            // 5. Build the React frontend reset link.
            var resetLink =
                $"{_frontendBaseUrl.TrimEnd('/')}/reset-password" +
                $"?email={Uri.EscapeDataString(email)}" +
                $"&token={Uri.EscapeDataString(token)}";

            // 6. Send the reset instructions.
            await _emailSender.SendResetLinkAsync(
                email,
                resetLink,
                cancellationToken);
        }

        public async Task<Result<bool>> ResetAsync(
            ResetPasswordCommand command,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // 1. Find the Identity account.
            var user = await _userManager.FindByEmailAsync(
                command.Email);

            if (user is null)
                return InvalidToken();

            // 2. Ensure the account belongs to a customer.
            bool customerExists = await _context.Customers.AnyAsync(
                c => c.ApplicationUserId == user.Id,
                cancellationToken);

            if (!customerExists)
                return InvalidToken();

            // 3. Require a verified email.
            if (!await _userManager.IsEmailConfirmedAsync(user))
                return InvalidToken();

            // 4. Validate the token and reset the password.
            var identityResult = await _userManager.ResetPasswordAsync(
                user,
                command.Token,
                command.NewPassword);

            if (!identityResult.Succeeded)
            {
                var errors = identityResult.Errors
                    .Select(error => new Error(
                        $"Customer.{error.Code}",
                        error.Code == "InvalidToken"
                            ? "Reset link is invalid or expired."
                            : error.Description,
                        error.Code.StartsWith(
                            "Password",
                            StringComparison.Ordinal)
                            ? nameof(command.NewPassword)
                            : string.Empty))
                    .ToList();

                return Result<bool>.Failure(errors);
            }

            // 5. Password has been successfully reset.
            return Result<bool>.Success(true);
        }

        private static Result<bool> InvalidToken()
        {
            return Result<bool>.Failure(
                new List<Error>
                {
                new Error(
                    "Customer.InvalidToken",
                    "Reset link is invalid or expired.",
                    string.Empty)
                });
        }
    }
}
