using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.SupplierInvoices.ListSupplierInvoices
{
    public sealed record SupplierInvoiceListItemResponse(
     long SupplierInvoiceId,
     long SupplierId,
     string SupplierName,
     string InvoiceNumber,
     DateTime InvoiceDateUtc,
     decimal TotalAmount,
     decimal PaidAmount,
     decimal RemainingAmount,
     string PaymentStatus
 );
}
