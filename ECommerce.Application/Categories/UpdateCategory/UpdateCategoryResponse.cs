using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Categories.UpdateCategory
{
    public sealed record UpdateCategoryResponse(
     long CategoryId,
     string Name,
     string Slug,
     string? Description,
     long? ParentCategoryId,
     int SortOrder,
     bool IsActive,
     DateTime UpdatedAtUtc);
}
