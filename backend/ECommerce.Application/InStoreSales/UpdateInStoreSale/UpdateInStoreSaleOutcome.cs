using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.InStoreSales.UpdateInStoreSale
{

    public enum UpdateInStoreSaleOutcome
    {
        Updated = 1,
        NotFound = 2,
        NotEditable = 3,
        ConcurrencyConflict = 4
    }

}
