using ECommerce.Application.Categories.Abstractions;
using ECommerce.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Categories.ReactivateCategory
{
    public class ReactivateCategoryService
    {
        private readonly ICategoryRepository _repository;
        private readonly TimeProvider _timeProvider;

        public ReactivateCategoryService(
            ICategoryRepository repository,
            TimeProvider timeProvider)
        {
            _repository = repository;
            _timeProvider = timeProvider;
        }

        public async Task<Result<ReactivateCategoryResponse>> ExecuteAsync(
            ReactivateCategoryCommand command,
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

            // STEP 2: Load the selected category.
            var category = await _repository.GetByIdAsync(
                command.CategoryId,
                cancellationToken);

            // STEP 3: Verify that the category exists.
            if (category is null)
            {
                return Fail(
                    "Category.NotFound",
                    "The selected category does not exist.",
                    nameof(command.CategoryId));
            }

            // STEP 4: Only inactive categories can be reactivated.
            if (category.IsActive)
            {
                return Fail(
                    "Category.AlreadyActive",
                    "The category is already active.",
                    nameof(command.CategoryId));
            }

            // STEP 5: Reactivate only this category.
            category.IsActive = true;

            // STEP 6: Update its modification timestamp.
            category.UpdatedAtUtc =
                _timeProvider.GetUtcNow().UtcDateTime;

            // STEP 7: Save the changes to SQL Server.
            await _repository.SaveChangesAsync(
                cancellationToken);

            // STEP 8: Return the new category state.
            return Result<ReactivateCategoryResponse>.Success(
                new ReactivateCategoryResponse(
                    category.CategoryId,
                    category.Name,
                    category.IsActive,
                    category.UpdatedAtUtc));
        }

        private static Result<ReactivateCategoryResponse> Fail(
            string code,
            string message,
            string field)
        {
            return Result<ReactivateCategoryResponse>.Failure(
                new[]
                {
                new Error(code, message, field)
                });
        }
    }
}
