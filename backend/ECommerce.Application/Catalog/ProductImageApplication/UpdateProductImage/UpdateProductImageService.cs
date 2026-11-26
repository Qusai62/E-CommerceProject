using ECommerce.Application.Common.Results;
using System;
using System.Collections.Generic;


namespace ECommerce.Application.Catalog.ProductImage.UpdateProductImage
{
    public class UpdateProductImageService
    {
        private readonly IProductImageUpdateRepository _repository;

        public UpdateProductImageService(
            IProductImageUpdateRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<UpdateProductImageResponse>> ExecuteAsync(
            UpdateProductImageCommand command,
            CancellationToken cancellationToken = default)
        {
            var errors = new List<Error>();

            // 1. Validate ProductId
            if (command.ProductId <= 0)
            {
                errors.Add(new Error(
                    "Product.InvalidId",
                    "ProductId must be greater than zero.",
                    nameof(command.ProductId)));
            }

            // 2. Validate ProductImageId
            if (command.ProductImageId <= 0)
            {
                errors.Add(new Error(
                    "Image.InvalidId",
                    "ProductImageId must be greater than zero.",
                    nameof(command.ProductImageId)));
            }

            // 3. Normalize AltText
            var altText = string.IsNullOrWhiteSpace(command.AltText)
                ? null
                : command.AltText.Trim();

            // 4. Validate AltText
            if (altText is not null && altText.Length > 250)
            {
                errors.Add(new Error(
                    "Image.AltTextTooLong",
                    "AltText cannot exceed 250 characters.",
                    nameof(command.AltText)));
            }

            // 5. Validate SortOrder
            if (command.SortOrder < 0)
            {
                errors.Add(new Error(
                    "Image.InvalidSortOrder",
                    "SortOrder cannot be negative.",
                    nameof(command.SortOrder)));
            }

            // 6. Return validation errors
            if (errors.Count > 0)
            {
                return Result<UpdateProductImageResponse>
                    .Failure(errors);
            }

            // 7. Get existing image (tracked)
            var image = await _repository.GetByIdAsync(
                command.ProductId,
                command.ProductImageId,
                cancellationToken);

            if (image is null)
            {
                return Result<UpdateProductImageResponse>.Failure(
                    new List<Error>
                    {
                    new Error(
                        "Image.NotFound",
                        "Product image was not found.",
                        nameof(command.ProductImageId))
                    });
            }

            // 8. Check whether anything changed
            var hasChanges =
                image.AltText != altText ||
                image.SortOrder != command.SortOrder ||
                image.IsPrimary != command.IsPrimary;

            if (hasChanges)
            {
                // 9. Update editable fields
                image.AltText = altText;
                image.SortOrder = command.SortOrder;
                image.IsPrimary = command.IsPrimary;
                image.UpdatedAtUtc = DateTime.UtcNow;

                // 10. Persist update
                if (command.IsPrimary)
                {
                    await _repository.SaveWithPrimarySwitchAsync(
                        image.ProductId,
                        image.ProductImageId,
                        cancellationToken);
                }
                else
                {
                    await _repository.SaveChangesAsync(
                        cancellationToken);
                }
            }

            // 11. Build response
            var response = new UpdateProductImageResponse
            {
                ProductImageId = image.ProductImageId,
                ProductId = image.ProductId,
                StorageKey = image.StorageKey,
                AltText = image.AltText,
                SortOrder = image.SortOrder,
                IsPrimary = image.IsPrimary,
                UpdatedAtUtc = image.UpdatedAtUtc
            };

            return Result<UpdateProductImageResponse>
                .Success(response);
        }
    }
}
