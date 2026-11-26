using ECommerce.Application.Inventory.StoreOwnerViewStock;
using ECommerce.Application.Inventory.ViewOutOfStock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Inventory.Abstractions
{
    public interface IStoreOwnerInventoryReadRepository
    {
        Task<(IReadOnlyList<StoreOwnerStockSnapshot> Items, int TotalCount)>
        GetPageAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<(
    IReadOnlyList<StoreOwnerStockSnapshot> Items,
    int TotalCount)> GetOutOfStockVariantsPageAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

        Task<(
            IReadOnlyList<OutOfStockProductSnapshot> Items,
            int TotalCount)> GetOutOfStockProductsPageAsync(
                int pageNumber,
                int pageSize,
                CancellationToken cancellationToken = default);
    }
}
