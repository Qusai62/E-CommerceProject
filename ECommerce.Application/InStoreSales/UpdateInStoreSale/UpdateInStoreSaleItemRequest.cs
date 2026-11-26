using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.InStoreSales.UpdateInStoreSale
{
    public sealed record UpdateInStoreSaleItemRequest(
     string ItemName,
     decimal UnitPrice,
     int Quantity
 );
}
