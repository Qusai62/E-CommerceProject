using ECommerce.Application.InStoreSales.GetInStoreSaleDetails;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Repositories.InStoreSales
{
    public sealed class GetInStoreSaleDetailsRepository
     : IGetInStoreSaleDetailsRepository
    {
        private readonly ApplicationDbContext _context;

        public GetInStoreSaleDetailsRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<GetInStoreSaleDetailsResponse?> GetByIdAsync(
            long inStoreSaleId,
            CancellationToken cancellationToken = default)
        {
            var sale = await _context.InStoreSales
                .AsNoTracking()
                .Where(s => s.InStoreSaleId == inStoreSaleId)
                .Select(s => new
                {
                    s.InStoreSaleId,
                    s.SaleNumber,
                    s.TotalAmount,
                    s.Status,
                    s.SoldAtUtc,
                    s.VoidedAtUtc,
                    s.RowVersion,

                    Items = s.Items
                        .OrderBy(i => i.InStoreSaleItemId)
                        .Select(i => new InStoreSaleItemDetailsResponse(
                            i.InStoreSaleItemId,
                            i.ItemName,
                            i.UnitPrice,
                            i.Quantity,
                            i.LineTotal))
                        .ToList()
                })
                .SingleOrDefaultAsync(cancellationToken);

            if (sale is null)
            {
                return null;
            }

            return new GetInStoreSaleDetailsResponse(
                InStoreSaleId: sale.InStoreSaleId,
                SaleNumber: sale.SaleNumber,
                TotalAmount: sale.TotalAmount,
                Status: sale.Status,
                SoldAtUtc: sale.SoldAtUtc,
                VoidedAtUtc: sale.VoidedAtUtc,
                Items: sale.Items,
                RowVersion: Convert.ToBase64String(sale.RowVersion));
        }

    }
}
