using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.StoreOrders.CancelOrder
{
    public sealed record CancelOrderRequest
    {
        public required string Reason { get; init; }
    }
}
