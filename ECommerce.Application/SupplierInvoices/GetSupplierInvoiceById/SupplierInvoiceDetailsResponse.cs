using ECommerce.Application.SupplierInvoices.CreateSupplierInvoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.SupplierInvoices.GetSupplierInvoiceById
{
    public sealed record SupplierInvoiceDetailsResponse(
     long SupplierInvoiceId,
     long SupplierId,
     string SupplierName,
     string InvoiceNumber,
     DateTime InvoiceDateUtc,
     decimal TotalAmount,
     decimal PaidAmount,
     decimal RemainingAmount,
     string PaymentStatus,
     IReadOnlyList<SupplierInvoiceItemResponse> Items,
     DateTime CreatedAtUtc,
     DateTime UpdatedAtUtc
 );
}
