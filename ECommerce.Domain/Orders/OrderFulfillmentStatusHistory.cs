using ECommerce.Domain.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Orders
{
    public sealed class OrderFulfillmentStatusHistory
    {
        // Used by EF Core.
        private OrderFulfillmentStatusHistory()
        {
        }

        internal OrderFulfillmentStatusHistory(
            Order order,
            OrderFulfillmentStatus status,
            DateTime changedAtUtc,
            Guid? changedByApplicationUserId = null,
            string? reason = null)
        {
            ArgumentNullException.ThrowIfNull(order);

            if (!Enum.IsDefined(status))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(status));
            }

            if (changedAtUtc.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException(
                    "The change timestamp must be UTC.",
                    nameof(changedAtUtc));
            }

            if (reason is { Length: > 500 })
            {
                throw new ArgumentException(
                    "Reason cannot exceed 500 characters.",
                    nameof(reason));
            }

            Order = order;
            Status = status;
            ChangedAtUtc = changedAtUtc;
            ChangedByApplicationUserId =
                changedByApplicationUserId;
            Reason = reason;
        }

        public long OrderFulfillmentStatusHistoryId
        {
            get;
            private set;
        }

        public long OrderId
        {
            get;
            private set;
        }

        public OrderFulfillmentStatus Status
        {
            get;
            private set;
        }

        public DateTime ChangedAtUtc
        {
            get;
            private set;
        }

        public Guid? ChangedByApplicationUserId
        {
            get;
            private set;
        }

        public string? Reason
        {
            get;
            private set;
        }

        public Order Order
        {
            get;
            private set;
        } = null!;
    }


}
