using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Categories.ViewCategories
{
    public sealed record CategoryListItemResponse(
     long CategoryId,
     string Name,
     string Slug,
     string? Description,
     long? ParentCategoryId,
     bool IsActive,
     int SortOrder,
     int ProductCount,
     DateTime CreatedAtUtc,
     DateTime UpdatedAtUtc);
}
