using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Reports.OrdersByStatus
{

    public interface IGetOrdersByStatusRepository
    {
        Task<IReadOnlyList<OrderStatusCountResponse>> GetCountsAsync(
            DateTime fromUtc,
            DateTime toUtc,
            CancellationToken cancellationToken = default);
    }
}
