using ECommerce.Application.Catalog.Common_repository_contract;
using ECommerce.Application.Common.Results;
using System;
using System.Collections.Generic;
using ECommerce.Application.Catalog.Response;

namespace ECommerce.Application.Catalog.DeactivateProduct
{
    public class DeactivateProductService
    {
        private readonly IProductActivationRepository _repository;

        public DeactivateProductService(
            IProductActivationRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<ProductActivationResponse>> ExecuteAsync(
            DeactivateProductCommand command,
            CancellationToken cancellationToken = default)
        {
            // 1. Validate ProductId
            if (command.ProductId <= 0)
            {
                return Result<ProductActivationResponse>.Failure(
                    new List<Error>
                    {
                    new Error(
                        "InvalidProductId",
                        "ProductId must be greater than zero.",
                        "ProductId")
                    });
            }

            // 2. Retrieve the tracked Product
            var product = await _repository.GetForUpdateAsync(
                command.ProductId,
                cancellationToken);

            // 3. Check if the Product exists
            if (product is null)
            {
                return Result<ProductActivationResponse>.Failure(
                    new List<Error>
                    {
                    new Error(
                        "ProductNotFound",
                        "Product was not found.",
                        "ProductId")
                    });
            }

            // 4. Check the current state
            if (!product.IsActive)
            {
                return Result<ProductActivationResponse>.Failure(
                    new List<Error>
                    {
                    new Error(
                        "ProductAlreadyInactive",
                        "Product is already inactive.",
                        "IsActive")
                    });
            }

            // 5. Change only the Product state
            product.IsActive = false;
            product.UpdatedAtUtc = DateTime.UtcNow;

            // 6. Persist the changes
            await _repository.SaveChangesAsync(cancellationToken);

            // 7. Return the updated state
            return Result<ProductActivationResponse>.Success(
                new ProductActivationResponse
                {
                    ProductId = product.ProductId,
                    IsActive = product.IsActive,
                    UpdatedAtUtc = product.UpdatedAtUtc
                });
        }
    }
}