using ECommerce.Application.Common.Pagination;
using ECommerce.Application.Orders.GetCustomerOrderDetails;
using ECommerce.Application.Orders.GetCustomerOrders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Repositories.Orders
{
    public class OrderHistoryReadRepository: IOrderHistoryReadRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderHistoryReadRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<CustomerOrderSummary>>
            GetPagedAsync(
                  Guid applicationUserId,
                int pageNumber,
                int pageSize,
                CancellationToken cancellationToken = default)
        {
            var query = _context.Orders
    .AsNoTracking()
    .Where(order =>
        order.Customer.ApplicationUserId == applicationUserId);

            var totalCount = await query.CountAsync(
                cancellationToken);

            var items = await query
                .OrderByDescending(order => order.PlacedAtUtc)
                .ThenByDescending(order => order.OrderId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(order => new CustomerOrderSummary(
                    order.OrderId,
                    order.OrderNumber,
                    order.PlacedAtUtc,
                    order.FulfillmentStatus,
                    order.Payment.Method,
                    order.Payment.Status,
                    order.TotalAmount
                ))
                .ToListAsync(cancellationToken);

            // Adapt this constructor to your existing PagedResult<T>.
            return new PagedResult<CustomerOrderSummary>(
                items,
                pageNumber,
                pageSize,
                totalCount
            );
        }

        public async Task<CustomerOrderDetailsResponse?> GetByIdAsync(
    Guid applicationUserId,
    long orderId,
    CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .AsNoTracking()

                .Where(order =>
                    order.OrderId == orderId &&
                    order.Customer.ApplicationUserId == applicationUserId)

                .Select(order => new CustomerOrderDetailsResponse(
                    order.OrderId,
                    order.OrderNumber,
                    order.PlacedAtUtc,

                    order.FulfillmentStatus,
                    order.Payment.Method,
                    order.Payment.Status,

                    order.DeliveryRecipientName,
                    order.DeliveryContactNumber,
                    order.DeliveryCity,
                    order.DeliveryNeighborhood,
                    order.DeliveryStreet,
                    order.DeliveryBuildingInfo,
                    order.DeliveryAdditionalInfo,

                    order.ShippingCharge,
                    order.TotalAmount,

                    order.Items
                        .OrderBy(item => item.OrderItemId)
                        .Select(item => new CustomerOrderItemDto(
                            item.OrderItemId,
                            item.ProductVariantId,
                            item.ProductNameSnapshot,
                            item.SkuSnapshot,
                            item.VariantAttributesSnapshot,
                            item.Quantity,
                            item.UnitPrice
                        ))
                        .ToList()
                ))

                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}

