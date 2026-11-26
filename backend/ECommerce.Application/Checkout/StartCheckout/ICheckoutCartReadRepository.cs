using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Checkout.StartCheckout
{
    public interface ICheckoutCartReadRepository
    {
        Task<CheckoutCartSnapshot?> GetAsync(
            Guid applicationUserId,
            CancellationToken cancellationToken = default);
    }
}
