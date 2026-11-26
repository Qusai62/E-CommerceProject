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
            //If the client provides a BrandId or CategoryId, it must be greater than zero.
            if (query.BrandId.HasValue && query.BrandId.Value <= 0)
            {
                errors.Add(new Error(
                    "InvalidBrandId",
                    "BrandId must be greater than zero.",
                    "BrandId"));
            }

            if (query.CategoryId.HasValue && query.CategoryId.Value <= 0)
            {
                errors.Add(new Error(
                    "InvalidCategoryId",
                    "CategoryId must be greater than zero.",
                    "CategoryId"));
            }

            //Add price validation
            if (query.MinPrice.HasValue && query.MinPrice.Value < 0)
            {
                errors.Add(new Error(
                    "InvalidMinPrice",
                    "Minimum price cannot be negative.",
                    "MinPrice"));
            }

            if (query.MaxPrice.HasValue && query.MaxPrice.Value < 0)
            {
                errors.Add(new Error(
                    "InvalidMaxPrice",
                    "Maximum price cannot be negative.",
                    "MaxPrice"));
            }

            if (query.MinPrice.HasValue &&
                query.MaxPrice.HasValue &&
                query.MinPrice.Value > query.MaxPrice.Value)
            {
                errors.Add(new Error(
                    "InvalidPriceRange",
                    "Minimum price cannot exceed maximum price.",
                    "MinPrice"));
            }

            //Now we need to validate and normalize the sorting parameters.
            var sortBy = query.SortBy?.Trim().ToLowerInvariant();
            var sortDirection = query.SortDirection?.Trim().ToLowerInvariant();

            // Set defaults
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                sortBy = "createdat";
            }

            if (string.IsNullOrWhiteSpace(sortDirection))
            {
                sortDirection = "desc";
            }

            // Validate supported sorting fields
            if (sortBy != "name" &&
                sortBy != "price" &&
                sortBy != "createdat")
            {
                errors.Add(new Error(
                    "InvalidSortBy",
                    "SortBy must be name, price, or createdAt.",
                    "SortBy"));
            }

            // Validate sorting direction
            if (sortDirection != "asc" &&
                sortDirection != "desc")
            {
                errors.Add(new Error(
                    "InvalidSortDirection",
                    "SortDirection must be asc or desc.",
                    "SortDirection"));
            }

            if (errors.Count > 0)
            {
                return Result<PagedResult<ProductListItemResponse>>
                    .Failure(errors);
            }
            //If no products match, returning HTTP 200 with an empty result is reasonable.
            var normalizedQuery = new GetProductsQuery
            {
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                Search = search,
                BrandId = query.BrandId,
                CategoryId = query.CategoryId,
                MinPrice = query.MinPrice,
                MaxPrice = query.MaxPrice,
                SortBy = sortBy,
                SortDirection = sortDirection

            };

            var products = await _repository.GetProductsAsync(
                normalizedQuery,
                cancellationToken);

            return Result<PagedResult<ProductListItemResponse>>
                .Success(products);
        }
    }
}
