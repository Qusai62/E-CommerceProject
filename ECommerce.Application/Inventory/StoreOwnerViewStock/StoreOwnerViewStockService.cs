using ECommerce.Application.Common.Pagination;
using ECommerce.Application.Common.Results;
using ECommerce.Application.Inventory.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Inventory.StoreOwnerViewStock
{
    public class StoreOwnerViewStockService
    {
        private const int MaxPageSize = 100;

        private readonly IStoreOwnerInventoryReadRepository _repository;

        public StoreOwnerViewStockService(
            IStoreOwnerInventoryReadRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<PagedResult<StoreOwnerStockResponse>>> ExecuteAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            // 1. Validate pagination
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
                    $"Page size must be between 1 and {MaxPageSize}.",
                    nameof(pageSize)));
            }

            if (errors.Count > 0)
            {
                return Result<PagedResult<StoreOwnerStockResponse>>
                    .Failure(errors);
            }

            // Prevent Skip() calculation from overflowing
            if (((long)pageNumber - 1) * pageSize > int.MaxValue)
            {
                return Result<PagedResult<StoreOwnerStockResponse>>
                    .Failure(new[]
                    {
                    new Error(
                        "Inventory.InvalidPagingRange",
                        "The requested page is too large.",
                        nameof(pageNumber))
                    });
            }

            // 2. Retrieve stock snapshots from the repository
            var (snapshots, totalCount) =
                await _repository.GetPageAsync(
                    pageNumber,
                    pageSize,
                    cancellationToken);

            // 3. Build the response collection
            var responses = new List<StoreOwnerStockResponse>(
                snapshots.Count);

            foreach (var snapshot in snapshots)
            {
                // Validate stock integrity
                if (snapshot.OnHandQuantity < 0 ||
                    snapshot.ReservedQuantity < 0 ||
                    snapshot.SecuredCODQuantity < 0)
                {
                    throw new InvalidOperationException(
                        $"Invalid inventory quantities for variant " +
                        $"{snapshot.ProductVariantId}.");
                }

                // Calculate available stock
                long availableQuantity =
                    (long)snapshot.OnHandQuantity
                    - snapshot.ReservedQuantity
                    - snapshot.SecuredCODQuantity;

                if (availableQuantity < 0)
                {
                    throw new InvalidOperationException(
                        $"Inventory inconsistency for variant " +
                        $"{snapshot.ProductVariantId}.");
                }

                // Map the snapshot to the response
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
                    availableQuantity > 0
                );

                responses.Add(response);
            }

            // 4. Create the paginated result
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
    }
}
