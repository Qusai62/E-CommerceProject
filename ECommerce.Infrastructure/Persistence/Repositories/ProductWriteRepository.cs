using ECommerce.Application.Catalog.Abstractions;
using ECommerce.Domain.Catalog;
using ECommerce.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistence.Repositories
{
    public class ProductWriteRepository : IProductWriteRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductWriteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<bool> BrandExistsAsync(
            long brandId,
            CancellationToken cancellationToken = default)
        {
            return _context.Brands
                .AnyAsync(b => b.BrandId == brandId && b.IsActive, cancellationToken);
        }

        public Task<bool> CategoryExistsAsync(
            long categoryId,
            CancellationToken cancellationToken = default)
        {
            return _context.Categories
                .AnyAsync(c => c.CategoryId == categoryId, cancellationToken);
        }

        public Task<bool> SlugExistsAsync(
            string slug,
            CancellationToken cancellationToken = default)
        {
            return _context.Products
                .AnyAsync(p => p.Slug == slug, cancellationToken);
        }

        public async Task AddAsync(
    Product product,
    CancellationToken cancellationToken = default)
        {
            _context.Products.Add(product);

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex)
                when (ex.InnerException is SqlException sqlEx
                      && sqlEx.Number is 2601 or 2627)
            {
                _context.Entry(product).State = EntityState.Detached;

                var slugExists = await _context.Products
                    .AsNoTracking()
                    .AnyAsync(
                        p => p.Slug == product.Slug,
                        cancellationToken);

                if (slugExists)
                {
                    throw new ProductSlugConflictException(
                        product.Slug,
                        ex);
                }

                throw;
            }
        }
    }
}