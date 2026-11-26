using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.InStoreSales.UpdateInStoreSale
{
    public interface IUpdateInStoreSaleRepository
    {
        Task<InStoreSale?> GetByIdForUpdateAsync(
            long inStoreSaleId,
            CancellationToken cancellationToken = default);

        Task<bool> TrySaveAsync(
            InStoreSale sale,
            byte[] expectedRowVersion,
            CancellationToken cancellationToken = default);
    }
}
