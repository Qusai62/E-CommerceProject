using ECommerce.Domain.Supplier_Invoice___Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.SupplierInvoices.CreateSupplierInvoice
{
    public interface ICreateSupplierInvoiceRepository
    {
        Task<bool> SupplierExistsAsync(
            long supplierId,
            CancellationToken cancellationToken = default);

        Task<bool> InvoiceNumberExistsAsync(
            long supplierId,
            string invoiceNumber,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            SupplierInvoice invoice,
            CancellationToken cancellationToken = default);

        Task SaveChangesAsync(
            CancellationToken cancellationToken = default);
    }
}
