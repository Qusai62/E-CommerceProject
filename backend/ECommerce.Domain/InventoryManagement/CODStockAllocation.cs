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
        public CODStockAllocationStatus Status { get;  set; }


        public DateTime SecuredAtUtc { get; set; }

        public DateTime? ReleasedAtUtc { get; set; }

        public DateTime? FinalizedAtUtc { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime UpdatedAtUtc { get; set; }

        public Order Order { get; set; } = null!;

        public Inventory Inventory { get; set; } = null!;



        public void Release(DateTime releasedAtUtc)
        {
            // 1. Validate UTC timestamp.
            if (releasedAtUtc.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException(
                    "Release timestamp must be UTC.",
                    nameof(releasedAtUtc));
            }

            // 2. Reject an already released allocation.
            if (Status == CODStockAllocationStatus.Released)
            {
                throw new InvalidOperationException(
                    "The COD stock allocation is already released.");
            }

            // 3. Reject finalized allocations.
            if (Status == CODStockAllocationStatus.Finalized)
            {
                throw new InvalidOperationException(
                    "A finalized COD stock allocation cannot be released.");
            }

            // 4. Only secured allocations may be released.
            if (Status != CODStockAllocationStatus.Secured)
            {
                throw new InvalidOperationException(
                    "Only secured COD stock allocations can be released.");
            }

            // 5. Update the allocation state.
            Status = CODStockAllocationStatus.Released;
            ReleasedAtUtc = releasedAtUtc;
            UpdatedAtUtc = releasedAtUtc;
        }

    }
}
