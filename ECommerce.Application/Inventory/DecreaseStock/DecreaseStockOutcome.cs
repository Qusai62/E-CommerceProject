using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Inventory.DecreaseStock
{
    public enum DecreaseStockOutcome
    {
        Decreased,
        VariantNotFound,
        InsufficientAvailableStock
    }
}
