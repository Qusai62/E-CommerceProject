using ECommerce.Application.Common.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.StoreOrders.ViewOrders
{
    public interface IStoreOrderReadRepository
    {
        Task<PagedResult<StoreOrderResponse>> GetPagedAsync(
        StoreOrderQueryCriteria criteria,
        CancellationToken cancellationToken = default);

        // Feature 3: Search Orders
        Task<StoreOrderResponse?> GetByOrderNumberAsync(
            string orderNumber,
            CancellationToken cancellationToken = default);
    }
}
