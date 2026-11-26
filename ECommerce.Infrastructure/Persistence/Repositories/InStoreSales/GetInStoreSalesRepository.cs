using ECommerce.Application.InStoreSales.GetInStoreSales;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Repositories.InStoreSales
{
    public sealed class GetInStoreSalesRepository
    : IGetInStoreSalesRepository
    {
        private readonly ApplicationDbContext _context;

        public GetInStoreSalesRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(
            IReadOnlyList<InStoreSaleListItemResponse> Items,
            int TotalCount)> GetPagedAsync(
            GetInStoreSalesRequest request,
            CancellationToken cancellationToken = default)
        {
            // 1. Start with a read-only query
            var query = _context.InStoreSales
                .AsNoTracking()
                .AsQueryable();

            // 2. Filter by start date
            if (request.FromUtc.HasValue)
            {
                query = query.Where(x =>
                    x.SoldAtUtc >= request.FromUtc.Value);
            }

            // 3. Filter by end date (exclusive)
            if (request.ToUtc.HasValue)
            {
                query = query.Where(x =>
                    x.SoldAtUtc < request.ToUtc.Value);
            }

            // 4. Filter by sale status
            if (request.Status.HasValue)
            {
                query = query.Where(x =>
                    x.Status == request.Status.Value);
            }

            // 5. Count all matching sales
            int totalCount = await query.CountAsync(
                cancellationToken);

            // 6. Apply stable ordering and pagination
            int skip = (int)(
                ((long)request.PageNumber - 1) *
                request.PageSize);

            var items = await query
                .OrderByDescending(x => x.SoldAtUtc)
                .ThenByDescending(x => x.InStoreSaleId)
                .Skip(skip)
                .Take(request.PageSize)
                .Select(x => new InStoreSaleListItemResponse(
                    x.InStoreSaleId,
                    x.SaleNumber,
                    x.TotalAmount,
                    x.Status,
                    x.SoldAtUtc))
                .ToListAsync(cancellationToken);

            // 7. Return page and total count
            return (items, totalCount);
        }
    }
}
