using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Suppliers.ListSuppliers
{
    public sealed record ListSuppliersRequest(
     string? Search = null,
     int PageNumber = 1,
     int PageSize = 10,
     string SortBy = "name",
     bool SortDescending = false
 );
}
