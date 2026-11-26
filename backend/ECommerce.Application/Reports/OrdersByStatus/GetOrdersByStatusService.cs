using ECommerce.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Reports.OrdersByStatus
{

    public sealed class GetOrdersByStatusService
    {
        private readonly IGetOrdersByStatusRepository _repository;

        public GetOrdersByStatusService(
            IGetOrdersByStatusRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetOrdersByStatusResponse> ExecuteAsync(
            GetOrdersByStatusRequest request,
            CancellationToken cancellationToken = default)
        {
            // Step 1: Validate request
            ArgumentNullException.ThrowIfNull(request);

            if (request.FromUtc.Kind != DateTimeKind.Utc ||
                request.ToUtc.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException(
                    "Reporting dates must be in UTC.");
            }

            if (request.FromUtc >= request.ToUtc)
            {
                throw new ArgumentException(
                    "FromUtc must be earlier than ToUtc.");
            }

            // Step 2: Retrieve current status counts
            var counts = await _repository.GetCountsAsync(
                request.FromUtc,
                request.ToUtc,
                cancellationToken);

            // Step 3: Build a lookup by fulfillment status
            var countLookup = counts.ToDictionary(
                x => x.Status,
                x => x.Count);

            // Step 4: Include all supported statuses,
            // even when there are no orders in a status.
            var statuses = Enum
                .GetValues<OrderFulfillmentStatus>()
                .Select(status => new OrderStatusCountResponse(
                    Status: status,
                    Count: countLookup.GetValueOrDefault(status)))
                .ToList();

            // Step 5: Calculate total orders
            int totalOrders = checked(
                statuses.Sum(x => x.Count));

            // Step 6: Return response
            return new GetOrdersByStatusResponse(
                FromUtc: request.FromUtc,
                ToUtc: request.ToUtc,
                TotalOrders: totalOrders,
                Statuses: statuses);
        }
    }

}
