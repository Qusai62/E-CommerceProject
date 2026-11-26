using ECommerce.Application.InStoreSales.VoidInStoreSale;
using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Repositories.InStoreSales
{
    public sealed class VoidInStoreSaleRepository
    : IVoidInStoreSaleRepository
    {
        private readonly ApplicationDbContext _context;

        public VoidInStoreSaleRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> TryVoidAsync(
            long inStoreSaleId,
            DateTime voidedAtUtc,
            CancellationToken cancellationToken = default)
        {
            int affectedRows = await _context.InStoreSales
                .Where(s =>
                    s.InStoreSaleId == inStoreSaleId &&
                    s.Status == InStoreSaleStatus.Completed)
                .ExecuteUpdateAsync(
                    setters => setters
                        .SetProperty(
                            s => s.Status,
                            InStoreSaleStatus.Voided)
                        .SetProperty(
                            s => s.VoidedAtUtc,
                            (DateTime?)voidedAtUtc),
                    cancellationToken);

            return affectedRows == 1;
        }

        public async Task<InStoreSaleStatus?> GetStatusAsync(
            long inStoreSaleId,
            CancellationToken cancellationToken = default)
        {
            return await _context.InStoreSales
                .AsNoTracking()
                .Where(s => s.InStoreSaleId == inStoreSaleId)
                .Select(s => (InStoreSaleStatus?)s.Status)
                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}
