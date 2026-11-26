using ECommerce.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Reports.OrdersByStatus
{
    public sealed record OrderStatusCountResponse(
    OrderFulfillmentStatus Status,
    int Count
);
}
