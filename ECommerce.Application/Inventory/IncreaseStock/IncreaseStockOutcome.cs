using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Inventory.IncreaseStock
{
    public enum IncreaseStockOutcome
    {
        Increased,
        VariantNotFound,
        QuantityLimitExceeded
    }
}
