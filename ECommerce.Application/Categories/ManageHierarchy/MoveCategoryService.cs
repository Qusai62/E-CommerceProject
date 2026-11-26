using ECommerce.Application.Categories.Abstractions;
using ECommerce.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Categories.ManageHierarchy
{
    public sealed class MoveCategoryService
    {
        private readonly ICategoryRepository _repository;

        public MoveCategoryService(ICategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<MoveCategoryResponse>> ExecuteAsync(
            MoveCategoryCommand command,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(command);

            // STEP 1: Validate category ID.
            if (command.CategoryId <= 0)
            {
                return Fail(
                    "Category.InvalidId",
                    "Category ID must be positive.",
                    nameof(command.CategoryId));
            }

            // STEP 2: Validate the optional parent ID.
            if (command.ParentCategoryId is <= 0)
            {
                return Fail(
                    "Category.InvalidParentId",
                    "Parent category ID must be positive.",
                    nameof(command.ParentCategoryId));
            }

            // STEP 3: Load the category being moved.
            var category = await _repository.GetByIdAsync(
                command.CategoryId,
                cancellationToken);

            if (category is null)
            {
                return Fail(
                    "Category.NotFound",
                    "Category does not exist.",
                    nameof(command.CategoryId));
            }

            // STEP 4: Reject self-parenting.
            if (command.ParentCategoryId == category.CategoryId)
            {
                return Fail(
                    "Category.SelfParent",
                    "A category cannot be its own parent.",
                    nameof(command.ParentCategoryId));
            }

            // STEP 5: Validate the new parent's ancestry.
            if (command.ParentCategoryId.HasValue)
            {
                var visited = new HashSet<long>();
                long? currentId = command.ParentCategoryId.Value;

                while (currentId.HasValue)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    // Detect existing cycles in the ancestor chain.
                    if (!visited.Add(currentId.Value))
                    {
                        return Fail(
                            "Category.CircularHierarchy",
                            "A circular hierarchy was detected.",
                            nameof(command.ParentCategoryId));
                    }

                    // Reaching the category being moved means
                    // the requested parent is one of its descendants.
                    if (currentId.Value == category.CategoryId)
                    {
                        return Fail(
                            "Category.CircularHierarchy",
                            "Cannot move a category under its descendant.",
                            nameof(command.ParentCategoryId));
                    }

                    var ancestor = await _repository.GetByIdAsync(
                        currentId.Value,
                        cancellationToken);

                    if (ancestor is null)
                    {
                        return Fail(
                            "Category.ParentNotFound",
                            "Parent category does not exist.",
                            nameof(command.ParentCategoryId));
                    }

                    currentId = ancestor.ParentCategoryId;
                }
            }

            // STEP 6: Return success if the parent is unchanged.
            if (category.ParentCategoryId == command.ParentCategoryId)
            {
                return Success(category);
            }

            // STEP 7: Check name uniqueness under the new parent.
            var duplicateExists = await _repository.SiblingNameExistsAsync(
                command.ParentCategoryId,
                category.Name,
                cancellationToken);

            if (duplicateExists)
            {
                return Fail(
                    "Category.DuplicateName",
                    "Another category with this name exists under the new parent.",
                    nameof(command.ParentCategoryId));
            }

            // STEP 8: Move the category.
            category.ParentCategoryId = command.ParentCategoryId;
            category.UpdatedAtUtc = DateTime.UtcNow;

            // STEP 9: Persist the change.
            await _repository.SaveChangesAsync(cancellationToken);

            // STEP 10: Return the result.
            return Success(category);
        }

        private static Result<MoveCategoryResponse> Success(
            ECommerce.Domain.Catalog.Category category)
        {
            return Result<MoveCategoryResponse>.Success(
                new MoveCategoryResponse(
                    category.CategoryId,
                    category.Name,
                    category.ParentCategoryId,
                    category.UpdatedAtUtc));
        }

        private static Result<MoveCategoryResponse> Fail(
            string code,
            string message,
            string field)
        {
            return Result<MoveCategoryResponse>.Failure(
                new[]
                {
                new Error(code, message, field)
                });
        }
    }
}
