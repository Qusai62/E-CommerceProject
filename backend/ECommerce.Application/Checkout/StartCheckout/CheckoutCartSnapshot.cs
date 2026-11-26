using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Checkout.StartCheckout
{
    public sealed record CheckoutCartSnapshot(
      long ShoppingCartId,
      IReadOnlyList<CheckoutCartItemSnapshot> Items
  );
}
