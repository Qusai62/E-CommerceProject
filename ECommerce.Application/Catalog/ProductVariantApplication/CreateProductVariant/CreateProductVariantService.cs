using ECommerce.Application.Catalog.Abstractions;
using ECommerce.Application.Common.Results;
using ECommerce.Domain.Catalog;
using ECommerce.Domain.InventoryManagement;
using System;
using System.Collections.Generic;
using ProductVariantEntity = ECommerce.Domain.Catalog.ProductVariant;


namespace ECommerce.Application.Catalog.ProductVariant.CreateProductVariant
{
    public class CreateProductVariantService
    {
        private readonly IProductVariantWriteRepository _repository;

        public CreateProductVariantService(IProductVariantWriteRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<CreateProductVariantResponse>> ExecuteAsync(
       CreateProductVariantCommand command,
       CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(command);

            var errors = new List<Error>();

            // 1. Normalize input
            var sku = command.Sku?.Trim().ToUpperInvariant();
            var attributes = command.Attributes;

            // 2. Validate ProductId
            if (command.ProductId <= 0)
            {
                errors.Add(new Error(
                    "Product.InvalidId",
                    "ProductId must be positive.",
                    "ProductId"));
            }

            // 3. Validate SKU
            if (string.IsNullOrWhiteSpace(sku))
            {
                errors.Add(new Error(
                    "Variant.SkuRequired",
                    "SKU is required.",
                    "Sku"));
            }
            else if (sku.Length > 100)
            {
                errors.Add(new Error(
                    "Variant.SkuTooLong",
                    "SKU cannot exceed 100 characters.",
                    "Sku"));
            }

            // 4. Validate price
            if (command.Price <= 0)
            {
                errors.Add(new Error(
                    "Variant.InvalidPrice",
                    "Price must be greater than zero.",
                    "Price"));
            }
            else if (command.Price > 999999999999999.999m ||
                     decimal.Round(command.Price, 3) != command.Price)
            {
                errors.Add(new Error(
                    "Variant.InvalidPricePrecision",
                    "Price must fit decimal(18,3).",
                    "Price"));
            }

            // 5. Validate attributes
            if (attributes is null)
            {
                errors.Add(new Error(
                    "Variant.AttributesRequired",
                    "Attributes must be a list, not null.",
                    "Attributes"));
            }
            else
            {
                var attributeNames = new HashSet<string>(
                    StringComparer.OrdinalIgnoreCase);

                for (var i = 0; i < attributes.Count; i++)
                {
                    var attribute = attributes[i];

                    if (attribute is null)
                    {
                        errors.Add(new Error(
                            "Variant.InvalidAttribute",
                            "Attribute cannot be null.",
                            $"Attributes[{i}]"));

                        continue;
                    }

                    var name = attribute.Name?.Trim();
                    var value = attribute.Value?.Trim();

                    if (string.IsNullOrWhiteSpace(name))
                    {
                        errors.Add(new Error(
                            "Variant.AttributeNameRequired",
                            "Attribute name is required.",
                            $"Attributes[{i}].Name"));
                    }
                    else if (name.Length > 100)
                    {
                        errors.Add(new Error(
                            "Variant.AttributeNameTooLong",
                            "Attribute name cannot exceed 100 characters.",
                            $"Attributes[{i}].Name"));
                    }

                    if (string.IsNullOrWhiteSpace(value))
                    {
                        errors.Add(new Error(
                            "Variant.AttributeValueRequired",
                            "Attribute value is required.",
                            $"Attributes[{i}].Value"));
                    }
                    else if (value.Length > 200)
                    {
                        errors.Add(new Error(
                            "Variant.AttributeValueTooLong",
                            "Attribute value cannot exceed 200 characters.",
                            $"Attributes[{i}].Value"));
                    }

                    if (!string.IsNullOrWhiteSpace(name) &&
                        !attributeNames.Add(name))
                    {
                        errors.Add(new Error(
                            "Variant.DuplicateAttributeName",
                            "Attribute names must be unique per variant.",
                            $"Attributes[{i}].Name"));
                    }
                }
            }

            // 6. Stop if basic validation failed
            if (errors.Count > 0)
            {
                return Result<CreateProductVariantResponse>
                    .Failure(errors);
            }

            // 7. Check parent product
            if (!await _repository.ProductExistsAsync(
                    command.ProductId, cancellationToken))
            {
                return Result<CreateProductVariantResponse>.Failure(
                    new[]
                    {
                    new Error(
                        "Product.NotFound",
                        "The selected product does not exist.",
                        "ProductId")
                    });
            }

            // 8. Check SKU uniqueness
            if (await _repository.SkuExistsAsync(
                    sku!, cancellationToken))
            {
                return Result<CreateProductVariantResponse>.Failure(
                    new[]
                    {
                    new Error(
                        "Variant.DuplicateSku",
                        "The SKU is already in use.",
                        "Sku")
                    });
            }

            // 9. Build the entity graph
            var now = DateTime.UtcNow;

            var variant = new ProductVariantEntity
            {
                ProductId = command.ProductId,
                Sku = sku!,
                Price = command.Price,
                IsActive = false,
                CreatedAtUtc = now,
                UpdatedAtUtc = now,

                Inventory = new Inventory
                {
                    OnHandQuantity = 0,
                    CreatedAtUtc = now,
                    UpdatedAtUtc = now
                }
            };

            foreach (var attribute in attributes!)
            {
                variant.Attributes.Add(new VariantAttribute
                {
                    Name = attribute.Name.Trim(),
                    Value = attribute.Value.Trim(),
                    SortOrder = attribute.SortOrder,
                    CreatedAtUtc = now,
                    UpdatedAtUtc = now
                });
            }

            // 10. Save variant, attributes and inventory
            await _repository.AddAsync(variant, cancellationToken);

            // 11. Create response
            var response = new CreateProductVariantResponse
            {
                ProductVariantId = variant.ProductVariantId,
                ProductId = variant.ProductId,
                Sku = variant.Sku,
                Price = variant.Price,
                IsActive = variant.IsActive,
                OnHandQuantity = variant.Inventory.OnHandQuantity,
                CreatedAtUtc = variant.CreatedAtUtc
            };

            return Result<CreateProductVariantResponse>.Success(response);
        }
    }
}
