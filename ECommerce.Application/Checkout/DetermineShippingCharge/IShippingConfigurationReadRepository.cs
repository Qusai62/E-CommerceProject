using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Checkout.DetermineShippingCharge
{
    public interface IShippingConfigurationReadRepository
    {
        Task<ShippingConfigurationSnapshot?> GetAsync(
            CancellationToken cancellationToken = default);
    }
}
