using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.SupplierInvoices.ListSupplierInvoices
{
    public sealed record PagedSupplierInvoicesResponse(
    IReadOnlyList<SupplierInvoiceListItemResponse> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages
);
}
