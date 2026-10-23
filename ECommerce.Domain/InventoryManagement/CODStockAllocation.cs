using ECommerce.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.InventoryManagement
{
    public class CODStockAllocation
    {
        public long CODStockAllocationId { get; set; }

        public long OrderId { get; set; }

        public long InventoryId { get; set; }

        public int Quantity { get; set; }

        public CODStockAllocationStatus Status { get; set; }

        public DateTime SecuredAtUtc { get; set; }

        public DateTime? ReleasedAtUtc { get; set; }

        public DateTime? FinalizedAtUtc { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime UpdatedAtUtc { get; set; }

        public Order Order { get; set; } = null!;

        public Inventory Inventory { get; set; } = null!;
    }
}
