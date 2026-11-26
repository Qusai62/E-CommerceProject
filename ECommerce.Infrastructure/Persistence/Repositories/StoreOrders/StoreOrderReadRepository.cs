using ECommerce.Application.Common.Pagination;
using ECommerce.Application.StoreOrders.ViewOrders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Repositories.StoreOrders
{
    public sealed class StoreOrderReadRepository : IStoreOrderReadRepository
    {
        private readonly ApplicationDbContext _context;

        public StoreOrderReadRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<StoreOrderResponse>> GetPagedAsync(
    StoreOrderQueryCriteria criteria,
    CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(criteria);

            // Step 1: Start with all orders.
            var query = _context.Orders
                .AsNoTracking()
                .AsQueryable();

            // Step 2: Filter by fulfillment status.
            if (criteria.FulfillmentStatus.HasValue)
            {
                query = query.Where(o =>
                    o.FulfillmentStatus ==
                    criteria.FulfillmentStatus.Value);
            }

            // Step 3: Filter by payment status.
            if (criteria.PaymentStatus.HasValue)
            {
                query = query.Where(o =>
                    o.Payment.Status ==
                    criteria.PaymentStatus.Value);
            }

            // Step 4: Filter by start date.
            if (criteria.StartUtc.HasValue)
            {
                query = query.Where(o =>
                    o.PlacedAtUtc >= criteria.StartUtc.Value);
            }

            // Step 5: Filter by exclusive end date.
            if (criteria.EndExclusiveUtc.HasValue)
            {
                query = query.Where(o =>
                    o.PlacedAtUtc < criteria.EndExclusiveUtc.Value);
            }

            // Step 6: Count matching orders.
            var totalCount = await query.CountAsync(
                cancellationToken);

            // Step 7: Calculate pagination offset.
            var skip = (criteria.PageNumber - 1)
                * criteria.PageSize;

            // Step 8: Sort, paginate, and project.
            var items = await query
                .OrderByDescending(o => o.PlacedAtUtc)
                .ThenByDescending(o => o.OrderId)
                .Skip(skip)
                .Take(criteria.PageSize)
                .Select(o => new StoreOrderResponse(
                    o.OrderId,
                    o.OrderNumber,
                    o.CustomerId,
                    o.PlacedAtUtc,
                    o.FulfillmentStatus,
                    o.Payment.Method,
                    o.Payment.Status,
                    o.TotalAmount
                ))
                .ToListAsync(cancellationToken);

            // Step 9: Return the filtered paginated result.
            return new PagedResult<StoreOrderResponse>(
                items,
                criteria.PageNumber,
                criteria.PageSize,
                totalCount);
        }
        public async Task<StoreOrderResponse?> GetByOrderNumberAsync(
    string orderNumber,
    CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .AsNoTracking()

                // Step 1: Find the order by its unique number.
                .Where(o => o.OrderNumber == orderNumber)

                // Step 2: Project into the existing response DTO.
                .Select(o => new StoreOrderResponse(
                    o.OrderId,
                    o.OrderNumber,
                    o.CustomerId,
                    o.PlacedAtUtc,
                    o.FulfillmentStatus,
                    o.Payment.Method,
                    o.Payment.Status,
                    o.TotalAmount
                ))

                // Step 3: Return the matching order or null.
                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}
