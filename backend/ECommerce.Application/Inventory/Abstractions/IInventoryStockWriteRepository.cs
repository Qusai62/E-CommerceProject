using ECommerce.Application.Inventory.DecreaseStock;
using ECommerce.Application.Inventory.IncreaseStock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Inventory.Abstractions
{
    public interface IInventoryStockWriteRepository
    {
        Task<IncreaseStockOutcome> IncreaseAsync(
        long productVariantId,
        int quantity,
        CancellationToken cancellationToken = default);

        Task<DecreaseStockOutcome> DecreaseAsync(
    long productVariantId,
    int quantity,
    CancellationToken cancellationToken = default);
    }
}
