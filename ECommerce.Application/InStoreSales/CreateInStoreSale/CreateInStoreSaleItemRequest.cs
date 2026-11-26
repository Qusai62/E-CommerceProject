using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.InStoreSales.CreateInStoreSale
{

    public sealed record CreateInStoreSaleItemRequest(
        string ItemName,
        decimal UnitPrice,
        int Quantity
    );
}
