using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Reports.SalesSummary
{
    public sealed record GetSalesSummaryRequest(
     DateTime FromUtc,
     DateTime ToUtc
 );
}
