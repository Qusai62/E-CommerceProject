using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entities
{
    public sealed record InStoreSaleItemInput(
     string ItemName,
     decimal UnitPrice,
     int Quantity
 );
}
