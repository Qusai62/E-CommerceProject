using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Reports.RevenueTrends
{
    public sealed record RevenueTrendItemResponse(
     DateTime PeriodStartUtc,
     decimal OnlineRevenue,
     decimal InStoreRevenue,
     decimal TotalRevenue
 );
}
