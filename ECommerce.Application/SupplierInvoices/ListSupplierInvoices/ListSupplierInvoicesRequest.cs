using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.SupplierInvoices.ListSupplierInvoices
{
    public sealed record ListSupplierInvoicesRequest(
      long? SupplierId = null,
      string? Search = null,
      string? PaymentStatus = null,
      int PageNumber = 1,
      int PageSize = 10,
      string SortBy = "invoiceDate",
      bool SortDescending = true
  );
}
