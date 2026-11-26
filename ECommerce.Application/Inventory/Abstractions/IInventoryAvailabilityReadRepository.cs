using ECommerce.Application.Inventory.GetVariantAvailability;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Inventory.Abstractions
{
    public interface IInventoryAvailabilityReadRepository
    {
        Task<VariantStockSnapshot?> GetAsync(
        long productVariantId,
        CancellationToken cancellationToken = default);
    }
}
