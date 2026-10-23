using ECommerce.Domain.CustomerAccount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Cart
{
    public class ShoppingCart
    {
        public long ShoppingCartId { get; set; }

        public long CustomerId { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime UpdatedAtUtc { get; set; }

        public Customer Customer { get; set; } = null!;
        public ICollection<CartItem> CartItems { get; set; }
    = new List<CartItem>();
    }
}
