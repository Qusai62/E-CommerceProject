using ECommerce.Domain.Orders;
using ECommerce.Domain.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.StoreOrders.ViewOrders
{
    public sealed class GetStoreOrdersRequest
    {
        public int PageNumber { get; init; } = 1;

        public int PageSize { get; init; } = 20;

        public OrderFulfillmentStatus? FulfillmentStatus
        { get; init; }

        public PaymentStatus? PaymentStatus
        { get; init; }

        public DateOnly? StartDate { get; init; }

        public DateOnly? EndDate { get; init; }
    }
}
