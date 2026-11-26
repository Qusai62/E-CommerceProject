using ECommerce.Application.Common.Results;
using ECommerce.Application.StoreOrders.ViewOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.StoreOrders.SearchOrders
{
    public sealed class SearchStoreOrdersService
    {
        private readonly IStoreOrderReadRepository _repository;

        public SearchStoreOrdersService(
            IStoreOrderReadRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<StoreOrderResponse>> ExecuteAsync(
            SearchStoreOrdersRequest request,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            // Step 1: Validate the order number.
            if (string.IsNullOrWhiteSpace(request.OrderNumber))
            {
                return Result<StoreOrderResponse>.Failure(
                    new[]
                    {
                    new Error(
                        "StoreOrders.InvalidOrderNumber",
                        "Order number is required.",
                        nameof(request.OrderNumber))
                    });
            }

            // Step 2: Normalize surrounding whitespace.
            var orderNumber = request.OrderNumber.Trim();

            // Step 3: Validate maximum length.
            if (orderNumber.Length > 50)
            {
                return Result<StoreOrderResponse>.Failure(
                    new[]
                    {
                    new Error(
                        "StoreOrders.InvalidOrderNumber",
                        "Order number cannot exceed 50 characters.",
                        nameof(request.OrderNumber))
                    });
            }

            // Step 4: Search the order repository.
            var order = await _repository.GetByOrderNumberAsync(
                orderNumber,
                cancellationToken);

            // Step 5: Handle order not found.
            if (order is null)
            {
                return Result<StoreOrderResponse>.Failure(
                    new[]
                    {
                    new Error(
                        "StoreOrders.OrderNotFound",
                        "No order was found with the supplied order number.",
                        nameof(request.OrderNumber))
                    });
            }

            // Step 6: Return the matching order.
            return Result<StoreOrderResponse>.Success(order);
        }
    }
}
