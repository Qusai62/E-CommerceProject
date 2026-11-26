using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.SupplierInvoices.RecordSupplierPayment
{
    public sealed record RecordSupplierPaymentResponse(
    long SupplierPaymentId,
    long SupplierInvoiceId,
    Guid SubmissionId,
    decimal PaymentAmount,
    DateTime PaidAtUtc,
    DateTime RecordedAtUtc,
    decimal TotalAmount,
    decimal PaidAmount,
    decimal RemainingAmount,
    string PaymentStatus
);
}
