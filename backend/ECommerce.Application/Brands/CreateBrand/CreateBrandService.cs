using ECommerce.Application.Brands.Abstractions;
using ECommerce.Application.Catalog.Common;
using ECommerce.Application.Common.Results;
using ECommerce.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Brands.CreateBrand
{
    public sealed class CreateBrandService
    {
        private readonly IBrandRepository _brandRepository;
        
        private readonly TimeProvider _timeProvider;

        public CreateBrandService(
            IBrandRepository brandRepository,
            TimeProvider timeProvider)
        {
            _brandRepository = brandRepository;
            _timeProvider = timeProvider;
        }

        public async Task<Result<CreateBrandResponse>> ExecuteAsync(
            CreateBrandCommand command,
            CancellationToken cancellationToken = default)
        {
            // Step 1: Validate name
            if (string.IsNullOrWhiteSpace(command.Name))
            {
                return Fail(
                    "Brand.NameRequired",
                    "Brand name is required.");
            }

            // Step 2: Normalize input
            string name = command.Name.Trim();

            string? description =
                string.IsNullOrWhiteSpace(command.Description)
                    ? null
                    : command.Description.Trim();

            string? logoStorageKey =
                string.IsNullOrWhiteSpace(command.LogoStorageKey)
                    ? null
                    : command.LogoStorageKey.Trim();

            // Step 3: Validate maximum lengths
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

            // Step 4: Check duplicate name
            bool nameExists =
                await _brandRepository.ExistsByNameAsync(
                    name,
                    cancellationToken);

            if (nameExists)
            {
                return Fail(
                    "Brand.NameAlreadyExists",
                    "A brand with this name already exists.");
            }

            // Step 5: Generate slug
            string slug = SlugGenerator.Generate(name);

            if (string.IsNullOrWhiteSpace(slug) ||
                slug.Length > 200)
            {
                return Fail(
                    "Brand.InvalidSlug",
                    "A valid slug could not be generated.");
            }

            bool slugExists =
                await _brandRepository.ExistsBySlugAsync(
                    slug,
                    cancellationToken);

            if (slugExists)
            {
                return Fail(
                    "Brand.SlugAlreadyExists",
                    "The generated brand slug already exists.");
            }

            // Step 6: Create domain entity
            DateTime now = _timeProvider
                .GetUtcNow()
                .UtcDateTime;

            var brand = new Brand
            {
                Name = name,
                Slug = slug,
                Description = description,
                LogoStorageKey = logoStorageKey,
                IsActive = false,
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            };

            // Step 7: Persist the brand
            await _brandRepository.AddAsync(
                brand,
                cancellationToken);

            await _brandRepository.SaveChangesAsync(
                cancellationToken);

            // Step 8: Create response
            var response = new CreateBrandResponse(
                brand.BrandId,
                brand.Name,
                brand.Slug,
                brand.Description,
                brand.LogoStorageKey,
                brand.IsActive,
                brand.CreatedAtUtc);

            return Result<CreateBrandResponse>.Success(response);
        }

        private static Result<CreateBrandResponse> Fail(
            string code,
            string message)
        {
            return Result<CreateBrandResponse>.Failure(
                new[]
                {
                new Error(code, message)
                });
        }
    }
}
