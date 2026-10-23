using ECommerce.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Shipping
{
    public class Shipment
    {
        public long ShipmentId { get; set; }

        public long OrderId { get; set; }

        public DeliveryStatus DeliveryStatus { get; set; }

        public string? TrackingReference { get; set; }

        public DateTime? ShippedAtUtc { get; set; }

        public DateTime? DeliveredAtUtc { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime UpdatedAtUtc { get; set; }

        public Order Order { get; set; } = null!;
    }
}
