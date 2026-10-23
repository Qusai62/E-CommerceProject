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
        public long OrderId { get; set; }

        public string OrderNumber { get; set; } = null!;

        public long CustomerId { get; set; }

        public OrderFulfillmentStatus FulfillmentStatus { get; set; }

        public decimal ShippingCharge { get; set; }

        public decimal TotalAmount { get; set; }

        // Delivery address snapshot
        public string DeliveryRecipientName { get; set; } = null!;

        public string DeliveryContactNumber { get; set; } = null!;

        public string DeliveryCity { get; set; } = null!;

        public string DeliveryNeighborhood { get; set; } = null!;

        public string DeliveryStreet { get; set; } = null!;

        public string DeliveryBuildingInfo { get; set; } = null!;

        public string? DeliveryAdditionalInfo { get; set; }

        public DateTime PlacedAtUtc { get; set; }

        public DateTime UpdatedAtUtc { get; set; }

        public Customer Customer { get; set; } = null!;
        public Payment Payment { get; set; } = null!;
        public Shipment Shipment { get; set; } = null!;
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
        public ICollection<OrderFulfillmentStatusHistory> FulfillmentStatusHistory { get; set; }
    = new List<OrderFulfillmentStatusHistory>();

        public ICollection<CODStockAllocation> CODStockAllocations { get; set; }
    = new List<CODStockAllocation>();
    }
}
