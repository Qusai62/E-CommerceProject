using ECommerce.Application.SupplierInvoices.CreateSupplierInvoice;
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
    public sealed class CreateSupplierInvoiceRepository
    : ICreateSupplierInvoiceRepository
    {
        private readonly ApplicationDbContext _context;

        public CreateSupplierInvoiceRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> SupplierExistsAsync(
            long supplierId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Suppliers
                .AsNoTracking()
                .AnyAsync(
                    s => s.SupplierId == supplierId,
                    cancellationToken);
        }

        public async Task<bool> InvoiceNumberExistsAsync(
            long supplierId,
            string invoiceNumber,
            CancellationToken cancellationToken = default)
        {
            return await _context.SupplierInvoices
                .AsNoTracking()
                .AnyAsync(
                    i => i.SupplierId == supplierId &&
                         i.InvoiceNumber == invoiceNumber,
                    cancellationToken);
        }

        public async Task AddAsync(
            SupplierInvoice invoice,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(invoice);

            await _context.SupplierInvoices.AddAsync(
                invoice,
                cancellationToken);
        }

        public async Task SaveChangesAsync(
      CancellationToken cancellationToken = default)
        {
            try
            {
                await _context.SaveChangesAsync(
                    cancellationToken);
            }
            catch (DbUpdateException ex)
                when (ex.InnerException is SqlException sqlEx &&
                      sqlEx.Number is 2601 or 2627)
            {
                // Only classify the error as a duplicate invoice
                // if that invoice number now exists for the supplier.
                var entry = ex.Entries
                    .FirstOrDefault(e =>
                        e.Entity is SupplierInvoice);

                if (entry?.Entity is SupplierInvoice invoice)
                {
                    var duplicate = await _context.SupplierInvoices
                        .AsNoTracking()
                        .AnyAsync(
                            i => i.SupplierId == invoice.SupplierId &&
                                 i.InvoiceNumber == invoice.InvoiceNumber,
                            cancellationToken);

                    if (duplicate)
                    {
                        throw new DuplicateSupplierInvoiceException(ex);
                    }
                }

                // A different unique constraint failed.
                throw;
            }
        }
    }
}
