using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Reports.SalesSummary
{
    public interface IGetSalesSummaryRepository
    {
        Task<int> CountOnlineOrdersAsync(
            DateTime fromUtc,
            DateTime toUtc,
            CancellationToken cancellationToken = default);

        Task<(int Count, decimal Revenue)> GetPaidOnlineSalesAsync(
            DateTime fromUtc,
            DateTime toUtc,
            CancellationToken cancellationToken = default);

        Task<(int Count, decimal Revenue)> GetCompletedInStoreSalesAsync(
            DateTime fromUtc,
            DateTime toUtc,
            CancellationToken cancellationToken = default);
    }
}
