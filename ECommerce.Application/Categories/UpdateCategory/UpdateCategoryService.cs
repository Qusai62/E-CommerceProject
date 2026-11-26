using ECommerce.Application.Categories.Abstractions;
using ECommerce.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Categories.UpdateCategory
{
    public sealed class UpdateCategoryService
    {
        private readonly ICategoryRepository _repository;

        public UpdateCategoryService(ICategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<UpdateCategoryResponse>> ExecuteAsync(
            UpdateCategoryCommand command,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(command);

            // 1. Validate the category ID.
            if (command.CategoryId <= 0)
            {
                return Fail(
                    "Category.InvalidId",
                    "Category ID must be positive.",
                    nameof(command.CategoryId));
            }

            // 2. Normalize and validate the name.
            var name = command.Name?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(name))
            {
                return Fail(
                    "Category.NameRequired",
                    "Category name is required.",
                    nameof(command.Name));
            }

            if (name.Length > 150)
            {
                return Fail(
                    "Category.NameTooLong",
                    "Category name cannot exceed 150 characters.",
                    nameof(command.Name));
            }

            // 3. Normalize and validate the description.
            var description = command.Description?.Trim();

            if (description?.Length > 1000)
            {
                return Fail(
                    "Category.DescriptionTooLong",
                    "Description cannot exceed 1000 characters.",
                    nameof(command.Description));
            }

            // 4. Load the existing category.
            var category = await _repository.GetByIdAsync(
                command.CategoryId,
                cancellationToken);

            if (category is null)
            {
                return Fail(
                    "Category.NotFound",
                    "The selected category does not exist.",
                    nameof(command.CategoryId));
            }

            // 5. Check name uniqueness only if the name changes.
            var nameChanged = !string.Equals(
                category.Name,
                name,
                StringComparison.OrdinalIgnoreCase);

            if (nameChanged)
            {
                var duplicateExists =
                    await _repository.SiblingNameExistsAsync(
                        category.ParentCategoryId,
                        name,
                        cancellationToken);

                if (duplicateExists)
                {
                    return Fail(
                        "Category.DuplicateName",
                        "Another category with this name exists under the same parent.",
                        nameof(command.Name));
                }
            }

            // 6. Update only the permitted properties.
            category.Name = name;
            category.Description = description;
            category.SortOrder = command.SortOrder;
            category.UpdatedAtUtc = DateTime.UtcNow;

            // 7. Persist changes.
            await _repository.SaveChangesAsync(cancellationToken);

            // 8. Return the updated category.
            return Result<UpdateCategoryResponse>.Success(
                new UpdateCategoryResponse(
                    category.CategoryId,
                    category.Name,
                    category.Slug,
                    category.Description,
                    category.ParentCategoryId,
                    category.SortOrder,
                    category.IsActive,
                    category.UpdatedAtUtc));
        }

        private static Result<UpdateCategoryResponse> Fail(
            string code,
            string message,
            string field)
        {
            return Result<UpdateCategoryResponse>.Failure(
                new[]
                {
                new Error(code, message, field)
                });
        }
    }
}
