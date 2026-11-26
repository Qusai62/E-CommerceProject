using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Categories.UpdateCategory
{
    public sealed record UpdateCategoryCommand(
    long CategoryId,
    string Name,
    string? Description,
    int SortOrder);
}
