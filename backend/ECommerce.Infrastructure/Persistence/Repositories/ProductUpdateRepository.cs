using ECommerce.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using System;
using ECommerce.Application.Catalog.Abstractions;

namespace ECommerce.Infrastructure.Persistence.Repositories
{
    public class ProductUpdateRepository : IProductUpdateRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductUpdateRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Product?> GetByIdAsync(
         long productId,
         CancellationToken cancellationToken = default)
        {
            return await _context.Products.FindAsync(
       new object[] { productId },
       cancellationToken);
        }

        public Task<bool> BrandExistsAsync(
            long brandId,
            CancellationToken cancellationToken = default)
        {
            return _context.Brands
                .AnyAsync(
                    b => b.BrandId == brandId,
                    cancellationToken);
        }

        public Task<bool> CategoryExistsAsync(
            long categoryId,
            CancellationToken cancellationToken = default)
        {
            return _context.Categories
                .AnyAsync(
                    c => c.CategoryId == categoryId,
                    cancellationToken);
        }

        public Task<bool> SlugExistsAsync(
            string slug,
            long excludedProductId,
            CancellationToken cancellationToken = default)
        {
            return _context.Products
                .AnyAsync(
                    p => p.Slug == slug &&
                         p.ProductId != excludedProductId,
                    cancellationToken);
        }

        public async Task SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
