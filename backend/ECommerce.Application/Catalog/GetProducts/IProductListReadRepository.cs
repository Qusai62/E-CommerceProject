using ECommerce.Application.Common.Pagination;
using System;


namespace ECommerce.Application.Catalog.GetProducts
{
    public interface IProductListReadRepository
    {
        Task<PagedResult<ProductListItemResponse>> GetProductsAsync(
        GetProductsQuery query,
        CancellationToken cancellationToken = default);
    }
}
