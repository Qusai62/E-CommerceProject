using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Catalog
{
    public class ProductImage
    {
        public long ProductImageId { get; set; }

        public long ProductId { get; set; }

        public string StorageKey { get; set; } = null!;

        public string? AltText { get; set; }

        public int SortOrder { get; set; }

        public bool IsPrimary { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime UpdatedAtUtc { get; set; }

        public Product Product { get; set; } = null!;
    }
}
