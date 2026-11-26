using ECommerce.Application.Customers.CustomerAccountManag.GetCustomerProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Customers.CustomerAccountManag.Abstractions
{
    public interface ICustomerProfileReadRepository
    {
        Task<GetCustomerProfileResponse?> GetByApplicationUserIdAsync(
        Guid applicationUserId,
        CancellationToken cancellationToken = default);
    }
}
