using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Categories.ManageHierarchy
{
    public sealed record MoveCategoryResponse(
     long CategoryId,
     string Name,
     long? ParentCategoryId,
     DateTime UpdatedAtUtc);
}
