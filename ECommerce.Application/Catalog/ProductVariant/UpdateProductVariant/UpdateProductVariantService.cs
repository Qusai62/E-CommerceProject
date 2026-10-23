using ECommerce.Application.Catalog.Abstractions;
using ECommerce.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Catalog.ProductVariant.UpdateProductVariant
{
    public class UpdateProductVariantService
    {
        private readonly IProductVariantUpdateRepository _repository;

        public UpdateProductVariantService(
            IProductVariantUpdateRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<UpdateProductVariantResponse>> ExecuteAsync(
            UpdateProductVariantCommand command,
            CancellationToken cancellationToken = default)
        {
            var errors = new List<Error>();

            // 1. Validate IDs
            if (command.ProductId <= 0)
            {
                errors.Add(new Error(
                    "InvalidProductId",
                    "ProductId must be greater than zero.",
                    nameof(command.ProductId)));
            }

            if (command.ProductVariantId <= 0)
            {
                errors.Add(new Error(
                    "InvalidVariantId",
                    "ProductVariantId must be greater than zero.",
                    nameof(command.ProductVariantId)));
            }

            // 2. Normalize SKU
            var sku = command.Sku?.Trim() ?? string.Empty;

            // 3. Validate SKU
            if (string.IsNullOrWhiteSpace(sku))
            {
                errors.Add(new Error(
                    "SkuRequired",
                    "SKU is required.",
                    nameof(command.Sku)));
            }

            // Use the maximum length configured in your database.
            // Example: 100 characters.
            else if (sku.Length > 100)
            {
                errors.Add(new Error(
                    "SkuTooLong",
                    "SKU cannot exceed 100 characters.",
                    nameof(command.Sku)));
            }

            // 4. Validate Price
            if (command.Price <= 0)
            {
                errors.Add(new Error(
                    "InvalidPrice",
                    "Price must be greater than zero.",
                    nameof(command.Price)));
            }

            // 5. Stop if validation failed
            if (errors.Count > 0)
            {
                return Result<UpdateProductVariantResponse>
                    .Failure(errors);
            }

            // 6. Get existing variant (tracked by EF Core)
            var variant = await _repository.GetByIdAsync(
                command.ProductId,
                command.ProductVariantId,
                cancellationToken);

            if (variant is null)
            {
                return Result<UpdateProductVariantResponse>.Failure(
                    new List<Error>
                    {
                    new Error(
                        "VariantNotFound",
                        "Product variant was not found.",
                        nameof(command.ProductVariantId))
                    });
            }

            // 7. Check SKU uniqueness only if SKU changes
            if (variant.Sku != sku)
            {
                var skuExists = await _repository.SkuExistsAsync(
                    sku,
                    variant.ProductVariantId,
                    cancellationToken);

                if (skuExists)
                {
                    return Result<UpdateProductVariantResponse>.Failure(
                        new List<Error>
                        {
                        new Error(
                            "DuplicateSku",
                            "Another variant already uses this SKU.",
                            nameof(command.Sku))
                        });
                }
            }

            // 8. Update entity properties
            variant.Sku = sku;
            variant.Price = command.Price;
            variant.IsActive = command.IsActive;
            variant.UpdatedAtUtc = DateTime.UtcNow;

            // 9. Persist changes
            await _repository.SaveChangesAsync(cancellationToken);

            // 10. Build response
            var response = new UpdateProductVariantResponse
            {
                ProductVariantId = variant.ProductVariantId,
                ProductId = variant.ProductId,
                Sku = variant.Sku,
                Price = variant.Price,
                IsActive = variant.IsActive,
                UpdatedAtUtc = variant.UpdatedAtUtc
            };

            return Result<UpdateProductVariantResponse>
                .Success(response);
        }
    }
}
