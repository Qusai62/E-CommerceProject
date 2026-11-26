using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.SupplierInvoices.CreateSupplierInvoice
{
    public sealed record CreateSupplierInvoiceResponse(
     long SupplierInvoiceId,
     long SupplierId,
     string InvoiceNumber,
     DateTime InvoiceDateUtc,
     decimal TotalAmount,
     decimal PaidAmount,
     decimal RemainingAmount,
     string PaymentStatus,
     IReadOnlyList<SupplierInvoiceItemResponse> Items,
     DateTime CreatedAtUtc
 );
}
