using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.SupplierInvoices.CreateSupplierInvoice
{
    public sealed record CreateSupplierInvoiceItemRequest(
     string ProductName,
     int Quantity,
     decimal UnitPrice
 );
}
