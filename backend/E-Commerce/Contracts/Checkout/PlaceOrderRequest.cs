using ECommerce.Domain.Payments;

namespace E_Commerce.Contracts.Checkout
{
    public sealed record PlaceOrderRequest(
    Guid CheckoutSubmissionId,
    long SavedAddressId,
    PaymentMethod PaymentMethod
);
}
