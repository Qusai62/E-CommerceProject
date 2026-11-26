using ECommerce.Application.Common.Results;
using ECommerce.Domain.InventoryManagement;
using ECommerce.Domain.Orders;
using ECommerce.Domain.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.StoreOrders.UpdateFulfillmentStatus
{

    public sealed class UpdateOrderFulfillmentStatusService
    {
        private readonly IOrderFulfillmentRepository _repository;
        private readonly TimeProvider _timeProvider;

        public UpdateOrderFulfillmentStatusService(
            IOrderFulfillmentRepository repository,
            TimeProvider timeProvider)
        {
            _repository = repository;
            _timeProvider = timeProvider;
        }

        public async Task<Result<UpdateOrderFulfillmentStatusResponse>>
            ExecuteAsync(
                long orderId,
                UpdateOrderFulfillmentStatusRequest request,
                Guid storeOwnerApplicationUserId,
                CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            // 1. Validate input before opening a transaction.
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
                    "A valid Store Owner identity is required.",
                    nameof(storeOwnerApplicationUserId)));
            }

            if (!Enum.IsDefined(request.NewStatus))
            {
                errors.Add(new Error(
                    "StoreOrders.InvalidFulfillmentStatus",
                    "The requested status is invalid.",
                    nameof(request.NewStatus)));
            }

            if (request.Reason is { Length: > 500 })
            {
                errors.Add(new Error(
                    "StoreOrders.InvalidReason",
                    "Reason cannot exceed 500 characters.",
                    nameof(request.Reason)));
            }

            if (errors.Count > 0)
            {
                return Result<UpdateOrderFulfillmentStatusResponse>
                    .Failure(errors);
            }

            var changedAtUtc =
                _timeProvider.GetUtcNow().UtcDateTime;

            try
            {
                // 2. Run the entire operation in one transaction.
                return await _repository.ExecuteAtomicallyAsync(
                    async ct =>
                    {
                        // 3. Load and lock the Order.
                        var order = await _repository.GetByIdAsync(
                            orderId,
                            ct);

                        if (order is null)
                        {
                            return Fail(
                                "StoreOrders.OrderNotFound",
                                "The requested order was not found.");
                        }

                        if (order.Payment is null)
                        {
                            throw new InvalidOperationException(
                                "The order has no Payment record.");
                        }

                        // V1 supports only Cash on Delivery.
                        if (order.Payment.Method !=
                            PaymentMethod.CashOnDelivery)
                        {
                            return Fail(
                                "StoreOrders.UnsupportedPaymentMethod",
                                "Only COD orders are supported.");
                        }

                        // 4. Validate allocations before changing
                        //    the Order to Fulfilled.
                        IReadOnlyList<CODStockAllocation> allocations =
                            Array.Empty<CODStockAllocation>();

                        if (request.NewStatus ==
                            OrderFulfillmentStatus.Fulfilled)
                        {
                            allocations =
                                await _repository.GetCODAllocationsAsync(
                                    order.OrderId,
                                    ct);

                            var allocationError =
                                ValidateCODAllocations(
                                    order,
                                    allocations);

                            if (allocationError is not null)
                            {
                                return Result<
                                    UpdateOrderFulfillmentStatusResponse>
                                    .Failure(new[] { allocationError });
                            }
                        }

                        // 5. Apply the Domain transition.
                        try
                        {
                            order.ChangeFulfillmentStatus(
                                request.NewStatus,
                                order.Payment.Method,
                                order.Payment.Status,
                                changedAtUtc,
                                storeOwnerApplicationUserId,
                                request.Reason);
                        }
                        catch (InvalidOperationException ex)
                            when (GetDomainErrorCode(ex.Message)
                                  is not null)
                        {
                            return Fail(
                                GetDomainErrorCode(ex.Message)!,
                                ex.Message,
                                nameof(request.NewStatus));
                        }

                        // 6. Finalize stock only when Fulfilled.
                        if (request.NewStatus ==
                            OrderFulfillmentStatus.Fulfilled)
                        {
                            foreach (var allocation in allocations)
                            {
                                allocation.Inventory.OnHandQuantity -=
                                    allocation.Quantity;

                                allocation.Inventory.UpdatedAtUtc =
                                    changedAtUtc;

                                allocation.Status =
                                    CODStockAllocationStatus.Finalized;

                                allocation.FinalizedAtUtc =
                                    changedAtUtc;

                                allocation.UpdatedAtUtc =
                                    changedAtUtc;
                            }
                        }

                        // 7. Return success.
                        // ExecuteAtomicallyAsync saves and commits.
                        return Result<
                            UpdateOrderFulfillmentStatusResponse>
                            .Success(
                                new UpdateOrderFulfillmentStatusResponse(
                                    order.OrderId,
                                    order.OrderNumber,
                                    order.FulfillmentStatus,
                                    order.UpdatedAtUtc));
                    },
                    cancellationToken);
            }
            catch (OrderFulfillmentConcurrencyException)
            {
                return Fail(
                    "StoreOrders.ConcurrencyConflict",
                    "The order or inventory changed. Reload and try again.",
                    nameof(orderId));
            }
        }

        private static Error? ValidateCODAllocations(
            Order order,
            IReadOnlyList<CODStockAllocation> allocations)
        {
            if (allocations.Count == 0 ||
                allocations.Count != order.Items.Count)
            {
                return new Error(
                    "StoreOrders.InvalidCODAllocations",
                    "COD allocations do not match the order items.");
            }

            // Verify that there is only one allocation per variant.
            if (allocations
                .GroupBy(a => a.Inventory.ProductVariantId)
                .Any(g => g.Count() != 1))
            {
                return new Error(
                    "StoreOrders.DuplicateCODAllocation",
                    "Duplicate COD allocations were found.");
            }

            foreach (var orderItem in order.Items)
            {
                var allocation = allocations.SingleOrDefault(a =>
                    a.Inventory.ProductVariantId ==
                    orderItem.ProductVariantId);

                if (allocation is null ||
                    allocation.Quantity != orderItem.Quantity)
                {
                    return new Error(
                        "StoreOrders.InvalidCODAllocations",
                        "A COD allocation does not match its order item.");
                }
            }

            foreach (var allocation in allocations)
            {
                if (allocation.Status !=
                    CODStockAllocationStatus.Secured)
                {
                    return new Error(
                        "StoreOrders.AllocationNotSecured",
                        "A COD allocation is not secured.");
                }

                if (allocation.Quantity <= 0 ||
                    allocation.Inventory.OnHandQuantity <
                    allocation.Quantity)
                {
                    return new Error(
                        "StoreOrders.InsufficientPhysicalStock",
                        "The COD allocation cannot be finalized.");
                }
            }

            return null;
        }

        private static Result<UpdateOrderFulfillmentStatusResponse>
            Fail(
                string code,
                string message,
                string? field = null)
        {
            return Result<UpdateOrderFulfillmentStatusResponse>
                .Failure(new[]
                {
                new Error(code, message, field)
                });
        }

        private static string? GetDomainErrorCode(string message)
            => message switch
            {
                "Fulfilled or cancelled orders cannot change status."
                    => "StoreOrders.TerminalOrder",

                "The order already has this fulfillment status."
                    => "StoreOrders.StatusUnchanged",

                "The requested fulfillment transition is not allowed."
                    => "StoreOrders.InvalidTransition",

                "Online payment must be completed before fulfillment progresses."
                    => "StoreOrders.PaymentRequired",

                _ => null
            };
    }

}
