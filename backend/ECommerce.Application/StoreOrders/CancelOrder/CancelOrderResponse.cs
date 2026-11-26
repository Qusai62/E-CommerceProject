using ECommerce.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.StoreOrders.CancelOrder
{
    public sealed record CancelOrderResponse(
     long OrderId,
     string OrderNumber,
     OrderFulfillmentStatus FulfillmentStatus,
     DateTime CancelledAtUtc,
     string CancellationReason
 );
}
