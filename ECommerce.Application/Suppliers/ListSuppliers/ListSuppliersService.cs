using ECommerce.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Suppliers.ListSuppliers
{
    public sealed class ListSuppliersService
    {
        private readonly IListSuppliersRepository _repository;

        public ListSuppliersService(
            IListSuppliersRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<PagedSuppliersResponse>> ExecuteAsync(
            ListSuppliersRequest request,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            if (request.PageNumber < 1)
            {
                return Result<PagedSuppliersResponse>.Failure(
                    new[]
                    {
                    new Error(
                        "Supplier.InvalidPageNumber",
                        "Page number must be greater than zero.")
                    });
            }

            if (request.PageSize is < 1 or > 100)
            {
                return Result<PagedSuppliersResponse>.Failure(
                    new[]
                    {
                    new Error(
                        "Supplier.InvalidPageSize",
                        "Page size must be between 1 and 100.")
                    });
            }

            if (request.Search?.Length > 200)
            {
                return Result<PagedSuppliersResponse>.Failure(
                    new[]
                    {
                    new Error(
                        "Supplier.InvalidSearch",
                        "Search text cannot exceed 200 characters.")
                    });
            }

            var sortBy = request.SortBy?.Trim().ToLowerInvariant();

            if (sortBy is not ("name" or "createdat"))
            {
                return Result<PagedSuppliersResponse>.Failure(
                    new[]
                    {
                    new Error(
                        "Supplier.InvalidSort",
                        "SortBy must be name or createdAt.")
                    });
            }

            // Prevent overflowing Skip calculation.
            if (((long)request.PageNumber - 1) * request.PageSize
                > int.MaxValue)
            {
                return Result<PagedSuppliersResponse>.Failure(
                    new[]
                    {
                    new Error(
                        "Supplier.InvalidPageNumber",
                        "Requested page exceeds the supported range.")
                    });
            }

            var normalizedRequest = request with
            {
                Search = request.Search?.Trim(),
                SortBy = sortBy
            };

            var response = await _repository.GetPagedAsync(
                normalizedRequest,
                cancellationToken);

            return Result<PagedSuppliersResponse>.Success(response);
        }
    }
}
