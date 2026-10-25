using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Catalog.ProductVariant.CreateProductVariant
{
    public class CreateProductVariantCommand
    {
        public long ProductId { get; set; }

        public string Sku { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public List<VariantAttributeCommand> Attributes { get; set; } = new();
    }
}
