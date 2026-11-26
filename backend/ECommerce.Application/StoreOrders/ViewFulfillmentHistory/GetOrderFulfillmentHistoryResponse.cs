using ECommerce.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.StoreOrders.ViewFulfillmentHistory
{
    public sealed record GetOrderFulfillmentHistoryResponse(
     long OrderId,
     string OrderNumber,
     OrderFulfillmentStatus CurrentStatus,
     IReadOnlyList<OrderFulfillmentHistoryResponse> History
 );
}
