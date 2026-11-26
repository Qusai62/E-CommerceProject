using ECommerce.Application.Suppliers.GetSupplierById;
using ECommerce.Application.Suppliers.ListSuppliers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Repositories.Suppliers
{
    public sealed class ListSuppliersRepository
      : IListSuppliersRepository
    {
        private readonly ApplicationDbContext _context;

        public ListSuppliersRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedSuppliersResponse> GetPagedAsync(
            ListSuppliersRequest request,
            CancellationToken cancellationToken = default)
        {
            // Step 1: Start a read-only database query.
            var query = _context.Suppliers
                .AsNoTracking()
                .AsQueryable();

            // Step 2: Apply optional name search.
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search;

                query = query.Where(s =>
                    s.Name.Contains(search));
            }

            // Step 3: Count all matching suppliers.
            var totalCount = await query.CountAsync(
                cancellationToken);

            // Step 4: Apply controlled sorting.
            var sortedQuery = request.SortBy switch
            {
                "createdat" when request.SortDescending =>
                    query.OrderByDescending(s => s.CreatedAtUtc)
                         .ThenByDescending(s => s.SupplierId),

                "createdat" =>
                    query.OrderBy(s => s.CreatedAtUtc)
                         .ThenBy(s => s.SupplierId),

                "name" when request.SortDescending =>
                    query.OrderByDescending(s => s.Name)
                         .ThenByDescending(s => s.SupplierId),

                _ =>
                    query.OrderBy(s => s.Name)
                         .ThenBy(s => s.SupplierId)
            };

            // Step 5: Calculate pagination offset.
            var skip = (request.PageNumber - 1) * request.PageSize;

            // Step 6: Retrieve only the requested page.
            var suppliers = await sortedQuery
                .Skip(skip)
                .Take(request.PageSize)
                .Select(s => new SupplierResponse(
                    s.SupplierId,
                    s.Name,
                    s.PhoneNumber,
                    s.Email,
                    s.CreatedAtUtc,
                    s.UpdatedAtUtc
                ))
                .ToListAsync(cancellationToken);

            // Step 7: Calculate total pages.
            var totalPages = (int)Math.Ceiling(
                (double)totalCount / request.PageSize);

            // Step 8: Return the paginated result.
            return new PagedSuppliersResponse(
                suppliers,
                request.PageNumber,
                request.PageSize,
                totalCount,
                totalPages);
        }
    }
}
