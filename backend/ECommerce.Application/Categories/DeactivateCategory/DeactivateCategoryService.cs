using ECommerce.Application.Categories.Abstractions;
using ECommerce.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Categories.DeactivateCategory
{
    public class DeactivateCategoryService
    {
        private readonly ICategoryRepository _repository;
        private readonly TimeProvider _timeProvider;

        public DeactivateCategoryService(
            ICategoryRepository repository,
            TimeProvider timeProvider)
        {
            _repository = repository;
            _timeProvider = timeProvider;
        }

        public async Task<Result<DeactivateCategoryResponse>> ExecuteAsync(
            DeactivateCategoryCommand command,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(command);

            // STEP 1: Validate the category ID.
            if (command.CategoryId <= 0)
            {
                return Fail(
                    "Category.InvalidId",
                    "Category ID must be positive.",
                    nameof(command.CategoryId));
            }

            // STEP 2: Load the category from the database.
            var category = await _repository.GetByIdAsync(
                command.CategoryId,
                cancellationToken);

            // STEP 3: Check whether the category exists.
            if (category is null)
            {
                return Fail(
                    "Category.NotFound",
                    "The selected category does not exist.",
                    nameof(command.CategoryId));
            }

            // STEP 4: Only active categories can be deactivated.
            if (!category.IsActive)
            {
                return Fail(
                    "Category.AlreadyInactive",
                    "The category is already inactive.",
                    nameof(command.CategoryId));
            }

            // STEP 5: Deactivate only this category.
            category.IsActive = false;

            // STEP 6: Update modification timestamp.
            category.UpdatedAtUtc =
                _timeProvider.GetUtcNow().UtcDateTime;

            // STEP 7: Persist the changes.
            await _repository.SaveChangesAsync(
                cancellationToken);

            // STEP 8: Return the updated status.
            return Result<DeactivateCategoryResponse>.Success(
                new DeactivateCategoryResponse(
                    category.CategoryId,
                    category.Name,
                    category.IsActive,
                    category.UpdatedAtUtc));
        }

        private static Result<DeactivateCategoryResponse> Fail(
            string code,
            string message,
            string field)
        {
            return Result<DeactivateCategoryResponse>.Failure(
                new[]
                {
                new Error(code, message, field)
                });
        }
    }
}
