using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Brands.GetBrands
{
    public sealed record GetBrandsQuery(
     int PageNumber = 1,
     int PageSize = 20,
     string? Search = null,
     bool? IsActive = null
 );
}
