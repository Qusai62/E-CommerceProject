using ECommerce.Application.StoreOrders.ViewOrderDetails;
using ECommerce.Domain.Shipping;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Repositories.StoreOrders
{
    public sealed class StoreOrderDetailsReadRepository
     : IStoreOrderDetailsReadRepository
    {
        private readonly ApplicationDbContext _context;

        public StoreOrderDetailsReadRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<StoreOrderDetailsResponse?> GetByIdAsync(
            long orderId,
            CancellationToken cancellationToken = default)
        {
            // Step 1: Retrieve the order, payment and shipment.
            var order = await _context.Orders
                .AsNoTracking()
                .Where(o => o.OrderId == orderId)
                .Select(o => new
                {
                    o.OrderId,
                    o.OrderNumber,
                    o.CustomerId,
                    o.PlacedAtUtc,
                    o.FulfillmentStatus,
                    o.ShippingCharge,
                    o.TotalAmount,

                    // Historical delivery snapshot
                    o.DeliveryRecipientName,
                    o.DeliveryContactNumber,
                    o.DeliveryCity,
                    o.DeliveryNeighborhood,
                    o.DeliveryStreet,
                    o.DeliveryBuildingInfo,
                    o.DeliveryAdditionalInfo,

                    // Current payment information
                    PaymentMethod = o.Payment.Method,
                    PaymentStatus = o.Payment.Status,
                    o.Payment.PaidAtUtc,

                    // Current shipment information
                    DeliveryStatus = o.Shipment == null
                        ? null
                        : (DeliveryStatus?)o.Shipment.DeliveryStatus,

                    TrackingReference = o.Shipment == null
                        ? null
                        : o.Shipment.TrackingReference,

                    ShippedAtUtc = o.Shipment == null
                        ? null
                        : o.Shipment.ShippedAtUtc,

                    DeliveredAtUtc = o.Shipment == null
                        ? null
                        : o.Shipment.DeliveredAtUtc
                })
                .SingleOrDefaultAsync(cancellationToken);

            // Step 2: Return null if the order does not exist.
            if (order is null)
            {
                return null;
            }

            // Step 3: Retrieve purchased items using snapshots.
            var items = await _context.OrderItems
                .AsNoTracking()
                .Where(i => i.OrderId == orderId)
                .OrderBy(i => i.OrderItemId)
                .Select(i => new StoreOrderItemResponse(
                    i.OrderItemId,
                    i.ProductVariantId,
                    i.ProductNameSnapshot,
                    i.SkuSnapshot,
                    i.VariantAttributesSnapshot,
                    i.Quantity,
                    i.UnitPrice
                ))
                .ToListAsync(cancellationToken);

            // Step 4: Build the original delivery snapshot
            // together with current shipment information.
            var delivery = new StoreOrderDeliveryResponse(
                order.DeliveryRecipientName,
                order.DeliveryContactNumber,
                order.DeliveryCity,
                order.DeliveryNeighborhood,
                order.DeliveryStreet,
                order.DeliveryBuildingInfo,
                order.DeliveryAdditionalInfo,
                order.DeliveryStatus,
                order.TrackingReference,
                order.ShippedAtUtc,
                order.DeliveredAtUtc
            );

            // Step 5: Return the complete response.
            return new StoreOrderDetailsResponse(
                order.OrderId,
                order.OrderNumber,
                order.CustomerId,
                order.PlacedAtUtc,
                order.FulfillmentStatus,
                order.ShippingCharge,
                order.TotalAmount,
                order.PaymentMethod,
                order.PaymentStatus,
                order.PaidAtUtc,
                delivery,
                items
            );
        }
    }
}
