using ECommerce.Domain.Orders;
using ECommerce.Domain.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.StoreOrders.ViewOrders
{
    public sealed record StoreOrderResponse(
    long OrderId,
    string OrderNumber,
    long CustomerId,
    DateTime PlacedAtUtc,
    OrderFulfillmentStatus FulfillmentStatus,
    PaymentMethod PaymentMethod,
    PaymentStatus PaymentStatus,
    decimal TotalAmount
);
}
