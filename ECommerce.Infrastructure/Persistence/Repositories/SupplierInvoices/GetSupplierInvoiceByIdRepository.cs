using ECommerce.Application.SupplierInvoices.CreateSupplierInvoice;
using ECommerce.Application.SupplierInvoices.GetSupplierInvoiceById;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Repositories.SupplierInvoices
{
    public sealed class GetSupplierInvoiceByIdRepository
    : IGetSupplierInvoiceByIdRepository
    {
        private readonly ApplicationDbContext _context;

        public GetSupplierInvoiceByIdRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SupplierInvoiceDetailsResponse?> GetByIdAsync(
            long supplierInvoiceId,
            CancellationToken cancellationToken = default)
        {
            // Step 1: Retrieve invoice with supplier and items.
            var invoice = await _context.SupplierInvoices
                .AsNoTracking()
                .Where(i => i.SupplierInvoiceId == supplierInvoiceId)
                .Select(i => new
                {
                    i.SupplierInvoiceId,
                    i.SupplierId,
                    SupplierName = i.Supplier.Name,
                    i.InvoiceNumber,
                    i.InvoiceDateUtc,
                    i.TotalAmount,
                    i.PaidAmount,
                    i.CreatedAtUtc,
                    i.UpdatedAtUtc,

                    Items = i.Items
                        .OrderBy(item => item.SupplierInvoiceItemId)
                        .Select(item => new SupplierInvoiceItemResponse(
                            item.SupplierInvoiceItemId,
                            item.ProductName,
                            item.Quantity,
                            item.UnitPrice,
                            item.Quantity * item.UnitPrice
                        ))
                        .ToList()
                })
                .SingleOrDefaultAsync(cancellationToken);

            // Step 2: Invoice not found.
            if (invoice is null)
            {
                return null;
            }

            // Step 3: Calculate the remaining balance.
            var remainingAmount =
                invoice.TotalAmount - invoice.PaidAmount;

            // Step 4: Determine current payment status.
            var paymentStatus = invoice.PaidAmount == 0m
                ? "Unpaid"
                : remainingAmount == 0m
                    ? "Paid"
                    : "PartiallyPaid";

            // Step 5: Return response.
            return new SupplierInvoiceDetailsResponse(
                invoice.SupplierInvoiceId,
                invoice.SupplierId,
                invoice.SupplierName,
                invoice.InvoiceNumber,
                invoice.InvoiceDateUtc,
                invoice.TotalAmount,
                invoice.PaidAmount,
                remainingAmount,
                paymentStatus,
                invoice.Items,
                invoice.CreatedAtUtc,
                invoice.UpdatedAtUtc
            );
        }
    }
}
