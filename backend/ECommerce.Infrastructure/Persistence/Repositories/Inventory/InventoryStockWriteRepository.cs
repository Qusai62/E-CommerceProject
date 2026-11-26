using ECommerce.Application.Inventory.Abstractions;
using ECommerce.Application.Inventory.DecreaseStock;
using ECommerce.Application.Inventory.IncreaseStock;
using ECommerce.Domain.InventoryManagement;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Repositories.Inventory
{
    public class InventoryStockWriteRepository : IInventoryStockWriteRepository
    {
        private readonly ApplicationDbContext _context;

        public InventoryStockWriteRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IncreaseStockOutcome> IncreaseAsync(
            long productVariantId,
            int quantity,
            CancellationToken cancellationToken = default)
        {
            if (quantity <= 0)
                throw new ArgumentOutOfRangeException(nameof(quantity));

            var utcNow = DateTime.UtcNow;

            // Atomically increase stock for this variant.
            var affectedRows = await _context.Inventories
                .Where(i =>
                    i.ProductVariantId == productVariantId &&
                    i.OnHandQuantity <= int.MaxValue - quantity)
                .ExecuteUpdateAsync(
                    setters => setters
                        .SetProperty(
                            i => i.OnHandQuantity,
                            i => i.OnHandQuantity + quantity)
                        .SetProperty(
                            i => i.UpdatedAtUtc,
                            utcNow),
                    cancellationToken);

            if (affectedRows == 1)
                return IncreaseStockOutcome.Increased;

            // Find out why no row was updated.
            var variantExists = await _context.ProductVariants
                .AsNoTracking()
                .AnyAsync(
                    v => v.ProductVariantId == productVariantId,
                    cancellationToken);

            if (!variantExists)
                return IncreaseStockOutcome.VariantNotFound;

            var inventoryExists = await _context.Inventories
                .AsNoTracking()
                .AnyAsync(
                    i => i.ProductVariantId == productVariantId,
                    cancellationToken);

            if (!inventoryExists)
            {
                throw new InvalidOperationException(
                    "Variant exists but has no Inventory record.");
            }

            return IncreaseStockOutcome.QuantityLimitExceeded;
        }

        public async Task<DecreaseStockOutcome> DecreaseAsync(
    long productVariantId,
    int quantity,
    CancellationToken cancellationToken = default)
        {
            if (quantity <= 0)
                throw new ArgumentOutOfRangeException(nameof(quantity));

            var utcNow = DateTime.UtcNow;

            // 1. Decrease stock only if enough available units exist.
            var affectedRows = await _context.Inventories
                .Where(i =>
                    i.ProductVariantId == productVariantId &&

                    (long)i.OnHandQuantity >=
                        (long)quantity +

                        (i.StockReservations
                            .Where(r =>
                                r.Status == StockReservationStatus.Active)
                            .Sum(r => (long?)r.Quantity) ?? 0L) +

                        (i.CODStockAllocations
                            .Where(a =>
                                a.Status == CODStockAllocationStatus.Secured)
                            .Sum(a => (long?)a.Quantity) ?? 0L)
                )
                .ExecuteUpdateAsync(
                    setters => setters
                        .SetProperty(
                            i => i.OnHandQuantity,
                            i => i.OnHandQuantity - quantity)
                        .SetProperty(
                            i => i.UpdatedAtUtc,
                            utcNow),
                    cancellationToken);

            // 2. Check whether the update succeeded.
            if (affectedRows == 1)
                return DecreaseStockOutcome.Decreased;

            // 3. Check if the variant exists.
            var variantExists = await _context.ProductVariants
                .AsNoTracking()
                .AnyAsync(
                    v => v.ProductVariantId == productVariantId,
                    cancellationToken);

            if (!variantExists)
                return DecreaseStockOutcome.VariantNotFound;

            // 4. Detect missing Inventory records.
            var inventoryExists = await _context.Inventories
                .AsNoTracking()
                .AnyAsync(
                    i => i.ProductVariantId == productVariantId,
                    cancellationToken);

            if (!inventoryExists)
            {
                throw new InvalidOperationException(
                    "Product variant exists without an Inventory record.");
            }

            // 5. Variant exists but not enough available stock.
            return DecreaseStockOutcome.InsufficientAvailableStock;
        }

    }
}
