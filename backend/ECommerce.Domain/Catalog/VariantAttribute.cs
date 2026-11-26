using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Catalog
{
    public class VariantAttribute
    {
        public long VariantAttributeId { get; set; }

        public long ProductVariantId { get; set; }

        public string Name { get; set; } = null!;

        public string Value { get; set; } = null!;

        public int SortOrder { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime UpdatedAtUtc { get; set; }

        public ProductVariant ProductVariant { get; set; } = null!;
    }
}
