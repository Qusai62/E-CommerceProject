using ECommerce.Application.Common.Results;
using ECommerce.Application.Customers.CustomerAccountManag.ChangeCustomerPassword;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Customers.CustomerAccountManag.Abstractions
{
    public interface ICustomerPasswordGateway
    {
        Task<Result<bool>> ChangeAsync(
            Guid applicationUserId,
            ChangeCustomerPasswordCommand command,
            CancellationToken cancellationToken = default);
    }
}
