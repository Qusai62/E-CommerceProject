using System.Text;
using ECommerce.Application.Catalog.Abstractions;
using ECommerce.Application.Common.Results;
using ECommerce.Domain.Catalog;
using ProductEntity = ECommerce.Domain.Catalog.Product;

namespace ECommerce.Application.Catalog.Product.CreateProduct
{
    public sealed class CreateProductService
    {
        private readonly IProductWriteRepository _repository;

        public CreateProductService(IProductWriteRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<CreateProductResponse>> ExecuteAsync(
            CreateProductCommand command,
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
                return Result<CreateProductResponse>.Failure(errors);

            // 3. Validate database references
            if (command.BrandId.HasValue)
            {
                var brandExists = await _repository.BrandExistsAsync(
                    command.BrandId.Value,
                    cancellationToken);

                if (!brandExists)
                {
                    errors.Add(new Error(
                        "Brand.NotFound",
                        "The selected brand does not exist.",
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
                return Result<CreateProductResponse>.Failure(errors);

            // 4. Generate slug
            // 4. Generate the base slug
            var baseSlug = GenerateSlug(name!);

            const int maxAttempts = 3;

            for (var attempt = 0; attempt < maxAttempts; attempt++)
            {
                // Find an available slug
                var slug = baseSlug;
                var suffix = 2;

                while (await _repository.SlugExistsAsync(
                    slug,
                    cancellationToken))
                {
                    var suffixText = $"-{suffix}";

                    slug = baseSlug.Length + suffixText.Length <= 200
                        ? baseSlug + suffixText
                        : baseSlug[..(200 - suffixText.Length)]
                            .TrimEnd('-') + suffixText;

                    suffix++;
                }

                // 5. Create a fresh Product for this attempt
                var nowUtc = DateTime.UtcNow;

                var product = new ProductEntity
                {
                    Name = name!,
                    Description = description,
                    BrandId = command.BrandId,
                    CategoryId = command.CategoryId,
                    Slug = slug,
                    IsActive = false,
                    CreatedAtUtc = nowUtc,
                    UpdatedAtUtc = nowUtc
                };

                try
                {
                    // 6. Attempt to save the product
                    await _repository.AddAsync(
                        product,
                        cancellationToken);

                    // 7. Build the successful response
                    var response = new CreateProductResponse
                    {
                        ProductId = product.ProductId,
                        Name = product.Name,
                        Slug = product.Slug,
                        IsActive = product.IsActive,
                        CreatedAtUtc = product.CreatedAtUtc
                    };

                    return Result<CreateProductResponse>.Success(response);
                }
                catch (ProductSlugConflictException)
                    when (attempt < maxAttempts - 1)
                {
                    // Another request inserted the slug first.
                    // Retry with a fresh Product and a new slug.
                }
            }

            // All attempts failed due to slug conflicts.
            return Result<CreateProductResponse>.Failure(
                new[]
                {
        new Error(
            "Product.SlugConflict",
            "Unable to generate a unique product slug. Please retry.",
            "Name")
                });
        }

        private static string GenerateSlug(string name)
        {
            var builder = new StringBuilder();//class that allows us to construct a string by appending characters efficiently.
            var previousWasSeparator = false;

            foreach (var character in name.ToLowerInvariant())
            {
                if (character is >= 'a' and <= 'z'
                    or >= '0' and <= '9')
                {
                    builder.Append(character);
                    previousWasSeparator = false;
                }
                else if (!previousWasSeparator && builder.Length > 0)
                {
                    builder.Append('-');
                    previousWasSeparator = true;
                }
            }

            var slug = builder.ToString().TrimEnd('-');

            if (string.IsNullOrEmpty(slug))
                slug = "product";

            return slug.Length <= 200
                ? slug
                : slug[..200].TrimEnd('-');
        }
    }
}