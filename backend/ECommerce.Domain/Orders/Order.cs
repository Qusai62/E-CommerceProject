using ECommerce.Domain.CustomerAccount;
using ECommerce.Domain.InventoryManagement;
using ECommerce.Domain.Payments;
using ECommerce.Domain.Shipping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Orders
{

    public class Order
    {
        // =========================
        // Identity
        // =========================

        public long OrderId { get; set; }

        public string OrderNumber { get; set; } = null!;

        public long CustomerId { get; set; }


        // =========================
        // Fulfillment
        // =========================

        public OrderFulfillmentStatus FulfillmentStatus
        {
            get;
            private set;
        } = OrderFulfillmentStatus.Pending;


        // =========================
        // Pricing
        // =========================

        public decimal ShippingCharge { get; set; }

        public decimal TotalAmount { get; set; }


        // =========================
        // Delivery Address Snapshot
        // =========================

        public string DeliveryRecipientName { get; set; } = null!;

        public string DeliveryContactNumber { get; set; } = null!;

        public string DeliveryCity { get; set; } = null!;

        public string DeliveryNeighborhood { get; set; } = null!;

        public string DeliveryStreet { get; set; } = null!;

        public string DeliveryBuildingInfo { get; set; } = null!;

        public string? DeliveryAdditionalInfo { get; set; }


        // =========================
        // Timestamps
        // =========================

        public DateTime PlacedAtUtc { get; set; }

        public DateTime UpdatedAtUtc { get;  set; }


        // =========================
        // Checkout Idempotency
        // =========================

        public Guid CheckoutSubmissionId { get; set; }

        public string CheckoutRequestFingerprint { get; set; } = null!;


        // =========================
        // Optimistic Concurrency
        // =========================

        public byte[] RowVersion { get; private set; }


        // =========================
        // Navigation Properties
        // =========================

        public Customer Customer { get; set; } = null!;

        public Payment Payment { get; set; } = null!;

        public Shipment? Shipment { get; set; }

        public ICollection<OrderItem> Items { get; set; }
            = new List<OrderItem>();

        public ICollection<CODStockAllocation> CODStockAllocations
        { get; set; } = new List<CODStockAllocation>();


        // =========================
        // Fulfillment Status History
        // =========================

        private readonly List<OrderFulfillmentStatusHistory>
            _fulfillmentStatusHistory = new();

        public IReadOnlyCollection<OrderFulfillmentStatusHistory>
            FulfillmentStatusHistory
                => _fulfillmentStatusHistory.AsReadOnly();


        // =========================
        // Change Fulfillment Status
        // =========================

        public void ChangeFulfillmentStatus(
            OrderFulfillmentStatus newStatus,
            PaymentMethod paymentMethod,
            PaymentStatus paymentStatus,
            DateTime changedAtUtc,
            Guid? changedByApplicationUserId = null,
            string? reason = null)
        {
            // Step 1: Validate inputs.

            if (!Enum.IsDefined(newStatus))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(newStatus));
            }

            if (!Enum.IsDefined(paymentMethod))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(paymentMethod));
            }

            if (!Enum.IsDefined(paymentStatus))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(paymentStatus));
            }

            if (changedAtUtc.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException(
                    "Timestamp must be UTC.",
                    nameof(changedAtUtc));
            }

            // Step 2: Reject terminal orders.

            if (FulfillmentStatus is
                OrderFulfillmentStatus.Fulfilled or
                OrderFulfillmentStatus.Cancelled)
            {
                throw new InvalidOperationException(
                    "Fulfilled or cancelled orders cannot change status.");
            }

            // Step 3: Reject duplicate status changes.

            if (FulfillmentStatus == newStatus)
            {
                throw new InvalidOperationException(
                    "The order already has this fulfillment status.");
            }

            // Step 4: Validate allowed transitions.

            bool isValidTransition =
                (FulfillmentStatus == OrderFulfillmentStatus.Pending &&
                 newStatus == OrderFulfillmentStatus.Processing)
                ||
                (FulfillmentStatus == OrderFulfillmentStatus.Processing &&
                 newStatus == OrderFulfillmentStatus.Fulfilled);

            if (!isValidTransition)
            {
                throw new InvalidOperationException(
                    "The requested fulfillment transition is not allowed.");
            }

            // Step 5: Validate online payment.

            if (paymentMethod == PaymentMethod.OnlinePayment &&
                paymentStatus != PaymentStatus.Paid)
            {
                throw new InvalidOperationException(
                    "Online payment must be completed before fulfillment progresses.");
            }

            // Step 6: Create the history record.
            // Constructor also validates reason and timestamp.

            var history = new OrderFulfillmentStatusHistory(
                this,
                newStatus,
                changedAtUtc,
                changedByApplicationUserId,
                reason);

            // Step 7: Update the current order status.

            FulfillmentStatus = newStatus;

            UpdatedAtUtc = changedAtUtc;

            // Step 8: Add the history record.

            _fulfillmentStatusHistory.Add(history);
        }


        // =========================
        // Record Initial Pending
        // =========================

        public void RecordInitialPendingStatus(
            DateTime placedAtUtc)
        {
            // Only new orders can record initial history.

            if (OrderId != 0)
            {
                throw new InvalidOperationException(
                    "Initial history can only be created for a new order.");
            }

            // A new order must start as Pending.

            if (FulfillmentStatus != OrderFulfillmentStatus.Pending)
            {
                throw new InvalidOperationException(
                    "A new order must begin as Pending.");
            }

            // Do not create duplicate initial history.

            if (_fulfillmentStatusHistory.Count != 0)
            {
                throw new InvalidOperationException(
                    "Initial status history already exists.");
            }

            // Require a UTC timestamp.

            if (placedAtUtc.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException(
                    "Timestamp must be UTC.",
                    nameof(placedAtUtc));
            }

            // Create initial Pending history.

            var history = new OrderFulfillmentStatusHistory(
                this,
                OrderFulfillmentStatus.Pending,
                placedAtUtc,
                null,
                "Order placed");

            _fulfillmentStatusHistory.Add(history);
        }


        public void Cancel(
            DateTime cancelledAtUtc,
            Guid cancelledByApplicationUserId,
            string reason)
        {
            // 1. Validate the cancellation timestamp.
            if (cancelledAtUtc.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException(
                    "Cancellation timestamp must be UTC.",
                    nameof(cancelledAtUtc));
            }

            // 2. Validate the Store Owner's identity.
            if (cancelledByApplicationUserId == Guid.Empty)
            {
                throw new ArgumentException(
                    "A valid Store Owner ID is required.",
                    nameof(cancelledByApplicationUserId));
            }

            // 3. Reject already cancelled orders.
            if (FulfillmentStatus == OrderFulfillmentStatus.Cancelled)
            {
                throw new InvalidOperationException(
                    "The order is already cancelled.");
            }

            // 4. Reject fulfilled orders.
            if (FulfillmentStatus == OrderFulfillmentStatus.Fulfilled)
            {
                throw new InvalidOperationException(
                    "A fulfilled order cannot be cancelled.");
            }

            // 5. Only Pending or Processing may be cancelled.
            if (FulfillmentStatus is not (
                OrderFulfillmentStatus.Pending or
                OrderFulfillmentStatus.Processing))
            {
                throw new InvalidOperationException(
                    "The order is not in a cancellable fulfillment stage.");
            }

            // 6. Validate cancellation reason.
            if (string.IsNullOrWhiteSpace(reason))
            {
                throw new ArgumentException(
                    "A cancellation reason is required.",
                    nameof(reason));
            }

            var normalizedReason = reason.Trim();

            if (normalizedReason.Length > 500)
            {
                throw new ArgumentException(
                    "Cancellation reason cannot exceed 500 characters.",
                    nameof(reason));
            }

            // 7. Construct history before modifying Order state.
            var historyEntry = new OrderFulfillmentStatusHistory(
                this,
                OrderFulfillmentStatus.Cancelled,
                cancelledAtUtc,
                cancelledByApplicationUserId,
                normalizedReason);

            // 8. Change the Order's fulfillment state.
            FulfillmentStatus = OrderFulfillmentStatus.Cancelled;
            UpdatedAtUtc = cancelledAtUtc;

            // 9. Preserve the cancellation event.
            _fulfillmentStatusHistory.Add(historyEntry);
        }

    }

}
