using System;
using System.Collections.Generic;
using System.Linq;
using ProductImageEntity = ECommerce.Domain.Catalog.ProductImage;

namespace ECommerce.Application.Catalog.ProductImageApplication.DeleteProductImage
{
    public interface IProductImageDeleteRepository
    {
        Task<ProductImageEntity?> GetByIdAsync(
        long productId,
        long imageId,
        CancellationToken cancellationToken = default);

        Task DeleteAsync(
            ProductImageEntity image,
            CancellationToken cancellationToken = default);
    }
}
