using ECommerce.Application.Common.Results;
using ECommerce.Application.Inventory.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Inventory.IncreaseStock
{
    public class IncreaseStockService
    {
        private readonly IInventoryStockWriteRepository _repository;

        public IncreaseStockService(
            IInventoryStockWriteRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<IncreaseStockResponse>> ExecuteAsync(
            IncreaseStockCommand command,
            CancellationToken cancellationToken = default)
        {
            // 1. Validate input
            var errors = new List<Error>();

            if (command.ProductVariantId <= 0)
            {
                errors.Add(new Error(
                    "Inventory.InvalidVariantId",
                    "Product variant ID must be greater than zero.",
                    nameof(command.ProductVariantId)));
            }

            if (command.Quantity <= 0)
            {
                errors.Add(new Error(
                    "Inventory.InvalidQuantity",
                    "Quantity must be greater than zero.",
                    nameof(command.Quantity)));
            }

            if (errors.Count > 0)
            {
                return Result<IncreaseStockResponse>.Failure(errors);
            }

            // 2. Execute stock increment
            var outcome = await _repository.IncreaseAsync(
                command.ProductVariantId,
                command.Quantity,
                cancellationToken);

            // 3. Handle repository outcomes
            return outcome switch
            {
                IncreaseStockOutcome.Increased =>
                    Result<IncreaseStockResponse>.Success(
                        new IncreaseStockResponse(
                            command.ProductVariantId,
                            command.Quantity)),

                IncreaseStockOutcome.VariantNotFound =>
                    Result<IncreaseStockResponse>.Failure(
                        new[]
                        {
                        new Error(
                            "Inventory.VariantNotFound",
                            "Product variant was not found.",
                            nameof(command.ProductVariantId))
                        }),

                IncreaseStockOutcome.QuantityLimitExceeded =>
                    Result<IncreaseStockResponse>.Failure(
                        new[]
                        {
                        new Error(
                            "Inventory.QuantityLimitExceeded",
                            "Increasing stock would exceed the maximum allowed quantity.",
                            nameof(command.Quantity))
                        }),

                _ => throw new InvalidOperationException(
                    "Unexpected inventory update outcome.")
            };
        }
    }
}
