using ECommerce.Domain.Orders;
using ECommerce.Domain.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.StoreOrders.ViewOrderDetails
{
    public sealed record StoreOrderDetailsResponse(
    long OrderId,
    string OrderNumber,
    long CustomerId,
    DateTime PlacedAtUtc,
    OrderFulfillmentStatus FulfillmentStatus,
    decimal ShippingCharge,
    decimal TotalAmount,
    PaymentMethod PaymentMethod,
    PaymentStatus PaymentStatus,
    DateTime? PaidAtUtc,
    StoreOrderDeliveryResponse Delivery,
    IReadOnlyList<StoreOrderItemResponse> Items
);
}
