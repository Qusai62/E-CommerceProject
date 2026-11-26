
using Microsoft.EntityFrameworkCore;
using System;
using ECommerce.Application.Catalog.ProductImage.UpdateProductImage;
using ProductImageEntity = ECommerce.Domain.Catalog.ProductImage;

namespace ECommerce.Infrastructure.Persistence.Repositories
{
    public class ProductImageUpdateRepository : IProductImageUpdateRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductImageUpdateRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<ProductImageEntity?> GetByIdAsync(
            long productId,
            long imageId,
            CancellationToken cancellationToken = default)
        {
            return _context.ProductImages
                .FirstOrDefaultAsync(
                    image => image.ProductId == productId
                          && image.ProductImageId == imageId,
                    cancellationToken);
        }
        public async Task SaveChangesAsync(
    CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        public Task<List<ProductImageEntity>> GetPrimaryImagesAsync(
            long productId,
            long excludeImageId,
            CancellationToken cancellationToken = default)
        {
            return _context.ProductImages
                .Where(image =>
                    image.ProductId == productId &&
                    image.ProductImageId != excludeImageId &&
                    image.IsPrimary)
                .ToListAsync(cancellationToken);
        }

        public async Task SaveWithPrimarySwitchAsync(
     long productId,
     long imageId,
     CancellationToken cancellationToken = default)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    System.Data.IsolationLevel.Serializable,
                    cancellationToken);

            // 1. Unmark other primary images for this product
            await _context.ProductImages
                .Where(image =>
                    image.ProductId == productId &&
                    image.ProductImageId != imageId &&
                    image.IsPrimary)
                .ExecuteUpdateAsync(
                    setters => setters
                        .SetProperty(image => image.IsPrimary, false)//For every database row matching our query, set IsPrimary to false.
                        .SetProperty(
                            image => image.UpdatedAtUtc,
                            DateTime.UtcNow),
                    cancellationToken);

            // 2. Persist the changes to the selected image
            await _context.SaveChangesAsync(cancellationToken);

            // 3. Commit both operations together
            await transaction.CommitAsync(cancellationToken);
        }
    }
}
