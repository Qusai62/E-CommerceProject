using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Categories.PublicCategories
{
    public sealed record PublicCategoryResponse(
     long CategoryId,
     string Name,
     string Slug,
     long? ParentCategoryId,
     int SortOrder);
}
