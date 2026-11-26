using ECommerce.Application.SupplierInvoices.RecordSupplierPayment;
using ECommerce.Domain.Supplier_Invoice___Payment;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Repositories.SupplierInvoices
{
    public sealed class RecordSupplierPaymentRepository
    : IRecordSupplierPaymentRepository
    {
        private readonly ApplicationDbContext _context;

        public RecordSupplierPaymentRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SupplierPayment?> GetBySubmissionIdAsync(
            Guid submissionId,
            CancellationToken cancellationToken = default)
        {
            return await _context.SupplierPayments
                .AsNoTracking()
                .Include(p => p.SupplierInvoice)
                .SingleOrDefaultAsync(
                    p => p.SubmissionId == submissionId,
                    cancellationToken);
        }

        public async Task<SupplierInvoice?> GetInvoiceForUpdateAsync(
            long supplierInvoiceId,
            CancellationToken cancellationToken = default)
        {
            return await _context.SupplierInvoices
                .SingleOrDefaultAsync(
                    i => i.SupplierInvoiceId == supplierInvoiceId,
                    cancellationToken);
        }

        public async Task SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new SupplierPaymentConcurrencyException(ex);
            }
            catch (DbUpdateException ex)
                when (ex.InnerException is SqlException sqlEx &&
                      sqlEx.Number is 2601 or 2627)
            {
                // Verify whether the failed save corresponds
                // to an already-existing submission ID.
                var pendingPayment = _context.ChangeTracker
                    .Entries<SupplierPayment>()
                    .Where(e => e.State == EntityState.Added)
                    .Select(e => e.Entity)
                    .FirstOrDefault();

                if (pendingPayment is not null)
                {
                    var alreadyExists = await _context.SupplierPayments
                        .AsNoTracking()
                        .AnyAsync(
                            p => p.SubmissionId ==
                                 pendingPayment.SubmissionId,
                            cancellationToken);

                    if (alreadyExists)
                    {
                        throw new DuplicateSupplierPaymentSubmissionException(ex);
                    }
                }

                // Do not hide unrelated database errors.
                throw;
            }
        }
    }
}
