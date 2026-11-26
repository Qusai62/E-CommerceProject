using ECommerce.Application.Common.Results;
using ECommerce.Application.StoreOrders.CancelOrder;
using ECommerce.Domain.InventoryManagement;
using ECommerce.Domain.Orders;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Repositories.StoreOrders
{
    public sealed class CancelOrderRepository
    : ICancelOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public CancelOrderRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        // ============================================
        // 1. Execute cancellation in one transaction
        // ============================================

        public async Task<Result<T>> ExecuteAtomicallyAsync<T>(
            Func<CancellationToken, Task<Result<T>>> operation,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(operation);

            try
            {
                await using var transaction =
                    await _context.Database.BeginTransactionAsync(
                        IsolationLevel.Serializable,
                        cancellationToken);

                // Execute all Application service operations:
                // loading, validation, release, cancellation, save.
                var result = await operation(cancellationToken);

                // Never commit an unsuccessful cancellation.
                if (!result.IsSuccess)
                {
                    await transaction.RollbackAsync(
                        cancellationToken);

                    return result;
                }

                // Commit only after every operation succeeds.
                await transaction.CommitAsync(
                    cancellationToken);

                return result;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new OrderCancellationConcurrencyException(
                    "The order was modified concurrently.",
                    ex);
            }
            catch (SqlException ex) when (
                ex.Number is 1205 or 3960)
            {
                throw new OrderCancellationConcurrencyException(
                    "A database concurrency conflict occurred during cancellation.",
                    ex);
            }
        }

        // ============================================
        // 2. Load Order with Payment
        // ============================================

        public async Task<Order?> GetByIdAsync(
            long orderId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Include(o => o.Payment)
                .SingleOrDefaultAsync(
                    o => o.OrderId == orderId,
                    cancellationToken);
        }

        // ============================================
        // 3. Load all COD allocations for Order
        // ============================================

        public async Task<IReadOnlyList<CODStockAllocation>>
            GetAllocationsAsync(
                long orderId,
                CancellationToken cancellationToken = default)
        {
            return await _context.CODStockAllocations
                .Where(a => a.OrderId == orderId)
                .OrderBy(a => a.CODStockAllocationId)
                .ToListAsync(cancellationToken);
        }

        // ============================================
        // 4. Persist all tracked changes
        // ============================================

        public async Task SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(
                cancellationToken);
        }
    }
}
