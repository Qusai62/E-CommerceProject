using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Catalog.Product.UpdateProduct
{
    public sealed class UpdateProductResponse
    {
        public long ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public long? BrandId { get; set; }
        public long? CategoryId { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
    }
}
