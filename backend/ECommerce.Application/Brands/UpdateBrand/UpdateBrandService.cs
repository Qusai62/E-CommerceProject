using ECommerce.Application.Brands.Abstractions;
using ECommerce.Application.Catalog.Common;
using ECommerce.Application.Common.Results;
using ECommerce.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Brands.UpdateBrand
{
    public class UpdateBrandService
    {
        private readonly IBrandRepository _brandRepository;
        private readonly TimeProvider _timeProvider;

        public UpdateBrandService(
            IBrandRepository brandRepository,
            TimeProvider timeProvider)
        {
            _brandRepository = brandRepository;
            _timeProvider = timeProvider;
        }

        public async Task<Result<UpdateBrandResponse>> ExecuteAsync(
            UpdateBrandCommand command,
            CancellationToken cancellationToken = default)
        {
            // Step 1: Validate BrandId
            if (command.BrandId <= 0)
            {
                return Fail(
                    "Brand.InvalidId",
                    "Brand ID must be greater than zero.");
            }

            // Step 2: Validate required name
            if (string.IsNullOrWhiteSpace(command.Name))
            {
                return Fail(
                    "Brand.NameRequired",
                    "Brand name is required.");
            }

            // Step 3: Normalize input
            string name = command.Name.Trim();

            string? description =
                string.IsNullOrWhiteSpace(command.Description)
                    ? null
                    : command.Description.Trim();

            string? logoStorageKey =
                string.IsNullOrWhiteSpace(command.LogoStorageKey)
                    ? null
                    : command.LogoStorageKey.Trim();

            // Step 4: Validate maximum lengths
            if (name.Length > 150)
            {
                return Fail(
                    "Brand.NameTooLong",
                    "Brand name cannot exceed 150 characters.");
            }

            if (description is { Length: > 1000 })
            {
                return Fail(
                    "Brand.DescriptionTooLong",
                    "Description cannot exceed 1000 characters.");
            }

            if (logoStorageKey is { Length: > 500 })
            {
                return Fail(
                    "Brand.LogoKeyTooLong",
                    "Logo storage key cannot exceed 500 characters.");
            }

            // Step 5: Retrieve the existing Brand
            Brand? brand = await _brandRepository.GetByIdAsync(
                command.BrandId,
                cancellationToken);

            if (brand is null)
            {
                return Fail(
                    "Brand.NotFound",
                    "The requested brand does not exist.");
            }

            // Step 6: Check name uniqueness
            bool nameExists =
                await _brandRepository.ExistsByNameAsync(
                    name,
                    command.BrandId,
                    cancellationToken);

            if (nameExists)
            {
                return Fail(
                    "Brand.NameAlreadyExists",
                    "Another brand already uses this name.");
            }

            // Step 7: Generate a slug only if name changed
            bool nameChanged = !string.Equals(
                brand.Name,
                name,
                StringComparison.Ordinal);

            string slug = brand.Slug;

            if (nameChanged)
            {
                slug = SlugGenerator.Generate(name);

                if (string.IsNullOrWhiteSpace(slug) ||
                    slug.Length > 200)
                {
                    return Fail(
                        "Brand.InvalidSlug",
                        "A valid brand slug could not be generated.");
                }

                bool slugExists =
                    await _brandRepository.ExistsBySlugAsync(
                        slug,
                        command.BrandId,
                        cancellationToken);

                if (slugExists)
                {
                    return Fail(
                        "Brand.SlugAlreadyExists",
                        "Another brand already uses this slug.");
                }
            }

            // Step 8: Determine whether anything changed
            bool hasChanges =
                nameChanged ||
                brand.Slug != slug ||
                brand.Description != description ||
                brand.LogoStorageKey != logoStorageKey;

            if (hasChanges)
            {
                // Step 9: Update only allowed properties
                brand.Name = name;
                brand.Slug = slug;
                brand.Description = description;
                brand.LogoStorageKey = logoStorageKey;

                // Do not modify IsActive or Products
                brand.UpdatedAtUtc = _timeProvider
                    .GetUtcNow()
                    .UtcDateTime;

                // Step 10: Persist changes
                await _brandRepository.SaveChangesAsync(
                    cancellationToken);
            }

            // Step 11: Return updated Brand
            var response = new UpdateBrandResponse(
                brand.BrandId,
                brand.Name,
                brand.Slug,
                brand.Description,
                brand.LogoStorageKey,
                brand.IsActive,
                brand.UpdatedAtUtc);

            return Result<UpdateBrandResponse>.Success(response);
        }

        private static Result<UpdateBrandResponse> Fail(
            string code,
            string message)
        {
            return Result<UpdateBrandResponse>.Failure(
                new[]
                {
                new Error(code, message)
                });
        }
    }
}
