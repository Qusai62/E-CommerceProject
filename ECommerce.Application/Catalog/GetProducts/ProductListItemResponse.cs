using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Catalog.GetProducts
{
    public class ProductListItemResponse
    {
        public long ProductId { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;

        public long? BrandId { get; set; }
        public long? CategoryId { get; set; }

        public decimal PriceFrom { get; set; }

        public string? PrimaryImageStorageKey { get; set; }
    }
}
