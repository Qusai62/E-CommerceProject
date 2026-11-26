using ECommerce.Application.Common.Results;
using ECommerce.Domain.InventoryManagement;
using ECommerce.Domain.Orders;
using ECommerce.Domain.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.StoreOrders.CancelOrder
{
    public sealed class CancelOrderService
    {
        private readonly ICancelOrderRepository _repository;
        private readonly TimeProvider _timeProvider;

        public CancelOrderService(
            ICancelOrderRepository repository,
            TimeProvider timeProvider)
        {
            _repository = repository;
            _timeProvider = timeProvider;
        }

        public async Task<Result<CancelOrderResponse>> ExecuteAsync(
            long orderId,
            CancelOrderRequest request,
            Guid storeOwnerApplicationUserId,
            CancellationToken cancellationToken = default)
        {
            // Step 1: Validate the request.
            var errors = new List<Error>();

            if (orderId <= 0)
            {
                errors.Add(new Error(
                    "StoreOrders.InvalidOrderId",
                    "Order ID must be greater than zero.",
                    nameof(orderId)));
            }

            if (storeOwnerApplicationUserId == Guid.Empty)
            {
                errors.Add(new Error(
                    "StoreOrders.InvalidActor",
                    "A valid Store Owner ID is required.",
                    nameof(storeOwnerApplicationUserId)));
            }

            if (request is null ||
                string.IsNullOrWhiteSpace(request.Reason))
            {
                errors.Add(new Error(
                    "StoreOrders.CancellationReasonRequired",
                    "Cancellation reason is required.",
                    nameof(CancelOrderRequest.Reason)));
            }
            else if (request.Reason.Trim().Length > 500)
            {
                errors.Add(new Error(
                    "StoreOrders.CancellationReasonTooLong",
                    "Cancellation reason cannot exceed 500 characters.",
                    nameof(CancelOrderRequest.Reason)));
            }

            if (errors.Count > 0)
            {
                return Result<CancelOrderResponse>.Failure(errors);
            }

            var normalizedReason = request!.Reason.Trim();

            try
            {
                // Step 2: Execute the cancellation atomically.
                return await _repository.ExecuteAtomicallyAsync(
                    async ct =>
                    {
                        // Step 3: Retrieve the Order and Payment.
                        var order = await _repository.GetByIdAsync(
                            orderId, ct);

                        if (order is null)
                        {
                            return Failure(
                                "StoreOrders.OrderNotFound",
                                "The requested order was not found.");
                        }

                        if (order.Payment is null)
                        {
                            return Failure(
                                "StoreOrders.PaymentMissing",
                                "The order has no payment record.");
                        }

                        // Step 4: Validate COD payment method.
                        if (order.Payment.Method !=
                            PaymentMethod.CashOnDelivery)
                        {
                            return Failure(
                                "StoreOrders.UnsupportedPaymentMethod",
                                "Only Cash on Delivery orders can be cancelled.");
                        }

                        // Step 5: Validate payment status.
                        if (order.Payment.Status != PaymentStatus.Unpaid)
                        {
                            return Failure(
                                "StoreOrders.OrderAlreadyPaid",
                                "A paid order cannot be cancelled.");
                        }

                        // Step 6: Validate fulfillment status.
                        if (order.FulfillmentStatus ==
                            OrderFulfillmentStatus.Cancelled)
                        {
                            return Failure(
                                "StoreOrders.AlreadyCancelled",
                                "The order is already cancelled.");
                        }

                        if (order.FulfillmentStatus ==
                            OrderFulfillmentStatus.Fulfilled)
                        {
                            return Failure(
                                "StoreOrders.OrderFulfilled",
                                "A fulfilled order cannot be cancelled.");
                        }

                        if (order.FulfillmentStatus is not (
                            OrderFulfillmentStatus.Pending or
                            OrderFulfillmentStatus.Processing))
                        {
                            return Failure(
                                "StoreOrders.InvalidCancellationStage",
                                "The order is not in a cancellable stage.");
                        }

                        // Step 7: Load all COD allocations.
                        var allocations =
                            await _repository.GetAllocationsAsync(
                                orderId, ct);

                        // Our current COD checkout process is expected
                        // to secure stock for an order.
                        if (allocations.Count == 0)
                        {
                            return Failure(
                                "StoreOrders.AllocationsMissing",
                                "No COD stock allocations were found.");
                        }

                        // Step 8: Verify every allocation is secured.
                        if (allocations.Any(a =>
                            a.Status != CODStockAllocationStatus.Secured))
                        {
                            return Failure(
                                "StoreOrders.InvalidAllocationState",
                                "The order contains stock allocations that cannot be released.");
                        }

                        // Step 9: Use one UTC timestamp.
                        var cancelledAtUtc =
                            _timeProvider.GetUtcNow().UtcDateTime;

                        // Step 10: Release secured stock.
                        foreach (var allocation in allocations)
                        {
                            allocation.Release(cancelledAtUtc);
                        }

                        // Step 11: Cancel Order and create history.
                        order.Cancel(
                            cancelledAtUtc,
                            storeOwnerApplicationUserId,
                            normalizedReason);

                        // Step 12: Persist within the transaction.
                        await _repository.SaveChangesAsync(ct);

                        // Step 13: Construct the success response.
                        var response = new CancelOrderResponse(
                            order.OrderId,
                            order.OrderNumber,
                            order.FulfillmentStatus,
                            cancelledAtUtc,
                            normalizedReason);

                        return Result<CancelOrderResponse>.Success(
                            response);
                    },
                    cancellationToken);
            }
            catch (OrderCancellationConcurrencyException)
            {
                return Failure(
                    "StoreOrders.ConcurrencyConflict",
                    "The order was modified concurrently. Reload it and try again.");
            }
        }

        private static Result<CancelOrderResponse> Failure(
            string code,
            string message)
        {
            return Result<CancelOrderResponse>.Failure(
                new[]
                {
                new Error(code, message, nameof(CancelOrderRequest))
                });
        }
    }
}
