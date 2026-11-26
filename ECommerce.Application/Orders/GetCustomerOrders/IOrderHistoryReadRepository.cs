using ECommerce.Application.Common.Pagination;
using ECommerce.Application.Orders.GetCustomerOrderDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Orders.GetCustomerOrders
{
    public interface IOrderHistoryReadRepository
    {
        Task<PagedResult<CustomerOrderSummary>> GetPagedAsync(
               Guid applicationUserId,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default
        );

        // New method: Feature 2
        Task<CustomerOrderDetailsResponse?> GetByIdAsync(
            Guid applicationUserId,
            long orderId,
            CancellationToken cancellationToken = default);
    }
}
