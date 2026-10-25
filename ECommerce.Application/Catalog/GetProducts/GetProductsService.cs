using ECommerce.Application.Common.Pagination;
using ECommerce.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Catalog.GetProducts
{
    public class GetProductsService
    {
        private const int MaxPageSize = 100;

        private readonly IProductListReadRepository _repository;

        public GetProductsService(
            IProductListReadRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<PagedResult<ProductListItemResponse>>>
            ExecuteAsync(
                GetProductsQuery query,
                CancellationToken cancellationToken = default)
        {
            var errors = new List<Error>();

            if (query.PageNumber < 1)
            {
                errors.Add(new Error(
                    "InvalidPageNumber",
                    "Page number must be greater than zero.",
                    "PageNumber"));
            }

            if (query.PageSize < 1 || query.PageSize > MaxPageSize)
            {
                errors.Add(new Error(
                    "InvalidPageSize",
                    $"Page size must be between 1 and {MaxPageSize}.",
                    "PageSize"));
            }

            if (errors.Count > 0)
            {
                return Result<PagedResult<ProductListItemResponse>>
                    .Failure(errors);
            }
            if (query.PageNumber > 0 &&
    query.PageSize > 0 &&
    (long)(query.PageNumber - 1) * query.PageSize > int.MaxValue)
            {
                errors.Add(new Error(
                    "InvalidPaginationOffset",
                    "The requested page is too large.",
                    "PageNumber"));
            }
            var products = await _repository.GetProductsAsync(
                query,
                cancellationToken);

            return Result<PagedResult<ProductListItemResponse>>
                .Success(products);
        }
    }
}
