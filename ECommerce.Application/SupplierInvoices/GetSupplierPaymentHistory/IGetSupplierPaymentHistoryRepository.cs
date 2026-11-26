using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.SupplierInvoices.GetSupplierPaymentHistory
{
    public interface IGetSupplierPaymentHistoryRepository
    {
        Task<SupplierPaymentHistoryResponse?> GetAsync(
            long supplierInvoiceId,
            CancellationToken cancellationToken = default);
    }
}
