using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Reports.RevenueTrends
{
    public sealed record GetRevenueTrendsResponse(
     DateTime FromUtc,
     DateTime ToUtc,
     RevenueGrouping GroupBy,
     IReadOnlyList<RevenueTrendItemResponse> Items,
     decimal TotalOnlineRevenue,
     decimal TotalInStoreRevenue,
     decimal TotalRevenue
 );
}
