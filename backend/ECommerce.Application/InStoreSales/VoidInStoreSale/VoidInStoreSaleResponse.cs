using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.InStoreSales.VoidInStoreSale
{
    public sealed record VoidInStoreSaleResponse(
     long InStoreSaleId,
     InStoreSaleStatus Status,
     DateTime VoidedAtUtc
 );
}
