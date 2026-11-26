using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Categories.ViewHierarchy
{
    public sealed class CategoryHierarchyResponse
    {
        public long CategoryId { get; init; }

        public string Name { get; init; } = string.Empty;

        public long? ParentCategoryId { get; init; }

        public bool IsActive { get; init; }

        public int SortOrder { get; init; }

        public List<CategoryHierarchyResponse> Children { get; } = new();
    }
}
