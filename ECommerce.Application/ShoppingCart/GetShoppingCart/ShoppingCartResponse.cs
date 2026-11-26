using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.ShoppingCart.GetShoppingCart
{
    public sealed record ShoppingCartResponse(
     long ShoppingCartId,
     IReadOnlyList<ShoppingCartItemResponse> Items,
     int TotalQuantity,
     decimal TotalAmount
 );
}
