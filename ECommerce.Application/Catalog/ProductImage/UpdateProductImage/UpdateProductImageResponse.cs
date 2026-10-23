using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Catalog.ProductImage.UpdateProductImage
{
    public class UpdateProductImageResponse
    {
        public long ProductImageId { get; set; }

        public long ProductId { get; set; }

        public string StorageKey { get; set; } = string.Empty;

        public string? AltText { get; set; }

        public int SortOrder { get; set; }

        public bool IsPrimary { get; set; }

        public DateTime UpdatedAtUtc { get; set; }
    }
}
