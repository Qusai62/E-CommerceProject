using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Brands.GetBrands
{
    public sealed record GetBrandsResponse(
    IReadOnlyList<BrandListItemResponse> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages
);
}
