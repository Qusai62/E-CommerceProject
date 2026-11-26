using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.StoreOrders.ViewOrderDetails
{
    public interface IStoreOrderDetailsReadRepository
    {
        Task<StoreOrderDetailsResponse?> GetByIdAsync(
            long orderId,
            CancellationToken cancellationToken = default);
    }
}
