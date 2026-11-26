using ECommerce.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Cart
{
    public class CartItem
    {
        public long CartItemId { get; set; }

        public long ShoppingCartId { get; set; }

        public long ProductVariantId { get; set; }

        public int Quantity { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime UpdatedAtUtc { get; set; }

        public ShoppingCart ShoppingCart { get; set; } = null!;

        public ProductVariant ProductVariant { get; set; } = null!;
    }
}
