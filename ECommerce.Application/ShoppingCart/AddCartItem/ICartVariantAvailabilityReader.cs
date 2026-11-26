using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.ShoppingCart.AddCartItem
{
    public interface ICartVariantAvailabilityReader
    {
        Task<CartVariantAvailability?> GetAsync(
        long productVariantId,
        CancellationToken cancellationToken = default);
    }

    public sealed record CartVariantAvailability(
    bool IsProductActive,
    bool IsVariantActive,
    long AvailableQuantity
);

}
