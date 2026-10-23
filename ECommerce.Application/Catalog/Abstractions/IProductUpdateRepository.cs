using ECommerce.Domain.Catalog;
using System;
using System.Collections.Generic;
using ProductEntity = ECommerce.Domain.Catalog.Product;

namespace ECommerce.Application.Catalog.Abstractions
{
    public interface IProductUpdateRepository
    {
        Task<ProductEntity?> GetByIdAsync(
       long productId,
       CancellationToken cancellationToken = default);

        Task<bool> BrandExistsAsync(
            long brandId,
            CancellationToken cancellationToken = default);

        Task<bool> CategoryExistsAsync(
            long categoryId,
            CancellationToken cancellationToken = default);

        Task<bool> SlugExistsAsync(
            string slug,
            long excludedProductId,
            CancellationToken cancellationToken = default);

        Task SaveChangesAsync(
            CancellationToken cancellationToken = default);
    }
}
