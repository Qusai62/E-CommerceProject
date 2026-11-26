using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.InStoreSales.GetInStoreSales
{
    public interface IGetInStoreSalesRepository
    {
        Task<(IReadOnlyList<InStoreSaleListItemResponse> Items,
              int TotalCount)> GetPagedAsync(
            GetInStoreSalesRequest request,
            CancellationToken cancellationToken = default);
    }
}
