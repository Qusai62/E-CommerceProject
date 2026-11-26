using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Checkout.StartCheckout
{
    public sealed record CheckoutCartItemSnapshot(
    long CartItemId,
    long ProductVariantId,
    string ProductName,
    string Sku,
    int Quantity,
    decimal UnitPrice,
    bool IsProductActive,
    bool IsVariantActive,
    long AvailableQuantity
);
}
