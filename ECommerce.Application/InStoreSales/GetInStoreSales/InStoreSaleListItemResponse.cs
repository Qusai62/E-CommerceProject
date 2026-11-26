using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.InStoreSales.GetInStoreSales
{
    public sealed record GetInStoreSalesResponse(
    IReadOnlyList<InStoreSaleListItemResponse> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages
);
}
