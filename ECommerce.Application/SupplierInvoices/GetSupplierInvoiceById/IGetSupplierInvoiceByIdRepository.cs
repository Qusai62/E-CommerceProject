using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.SupplierInvoices.GetSupplierInvoiceById
{
    public interface IGetSupplierInvoiceByIdRepository
    {
        Task<SupplierInvoiceDetailsResponse?> GetByIdAsync(
            long supplierInvoiceId,
            CancellationToken cancellationToken = default);
    }
}
