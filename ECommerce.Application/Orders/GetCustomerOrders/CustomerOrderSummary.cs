using ECommerce.Domain.Orders;
using ECommerce.Domain.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Orders.GetCustomerOrders
{
    public sealed record CustomerOrderSummary(
      long OrderId,
      string OrderNumber,
      DateTime PlacedAtUtc,
      OrderFulfillmentStatus FulfillmentStatus,
      PaymentMethod PaymentMethod,
      PaymentStatus PaymentStatus,
      decimal TotalAmount
  );
}
