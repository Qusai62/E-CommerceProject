using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.ShoppingCart.ClearCart
{
    public interface IClearCartRepository
    {
        Task<bool> ClearAsync(
            Guid applicationUserId,
            CancellationToken cancellationToken = default);
    }
}
