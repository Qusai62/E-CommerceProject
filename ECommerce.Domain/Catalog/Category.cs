using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Catalog
{
    public class Category
    {
        public long CategoryId { get; set; }

        public long? ParentCategoryId { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string Slug { get; set; } = null!;

        public bool IsActive { get; set; }

        public int SortOrder { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime UpdatedAtUtc { get; set; }

        public Category? ParentCategory { get; set; }

        public ICollection<Category> ChildCategories { get; set; }
            = new List<Category>();

        public ICollection<Product> Products { get; set; }
            = new List<Product>();
    }
}
