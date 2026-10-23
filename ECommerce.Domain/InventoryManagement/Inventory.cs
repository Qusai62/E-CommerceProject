using ECommerce.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.InventoryManagement
{
    public class Inventory
    {
        public long InventoryId { get; set; }

        public long ProductVariantId { get; set; }

        public int OnHandQuantity { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime UpdatedAtUtc { get; set; }

        public ProductVariant ProductVariant { get; set; } = null!;
        public ICollection<StockReservation> StockReservations { get; set; }
    = new List<StockReservation>();

        public ICollection<CODStockAllocation> CODStockAllocations { get; set; }
            = new List<CODStockAllocation>();

    }
}
