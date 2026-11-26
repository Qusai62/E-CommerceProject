using ECommerce.Application.Checkout.SelectSavedAddress;
using ECommerce.Application.Checkout.StartCheckout;
using ECommerce.Domain.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Checkout.ReviewOrder
{
    public sealed record ReviewCheckoutResponse(
    long ShoppingCartId,
    IReadOnlyList<CheckoutItemResponse> Items,
    SelectCheckoutAddressResponse DeliveryAddress,
    PaymentMethod PaymentMethod,
    decimal ProductsTotal,
    decimal ShippingCharge,
    decimal FinalTotal
);
}
