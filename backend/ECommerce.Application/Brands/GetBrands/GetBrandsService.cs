using ECommerce.Application.Brands.Abstractions;
using ECommerce.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Brands.GetBrands
{
    public class GetBrandsService
    {
        private readonly IBrandReadRepository _brandReadRepository;

        public GetBrandsService(
            IBrandReadRepository brandReadRepository)
        {
            _brandReadRepository = brandReadRepository;
        }

        public async Task<Result<GetBrandsResponse>> ExecuteAsync(
            GetBrandsQuery query,
            CancellationToken cancellationToken = default)
        {
            // Step 1: Validate page number
            if (query.PageNumber < 1)
            {
                return Fail(
                    "Brand.InvalidPageNumber",
                    "Page number must be greater than zero.");
            }

            // Step 2: Validate page size
            if (query.PageSize is < 1 or > 100)
            {
                return Fail(
                    "Brand.InvalidPageSize",
                    "Page size must be between 1 and 100.");
            }

            // Step 3: Prevent pagination offset overflow
            long skip = ((long)query.PageNumber - 1)
                * query.PageSize;

            if (skip > int.MaxValue)
            {
                return Fail(
                    "Brand.InvalidPagination",
                    "The requested page is too large.");
            }

            // Step 4: Normalize search
            string? search = string.IsNullOrWhiteSpace(query.Search)
                ? null
                : query.Search.Trim();

            // Step 5: Construct validated query
            var validatedQuery = query with
            {
                Search = search
            };

            // Step 6: Retrieve brands
            var (items, totalCount) =
                await _brandReadRepository.GetPagedAsync(
                    validatedQuery,
                    cancellationToken);

            // Step 7: Calculate total pages
            int totalPages = (int)Math.Ceiling(
                (double)totalCount / query.PageSize);

            // Step 8: Build response
            var response = new GetBrandsResponse(
                items,
                query.PageNumber,
                query.PageSize,
                totalCount,
                totalPages);

            return Result<GetBrandsResponse>.Success(response);
        }

        private static Result<GetBrandsResponse> Fail(
            string code,
            string message)
        {
            return Result<GetBrandsResponse>.Failure(
                new[]
                {
                new Error(code, message)
                });
        }
    }
}
