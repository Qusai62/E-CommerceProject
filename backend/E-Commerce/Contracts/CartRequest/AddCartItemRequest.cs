namespace E_Commerce.Contracts.CartRequest
{
    public sealed record AddCartItemRequest(
     long ProductVariantId,
     int Quantity
 );
}
