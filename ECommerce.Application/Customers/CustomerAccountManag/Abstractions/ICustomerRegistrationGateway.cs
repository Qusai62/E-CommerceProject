using ECommerce.Application.Common.Results;
using ECommerce.Application.Customers.CustomerAccountManag.RegisterCustomer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Customers.CustomerAccountManag.Abstractions
{
    public interface ICustomerRegistrationGateway
    {
        Task<Result<RegisterCustomerResponse>> RegisterAsync(
            RegisterCustomerCommand command,
            CancellationToken cancellationToken = default);
    }
}
