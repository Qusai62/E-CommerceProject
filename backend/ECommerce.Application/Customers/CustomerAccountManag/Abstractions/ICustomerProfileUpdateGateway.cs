using ECommerce.Application.Common.Results;
using ECommerce.Application.Customers.CustomerAccountManag.UpdateCustomerProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Customers.CustomerAccountManag.Abstractions
{
    public interface ICustomerProfileUpdateGateway
    {
        Task<Result<UpdateCustomerProfileResponse>> UpdateAsync(
        Guid applicationUserId,
        UpdateCustomerProfileCommand command,
        CancellationToken cancellationToken = default);
    }
}
