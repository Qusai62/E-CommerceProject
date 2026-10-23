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
    public class ProductVariantWriteRepository : IProductVariantWriteRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductVariantWriteRepository(
       ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ProductVariant variant, CancellationToken cancellationToken = default)
        {
            _context.ProductVariants.Add(variant);

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> ProductExistsAsync(long productId, CancellationToken cancellationToken = default)
        {
            return await _context.Products
           .AnyAsync(
               p => p.ProductId == productId,
               cancellationToken);
        }

        public async Task<bool> SkuExistsAsync(string sku, CancellationToken cancellationToken = default)
        {
            return await _context.ProductVariants
            .AnyAsync(
                v => v.Sku == sku,
                cancellationToken);
        }
    }
}
