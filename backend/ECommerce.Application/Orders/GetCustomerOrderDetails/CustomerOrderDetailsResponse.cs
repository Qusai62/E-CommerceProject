using ECommerce.Domain.Orders;
using ECommerce.Domain.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Orders.GetCustomerOrderDetails
{
    public sealed record CustomerOrderDetailsResponse(
    long OrderId,
    string OrderNumber,
    DateTime PlacedAtUtc,

    OrderFulfillmentStatus FulfillmentStatus,
    PaymentMethod PaymentMethod,
    PaymentStatus PaymentStatus,

    string DeliveryRecipientName,
    string DeliveryContactNumber,
    string DeliveryCity,
    string DeliveryNeighborhood,
    string DeliveryStreet,
    string DeliveryBuildingInfo,
    string? DeliveryAdditionalInfo,

    decimal ShippingCharge,
    decimal TotalAmount,

    IReadOnlyList<CustomerOrderItemDto> Items
)
    {
        public decimal Subtotal => Items.Sum(
            item => item.Subtotal);
    }
}
