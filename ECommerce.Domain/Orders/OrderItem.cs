using ECommerce.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Orders
{
    public class OrderItem
    {
        public long OrderItemId { get; set; }

        public long OrderId { get; set; }

        public long ProductVariantId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public string ProductNameSnapshot { get; set; } = null!;

        public string SkuSnapshot { get; set; } = null!;

        public string VariantAttributesSnapshot { get; set; } = null!;

        public DateTime CreatedAtUtc { get; set; }

        public Order Order { get; set; } = null!;

        public ProductVariant ProductVariant { get; set; } = null!;
    }
}
