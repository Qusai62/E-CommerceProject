using ECommerce.Application.Customers.CustomerAccountManag.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Customers.CustomerAccountManag.ResetCustomerPassword
{
    public class ForgotPasswordService
    {
        private readonly ICustomerPasswordResetGateway _gateway;

        public ForgotPasswordService(
            ICustomerPasswordResetGateway gateway)
        {
            _gateway = gateway;
        }

        public async Task ExecuteAsync(
            ForgotPasswordCommand command,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(command);

            string email = command.Email?.Trim() ?? string.Empty;

            // Return the same public result for invalid input.
            if (string.IsNullOrWhiteSpace(email) ||
                email.Length > 256 ||
                !new EmailAddressAttribute().IsValid(email))
            {
                return;
            }

            await _gateway.RequestResetAsync(
                email,
                cancellationToken);
        }
    }
}

