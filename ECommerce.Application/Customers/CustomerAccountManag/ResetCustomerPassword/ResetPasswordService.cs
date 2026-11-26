using ECommerce.Application.Common.Results;
using ECommerce.Application.Customers.CustomerAccountManag.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Customers.CustomerAccountManag.ResetCustomerPassword
{
    public class ResetPasswordService
    {
        private readonly ICustomerPasswordResetGateway _gateway;

        public ResetPasswordService(
            ICustomerPasswordResetGateway gateway)
        {
            _gateway = gateway;
        }

        public async Task<Result<bool>> ExecuteAsync(
            ResetPasswordCommand command,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(command);

            var errors = new List<Error>();

            if (string.IsNullOrWhiteSpace(command.Email))
            {
                errors.Add(new Error(
                    "Customer.EmailRequired",
                    "Email is required.",
                    nameof(command.Email)));
            }

            if (string.IsNullOrWhiteSpace(command.Token))
            {
                errors.Add(new Error(
                    "Customer.ResetTokenRequired",
                    "Password reset token is required.",
                    nameof(command.Token)));
            }

            if (string.IsNullOrEmpty(command.NewPassword))
            {
                errors.Add(new Error(
                    "Customer.NewPasswordRequired",
                    "New password is required.",
                    nameof(command.NewPassword)));
            }

            if (errors.Count > 0)
            {
                return Result<bool>.Failure(errors);
            }

            var normalizedCommand = new ResetPasswordCommand
            {
                Email = command.Email.Trim(),
                Token = command.Token,
                NewPassword = command.NewPassword
            };

            return await _gateway.ResetAsync(
                normalizedCommand,
                cancellationToken);
        }
    }
}
