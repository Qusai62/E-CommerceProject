using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Inventory.IncreaseStock
{
    public sealed record IncreaseStockCommand(
     long ProductVariantId,
     int Quantity
 );
}
