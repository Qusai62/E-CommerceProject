using System;


namespace ECommerce.Domain.Catalog
{
    public class Brand
    {
        public long BrandId { get; set; }

        public string Name { get; set; } = null!;

        public string Slug { get; set; } = null!;

        public string? Description { get; set; }

        public string? LogoStorageKey { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime UpdatedAtUtc { get; set; }

        public ICollection<Product> Products { get; set; }
            = new List<Product>();
    }
}
