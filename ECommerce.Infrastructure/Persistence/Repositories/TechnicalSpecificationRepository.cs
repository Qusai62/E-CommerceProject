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
    public class TechnicalSpecificationRepository : ITechnicalSpecificationRepository
    {
        private readonly ApplicationDbContext _context;

        public TechnicalSpecificationRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ProductExistsAsync(
            long productId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .AnyAsync(
                    p => p.ProductId == productId,
                    cancellationToken);
        }

        public async Task<bool> SpecificationNameExistsAsync(
            long productId,
            string name,
            CancellationToken cancellationToken = default)
        {
            return await _context.TechnicalSpecifications
                .AnyAsync(
                    s => s.ProductId == productId &&
                         s.Name == name,
                    cancellationToken);
        }

        public async Task AddAsync(
            TechnicalSpecification specification,
            CancellationToken cancellationToken = default)
        {
            _context.TechnicalSpecifications.Add(specification);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
