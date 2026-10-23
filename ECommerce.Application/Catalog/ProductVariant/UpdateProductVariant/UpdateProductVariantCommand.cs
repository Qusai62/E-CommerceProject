using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Catalog.ProductVariant.UpdateProductVariant
{
    public class UpdateProductVariantCommand
    {
        public long ProductId { get; set; }
        public long ProductVariantId { get; set; }

        public string Sku { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
    }
}
