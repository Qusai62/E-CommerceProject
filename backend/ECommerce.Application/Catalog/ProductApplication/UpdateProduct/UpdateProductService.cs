using ECommerce.Application.Catalog.Abstractions;
using ECommerce.Application.Common.Results;
using ECommerce.Domain.Catalog;
using ECommerce.Application.Catalog.Common;

namespace ECommerce.Application.Catalog.Product.UpdateProduct
{
    public class UpdateProductService 
    {
        private readonly IProductUpdateRepository _repository;

        public UpdateProductService(
            IProductUpdateRepository repository)
        {
            _repository = repository;
        }
        public async Task<Result<UpdateProductResponse>> ExecuteAsync(
           UpdateProductCommand command,
           CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(command);

            // 1. Normalize input
            var name = command.Name?.Trim();
            var description = command.Description?.Trim();

            if (string.IsNullOrWhiteSpace(description))
                description = null;

            // 2. Validate basic information
            var errors = new List<Error>();

            if (command.ProductId <= 0)
            {
                errors.Add(new Error(
                    "Product.InvalidId",
                    "ProductId must be positive.",
                    "ProductId"));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                errors.Add(new Error(
                    "Product.NameRequired",
                    "Product name is required.",
                    "Name"));
            }
            else if (name.Length > 200)
            {
                errors.Add(new Error(
                    "Product.NameTooLong",
                    "Product name cannot exceed 200 characters.",
                    "Name"));
            }

            if (description is not null && description.Length > 2000)
            {
                errors.Add(new Error(
                    "Product.DescriptionTooLong",
                    "Product description cannot exceed 2000 characters.",
                    "Description"));
            }

            if (command.BrandId.HasValue && command.BrandId.Value <= 0)
            {
                errors.Add(new Error(
                    "Brand.InvalidId",
                    "BrandId must be positive.",
                    "BrandId"));
            }

            if (command.CategoryId.HasValue && command.CategoryId.Value <= 0)
            {
                errors.Add(new Error(
                    "Category.InvalidId",
                    "CategoryId must be positive.",
                    "CategoryId"));
            }

            if (errors.Count > 0)
                return Result<UpdateProductResponse>.Failure(errors);

            // 3. Find existing product
            var product = await _repository.GetByIdAsync(
                command.ProductId,
                cancellationToken);

            if (product is null)
            {
                return Result<UpdateProductResponse>.Failure(
                    new[]
                    {
                        new Error(
                            "Product.NotFound",
                            "The selected product does not exist.",
                            "ProductId")
                    });
            }

            // 4. Validate database references
            if (command.BrandId.HasValue)
            {
                var brandExists = await _repository.BrandExistsAsync(
                    command.BrandId.Value,
                    cancellationToken);

                if (!brandExists)
                {
                    errors.Add(new Error(
                        "Brand.NotFound",
                        "The selected brand does not exist or inactive.",
                        "BrandId"));
                }
            }

            if (command.CategoryId.HasValue)
            {
                var categoryExists = await _repository.CategoryExistsAsync(
                    command.CategoryId.Value,
                    cancellationToken);

                if (!categoryExists)
                {
                    errors.Add(new Error(
                        "Category.NotFound",
                        "The selected category does not exist.",
                        "CategoryId"));
                }
            }

            if (errors.Count > 0)
                return Result<UpdateProductResponse>.Failure(errors);

            // 5. Generate a new slug only when the name changes
            if (!string.Equals(
                product.Name,
                name,
                StringComparison.Ordinal))
            {
                var baseSlug = SlugGenerator.Generate(name!);
                var slug = baseSlug;
                var suffix = 2;

                while (await _repository.SlugExistsAsync(
                    slug,
                    product.ProductId,
                    cancellationToken))
                {
                    var suffixText = $"-{suffix}";

                    slug = baseSlug.Length + suffixText.Length <= 200
                        ? baseSlug + suffixText
                        : baseSlug[..(200 - suffixText.Length)]
                            .TrimEnd('-') + suffixText;

                    suffix++;
                }

                product.Slug = slug;
            }

            // 6. Update existing product
            product.Name = name!;
            product.Description = description;
            product.BrandId = command.BrandId;
            product.CategoryId = command.CategoryId;
            product.UpdatedAtUtc = DateTime.UtcNow;

            // 7. Persist changes
            await _repository.SaveChangesAsync(cancellationToken);

            // 8. Build the response
            var response = new UpdateProductResponse
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Slug = product.Slug,
                Description = product.Description,
                BrandId = product.BrandId,
                CategoryId = product.CategoryId,
                UpdatedAtUtc = product.UpdatedAtUtc
            };

            return Result<UpdateProductResponse>.Success(response);
        }
    }

}

