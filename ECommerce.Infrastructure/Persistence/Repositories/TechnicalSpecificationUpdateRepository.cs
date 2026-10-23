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
    public class TechnicalSpecificationUpdateRepository :ITechnicalSpecificationUpdateRepository
    {
        private readonly ApplicationDbContext _context;

        public TechnicalSpecificationUpdateRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TechnicalSpecification?> GetByIdAsync(
            long productId,
            long specificationId,
            CancellationToken cancellationToken = default)
        {
            return await _context.TechnicalSpecifications
                .SingleOrDefaultAsync(
                    s => s.ProductId == productId &&
                         s.TechnicalSpecificationId == specificationId,
                    cancellationToken);
        }

        public async Task<bool> SpecificationNameExistsAsync(
            long productId,
            string name,
            long excludedSpecificationId,
            CancellationToken cancellationToken = default)
        {
            return await _context.TechnicalSpecifications
                .AnyAsync(
                    s => s.ProductId == productId &&
                         s.Name == name &&
                         s.TechnicalSpecificationId != excludedSpecificationId,
                    cancellationToken);
        }

        public async Task SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
