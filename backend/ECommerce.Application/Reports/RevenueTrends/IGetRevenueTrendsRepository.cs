using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Reports.RevenueTrends
{
    public interface IGetRevenueTrendsRepository
    {
        Task<IReadOnlyList<RevenueBucket>> GetOnlineRevenueAsync(
            DateTime fromUtc,
            DateTime toUtc,
            RevenueGrouping groupBy,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<RevenueBucket>> GetInStoreRevenueAsync(
            DateTime fromUtc,
            DateTime toUtc,
            RevenueGrouping groupBy,
            CancellationToken cancellationToken = default);
    }
}
