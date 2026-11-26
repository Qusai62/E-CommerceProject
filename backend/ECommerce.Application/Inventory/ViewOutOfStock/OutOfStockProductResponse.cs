using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Inventory.ViewOutOfStock
{
    public sealed record OutOfStockProductResponse(
    long ProductId,
    string ProductName,
    bool ProductIsActive
);
}
