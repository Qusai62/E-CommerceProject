using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.InStoreSales.GetInStoreSales
{
    public sealed record GetInStoreSalesRequest(
     int PageNumber = 1,
     int PageSize = 10,
     DateTime? FromUtc = null,
     DateTime? ToUtc = null,
     InStoreSaleStatus? Status = null
 );
}
