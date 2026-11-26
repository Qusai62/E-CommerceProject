using ECommerce.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Checkout.StartCheckout
{
    public class GetCheckoutSummaryService
    {
        private readonly ICheckoutCartReadRepository _repository;

        public GetCheckoutSummaryService(
            ICheckoutCartReadRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<CheckoutSummaryResponse>> ExecuteAsync(
            Guid applicationUserId,
            CancellationToken cancellationToken = default)
        {
            // 1. Validate the authenticated user identifier
            if (applicationUserId == Guid.Empty)
            {
                return Result<CheckoutSummaryResponse>.Failure(
                    new[]
                    {
                    new Error(
                        "Checkout.InvalidUser",
                        "A valid customer identity is required.")
                    });
            }

            // 2. Get the customer's shopping cart
            var cart = await _repository.GetAsync(
                applicationUserId,
                cancellationToken);

            if (cart is null || cart.Items.Count == 0)
            {
                return Result<CheckoutSummaryResponse>.Failure(
                    new[]
                    {
                    new Error(
                        "Checkout.EmptyCart",
                        "The shopping cart is empty.")
                    });
            }

            // 3. Validate every cart item
            foreach (var item in cart.Items)
            {
                if (!item.IsProductActive || !item.IsVariantActive)
                {
                    return Result<CheckoutSummaryResponse>.Failure(
                        new[]
                        {
                        new Error(
                            "Checkout.InactiveVariant",
                            $"Variant {item.ProductVariantId} is unavailable.")
                        });
                }

                if (item.Quantity <= 0 ||
                    item.Quantity > item.AvailableQuantity)
                {
                    return Result<CheckoutSummaryResponse>.Failure(
                        new[]
                        {
                        new Error(
                            "Checkout.InsufficientStock",
                            $"Variant {item.ProductVariantId} has insufficient stock.")
                        });
                }
            }

            // 4. Calculate current product subtotals
            var items = cart.Items.Select(item =>
                new CheckoutItemResponse(
                    item.CartItemId,
                    item.ProductVariantId,
                    item.ProductName,
                    item.Sku,
                    item.Quantity,
                    item.UnitPrice,
                    item.UnitPrice * item.Quantity
                )
            ).ToList();

            decimal productsTotal = items.Sum(x => x.Subtotal);

            // 5. Return the checkout summary
            return Result<CheckoutSummaryResponse>.Success(
                new CheckoutSummaryResponse(
                    cart.ShoppingCartId,
                    items,
                    productsTotal
                ));
        }
    }
}
