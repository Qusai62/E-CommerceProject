using ECommerce.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Payments
{
    public class Payment
    {
        public long PaymentId { get; set; }

        public long OrderId { get; set; }

        public PaymentMethod Method { get; set; }

        public PaymentStatus Status { get; set; }

        public decimal ExpectedAmount { get; set; }

        public DateTime? PaidAtUtc { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime UpdatedAtUtc { get; set; }

        public Order Order { get; set; } = null!;

        public ICollection<PaymentAttempt> PaymentAttempts { get; set; }
            = new List<PaymentAttempt>();
    }
}
