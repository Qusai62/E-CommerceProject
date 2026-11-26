using ECommerce.Application.Categories.Abstractions;
using ECommerce.Application.Common.Results;
using ECommerce.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Categories.CreateCategory
{
    public sealed class CreateCategoryService
    {
        private readonly ICategoryRepository _repository;

        public CreateCategoryService(ICategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<CreateCategoryResponse>> ExecuteAsync(
            CreateCategoryCommand command,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(command);

            cancellationToken.ThrowIfCancellationRequested();

            // STEP 1: Normalize category name.
            var name = command.Name?.Trim() ?? string.Empty;

            // STEP 2: Validate category name.
            if (string.IsNullOrWhiteSpace(name))
            {
                return Failure(
                    "Category.NameRequired",
                    "Category name is required.",
                    nameof(command.Name));
            }

            if (name.Length > 150)
            {
                return Failure(
                    "Category.NameTooLong",
                    "Category name cannot exceed 150 characters.",
                    nameof(command.Name));
            }

            // STEP 3: Normalize and validate description.
            var description = command.Description?.Trim();

            if (description?.Length > 1000)
            {
                return Failure(
                    "Category.DescriptionTooLong",
                    "Description cannot exceed 1000 characters.",
                    nameof(command.Description));
            }

            // STEP 4: Validate the optional parent.
            if (command.ParentCategoryId.HasValue)
            {
                if (command.ParentCategoryId.Value <= 0)
                {
                    return Failure(
                        "Category.InvalidParent",
                        "Parent category ID must be positive.",
                        nameof(command.ParentCategoryId));
                }

                var parentExists = await _repository.ParentExistsAsync(
                    command.ParentCategoryId.Value,
                    cancellationToken);

                if (!parentExists)
                {
                    return Failure(
                        "Category.ParentNotFound",
                        "The selected parent category does not exist.",
                        nameof(command.ParentCategoryId));
                }
            }

            // STEP 5: Validate sibling-name uniqueness.
            var nameExists = await _repository.SiblingNameExistsAsync(
                command.ParentCategoryId,
                name,
                cancellationToken);

            if (nameExists)
            {
                return Failure(
                    "Category.DuplicateName",
                    "A category with this name already exists under the selected parent.",
                    nameof(command.Name));
            }

            // STEP 6: Generate the category slug.
            var baseSlug = GenerateSlug(name);

            if (string.IsNullOrWhiteSpace(baseSlug))
            {
                return Failure(
                    "Category.InvalidSlug",
                    "The category name cannot produce a valid URL slug.",
                    nameof(command.Name));
            }

            // STEP 7: Ensure global slug uniqueness.
            var slug = baseSlug;
            var suffix = 2;

            while (await _repository.SlugExistsAsync(
                slug,
                cancellationToken))
            {
                var suffixText = $"-{suffix}";

                var allowedLength = 200 - suffixText.Length;

                slug = baseSlug[..Math.Min(
                    baseSlug.Length,
                    allowedLength)] + suffixText;

                suffix++;
            }

            // STEP 8: Create the Category entity.
            var now = DateTime.UtcNow;

            var category = new Category
            {
                Name = name,
                Description = description,
                Slug = slug,
                ParentCategoryId = command.ParentCategoryId,
                SortOrder = command.SortOrder,
                IsActive = false,
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            };

            // STEP 9: Persist the Category.
            await _repository.AddAsync(
                category,
                cancellationToken);

            // STEP 10: Return the response.
            return Result<CreateCategoryResponse>.Success(
                new CreateCategoryResponse
                {
                    CategoryId = category.CategoryId,
                    Name = category.Name,
                    Slug = category.Slug,
                    ParentCategoryId = category.ParentCategoryId,
                    IsActive = category.IsActive,
                    CreatedAtUtc = category.CreatedAtUtc
                });
        }

        private static string GenerateSlug(string value)
        {
            var normalized = value.Trim().ToLowerInvariant();

            var characters = normalized
                .Select(character =>
                    char.IsLetterOrDigit(character)
                        ? character
                        : '-')
                .ToArray();

            var slug = string.Join(
                '-',
                new string(characters)
                    .Split('-', StringSplitOptions.RemoveEmptyEntries));

            return slug.Length <= 200
                ? slug
                : slug[..200].TrimEnd('-');
        }

        private static Result<CreateCategoryResponse> Failure(
            string code,
            string message,
            string field)
        {
            return Result<CreateCategoryResponse>.Failure(
                new[]
                {
                new Error(code, message, field)
                });
        }
    }
}
