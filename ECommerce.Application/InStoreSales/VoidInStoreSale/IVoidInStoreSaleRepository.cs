using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.InStoreSales.VoidInStoreSale
{
    public interface IVoidInStoreSaleRepository
    {
        Task<bool> TryVoidAsync(
            long inStoreSaleId,
            DateTime voidedAtUtc,
            CancellationToken cancellationToken = default);

        Task<InStoreSaleStatus?> GetStatusAsync(
            long inStoreSaleId,
            CancellationToken cancellationToken = default);
    }
}
