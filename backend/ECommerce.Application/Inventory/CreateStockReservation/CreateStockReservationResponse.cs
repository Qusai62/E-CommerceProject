using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Inventory.CreateStockReservation
{
    public sealed record CreateStockReservationResponse(
     long PaymentAttemptId,
     DateTime ExpiresAtUtc
 );
}
