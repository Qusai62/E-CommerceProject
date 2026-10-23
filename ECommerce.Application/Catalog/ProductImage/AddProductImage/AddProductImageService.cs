using ECommerce.Application.Catalog.Abstractions;
using ECommerce.Application.Catalog.Common.Abstractions;
using ECommerce.Application.Common.Results;
using ECommerce.Domain.Catalog;

using Microsoft.Extensions.Logging;
using ProductImageEntity = ECommerce.Domain.Catalog.ProductImage;


namespace ECommerce.Application.Catalog.ProductImage.AddProductImage
{
    public class AddProductImageService
    {
        private readonly IProductImageRepository _repository;
        private readonly IImageStorage _storage;
        private readonly IImageValidator _validator;
        private readonly ILogger<AddProductImageService> _logger;

        public AddProductImageService(
            IProductImageRepository repository,
            IImageStorage storage,
            IImageValidator validator,
            ILogger<AddProductImageService> logger)
        {
            _repository = repository;
            _storage = storage;
            _validator = validator;
            _logger = logger;
        }

        public async Task<Result<AddProductImageResponse>> ExecuteAsync(
            AddProductImageCommand command,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(command);

            // 1. Validate metadata
            var errors = new List<Error>();
            var altText = command.AltText?.Trim();

            if (command.ProductId <= 0)
            {
                errors.Add(new Error(
                    "Product.InvalidId",
                    "ProductId must be positive.",
                    "ProductId"));
            }

            if (command.FileContent is null ||
                command.FileContent == Stream.Null)
            {
                errors.Add(new Error(
                    "Image.FileRequired",
                    "An image file is required.",
                    "File"));
            }

            if (altText?.Length > 250)
            {
                errors.Add(new Error(
                    "Image.AltTextTooLong",
                    "AltText cannot exceed 250 characters.",
                    "AltText"));
            }

            if (command.SortOrder < 0)
            {
                errors.Add(new Error(
                    "Image.InvalidSortOrder",
                    "SortOrder cannot be negative.",
                    "SortOrder"));
            }

            if (errors.Count > 0)
            {
                return Result<AddProductImageResponse>.Failure(errors);
            }

            // 2. Check parent product
            var productExists = await _repository.ProductExistsAsync(
                command.ProductId,
                cancellationToken);

            if (!productExists)
            {
                return Result<AddProductImageResponse>.Failure(
                    new[]
                    {
                    new Error(
                        "Product.NotFound",
                        "The selected product does not exist.",
                        "ProductId")
                    });
            }

            // 3. Validate actual image content
            ValidatedImage validatedImage;

            try
            {
                validatedImage = await _validator.ValidateAsync(
                    command.FileContent,
                    cancellationToken);
            }
            catch (InvalidImageException ex)
            {
                return Result<AddProductImageResponse>.Failure(
                    new[]
                    {
                    new Error(
                        "Image.InvalidFile",
                        ex.Message,
                        "File")
                    });
            }

            using (validatedImage)
            {
                // 4. Determine primary-image status
                var hasImages = await _repository.HasImagesAsync(
                    command.ProductId,
                    cancellationToken);

                var now = DateTime.UtcNow;
                string? storageKey = null;

                // 5. Save the physical file
                storageKey = await _storage.SaveAsync(
                    validatedImage.Content,
                    validatedImage.Extension,
                    cancellationToken);

                // 6. Create database entity
                var image = new ProductImageEntity
                {
                    ProductId = command.ProductId,
                    StorageKey = storageKey,
                    AltText = altText,
                    SortOrder = command.SortOrder,
                    IsPrimary = !hasImages,
                    CreatedAtUtc = now,
                    UpdatedAtUtc = now
                };

                try
                {
                    // 7. Save image metadata
                    await _repository.AddAsync(
                        image,
                        cancellationToken);
                }
                catch (Exception ex)
                {
                    // 8. Compensate for failed SQL insertion
                    try
                    {
                        await _storage.DeleteAsync(
                            storageKey,
                            CancellationToken.None);
                    }
                    catch (Exception cleanupEx)
                    {
                        _logger.LogError(
                            cleanupEx,
                            "Failed to clean up image {StorageKey}",
                            storageKey);
                    }

                    _logger.LogError(
                        ex,
                        "Failed to persist image for product {ProductId}",
                        command.ProductId);

                    throw;
                }

                // 9. Return successful response
                return Result<AddProductImageResponse>.Success(
                    new AddProductImageResponse
                    {
                        ProductImageId = image.ProductImageId,
                        ProductId = image.ProductId,
                        StorageKey = image.StorageKey,
                        AltText = image.AltText,
                        SortOrder = image.SortOrder,
                        IsPrimary = image.IsPrimary,
                        CreatedAtUtc = image.CreatedAtUtc
                    });
            }
        }


    }
}
