using ECommerce.Application.Brands.GetBrands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Brands.Abstractions
{
    public interface IBrandReadRepository
    {
        Task<(IReadOnlyList<BrandListItemResponse> Items,
              int TotalCount)> GetPagedAsync(
            GetBrandsQuery query,
            CancellationToken cancellationToken = default);
    }
}
