using ECommerce.Application.Catalog.Abstractions;
using ECommerce.Application.Common.Results;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Catalog.TechnicalSpecification.UpdateTechnicalSpecification
{
    public class UpdateTechnicalSpecificationService
    {
        private readonly ITechnicalSpecificationUpdateRepository _repository;

        public UpdateTechnicalSpecificationService(
            ITechnicalSpecificationUpdateRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<UpdateTechnicalSpecificationResponse>> ExecuteAsync(
            UpdateTechnicalSpecificationCommand command,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(command);

            var errors = new List<Error>();

            // 1. Normalize
            var name = command.Name?.Trim();
            var value = command.Value?.Trim();

            // 2. Validate route IDs
            if (command.ProductId <= 0)
            {
                errors.Add(new Error(
                    "Product.InvalidId",
                    "ProductId must be positive.",
                    "ProductId"));
            }

            if (command.SpecificationId <= 0)
            {
                errors.Add(new Error(
                    "Specification.InvalidId",
                    "SpecificationId must be positive.",
                    "SpecificationId"));
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

            // 6. Return validation errors
            if (errors.Count > 0)
            {
                return Result<UpdateTechnicalSpecificationResponse>
                    .Failure(errors);
            }

            // 7. Find the specification belonging to this product
            //EF Core does two things:
            //1.Executes a SELECT query to retrieve the specification.
            //2.Registers the returned entity in the DbContext.ChangeTracker.
            var specification = await _repository.GetByIdAsync(
                command.ProductId,
                command.SpecificationId,
                cancellationToken);

            if (specification is null)
            {
                return Result<UpdateTechnicalSpecificationResponse>.Failure(
                    new[]
                    {
                    new Error(
                        "Specification.NotFound",
                        "The selected specification does not exist for this product.",
                        "SpecificationId")
                    });
            }

            // 8. Check name uniqueness, excluding current specification
            var duplicateName = await _repository.SpecificationNameExistsAsync(
                command.ProductId,
                name!,
                command.SpecificationId,
                cancellationToken);

            if (duplicateName)
            {
                return Result<UpdateTechnicalSpecificationResponse>.Failure(
                    new[]
                    {
                    new Error(
                        "Specification.DuplicateName",
                        "The product already has another specification with this name.",
                        "Name")
                    });
            }

            // 9. Modify the tracked entity
            specification.Name = name!;
            specification.Value = value!;
            specification.SortOrder = command.SortOrder;
            specification.UpdatedAtUtc = DateTime.UtcNow;

            // 10. Save changes
            await _repository.SaveChangesAsync(cancellationToken);

            // 11. Return response
            return Result<UpdateTechnicalSpecificationResponse>.Success(
                new UpdateTechnicalSpecificationResponse
                {
                    TechnicalSpecificationId =
                        specification.TechnicalSpecificationId,
                    ProductId = specification.ProductId,
                    Name = specification.Name,
                    Value = specification.Value,
                    SortOrder = specification.SortOrder,
                    UpdatedAtUtc = specification.UpdatedAtUtc
                });
        }
    }
}