using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.ShoppingCart.UpdateCartItemQuantity
{
    public sealed record UpdateCartItemQuantityResponse(
      long CartItemId,
      long ProductVariantId,
      int Quantity
  );
}
