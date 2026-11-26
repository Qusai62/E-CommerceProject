using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entities
{
    public class InStoreSaleItem
    {
        public long InStoreSaleItemId { get; set; }

        public long InStoreSaleId { get; set; }

        public string ItemName { get; set; } = null!;

        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        public decimal LineTotal { get; set; }

        public InStoreSale InStoreSale { get; set; } = null!;
    }
}
