using ECommerce.Domain.Orders;
using ECommerce.Domain.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.StoreOrders.ViewOrders
{
    public sealed record StoreOrderQueryCriteria(
     int PageNumber,
     int PageSize,
     OrderFulfillmentStatus? FulfillmentStatus,
     PaymentStatus? PaymentStatus,
     DateTime? StartUtc,
     DateTime? EndExclusiveUtc
 );
}
