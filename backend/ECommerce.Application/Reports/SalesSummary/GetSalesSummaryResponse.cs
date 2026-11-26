using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Reports.SalesSummary
{
    public sealed record GetSalesSummaryResponse(
     DateTime FromUtc,
     DateTime ToUtc,

     int OnlineOrdersCount,
     int PaidOnlineOrdersCount,
     int InStoreSalesCount,

     decimal OnlineRevenue,
     decimal InStoreRevenue,
     decimal TotalRevenue
 );
}
