using ECommerce.Domain.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.InventoryManagement
{
    public class StockReservation
    {
        public long StockReservationId { get; set; }

        public long InventoryId { get; set; }

        public long PaymentAttemptId { get; set; }

        public int Quantity { get; set; }

        public StockReservationStatus Status { get; set; }

        public DateTime ExpiresAtUtc { get; set; }

        public DateTime StatusChangedAtUtc { get; set; }

        public string? ReleaseReason { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime UpdatedAtUtc { get; set; }

        public Inventory Inventory { get; set; } = null!;

        public PaymentAttempt PaymentAttempt { get; set; } = null!;
    }
}
