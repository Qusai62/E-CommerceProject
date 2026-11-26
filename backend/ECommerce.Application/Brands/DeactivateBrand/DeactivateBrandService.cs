using ECommerce.Application.Brands.Abstractions;
using ECommerce.Application.Common.Results;
using ECommerce.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Brands.DeactivateBrand
{
    public sealed class DeactivateBrandService
    {
        private readonly IBrandRepository _brandRepository;
        private readonly TimeProvider _timeProvider;

        public DeactivateBrandService(
            IBrandRepository brandRepository,
            TimeProvider timeProvider)
        {
            _brandRepository = brandRepository;
            _timeProvider = timeProvider;
        }

        public async Task<Result<DeactivateBrandResponse>> ExecuteAsync(
            DeactivateBrandCommand command,
            CancellationToken cancellationToken = default)
        {
            // Step 1: Validate BrandId
            if (command.BrandId <= 0)
            {
                return Fail(
                    "Brand.InvalidId",
                    "Brand ID must be greater than zero.");
            }

            // Step 2: Retrieve the Brand
            Brand? brand = await _brandRepository.GetByIdAsync(
                command.BrandId,
                cancellationToken);

            if (brand is null)
            {
                return Fail(
                    "Brand.NotFound",
                    "The requested brand does not exist.");
            }

            // Step 3: Only active brands can be deactivated
            if (!brand.IsActive)
            {
                return Fail(
                    "Brand.AlreadyInactive",
                    "The brand is already inactive.");
            }

            // Step 4: Change only Brand state
            brand.IsActive = false;

            DateTime now = _timeProvider.GetUtcNow().UtcDateTime;

            // Step 6: Persist changes
            bool deactivated = await _brandRepository.TryDeactivateAsync(
     brand.BrandId,
     now,
     cancellationToken);

            if (!deactivated)
            {
                return Fail(
                    "Brand.AlreadyInactive",
                    "The brand is no longer active.");
            }

            // Step 7: Build response
            var response = new DeactivateBrandResponse(
                brand.BrandId,
                brand.Name,
                brand.IsActive,
                brand.UpdatedAtUtc);

            return Result<DeactivateBrandResponse>.Success(response);
        }

        private static Result<DeactivateBrandResponse> Fail(
            string code,
            string message)
        {
            return Result<DeactivateBrandResponse>.Failure(
                new[]
                {
                new Error(code, message)
                });
        }
    }
}
