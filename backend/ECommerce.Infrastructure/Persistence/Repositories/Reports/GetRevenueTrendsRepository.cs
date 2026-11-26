using ECommerce.Application.Reports.RevenueTrends;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Payments;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Repositories.Reports
{
    public sealed class GetRevenueTrendsRepository
     : IGetRevenueTrendsRepository
    {
        private readonly ApplicationDbContext _context;

        public GetRevenueTrendsRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Group website COD revenue
        public async Task<IReadOnlyList<RevenueBucket>>
            GetOnlineRevenueAsync(
                DateTime fromUtc,
                DateTime toUtc,
                RevenueGrouping groupBy,
                CancellationToken cancellationToken = default)
        {
            bool groupDaily = groupBy == RevenueGrouping.Day;

            var results = await _context.Orders
                .AsNoTracking()
                .Where(o =>
                    o.Payment.Method == PaymentMethod.CashOnDelivery &&
                    o.Payment.Status == PaymentStatus.Paid &&
                    o.Payment.PaidAtUtc.HasValue &&
                    o.Payment.PaidAtUtc.Value >= fromUtc &&
                    o.Payment.PaidAtUtc.Value < toUtc)
                .GroupBy(o => new
                {
                    Year = o.Payment.PaidAtUtc!.Value.Year,
                    Month = o.Payment.PaidAtUtc!.Value.Month,
                    Day = groupDaily
                        ? o.Payment.PaidAtUtc!.Value.Day
                        : 1
                })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    g.Key.Day,
                    Revenue = g.Sum(o => o.TotalAmount)
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ThenBy(x => x.Day)
                .ToListAsync(cancellationToken);

            return results
                .Select(x => new RevenueBucket(
                    new DateTime(
                        x.Year,
                        x.Month,
                        x.Day,
                        0, 0, 0,
                        DateTimeKind.Utc),
                    x.Revenue))
                .ToList();
        }

        // 2. Group physical-store POS revenue
        public async Task<IReadOnlyList<RevenueBucket>>
            GetInStoreRevenueAsync(
                DateTime fromUtc,
                DateTime toUtc,
                RevenueGrouping groupBy,
                CancellationToken cancellationToken = default)
        {
            bool groupDaily = groupBy == RevenueGrouping.Day;

            var results = await _context.InStoreSales
                .AsNoTracking()
                .Where(s =>
                    s.Status == InStoreSaleStatus.Completed &&
                    s.SoldAtUtc >= fromUtc &&
                    s.SoldAtUtc < toUtc)
                .GroupBy(s => new
                {
                    Year = s.SoldAtUtc.Year,
                    Month = s.SoldAtUtc.Month,
                    Day = groupDaily
                        ? s.SoldAtUtc.Day
                        : 1
                })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    g.Key.Day,
                    Revenue = g.Sum(s => s.TotalAmount)
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ThenBy(x => x.Day)
                .ToListAsync(cancellationToken);

            return results
                .Select(x => new RevenueBucket(
                    new DateTime(
                        x.Year,
                        x.Month,
                        x.Day,
                        0, 0, 0,
                        DateTimeKind.Utc),
                    x.Revenue))
                .ToList();
        }
    }
}
