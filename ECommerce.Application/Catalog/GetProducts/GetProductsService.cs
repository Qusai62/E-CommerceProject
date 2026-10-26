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

            var search = query.Search?.Trim();

            if (string.IsNullOrWhiteSpace(search))
            {
                search = null;
            }

            if (search is not null && search.Length > 100)
            {
                errors.Add(new Error(
                    "InvalidSearch",
                    "Search cannot exceed 100 characters.",
                    "Search"));
            }

            var normalizedQuery = new GetProductsQuery
            {
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                Search = search
            };

            var products = await _repository.GetProductsAsync(
                normalizedQuery,
                cancellationToken);

            return Result<PagedResult<ProductListItemResponse>>
                .Success(products);
        }
    }
}
