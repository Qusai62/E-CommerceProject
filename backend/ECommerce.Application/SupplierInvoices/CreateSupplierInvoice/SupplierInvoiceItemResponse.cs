using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.SupplierInvoices.CreateSupplierInvoice
{
    public sealed record SupplierInvoiceItemResponse(
      long SupplierInvoiceItemId,
      string ProductName,
      int Quantity,
      decimal UnitPrice,
      decimal LineTotal
  );
}
