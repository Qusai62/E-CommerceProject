using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.InStoreSales.GetInStoreSaleDetails
{
    public interface IGetInStoreSaleDetailsRepository
    {
        Task<GetInStoreSaleDetailsResponse?> GetByIdAsync(
            long inStoreSaleId,
            CancellationToken cancellationToken = default);
    }
}
