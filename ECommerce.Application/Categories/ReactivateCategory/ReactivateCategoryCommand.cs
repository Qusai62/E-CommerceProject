using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Categories.ReactivateCategory
{
    public sealed record ReactivateCategoryCommand(
      long CategoryId);
}
