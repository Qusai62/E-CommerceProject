using ECommerce.Application.Catalog.Abstractions;
using ECommerce.Application.Common.Results;
using ECommerce.Domain.Catalog;
using System;
using System.Collections.Generic;
using TechnicalSpecificationEntity = ECommerce.Domain.Catalog.TechnicalSpecification;


namespace ECommerce.Application.Catalog.TechnicalSpecification.AddTechnicalSpecification
{
    public class AddTechnicalSpecificationService
    {
        private readonly ITechnicalSpecificationRepository _repository;

        public AddTechnicalSpecificationService(
            ITechnicalSpecificationRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<AddTechnicalSpecificationResponse>> ExecuteAsync(
        AddTechnicalSpecificationCommand command,
        CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(command);

            var errors = new List<Error>();

            // 1. Normalize
            var name = command.Name?.Trim();
            var value = command.Value?.Trim();

            // 2. Validate ProductId
            if (command.ProductId <= 0)
            {
                errors.Add(new Error(
                    "Product.InvalidId",
                    "ProductId must be positive.",
                    "ProductId"));
            }

            // 3. Validate Name
            if (string.IsNullOrWhiteSpace(name))
            {
                errors.Add(new Error(
                    "Specification.NameRequired",
                    "Specification name is required.",
                    "Name"));
            }
            else if (name.Length > 100)
            {
                errors.Add(new Error(
                    "Specification.NameTooLong",
                    "Specification name cannot exceed 100 characters.",
                    "Name"));
            }

            // 4. Validate Value
            if (string.IsNullOrWhiteSpace(value))
            {
                errors.Add(new Error(
                    "Specification.ValueRequired",
                    "Specification value is required.",
                    "Value"));
            }
            else if (value.Length > 500)
            {
                errors.Add(new Error(
                    "Specification.ValueTooLong",
                    "Specification value cannot exceed 500 characters.",
                    "Value"));
            }

            // 5. Validate SortOrder
            if (command.SortOrder < 0)
            {
                errors.Add(new Error(
                    "Specification.InvalidSortOrder",
                    "SortOrder cannot be negative.",
                    "SortOrder"));
            }


            // 6. Return all basic validation errors
            if (errors.Count > 0)
            {
                return Result<AddTechnicalSpecificationResponse>
                    .Failure(errors);
            }

            // 7. Verify parent product exists
            var productExists = await _repository.ProductExistsAsync(
                command.ProductId,
                cancellationToken);

            if (!productExists)
            {
                return Result<AddTechnicalSpecificationResponse>.Failure(
                    new[]
                    {
                    new Error(
                        "Product.NotFound",
                        "The selected product does not exist.",
                        "ProductId")
                    });
            }

            // 8. Verify specification name is unique for this product
            var nameExists = await _repository.SpecificationNameExistsAsync(
                command.ProductId,
                name!,
                cancellationToken);

            if (nameExists)
            {
                return Result<AddTechnicalSpecificationResponse>.Failure(
                    new[]
                    {
                    new Error(
                        "Specification.DuplicateName",
                        "The product already has a specification with this name.",
                        "Name")
                    });
            }

            // 9. Create entity
            var now = DateTime.UtcNow;

            var specification = new TechnicalSpecificationEntity
            {
                ProductId = command.ProductId,
                Name = name!,
                Value = value!,
                SortOrder = command.SortOrder,
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            };

            // 10. Persist entity
            await _repository.AddAsync(
                specification,
                cancellationToken);

            // 11. Build response
            var response = new AddTechnicalSpecificationResponse
            {
                TechnicalSpecificationId =
                    specification.TechnicalSpecificationId,
                ProductId = specification.ProductId,
                Name = specification.Name,
                Value = specification.Value,
                SortOrder = specification.SortOrder,
                CreatedAtUtc = specification.CreatedAtUtc
            };

            return Result<AddTechnicalSpecificationResponse>
                .Success(response);
        }
    }
}
