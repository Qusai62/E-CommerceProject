using ECommerce.Application.Suppliers.GetSupplierById;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Suppliers.ListSuppliers
{
    public sealed record PagedSuppliersResponse(
    IReadOnlyList<SupplierResponse> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages
);
}
