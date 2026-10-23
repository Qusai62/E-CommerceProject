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
    public class ProductImageRepository : IProductImageRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductImageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ProductExistsAsync(
            long productId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .AnyAsync(p => p.ProductId == productId,
                    cancellationToken);
        }

        public async Task<bool> HasImagesAsync(
            long productId,
            CancellationToken cancellationToken = default)
        {
            return await _context.ProductImages
                .AnyAsync(i => i.ProductId == productId,
                    cancellationToken);
        }

        public async Task AddAsync(
            ProductImage image,
            CancellationToken cancellationToken = default)
        {
            _context.ProductImages.Add(image);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
