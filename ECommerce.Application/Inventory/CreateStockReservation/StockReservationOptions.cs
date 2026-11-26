using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Inventory.CreateStockReservation
{
    public sealed class StockReservationOptions
    {
        public const string SectionName = "StockReservation";

        public int DurationMinutes { get; set; }
    }
}
