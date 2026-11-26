using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.ShoppingCart.AddCartItem
{
    public sealed record AddCartItemCommand(
    long ProductVariantId,
    int Quantity
);
}
