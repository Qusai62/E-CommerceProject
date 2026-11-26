using ECommerce.Application.Common.Pagination;
using ECommerce.Application.Common.Results;
using ECommerce.Application.Inventory.Abstractions;
using ECommerce.Application.Inventory.StoreOwnerViewStock;
using System;

namespace ECommerce.Application.Inventory.ViewOutOfStock
{
    public class ViewOutOfStockService
    {
        private const int MaxPageSize = 100;

        private readonly IStoreOwnerInventoryReadRepository _repository;

        public ViewOutOfStockService(
            IStoreOwnerInventoryReadRepository repository)
        {
            _repository = repository;
        }

        private static List<Error> ValidatePagination(
            int pageNumber,
            int pageSize)
        {
            var errors = new List<Error>();

            if (pageNumber <= 0)
            {
                errors.Add(new Error(
                    "Inventory.InvalidPageNumber",
                    "Page number must be greater than zero.",
                    nameof(pageNumber)));
            }

            if (pageSize <= 0 || pageSize > MaxPageSize)
            {
                errors.Add(new Error(
                    "Inventory.InvalidPageSize",
                    "Page size must be between 1 and 100.",
                    nameof(pageSize)));
            }

            if (pageNumber > 0 && pageSize > 0 &&
                ((long)pageNumber - 1) * pageSize > int.MaxValue)
            {
                errors.Add(new Error(
                    "Inventory.InvalidPagingRange",
                    "The requested page is too large.",
                    nameof(pageNumber)));
            }

            return errors;
        }
        public async Task<Result<PagedResult<StoreOwnerStockResponse>>>
    GetVariantsAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
        {
            // 1. Validate pagination
            var errors = ValidatePagination(pageNumber, pageSize);

            if (errors.Count > 0)
            {
                return Result<PagedResult<StoreOwnerStockResponse>>
                    .Failure(errors);
            }

            // 2. Retrieve out-of-stock variants
            var (snapshots, totalCount) =
                await _repository.GetOutOfStockVariantsPageAsync(
                    pageNumber,
                    pageSize,
                    cancellationToken);

            // 3. Build the response collection
            var responses = new List<StoreOwnerStockResponse>(
                snapshots.Count);

            foreach (var snapshot in snapshots)
            {
                long availableQuantity =
                    (long)snapshot.OnHandQuantity
                    - snapshot.ReservedQuantity
                    - snapshot.SecuredCODQuantity;

                // Validate the repository result
                if (snapshot.OnHandQuantity < 0 ||
                    snapshot.ReservedQuantity < 0 ||
                    snapshot.SecuredCODQuantity < 0 ||
                    availableQuantity != 0)
                {
                    throw new InvalidOperationException(
                        $"Invalid out-of-stock snapshot for variant " +
                        $"{snapshot.ProductVariantId}.");
                }

                var response = new StoreOwnerStockResponse(
                    snapshot.ProductVariantId,
                    snapshot.ProductId,
                    snapshot.ProductName,
                    snapshot.Sku,
                    snapshot.ProductIsActive,
                    snapshot.VariantIsActive,
                    snapshot.OnHandQuantity,
                    snapshot.ReservedQuantity,
                    snapshot.SecuredCODQuantity,
                    (int)availableQuantity,
                    false
                );

                responses.Add(response);
            }

            // 4. Build PagedResult
            var pagedResult = new PagedResult<StoreOwnerStockResponse>(
                responses,
                pageNumber,
                pageSize,
                totalCount
            );

            // 5. Return success
            return Result<PagedResult<StoreOwnerStockResponse>>
                .Success(pagedResult);
        }

        public async Task<Result<PagedResult<OutOfStockProductResponse>>>
    GetProductsAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
        {
            // 1. Validate pagination
            var errors = ValidatePagination(pageNumber, pageSize);

            if (errors.Count > 0)
            {
                return Result<PagedResult<OutOfStockProductResponse>>
                    .Failure(errors);
            }

            // 2. Retrieve out-of-stock products
            var (snapshots, totalCount) =
                await _repository.GetOutOfStockProductsPageAsync(
                    pageNumber,
                    pageSize,
                    cancellationToken);

            // 3. Map snapshots to responses
            var responses = snapshots
                .Select(snapshot => new OutOfStockProductResponse(
                    snapshot.ProductId,
                    snapshot.ProductName,
                    snapshot.ProductIsActive
                ))
                .ToList();

            // 4. Build PagedResult
            var pagedResult = new PagedResult<OutOfStockProductResponse>(
                responses,
                pageNumber,
                pageSize,
                totalCount
            );

            // 5. Return success
            return Result<PagedResult<OutOfStockProductResponse>>
                .Success(pagedResult);
        }
    }
}
