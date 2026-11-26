using ECommerce.Application.Inventory.Abstractions;
using ECommerce.Application.Inventory.StoreOwnerViewStock;
using ECommerce.Application.Inventory.ViewOutOfStock;
using ECommerce.Domain.InventoryManagement;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Repositories.Inventory
{
    public class StoreOwnerInventoryReadRepository : IStoreOwnerInventoryReadRepository
    {
        private readonly ApplicationDbContext _context;

        public StoreOwnerInventoryReadRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(
            IReadOnlyList<StoreOwnerStockSnapshot> Items,
            int TotalCount)> GetPageAsync(
                int pageNumber,
                int pageSize,
                CancellationToken cancellationToken = default)
        {
            // 1. Start with all inventory records
            var query = _context.Inventories
                .AsNoTracking();

            // 2. Count all inventory records
            var totalCount = await query.CountAsync(
                cancellationToken);

            // 3. Retrieve one page of inventory information
            var items = await query
                .OrderBy(i => i.ProductVariantId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(i => new StoreOwnerStockSnapshot(
                    i.ProductVariantId,
                    i.ProductVariant.ProductId,
                    i.ProductVariant.Product.Name,
                    i.ProductVariant.Sku,
                    i.ProductVariant.Product.IsActive,
                    i.ProductVariant.IsActive,
                    i.OnHandQuantity,

                    i.StockReservations
                        .Where(r =>
                            r.Status == StockReservationStatus.Active)
                        .Sum(r => (int?)r.Quantity) ?? 0,

                    i.CODStockAllocations
                        .Where(a =>
                            a.Status == CODStockAllocationStatus.Secured)
                        .Sum(a => (int?)a.Quantity) ?? 0
                ))
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task<(
    IReadOnlyList<StoreOwnerStockSnapshot> Items,
    int TotalCount)> GetOutOfStockVariantsPageAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
        {
            // 1. Build the inventory query
            var query = _context.Inventories
                .AsNoTracking()
                .Select(i => new
                {
                    i.ProductVariantId,
                    i.ProductVariant.ProductId,
                    ProductName = i.ProductVariant.Product.Name,
                    i.ProductVariant.Sku,
                    ProductIsActive = i.ProductVariant.Product.IsActive,
                    VariantIsActive = i.ProductVariant.IsActive,
                    i.OnHandQuantity,

                    ReservedQuantity = i.StockReservations
                        .Where(r =>
                            r.Status == StockReservationStatus.Active)
                        .Sum(r => (int?)r.Quantity) ?? 0,

                    SecuredCODQuantity = i.CODStockAllocations
                        .Where(a =>
                            a.Status == CODStockAllocationStatus.Secured)
                        .Sum(a => (int?)a.Quantity) ?? 0
                })

                // 2. Include only variants with zero available stock
                .Where(x =>
                    (long)x.OnHandQuantity
                    - x.ReservedQuantity
                    - x.SecuredCODQuantity == 0L);

            // 3. Count matching variants
            var totalCount = await query.CountAsync(
                cancellationToken);

            // 4. Apply pagination and map to existing snapshot
            var items = await query
                .OrderBy(x => x.ProductVariantId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new StoreOwnerStockSnapshot(
                    x.ProductVariantId,
                    x.ProductId,
                    x.ProductName,
                    x.Sku,
                    x.ProductIsActive,
                    x.VariantIsActive,
                    x.OnHandQuantity,
                    x.ReservedQuantity,
                    x.SecuredCODQuantity
                ))
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task<(
    IReadOnlyList<OutOfStockProductSnapshot> Items,
    int TotalCount)> GetOutOfStockProductsPageAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
        {
            var query = _context.Products
                .AsNoTracking()
                .Where(p => !p.Variants.Any(v =>
                    v.IsActive &&
                    v.Inventory != null &&

                    (long)v.Inventory.OnHandQuantity >

                        (v.Inventory.StockReservations
                            .Where(r =>
                                r.Status == StockReservationStatus.Active)
                            .Sum(r => (long?)r.Quantity) ?? 0L)

                        +

                        (v.Inventory.CODStockAllocations
                            .Where(a =>
                                a.Status == CODStockAllocationStatus.Secured)
                            .Sum(a => (long?)a.Quantity) ?? 0L)
                ));

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(p => p.ProductId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new OutOfStockProductSnapshot(
                    p.ProductId,
                    p.Name,
                    p.IsActive
                ))
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }
    }
}
