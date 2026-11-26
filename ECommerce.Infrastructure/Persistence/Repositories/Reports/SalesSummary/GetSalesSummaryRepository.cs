using ECommerce.Application.Reports.SalesSummary;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Payments;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Repositories.Reports.SalesSummary
{
    public sealed class GetSalesSummaryRepository
     : IGetSalesSummaryRepository
    {
        private readonly ApplicationDbContext _context;

        public GetSalesSummaryRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Count website orders placed during the period
        public async Task<int> CountOnlineOrdersAsync(
            DateTime fromUtc,
            DateTime toUtc,
            CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .AsNoTracking()
                .CountAsync(o =>
                    o.PlacedAtUtc >= fromUtc &&
                    o.PlacedAtUtc < toUtc,
                    cancellationToken);
        }

        // 2. Count paid COD orders and sum collected revenue
        public async Task<(int Count, decimal Revenue)>
            GetPaidOnlineSalesAsync(
                DateTime fromUtc,
                DateTime toUtc,
                CancellationToken cancellationToken = default)
        {
            var result = await _context.Orders
                .AsNoTracking()
                .Where(o =>
                    o.Payment.Method == PaymentMethod.CashOnDelivery &&
                    o.Payment.Status == PaymentStatus.Paid &&
                    o.Payment.PaidAtUtc >= fromUtc &&
                    o.Payment.PaidAtUtc < toUtc)
                .GroupBy(o => 1)
                .Select(g => new
                {
                    Count = g.Count(),
                    Revenue = g.Sum(o => o.TotalAmount)
                })
                .SingleOrDefaultAsync(cancellationToken);

            return result is null
                ? (0, 0m)
                : (result.Count, result.Revenue);
        }

        // 3. Count completed POS sales and sum revenue
        public async Task<(int Count, decimal Revenue)>
            GetCompletedInStoreSalesAsync(
                DateTime fromUtc,
                DateTime toUtc,
                CancellationToken cancellationToken = default)
        {
            var result = await _context.InStoreSales
                .AsNoTracking()
                .Where(s =>
                    s.Status == InStoreSaleStatus.Completed &&
                    s.SoldAtUtc >= fromUtc &&
                    s.SoldAtUtc < toUtc)
                .GroupBy(s => 1)
                .Select(g => new
                {
                    Count = g.Count(),
                    Revenue = g.Sum(s => s.TotalAmount)
                })
                .SingleOrDefaultAsync(cancellationToken);

            return result is null
                ? (0, 0m)
                : (result.Count, result.Revenue);
        }
    }
}
