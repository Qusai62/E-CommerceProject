using ECommerce.Domain.Catalog;

using ProductEntity = ECommerce.Domain.Catalog.Product;

namespace ECommerce.Application.Catalog.Abstractions
{
    public interface IProductWriteRepository
    {
        Task<bool> BrandExistsAsync(long brandId,CancellationToken cancellationToken = default);

        Task<bool> CategoryExistsAsync(
          long categoryId,
          CancellationToken cancellationToken = default);

        Task<bool> SlugExistsAsync(
            string slug,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            ProductEntity product,
            CancellationToken cancellationToken = default);
    }
}
