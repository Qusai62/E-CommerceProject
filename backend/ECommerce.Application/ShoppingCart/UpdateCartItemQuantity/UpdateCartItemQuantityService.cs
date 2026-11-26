using ECommerce.Application.Common.Results;
using ECommerce.Application.ShoppingCart.AddCartItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.ShoppingCart.UpdateCartItemQuantity
{
    public class UpdateCartItemQuantityService
    {
        private readonly IUpdateCartItemQuantityRepository
        _repository;

        private readonly ICartVariantAvailabilityReader
            _availabilityReader;

        public UpdateCartItemQuantityService(
            IUpdateCartItemQuantityRepository repository,
            ICartVariantAvailabilityReader availabilityReader)
        {
            _repository = repository;
            _availabilityReader = availabilityReader;
        }

        public async Task<Result<UpdateCartItemQuantityResponse>>
            ExecuteAsync(
                Guid applicationUserId,
                UpdateCartItemQuantityCommand command,
                CancellationToken cancellationToken = default)
        {
            // Step 1: Validate input
            if (command.CartItemId <= 0 || command.Quantity <= 0)
            {
                return Fail(
                    "Cart.InvalidQuantity",
                    "Cart item ID and quantity must be positive.");
            }

            // Step 2: Retrieve the variant from the customer's cart
            var productVariantId =
                await _repository.GetOwnedVariantIdAsync(
                    applicationUserId,
                    command.CartItemId,
                    cancellationToken);

            if (productVariantId is null)
            {
                return Fail(
                    "Cart.ItemNotFound",
                    "Cart item was not found.");
            }

            // Step 3: Retrieve current inventory availability
            var availability = await _availabilityReader.GetAsync(
                productVariantId.Value,
                cancellationToken);

            if (availability is null)
            {
                return Fail(
                    "Cart.VariantNotFound",
                    "Product variant was not found.");
            }

            // Step 4: Validate activation
            if (!availability.IsProductActive ||
                !availability.IsVariantActive)
            {
                return Fail(
                    "Cart.VariantInactive",
                    "Product and variant must be active.");
            }

            // Step 5: Validate available stock
            if (command.Quantity > availability.AvailableQuantity)
            {
                return Fail(
                    "Cart.InsufficientStock",
                    "Requested quantity exceeds available stock.");
            }

            // Step 6: Update cart item
            var updateResult = await _repository.UpdateAsync(
                applicationUserId,
                command.CartItemId,
                command.Quantity,
                availability.AvailableQuantity,
                cancellationToken);

            // Step 7: Return application result
            return updateResult.Status switch
            {
                UpdateCartItemStatus.Updated
                    when updateResult.Item is not null =>
                    Result<UpdateCartItemQuantityResponse>.Success(
                        updateResult.Item),

                UpdateCartItemStatus.NotFound =>
                    Fail(
                        "Cart.ItemNotFound",
                        "Cart item was not found."),

                UpdateCartItemStatus.InsufficientStock =>
                    Fail(
                        "Cart.InsufficientStock",
                        "Requested quantity exceeds available stock."),

                _ => throw new InvalidOperationException(
                    "Unexpected cart update result.")
            };
        }

        private static Result<UpdateCartItemQuantityResponse> Fail(
            string code,
            string message)
        {
            return Result<UpdateCartItemQuantityResponse>.Failure(
                new[] { new Error(code, message) });
        }
    }
}
