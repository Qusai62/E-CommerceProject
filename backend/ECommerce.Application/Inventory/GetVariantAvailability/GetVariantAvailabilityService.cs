using ECommerce.Application.Common.Results;
using ECommerce.Application.Inventory.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Inventory.GetVariantAvailability
{
    public sealed class GetVariantAvailabilityService
    {
        private readonly IInventoryAvailabilityReadRepository _repository;

        public GetVariantAvailabilityService(
            IInventoryAvailabilityReadRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<GetVariantAvailabilityResponse>> ExecuteAsync(
            long productVariantId,
            CancellationToken cancellationToken = default)
        {
            // 1. Validate the variant ID
            if (productVariantId <= 0)
            {
                return Result<GetVariantAvailabilityResponse>.Failure(
                    new[]
                    {
                    new Error(
                        "Inventory.InvalidVariantId",
                        "Product variant ID must be greater than zero.",
                        nameof(productVariantId))
                    });
            }

            // 2. Retrieve stock information
            var snapshot = await _repository.GetAsync(
                productVariantId,
                cancellationToken);

            // 3. Check if the variant exists
            if (snapshot is null)
            {
                return Result<GetVariantAvailabilityResponse>.Failure(
                    new[]
                    {
                    new Error(
                        "Inventory.VariantNotFound",
                        "Product variant was not found or is unavailable.",
                        nameof(productVariantId))
                    });
            }

            // 4. Calculate available quantity
            long availableQuantity =
                (long)snapshot.OnHandQuantity
                - snapshot.ReservedQuantity
                - snapshot.SecuredCODQuantity;

            // 5. Protect against inconsistent inventory data
            if (availableQuantity < 0)
            {
                throw new InvalidOperationException(
                    $"Inventory inconsistency for variant {productVariantId}.");
            }

            // 6. Determine stock status
            bool isInStock = availableQuantity > 0;

            // 7. Return the result
            var response = new GetVariantAvailabilityResponse(
                snapshot.ProductVariantId,
                (int)availableQuantity,
                isInStock
            );

            return Result<GetVariantAvailabilityResponse>.Success(response);
        }
    }
}
