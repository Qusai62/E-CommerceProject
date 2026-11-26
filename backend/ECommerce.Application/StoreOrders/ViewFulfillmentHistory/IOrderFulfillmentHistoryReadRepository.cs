using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.StoreOrders.ViewFulfillmentHistory
{
    public interface IOrderFulfillmentHistoryReadRepository
    {
        Task<GetOrderFulfillmentHistoryResponse?> GetByOrderIdAsync(
            long orderId,
            CancellationToken cancellationToken = default);
    }
}
