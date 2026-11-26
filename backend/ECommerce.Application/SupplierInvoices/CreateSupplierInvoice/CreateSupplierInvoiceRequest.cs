using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.SupplierInvoices.CreateSupplierInvoice
{
    public sealed record CreateSupplierInvoiceRequest(
     long SupplierId,
     string InvoiceNumber,
     DateTime InvoiceDateUtc,
     IReadOnlyCollection<CreateSupplierInvoiceItemRequest> Items
 );
}
