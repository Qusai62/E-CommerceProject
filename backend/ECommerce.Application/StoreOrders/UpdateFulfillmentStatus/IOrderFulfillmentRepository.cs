using ECommerce.Application.Common.Results;
using ECommerce.Domain.InventoryManagement;
using ECommerce.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.StoreOrders.UpdateFulfillmentStatus
{
   
        public interface IOrderFulfillmentRepository
        {
            Task<Result<T>> ExecuteAtomicallyAsync<T>(
        Func<CancellationToken, Task<Result<T>>> operation,
        CancellationToken cancellationToken = default);

            Task<Order?> GetByIdAsync(
                long orderId,
                CancellationToken cancellationToken = default);

            Task<IReadOnlyList<CODStockAllocation>> GetCODAllocationsAsync(
                long orderId,
                CancellationToken cancellationToken = default);
        }
    
}
