using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Catalog
{
    public class Product
    {
        public long ProductId { get; set; }

        public long? BrandId { get; set; }

        public long? CategoryId { get; set; }

        public string Name { get; set; } = null!;

        public string Slug { get; set; } = null!;

        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime UpdatedAtUtc { get; set; }

        public Brand? Brand { get; set; }

        public Category? Category { get; set; }

        public ICollection<ProductImage> Images { get; set; }
            = new List<ProductImage>();

        public ICollection<TechnicalSpecification> TechnicalSpecifications { get; set; }
            = new List<TechnicalSpecification>();

        public ICollection<ProductVariant> Variants { get; set; }
            = new List<ProductVariant>();
    }
}
