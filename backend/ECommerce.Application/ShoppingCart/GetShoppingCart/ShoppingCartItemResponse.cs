using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.ShoppingCart.GetShoppingCart
{
    public sealed record ShoppingCartItemResponse(
      long CartItemId,
      long ProductVariantId,
      string ProductName,
      string Sku,
      int Quantity,
      decimal UnitPrice,
      decimal Subtotal
  );
}
