using ECommerce.Application.InStoreSales.UpdateInStoreSale;
using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Repositories.InStoreSales
{
    public sealed class UpdateInStoreSaleRepository
     : IUpdateInStoreSaleRepository
    {
        private readonly ApplicationDbContext _context;

        public UpdateInStoreSaleRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        // Step 1: Load the sale and all its items for editing.
        public async Task<InStoreSale?> GetByIdForUpdateAsync(
            long inStoreSaleId,
            CancellationToken cancellationToken = default)
        {
            return await _context.InStoreSales
                .Include(s => s.Items)
                .SingleOrDefaultAsync(
                    s => s.InStoreSaleId == inStoreSaleId,
                    cancellationToken);
        }

        // Step 2: Save with optimistic concurrency checking.
        public async Task<bool> TrySaveAsync(
            InStoreSale sale,
            byte[] expectedRowVersion,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(sale);
            ArgumentNullException.ThrowIfNull(expectedRowVersion);

            if (expectedRowVersion.Length != 8)
            {
                throw new ArgumentException(
                    "Invalid row version.",
                    nameof(expectedRowVersion));
            }

            var entry = _context.Entry(sale);

            if (entry.State == EntityState.Detached)
            {
                throw new InvalidOperationException(
                    "The sale must be tracked before saving.");
            }

            // Use the version the frontend originally received,
            // not the latest version loaded from SQL Server.
            entry.Property(s => s.RowVersion).OriginalValue =
                expectedRowVersion;

            // Force a parent UPDATE even when only item names
            // or quantities change without changing the total.
            entry.Property(s => s.UpdatedAtUtc).IsModified = true;

            try
            {
                await _context.SaveChangesAsync(
                    cancellationToken);

                return true;
            }
            catch (DbUpdateConcurrencyException ex)
                when (ex.Entries.Any(e =>
                    e.Entity is InStoreSale))
            {
                return false;
            }
        }
    }
}
