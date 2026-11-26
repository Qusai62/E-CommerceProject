using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Inventory.CreateStockReservation
{
    public enum CreateStockReservationOutcome
    {
        Created,//All required stock successfully reserved
        PaymentAttemptNotFound,//Invalid payment attempt
        PaymentNotEligible,//Payment is paid, COD, or the attempt isn't pending
        OrderNotEligible,
        OrderHasNoItems, //No items exist to reserve
        AlreadyReserved, //A conflicting reservation already exists
        InsufficientStock//At least one variant lacks enough available quantity

    }
}