using ECommerce.Application.Common.Results;
using ECommerce.Application.Customers.CustomerAccountManag.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Customers.CustomerAccountManag.ChangeCustomerPassword
{
    public sealed class ChangeCustomerPasswordService
    {
        private readonly ICustomerPasswordGateway _gateway;

        public ChangeCustomerPasswordService(
            ICustomerPasswordGateway gateway)
        {
            _gateway = gateway;
        }

        public async Task<Result<bool>> ExecuteAsync(
            Guid applicationUserId,
            ChangeCustomerPasswordCommand command,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(command);

            var errors = new List<Error>();

            // BR-019: Current password is required.
            if (string.IsNullOrEmpty(command.CurrentPassword))
            {
                errors.Add(new Error(
                    "Customer.CurrentPasswordRequired",
                    "Current password is required.",
                    nameof(command.CurrentPassword)));
            }

            // BR-021: New password is required.
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

            return await _gateway.ChangeAsync(
                applicationUserId,
                command,
                cancellationToken);
        }
    }
}
