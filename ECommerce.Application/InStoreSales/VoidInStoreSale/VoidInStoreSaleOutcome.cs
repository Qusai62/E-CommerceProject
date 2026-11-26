using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.InStoreSales.VoidInStoreSale
{
    public enum VoidInStoreSaleOutcome
    {
        Voided = 1,
        NotFound = 2,
        AlreadyVoided = 3
    }
}
