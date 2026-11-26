using ECommerce.Application.Authentication.Abstractions;
using ECommerce.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Authentication.Login
{
    public class LoginService
    {
        private readonly ILoginGateway _gateway;

        public LoginService(ILoginGateway gateway)
        {
            _gateway = gateway;
        }

        public async Task<Result<LoginResponse>> ExecuteAsync(
            LoginCommand command,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(command);

            var errors = new List<Error>();

            // Normalize the email input.
            string email = command.Email?.Trim() ?? string.Empty;

            // Validate email.
            if (string.IsNullOrWhiteSpace(email))
            {
                errors.Add(new Error(
                    "Authentication.EmailRequired",
                    "Email is required.",
                    nameof(command.Email)));
            }

            // Validate password.
            if (string.IsNullOrWhiteSpace(command.Password))
            {
                errors.Add(new Error(
                    "Authentication.PasswordRequired",
                    "Password is required.",
                    nameof(command.Password)));
            }

            // Return validation errors before accessing Infrastructure.
            if (errors.Count > 0)
            {
                return Result<LoginResponse>.Failure(errors);
            }

            // Create normalized input for the gateway.
            var normalizedCommand = new LoginCommand
            {
                Email = email,
                Password = command.Password
            };

            // Delegate Identity authentication to Infrastructure.
            return await _gateway.LoginAsync(
                normalizedCommand,
                cancellationToken);
        }
    }
}
