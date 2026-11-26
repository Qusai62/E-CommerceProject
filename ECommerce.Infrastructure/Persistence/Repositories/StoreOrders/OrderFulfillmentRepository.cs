using ECommerce.Application.Common.Results;
using ECommerce.Application.StoreOrders.UpdateFulfillmentStatus;
using ECommerce.Domain.InventoryManagement;
using ECommerce.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Repositories.StoreOrders
{
    public sealed class OrderFulfillmentRepository
     : IOrderFulfillmentRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderFulfillmentRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Execute the entire fulfillment operation atomically.
        public async Task<Result<T>> ExecuteAtomicallyAsync<T>(
            Func<CancellationToken, Task<Result<T>>> operation,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(operation);

            var strategy = _context.Database.CreateExecutionStrategy();

            try
            {
                return await strategy.ExecuteAsync(async () =>
                {
                    // Start every retry with a clean ChangeTracker.
                    // Requires a dedicated DbContext for this operation.
                    _context.ChangeTracker.Clear();

                    await using var transaction =
                        await _context.Database.BeginTransactionAsync(
                            IsolationLevel.ReadCommitted,
                            cancellationToken);

                    // Execute the Application Service logic.
                    var result = await operation(cancellationToken);

                    // Business validation failed: do not save or commit.
                    if (result.IsFailure)
                    {
                        _context.ChangeTracker.Clear();
                        return result;
                    }

                    // Save Order, history, Inventory and allocations.
                    await _context.SaveChangesAsync(cancellationToken);

                    // Commit all changes together.
                    await transaction.CommitAsync(cancellationToken);

                    return result;
                });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new OrderFulfillmentConcurrencyException(
                    "The order or inventory was modified by another operation.",
                    ex);
            }
        }

        // 2. Load and lock the Order.
        public async Task<Order?> GetByIdAsync(
            long orderId,
            CancellationToken cancellationToken = default)
        {
            RequireTransaction();

            var orders = await _context.Orders
                .FromSqlInterpolated($"""
                SELECT *
                FROM dbo.[Order] WITH (UPDLOCK, HOLDLOCK)
                WHERE OrderId = {orderId}
                """)
                .Include(o => o.Payment)
                .Include(o => o.Items)
                .AsSingleQuery()
                .ToListAsync(cancellationToken);

            return orders.SingleOrDefault();
        }

        // 3. Load allocations and lock Inventory in a stable order.
        public async Task<IReadOnlyList<CODStockAllocation>>
            GetCODAllocationsAsync(
                long orderId,
                CancellationToken cancellationToken = default)
        {
            RequireTransaction();

            var allocations = await _context.CODStockAllocations
                .Where(a => a.OrderId == orderId)
                .OrderBy(a => a.InventoryId)
                .ToListAsync(cancellationToken);

            var inventoryIds = allocations
                .Select(a => a.InventoryId)
                .Distinct()
                .OrderBy(id => id)
                .ToList();

            foreach (long inventoryId in inventoryIds)
            {
                var inventoryRows = await _context.Inventories
                    .FromSqlInterpolated($"""
                    SELECT *
                    FROM dbo.Inventory WITH (UPDLOCK, HOLDLOCK)
                    WHERE InventoryId = {inventoryId}
                    """)
                    .ToListAsync(cancellationToken);

                if (inventoryRows.Count != 1)
                {
                    throw new InvalidOperationException(
                        $"Inventory {inventoryId} was not found.");
                }

                // Attach the tracked Inventory to its allocations.
                var inventory = inventoryRows[0];

                foreach (var allocation in allocations
                    .Where(a => a.InventoryId == inventoryId))
                {
                    allocation.Inventory = inventory;
                }
            }

            return allocations;
        }

        private void RequireTransaction()
        {
            if (_context.Database.CurrentTransaction is null)
            {
                throw new InvalidOperationException(
                    "An active transaction is required.");
            }
        }
    }
}
