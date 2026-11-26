using ECommerce.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.ShoppingCart.RemoveCartItem
{
    public class RemoveCartItemService
    {
        private readonly IRemoveCartItemRepository _repository;

        public RemoveCartItemService(
            IRemoveCartItemRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<RemoveCartItemResult>> ExecuteAsync(
            Guid applicationUserId,
            RemoveCartItemCommand command,
            CancellationToken cancellationToken = default)
        {
            // 1. Validate cart item identifier
            if (command.CartItemId <= 0)
            {
                return Result<RemoveCartItemResult>.Failure(
                    new[]
                    {
                    new Error(
                        "Cart.InvalidItemId",
                        "Cart item ID must be greater than zero.")
                    });
            }

            // 2. Remove item belonging to authenticated customer
            var removed = await _repository.RemoveAsync(
                applicationUserId,
                command.CartItemId,
                cancellationToken);

            // 3. Handle item not found
            if (!removed)
            {
                return Result<RemoveCartItemResult>.Failure(
                    new[]
                    {
                    new Error(
                        "Cart.ItemNotFound",
                        "Cart item was not found.")
                    });
            }

            // 4. Return success
            return Result<RemoveCartItemResult>.Success(
                new RemoveCartItemResult(true));
        }
    }

    public sealed record RemoveCartItemResult(
    bool Removed
);
}
