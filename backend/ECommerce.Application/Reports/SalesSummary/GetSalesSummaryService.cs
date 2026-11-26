using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Reports.SalesSummary
{
    public sealed class GetSalesSummaryService
    {
        private readonly IGetSalesSummaryRepository _repository;

        public GetSalesSummaryService(
            IGetSalesSummaryRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetSalesSummaryResponse> ExecuteAsync(
            GetSalesSummaryRequest request,
            CancellationToken cancellationToken = default)
        {
            // Step 1: Validate request
            ArgumentNullException.ThrowIfNull(request);

            // Step 2: Validate UTC timestamps
            if (request.FromUtc.Kind != DateTimeKind.Utc ||
                request.ToUtc.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException(
                    "Reporting dates must be in UTC.");
            }

            // Step 3: Validate date range
            if (request.FromUtc >= request.ToUtc)
            {
                throw new ArgumentException(
                    "FromUtc must be earlier than ToUtc.");
            }

            // Step 4: Count website orders placed in the period
            int onlineOrdersCount =
                await _repository.CountOnlineOrdersAsync(
                    request.FromUtc,
                    request.ToUtc,
                    cancellationToken);

            // Step 5: Retrieve paid website sales
            var (paidOnlineOrdersCount, onlineRevenue) =
                await _repository.GetPaidOnlineSalesAsync(
                    request.FromUtc,
                    request.ToUtc,
                    cancellationToken);

            // Step 6: Retrieve completed physical-store sales
            var (inStoreSalesCount, inStoreRevenue) =
                await _repository.GetCompletedInStoreSalesAsync(
                    request.FromUtc,
                    request.ToUtc,
                    cancellationToken);

            // Step 7: Calculate combined collected revenue
            decimal totalRevenue = checked(
                onlineRevenue + inStoreRevenue);

            // Step 8: Return response
            return new GetSalesSummaryResponse(
                FromUtc: request.FromUtc,
                ToUtc: request.ToUtc,
                OnlineOrdersCount: onlineOrdersCount,
                PaidOnlineOrdersCount: paidOnlineOrdersCount,
                InStoreSalesCount: inStoreSalesCount,
                OnlineRevenue: onlineRevenue,
                InStoreRevenue: inStoreRevenue,
                TotalRevenue: totalRevenue);
        }
    }
}
