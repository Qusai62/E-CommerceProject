using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Checkout.StartCheckout
{
    public sealed record CheckoutItemResponse(
    long CartItemId,
    long ProductVariantId,
    string ProductName,
    string Sku,
    int Quantity,
    decimal UnitPrice,
    decimal Subtotal
);

    public sealed record CheckoutSummaryResponse(
        long ShoppingCartId,
        IReadOnlyList<CheckoutItemResponse> Items,
        decimal ProductsTotal
    );
}
