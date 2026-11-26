using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.SupplierInvoices.GetSupplierPaymentHistory
{
    public sealed record SupplierPaymentHistoryItemResponse(
     long SupplierPaymentId,
     Guid SubmissionId,
     decimal Amount,
     DateTime PaidAtUtc,
     DateTime RecordedAtUtc
 );
}
