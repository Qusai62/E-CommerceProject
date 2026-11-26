using ECommerce.Domain.Shipping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Checkout.DetermineShippingCharge
{
    public sealed record ShippingConfigurationSnapshot(
    ShippingMode ShippingMode,
    decimal ShippingCharge
);
}
