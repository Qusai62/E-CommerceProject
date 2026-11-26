using ECommerce.Domain.Shipping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.StoreOrders.ViewOrderDetails
{
    public sealed record StoreOrderDeliveryResponse(
      string RecipientName,
      string ContactNumber,
      string City,
      string Neighborhood,
      string Street,
      string BuildingInfo,
      string? AdditionalInfo,
      DeliveryStatus? DeliveryStatus,
      string? TrackingReference,
      DateTime? ShippedAtUtc,
      DateTime? DeliveredAtUtc
  );
}
