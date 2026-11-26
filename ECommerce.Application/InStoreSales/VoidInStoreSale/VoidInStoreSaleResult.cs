using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.InStoreSales.VoidInStoreSale
{
    public sealed record VoidInStoreSaleResult(
     VoidInStoreSaleOutcome Outcome,
     VoidInStoreSaleResponse? Response
 );
}
