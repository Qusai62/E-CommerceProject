using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Orders
{
    public class OrderFulfillmentStatusHistory
    {
        public long OrderFulfillmentStatusHistoryId { get; set; }

        public long OrderId { get; set; }

        public OrderFulfillmentStatus Status { get; set; }

        public DateTime ChangedAtUtc { get; set; }

        public Guid? ChangedByApplicationUserId { get; set; }

        public string? Reason { get; set; }

        public Order Order { get; set; } = null!;
    }
}
