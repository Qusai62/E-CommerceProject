using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.SupplierInvoices.RecordSupplierPayment
{
    public sealed record RecordSupplierPaymentRequest(
      Guid SubmissionId,
      decimal Amount,
      DateTime PaidAtUtc
  );
}
