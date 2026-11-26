using ECommerce.Application.Catalog.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Repositories
{
    public class TechnicalSpecificationDeleteRepository: ITechnicalSpecificationDeleteRepository
    {
        private readonly ApplicationDbContext _context;

        public TechnicalSpecificationDeleteRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteAsync(
            long productId,
            long specificationId,
            CancellationToken cancellationToken = default)
        {
            var affectedRows = await _context.TechnicalSpecifications
                .Where(s =>
                    s.ProductId == productId &&
                    s.TechnicalSpecificationId == specificationId)
                .ExecuteDeleteAsync(cancellationToken);

            return affectedRows == 1;
        }
    }
}
