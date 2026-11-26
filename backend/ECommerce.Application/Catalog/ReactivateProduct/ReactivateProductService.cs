using ECommerce.Application.Catalog.Common_repository_contract;
using ECommerce.Application.Common.Results;
using System;
using System.Collections.Generic;
using ECommerce.Application.Catalog.Response;

namespace ECommerce.Application.Catalog.ReactivateProduct
{
    public class ReactivateProductService
    {
        private readonly IProductActivationRepository _repository;

        public ReactivateProductService(
            IProductActivationRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<ProductActivationResponse>> ExecuteAsync(
            ReactivateProductCommand command,
            CancellationToken cancellationToken = default)
        {
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

            var product = await _repository.GetForUpdateAsync(
                command.ProductId,
                cancellationToken);

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

            // BR-372: Only inactive products may be reactivated
            if (product.IsActive)
            {
                return Result<ProductActivationResponse>.Failure(
                    new List<Error>
                    {
                    new Error(
                        "ProductAlreadyActive",
                        "Product is already active.",
                        "IsActive")
                    });
            }

            // BR-373: Validate core information
            if (string.IsNullOrWhiteSpace(product.Name) ||
                string.IsNullOrWhiteSpace(product.Slug))
            {
                return Result<ProductActivationResponse>.Failure(
                    new List<Error>
                    {
                    new Error(
                        "InvalidProductInformation",
                        "Product name and slug are required.",
                        "Product")
                    });
            }

            var hasValidReferences =
                await _repository.HasValidCatalogReferencesAsync(
                    product.ProductId,
                    cancellationToken);

            if (!hasValidReferences)
            {
                return Result<ProductActivationResponse>.Failure(
                    new List<Error>
                    {
                    new Error(
                        "InvalidCatalogReferences",
                        "Product requires an active Brand and Category.",
                        "Product")
                    });
            }

            product.IsActive = true;
            product.UpdatedAtUtc = DateTime.UtcNow;

            await _repository.SaveChangesAsync(cancellationToken);

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
