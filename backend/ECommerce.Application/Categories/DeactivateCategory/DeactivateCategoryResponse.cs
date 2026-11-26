using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Categories.DeactivateCategory
{
    public sealed record DeactivateCategoryResponse(
     long CategoryId,
     string Name,
     bool IsActive,
     DateTime UpdatedAtUtc);
}
