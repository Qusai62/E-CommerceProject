using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Categories.ViewHierarchy
{
    public sealed record CategoryHierarchyRow(
       long CategoryId,
       string Name,
       long? ParentCategoryId,
       bool IsActive,
       int SortOrder);
}
