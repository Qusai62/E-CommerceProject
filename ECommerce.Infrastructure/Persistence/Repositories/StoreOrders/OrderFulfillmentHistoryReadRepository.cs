using ECommerce.Application.StoreOrders.ViewFulfillmentHistory;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Repositories.StoreOrders
{
    public sealed class OrderFulfillmentHistoryReadRepository
    : IOrderFulfillmentHistoryReadRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderFulfillmentHistoryReadRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<GetOrderFulfillmentHistoryResponse?>
            GetByOrderIdAsync(
                long orderId,
                CancellationToken cancellationToken = default)
        {
            // Step 1: Retrieve the order information.
            var order = await _context.Orders
                .AsNoTracking()
                .Where(o => o.OrderId == orderId)
                .Select(o => new
                {
                    o.OrderId,
                    o.OrderNumber,
                    o.FulfillmentStatus
                })
                .SingleOrDefaultAsync(cancellationToken);

            // Step 2: Return null if the order does not exist.
            if (order is null)
            {
                return null;
            }

            // Step 3: Retrieve the complete fulfillment history.
            var history = await _context.OrderFulfillmentStatusHistories
                .AsNoTracking()
                .Where(h => h.OrderId == orderId)
                .OrderBy(h => h.ChangedAtUtc)
                .ThenBy(h => h.OrderFulfillmentStatusHistoryId)
                .Select(h => new OrderFulfillmentHistoryResponse(
                    h.OrderFulfillmentStatusHistoryId,
                    h.Status,
                    h.ChangedAtUtc,
                    h.ChangedByApplicationUserId,
                    h.Reason))
                .ToListAsync(cancellationToken);

            // Step 4: Construct the final response.
            return new GetOrderFulfillmentHistoryResponse(
                order.OrderId,
                order.OrderNumber,
                order.FulfillmentStatus,
                history);
        }
    }
}
