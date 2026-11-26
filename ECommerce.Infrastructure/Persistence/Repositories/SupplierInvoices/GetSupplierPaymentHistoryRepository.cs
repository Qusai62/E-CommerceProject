using ECommerce.Application.SupplierInvoices.GetSupplierPaymentHistory;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Repositories.SupplierInvoices
{
    public sealed class GetSupplierPaymentHistoryRepository
    : IGetSupplierPaymentHistoryRepository
    {
        private readonly ApplicationDbContext _context;

        public GetSupplierPaymentHistoryRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SupplierPaymentHistoryResponse?> GetAsync(
            long supplierInvoiceId,
            CancellationToken cancellationToken = default)
        {
            // Step 1: Retrieve invoice financial summary.
            var invoice = await _context.SupplierInvoices
                .AsNoTracking()
                .Where(i => i.SupplierInvoiceId == supplierInvoiceId)
                .Select(i => new
                {
                    i.SupplierInvoiceId,
                    i.InvoiceNumber,
                    i.TotalAmount,
                    i.PaidAmount
                })
                .SingleOrDefaultAsync(cancellationToken);

            // Step 2: Return null when invoice doesn't exist.
            if (invoice is null)
            {
                return null;
            }

            // Step 3: Retrieve its recorded payments.
            var payments = await _context.SupplierPayments
                .AsNoTracking()
                .Where(p => p.SupplierInvoiceId == supplierInvoiceId)
                .OrderBy(p => p.RecordedAtUtc)
                .ThenBy(p => p.SupplierPaymentId)
                .Select(p => new SupplierPaymentHistoryItemResponse(
                    p.SupplierPaymentId,
                    p.SubmissionId,
                    p.Amount,
                    p.PaidAtUtc,
                    p.RecordedAtUtc
                ))
                .ToListAsync(cancellationToken);

            // Step 4: Calculate current invoice balance.
            var remainingAmount =
                invoice.TotalAmount - invoice.PaidAmount;

            // Step 5: Calculate current payment status.
            var paymentStatus = invoice.PaidAmount == 0m
                ? "Unpaid"
                : remainingAmount == 0m
                    ? "Paid"
                    : "PartiallyPaid";

            // Step 6: Construct the response.
            return new SupplierPaymentHistoryResponse(
                invoice.SupplierInvoiceId,
                invoice.InvoiceNumber,
                invoice.TotalAmount,
                invoice.PaidAmount,
                remainingAmount,
                paymentStatus,
                payments);
        }
    }
}
