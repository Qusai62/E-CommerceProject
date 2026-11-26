using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Categories.ManageHierarchy
{
    public sealed record MoveCategoryCommand(
    long CategoryId,
    long? ParentCategoryId);
}
