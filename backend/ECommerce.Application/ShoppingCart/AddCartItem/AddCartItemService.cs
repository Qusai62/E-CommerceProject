using ECommerce.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.ShoppingCart.AddCartItem
{
    public class AddCartItemService
    {
        private readonly ICartVariantAvailabilityReader
        _availabilityReader;

        private readonly IAddCartItemRepository
            _cartRepository;

        public AddCartItemService(
            ICartVariantAvailabilityReader availabilityReader,
            IAddCartItemRepository cartRepository)
        {
            _availabilityReader = availabilityReader;
            _cartRepository = cartRepository;
        }

        public async Task<Result<AddCartItemResponse>> ExecuteAsync(
            Guid applicationUserId,
            AddCartItemCommand command,
            CancellationToken cancellationToken = default)
        {
            // 1. Validate requested quantity
            if (command.Quantity <= 0)
            {
                return Fail(
                    "Cart.InvalidQuantity",
                    "Quantity must be greater than zero.");
            }

            // 2. Validate product variant identifier
            if (command.ProductVariantId <= 0)
            {
                return Fail(
                    "Cart.InvalidVariant",
                    "A valid product variant is required.");
            }

            // 3. Get current variant availability
            var availability = await _availabilityReader.GetAsync(
                command.ProductVariantId,
                cancellationToken);

            if (availability is null)
            {
                return Fail(
                    "Cart.VariantNotFound",
                    "Product variant was not found.");
            }

            // 4. Check product and variant activation
            if (!availability.IsProductActive ||
                !availability.IsVariantActive)
            {
                return Fail(
                    "Cart.VariantInactive",
                    "Product and variant must be active.");
            }

            // 5. Check initial available stock
            if (availability.AvailableQuantity < command.Quantity)
            {
                return Fail(
                    "Cart.InsufficientStock",
                    "Requested quantity exceeds available stock.");
            }

            // 6. Add or increase the cart item
            var writeResult = await _cartRepository.AddAsync(
                applicationUserId,
                command.ProductVariantId,
                command.Quantity,
                availability.AvailableQuantity,
                cancellationToken);

            // 7. Handle repository outcomes
            return writeResult.Status switch
            {
                AddCartItemStatus.Added
                    when writeResult.Item is not null =>
                    Result<AddCartItemResponse>.Success(
                        writeResult.Item),

                AddCartItemStatus.CartNotFound =>
                    Fail(
                        "Cart.NotFound",
                        "Shopping cart was not found."),

                AddCartItemStatus.InsufficientStock =>
                    Fail(
                        "Cart.InsufficientStock",
                        "Total quantity exceeds available stock."),

                _ => throw new InvalidOperationException(
                    "Unexpected cart repository result.")
            };
        }

        private static Result<AddCartItemResponse> Fail(
            string code,
            string message)
        {
            return Result<AddCartItemResponse>.Failure(
                new[] { new Error(code, message) });
        }
    }
}
