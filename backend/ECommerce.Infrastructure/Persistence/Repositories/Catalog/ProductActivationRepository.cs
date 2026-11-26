using ECommerce.Application.Catalog.Common_repository_contract;
using ECommerce.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Repositories.Catalog
{
    public class ProductActivationRepository:IProductActivationRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductActivationRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<Product?> GetForUpdateAsync(
            long productId,
            CancellationToken cancellationToken = default)
        {
            return _context.Products
                .FirstOrDefaultAsync(
                    p => p.ProductId == productId,
                    cancellationToken);
        }


        public Task<bool> HasValidCatalogReferencesAsync(
            long productId,
            CancellationToken cancellationToken = default)
        {
            return _context.Products
                .AsNoTracking()
                .AnyAsync(p =>
                    p.ProductId == productId &&
                    p.BrandId != null &&
                    p.Brand != null &&
                    p.Brand.IsActive &&
                    p.CategoryId != null &&
                    p.Category != null &&
                    p.Category.IsActive,
                    cancellationToken);
        }


        public async Task SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

    }
}
