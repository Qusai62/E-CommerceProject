using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.ShoppingCart.UpdateCartItemQuantity
{
    public interface IUpdateCartItemQuantityRepository
    {
        Task<long?> GetOwnedVariantIdAsync(
        Guid applicationUserId,
        long cartItemId,
        CancellationToken cancellationToken = default);

        Task<UpdateCartItemResult> UpdateAsync(
            Guid applicationUserId,
            long cartItemId,
            int quantity,
            long availableQuantity,
            CancellationToken cancellationToken = default);
    }

    public enum UpdateCartItemStatus
    {
        Updated,
        NotFound,
        InsufficientStock
    }

    public sealed record UpdateCartItemResult(
        UpdateCartItemStatus Status,
        UpdateCartItemQuantityResponse? Item
    );
}
