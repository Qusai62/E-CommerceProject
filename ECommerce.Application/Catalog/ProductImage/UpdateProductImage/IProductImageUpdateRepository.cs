using System;
using ECommerce.Domain.Catalog;
using ProductImageEntity = ECommerce.Domain.Catalog.ProductImage;

namespace ECommerce.Application.Catalog.ProductImage.UpdateProductImage
{
    public interface IProductImageUpdateRepository
    {
        public interface IProductImageUpdateRepository
        {
            Task<ProductImageEntity?> GetByIdAsync(
                long productId,
                long imageId,
                CancellationToken cancellationToken = default);

            Task<List<ProductImageEntity>> GetPrimaryImagesAsync(
                long productId,
                long excludeImageId,
                CancellationToken cancellationToken = default);

            Task SaveChangesAsync(
                CancellationToken cancellationToken = default);
        }
    }
}

