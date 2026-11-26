using ECommerce.Domain.Payments;

namespace E_Commerce.Contracts.Checkout
{
    public sealed record ReviewCheckoutRequest(
     long SavedAddressId,
     PaymentMethod PaymentMethod
 );
}
