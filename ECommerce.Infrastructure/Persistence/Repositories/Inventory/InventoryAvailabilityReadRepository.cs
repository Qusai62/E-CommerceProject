using ECommerce.Application.Inventory.Abstractions;
using ECommerce.Application.Inventory.GetVariantAvailability;
using ECommerce.Domain.InventoryManagement;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Repositories.Inventory
{
    public class InventoryAvailabilityReadRepository:IInventoryAvailabilityReadRepository
    {
        private readonly ApplicationDbContext _context;

        public InventoryAvailabilityReadRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<VariantStockSnapshot?> GetAsync(
            long productVariantId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Inventories
                .AsNoTracking()//This tells EF Core that we don't need to track the resulting entities for updates.

                .Where(i =>//The variable i represents one Inventory entity being evaluated by the query.
                    i.ProductVariantId == productVariantId &&
                    i.ProductVariant.IsActive &&
                    i.ProductVariant.Product.IsActive)

                .Select(i => new VariantStockSnapshot(

                    i.ProductVariantId,

                    i.OnHandQuantity,

                    i.StockReservations
                        .Where(r =>
                            r.Status == StockReservationStatus.Active)
                        .Sum(r => (int?)r.Quantity) ?? 0,//The (int?) makes the sum nullable, while ?? 0 handles the case where no qualifying rows exist.

                    i.CODStockAllocations
                        .Where(a =>
                            a.Status == CODStockAllocationStatus.Secured)
                        .Sum(a => (int?)a.Quantity) ?? 0

                ))
                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}
