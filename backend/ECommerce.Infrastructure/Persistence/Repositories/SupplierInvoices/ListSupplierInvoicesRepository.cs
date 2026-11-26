using ECommerce.Application.SupplierInvoices.ListSupplierInvoices;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Repositories.SupplierInvoices
{
    public sealed class ListSupplierInvoicesRepository
    : IListSupplierInvoicesRepository
    {
        private readonly ApplicationDbContext _context;

        public ListSupplierInvoicesRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedSupplierInvoicesResponse> GetPagedAsync(
            ListSupplierInvoicesRequest request,
            CancellationToken cancellationToken = default)
        {
            // 1. Start the read-only query.
            var query = _context.SupplierInvoices
                .AsNoTracking()
                .AsQueryable();

            // 2. Filter by supplier.
            if (request.SupplierId.HasValue)
            {
                query = query.Where(i =>
                    i.SupplierId == request.SupplierId.Value);
            }

            // 3. Search invoice numbers.
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search;

                query = query.Where(i =>
                    i.InvoiceNumber.Contains(search));
            }

            // 4. Filter by payment status.
            query = request.PaymentStatus switch
            {
                "unpaid" =>
                    query.Where(i => i.PaidAmount == 0m),

                "partiallypaid" =>
                    query.Where(i =>
                        i.PaidAmount > 0m &&
                        i.PaidAmount < i.TotalAmount),

                "paid" =>
                    query.Where(i =>
                        i.PaidAmount == i.TotalAmount),

                _ => query
            };

            // 5. Count matching invoices.
            var totalCount = await query.CountAsync(
                cancellationToken);

            // 6. Apply sorting.
            var sortedQuery = request.SortBy switch
            {
                "totalamount" when request.SortDescending =>
                    query.OrderByDescending(i => i.TotalAmount)
                         .ThenByDescending(i => i.SupplierInvoiceId),

                "totalamount" =>
                    query.OrderBy(i => i.TotalAmount)
                         .ThenBy(i => i.SupplierInvoiceId),

                "remainingamount" when request.SortDescending =>
                    query.OrderByDescending(i =>
                        i.TotalAmount - i.PaidAmount)
                         .ThenByDescending(i => i.SupplierInvoiceId),

                "remainingamount" =>
                    query.OrderBy(i =>
                        i.TotalAmount - i.PaidAmount)
                         .ThenBy(i => i.SupplierInvoiceId),

                "invoicedate" when request.SortDescending =>
                    query.OrderByDescending(i => i.InvoiceDateUtc)
                         .ThenByDescending(i => i.SupplierInvoiceId),

                _ =>
                    query.OrderBy(i => i.InvoiceDateUtc)
                         .ThenBy(i => i.SupplierInvoiceId)
            };

            // 7. Calculate pagination offset.
            var skip =
                (request.PageNumber - 1) * request.PageSize;

            // 8. Project only the requested page.
            var invoices = await sortedQuery
                .Skip(skip)
                .Take(request.PageSize)
                .Select(i => new
                {
                    i.SupplierInvoiceId,
                    i.SupplierId,
                    SupplierName = i.Supplier.Name,
                    i.InvoiceNumber,
                    i.InvoiceDateUtc,
                    i.TotalAmount,
                    i.PaidAmount
                })
                .ToListAsync(cancellationToken);

            // 9. Construct the response DTOs.
            var items = invoices
                .Select(i =>
                {
                    var remaining =
                        i.TotalAmount - i.PaidAmount;

                    var status = i.PaidAmount == 0m
                        ? "Unpaid"
                        : remaining == 0m
                            ? "Paid"
                            : "PartiallyPaid";

                    return new SupplierInvoiceListItemResponse(
                        i.SupplierInvoiceId,
                        i.SupplierId,
                        i.SupplierName,
                        i.InvoiceNumber,
                        i.InvoiceDateUtc,
                        i.TotalAmount,
                        i.PaidAmount,
                        remaining,
                        status);
                })
                .ToList();

            // 10. Calculate total pages.
            var totalPages = (int)Math.Ceiling(
                (double)totalCount / request.PageSize);

            return new PagedSupplierInvoicesResponse(
                items,
                request.PageNumber,
                request.PageSize,
                totalCount,
                totalPages);
        }
    }
}
