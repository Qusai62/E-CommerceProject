using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.ShoppingCart.AddCartItem
{
    public interface IAddCartItemRepository
    {
        Task<AddCartItemResult> AddAsync(
        Guid applicationUserId,
        long productVariantId,
        int quantity,
        long availableQuantity,
        CancellationToken cancellationToken = default);
    }
    public enum AddCartItemStatus
    {
        Added,
        CartNotFound,
        InsufficientStock,
        ProductUnavailable
    }

    public sealed record AddCartItemResult(
        AddCartItemStatus Status,
        AddCartItemResponse? Item
    );
}
