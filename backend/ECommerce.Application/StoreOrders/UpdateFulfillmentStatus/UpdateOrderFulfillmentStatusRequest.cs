using ECommerce.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.StoreOrders.UpdateFulfillmentStatus
{
    public sealed class UpdateOrderFulfillmentStatusRequest
    {
        public OrderFulfillmentStatus NewStatus { get; init; }

        public string? Reason { get; init; }
    }
}
