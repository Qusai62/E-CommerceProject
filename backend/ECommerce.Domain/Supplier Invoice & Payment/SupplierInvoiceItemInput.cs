using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Supplier_Invoice___Payment
{
    public sealed record SupplierInvoiceItemInput(
    string ProductName,
    int Quantity,
    decimal UnitPrice);
}
