using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Checkout.SelectSavedAddress
{
    public interface ICheckoutAddressReadRepository
    {
        Task<CheckoutAddressSnapshot?> GetAsync(
            Guid applicationUserId,
            long savedAddressId,
            CancellationToken cancellationToken = default);
    }
}
