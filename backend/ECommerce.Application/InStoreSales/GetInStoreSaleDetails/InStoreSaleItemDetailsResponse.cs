using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.InStoreSales.GetInStoreSaleDetails
{

    public sealed record InStoreSaleItemDetailsResponse(
        long InStoreSaleItemId,
        string ItemName,
        decimal UnitPrice,
        int Quantity,
        decimal LineTotal
    );
}
