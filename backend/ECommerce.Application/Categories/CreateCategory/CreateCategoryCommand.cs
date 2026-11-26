using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Categories.CreateCategory
{
    public sealed class CreateCategoryCommand
    {
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public long? ParentCategoryId { get; set; }

        public int SortOrder { get; set; }
    }
}
