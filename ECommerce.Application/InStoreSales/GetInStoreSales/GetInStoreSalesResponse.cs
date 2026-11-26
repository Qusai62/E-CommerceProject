using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.InStoreSales.GetInStoreSales
{

    public sealed record InStoreSaleListItemResponse(
        long InStoreSaleId,
        string SaleNumber,
        decimal TotalAmount,
        InStoreSaleStatus Status,
        DateTime SoldAtUtc
    );
}
