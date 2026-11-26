using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Reports.RevenueTrends
{
    public sealed class GetRevenueTrendsService
    {
        private const int MaxReportingPeriods = 366;

        private readonly IGetRevenueTrendsRepository _repository;

        public GetRevenueTrendsService(
            IGetRevenueTrendsRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetRevenueTrendsResponse> ExecuteAsync(
            GetRevenueTrendsRequest request,
            CancellationToken cancellationToken = default)
        {
            // Step 1: Validate request
            ArgumentNullException.ThrowIfNull(request);

            if (request.FromUtc.Kind != DateTimeKind.Utc ||
                request.ToUtc.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException(
                    "Reporting dates must be UTC.");
            }

            if (request.FromUtc >= request.ToUtc)
            {
                throw new ArgumentException(
                    "FromUtc must be earlier than ToUtc.");
            }

            if (!Enum.IsDefined(request.GroupBy))
            {
                throw new ArgumentException(
                    "Invalid revenue grouping.");
            }

            // Step 2: Determine the first reporting period
            DateTime periodStart = request.GroupBy switch
            {
                RevenueGrouping.Day =>
                    request.FromUtc.Date,

                RevenueGrouping.Month =>
                    new DateTime(
                        request.FromUtc.Year,
                        request.FromUtc.Month,
                        1,
                        0, 0, 0,
                        DateTimeKind.Utc),

                _ => throw new ArgumentOutOfRangeException(
                    nameof(request.GroupBy))
            };

            // Step 3: Generate periods and enforce a size limit
            var periods = new List<DateTime>();

            for (var current = periodStart;
                 current < request.ToUtc;)
            {
                if (periods.Count >= MaxReportingPeriods)
                {
                    throw new ArgumentException(
                        "The selected reporting range is too large.");
                }

                periods.Add(current);

                // Prevent DateTime overflow
                if (request.GroupBy == RevenueGrouping.Day)
                {
                    if (current > DateTime.MaxValue.AddDays(-1))
                        break;

                    current = current.AddDays(1);
                }
                else
                {
                    if (current.Year == 9999 &&
                        current.Month == 12)
                        break;

                    current = current.AddMonths(1);
                }
            }

            // Step 4: Retrieve website COD revenue
            var onlineBuckets =
                await _repository.GetOnlineRevenueAsync(
                    request.FromUtc,
                    request.ToUtc,
                    request.GroupBy,
                    cancellationToken);

            // Step 5: Retrieve physical-store revenue
            var inStoreBuckets =
                await _repository.GetInStoreRevenueAsync(
                    request.FromUtc,
                    request.ToUtc,
                    request.GroupBy,
                    cancellationToken);

            // Step 6: Build lookup dictionaries
            var onlineLookup = onlineBuckets.ToDictionary(
                x => x.PeriodStartUtc,
                x => x.Revenue);

            var inStoreLookup = inStoreBuckets.ToDictionary(
                x => x.PeriodStartUtc,
                x => x.Revenue);

            // Step 7: Merge revenue by reporting period
            var items = new List<RevenueTrendItemResponse>();

            foreach (var period in periods)
            {
                decimal onlineRevenue =
                    onlineLookup.GetValueOrDefault(period);

                decimal inStoreRevenue =
                    inStoreLookup.GetValueOrDefault(period);

                decimal totalRevenue = checked(
                    onlineRevenue + inStoreRevenue);

                items.Add(new RevenueTrendItemResponse(
                    PeriodStartUtc: period,
                    OnlineRevenue: onlineRevenue,
                    InStoreRevenue: inStoreRevenue,
                    TotalRevenue: totalRevenue));
            }

            // Step 8: Calculate report totals
            decimal totalOnlineRevenue = checked(
                items.Sum(x => x.OnlineRevenue));

            decimal totalInStoreRevenue = checked(
                items.Sum(x => x.InStoreRevenue));

            decimal totalRevenueAll = checked(
                totalOnlineRevenue + totalInStoreRevenue);

            // Step 9: Return response
            return new GetRevenueTrendsResponse(
                FromUtc: request.FromUtc,
                ToUtc: request.ToUtc,
                GroupBy: request.GroupBy,
                Items: items,
                TotalOnlineRevenue: totalOnlineRevenue,
                TotalInStoreRevenue: totalInStoreRevenue,
                TotalRevenue: totalRevenueAll);
        }
    }
}
