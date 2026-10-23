using ECommerce.Application.Catalog.Abstractions;
using ECommerce.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Repositories
{
    public class ProductVariantUpdateRepository: IProductVariantUpdateRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductVariantUpdateRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<ProductVariant?> GetByIdAsync(
            long productId,
            long variantId,
            CancellationToken cancellationToken = default)
        {
            return _context.ProductVariants
                .FirstOrDefaultAsync(
                    v => v.ProductVariantId == variantId
                      && v.ProductId == productId,
                    cancellationToken);
        }

        public Task<bool> SkuExistsAsync(
            string sku,
            long excludeVariantId,
            CancellationToken cancellationToken = default)
        {
            return _context.ProductVariants
                .AnyAsync(
                    v => v.Sku == sku
                      && v.ProductVariantId != excludeVariantId,
                    cancellationToken);
        }

        public async Task SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
