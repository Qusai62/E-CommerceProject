using ECommerce.Application.Common.Results;
using ECommerce.Application.Inventory.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Inventory.DecreaseStock
{
    public class DecreaseStockService
    {
        private readonly IInventoryStockWriteRepository _repository;

        public DecreaseStockService(
            IInventoryStockWriteRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<DecreaseStockResponse>> ExecuteAsync(
            DecreaseStockCommand command,
            CancellationToken cancellationToken = default)
        {
            // 1. Validate the command
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

            // 2. Return validation errors
            if (errors.Count > 0)
            {
                return Result<DecreaseStockResponse>.Failure(errors);
            }

            // 3. Call the repository
            var outcome = await _repository.DecreaseAsync(
                command.ProductVariantId,
                command.Quantity,
                cancellationToken);

            // 4. Handle repository outcomes
            return outcome switch
            {
                DecreaseStockOutcome.Decreased =>
                    Result<DecreaseStockResponse>.Success(
                        new DecreaseStockResponse(
                            command.ProductVariantId,
                            command.Quantity)),

                DecreaseStockOutcome.VariantNotFound =>
                    Result<DecreaseStockResponse>.Failure(
                        new[]
                        {
                        new Error(
                            "Inventory.VariantNotFound",
                            "Product variant was not found.",
                            nameof(command.ProductVariantId))
                        }),

                DecreaseStockOutcome.InsufficientAvailableStock =>
                    Result<DecreaseStockResponse>.Failure(
                        new[]
                        {
                        new Error(
                            "Inventory.InsufficientAvailableStock",
                            "The requested quantity exceeds available stock.",
                            nameof(command.Quantity))
                        }),

                _ => throw new InvalidOperationException(
                    "Unexpected inventory decrease outcome.")
            };
        }
    }
}
