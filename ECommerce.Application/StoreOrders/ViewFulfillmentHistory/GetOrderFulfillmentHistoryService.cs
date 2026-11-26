using ECommerce.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.StoreOrders.ViewFulfillmentHistory
{
    public sealed class GetOrderFulfillmentHistoryService
    {
        private readonly IOrderFulfillmentHistoryReadRepository
            _repository;

        public GetOrderFulfillmentHistoryService(
            IOrderFulfillmentHistoryReadRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<GetOrderFulfillmentHistoryResponse>>
            ExecuteAsync(
                long orderId,
                CancellationToken cancellationToken = default)
        {
            // Step 1: Validate OrderId.
            if (orderId <= 0)
            {
                return Result<GetOrderFulfillmentHistoryResponse>
                    .Failure(new[]
                    {
                    new Error(
                        "StoreOrders.InvalidOrderId",
                        "Order ID must be greater than zero.",
                        nameof(orderId))
                    });
            }

            // Step 2: Retrieve the order and its history.
            var response = await _repository.GetByOrderIdAsync(
                orderId,
                cancellationToken);

            // Step 3: Handle a nonexistent order.
            if (response is null)
            {
                return Result<GetOrderFulfillmentHistoryResponse>
                    .Failure(new[]
                    {
                    new Error(
                        "StoreOrders.OrderNotFound",
                        "The requested order was not found.",
                        nameof(orderId))
                    });
            }

            // Step 4: Return the retrieved history.
            return Result<GetOrderFulfillmentHistoryResponse>
                .Success(response);
        }
    }
}
