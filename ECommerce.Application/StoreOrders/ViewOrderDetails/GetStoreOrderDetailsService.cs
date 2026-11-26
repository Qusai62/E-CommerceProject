using ECommerce.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.StoreOrders.ViewOrderDetails
{
    public sealed class GetStoreOrderDetailsService
    {
        private readonly IStoreOrderDetailsReadRepository _repository;

        public GetStoreOrderDetailsService(
            IStoreOrderDetailsReadRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<StoreOrderDetailsResponse>>
            ExecuteAsync(
                long orderId,
                CancellationToken cancellationToken = default)
        {
            // Step 1: Validate the order identifier.
            if (orderId <= 0)
            {
                return Result<StoreOrderDetailsResponse>.Failure(
                    new[]
                    {
                    new Error(
                        "StoreOrders.InvalidOrderId",
                        "Order ID must be greater than zero.",
                        nameof(orderId))
                    });
            }

            // Step 2: Retrieve the order details.
            var order = await _repository.GetByIdAsync(
                orderId,
                cancellationToken);

            // Step 3: Handle an order that does not exist.
            if (order is null)
            {
                return Result<StoreOrderDetailsResponse>.Failure(
                    new[]
                    {
                    new Error(
                        "StoreOrders.OrderNotFound",
                        "The requested order was not found.",
                        nameof(orderId))
                    });
            }

            // Step 4: Return the order details.
            return Result<StoreOrderDetailsResponse>.Success(order);
        }
    }
}
