using ECommerce.Domain.Catalog;
using System;
using System.Collections.Generic;
using ProductVariantEntity = ECommerce.Domain.Catalog.ProductVariant;

namespace ECommerce.Application.Catalog.Abstractions
{
    public interface IProductVariantUpdateRepository
    {
        Task<ProductVariantEntity?> GetByIdAsync(
        long productId,
        long variantId,
        CancellationToken cancellationToken = default);

        Task<bool> SkuExistsAsync(
            string sku,
            long excludeVariantId,
            CancellationToken cancellationToken = default);

        Task SaveChangesAsync(
            CancellationToken cancellationToken = default);
    }
}
