using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.ShoppingCart.RemoveCartItem
{
    public interface IRemoveCartItemRepository
    {
        Task<bool> RemoveAsync(
            Guid applicationUserId,
            long cartItemId,
            CancellationToken cancellationToken = default);
    }
}
