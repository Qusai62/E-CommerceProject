using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Inventory.DecreaseStock
{
    public sealed record DecreaseStockCommand(
    long ProductVariantId,
    int Quantity
);
}
