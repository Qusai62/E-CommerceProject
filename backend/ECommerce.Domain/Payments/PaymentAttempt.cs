using ECommerce.Domain.InventoryManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Payments
{
    public class PaymentAttempt
    {
        public long PaymentAttemptId { get; set; }

        public long PaymentId { get; set; }

        public int AttemptNumber { get; set; }

        public decimal Amount { get; set; }

        public PaymentAttemptStatus Status { get; set; }

        public string PaymentReference { get; set; } = null!;

        public string? ProviderReference { get; set; }

        public string? FailureCode { get; set; }

        public string? FailureReason { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime? CompletedAtUtc { get; set; }

        public Payment Payment { get; set; } = null!; 
        public ICollection<StockReservation> StockReservations { get; set; }
    = new List<StockReservation>();
    }
}
