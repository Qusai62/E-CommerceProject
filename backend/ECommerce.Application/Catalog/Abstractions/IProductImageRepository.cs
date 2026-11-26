using ECommerce.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductImageEntity = ECommerce.Domain.Catalog.ProductImage;

namespace ECommerce.Application.Catalog.Abstractions
{
    public interface IProductImageRepository
    {
        Task<bool> ProductExistsAsync(
        long productId,
        CancellationToken cancellationToken = default);

        Task<bool> HasImagesAsync(
            long productId,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            ProductImageEntity image,
            CancellationToken cancellationToken = default);
    }
}
