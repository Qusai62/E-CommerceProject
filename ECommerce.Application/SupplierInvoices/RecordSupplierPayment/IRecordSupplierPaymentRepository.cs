using ECommerce.Domain.Supplier_Invoice___Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.SupplierInvoices.RecordSupplierPayment
{
    public interface IRecordSupplierPaymentRepository
    {
        Task<SupplierPayment?> GetBySubmissionIdAsync(
            Guid submissionId,
            CancellationToken cancellationToken = default);

        Task<SupplierInvoice?> GetInvoiceForUpdateAsync(
            long supplierInvoiceId,
            CancellationToken cancellationToken = default);

        Task SaveChangesAsync(
            CancellationToken cancellationToken = default);
    }
}
