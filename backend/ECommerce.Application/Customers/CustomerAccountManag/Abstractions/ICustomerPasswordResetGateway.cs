using ECommerce.Application.Common.Results;
using ECommerce.Application.Customers.CustomerAccountManag.ResetCustomerPassword;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Customers.CustomerAccountManag.Abstractions
{
    public interface ICustomerPasswordResetGateway
    {
        Task RequestResetAsync(
            string email,
            CancellationToken cancellationToken = default);

        Task<Result<bool>> ResetAsync(
            ResetPasswordCommand command,
            CancellationToken cancellationToken = default);
    }
}
