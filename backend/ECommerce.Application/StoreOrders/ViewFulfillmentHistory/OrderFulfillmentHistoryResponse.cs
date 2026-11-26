using ECommerce.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.StoreOrders.ViewFulfillmentHistory
{
    public sealed record OrderFulfillmentHistoryResponse(
     long OrderFulfillmentStatusHistoryId,
     OrderFulfillmentStatus Status,
     DateTime ChangedAtUtc,
     Guid? ChangedByApplicationUserId,
     string? Reason
 );
}
