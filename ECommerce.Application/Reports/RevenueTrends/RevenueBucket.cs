using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Reports.RevenueTrends
{
    public sealed record RevenueBucket(
     DateTime PeriodStartUtc,
     decimal Revenue
 );
}
