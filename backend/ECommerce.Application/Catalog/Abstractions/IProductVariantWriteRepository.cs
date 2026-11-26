using ECommerce.Domain.Catalog;
using System;
using System.Collections.Generic;
using ProductVariantEntity = ECommerce.Domain.Catalog.ProductVariant;


namespace ECommerce.Application.Catalog.Abstractions
{
    public interface IProductVariantWriteRepository
    {
        Task<bool> ProductExistsAsync(
        long productId,
        CancellationToken cancellationToken = default);

        Task<bool> SkuExistsAsync(
            string sku,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            ProductVariantEntity variant,
            CancellationToken cancellationToken = default);
    }
}
