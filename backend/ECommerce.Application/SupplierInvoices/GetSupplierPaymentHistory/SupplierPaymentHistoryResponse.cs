using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.SupplierInvoices.GetSupplierPaymentHistory
{
    public sealed record SupplierPaymentHistoryResponse(
      long SupplierInvoiceId,
      string InvoiceNumber,
      decimal TotalAmount,
      decimal PaidAmount,
      decimal RemainingAmount,
      string PaymentStatus,
      IReadOnlyList<SupplierPaymentHistoryItemResponse> Payments
  );
}
