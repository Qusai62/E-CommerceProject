using ECommerce.Application.Reports.OrdersByStatus;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Repositories.Reports
{


    // Add your Application and DbContext namespaces.

    public sealed class GetOrdersByStatusRepository
        : IGetOrdersByStatusRepository
    {
        private readonly ApplicationDbContext _context;

        public GetOrdersByStatusRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<OrderStatusCountResponse>>
            GetCountsAsync(
                DateTime fromUtc,
                DateTime toUtc,
                CancellationToken cancellationToken = default)
        {
            var results = await _context.Orders
                .AsNoTracking()
                .Where(o =>
                    o.PlacedAtUtc >= fromUtc &&
                    o.PlacedAtUtc < toUtc)
                .GroupBy(o => o.FulfillmentStatus)
                .Select(g => new OrderStatusCountResponse(
                    g.Key,
                    g.Count()))
                .ToListAsync(cancellationToken);

            return results;
        }
    }

}
