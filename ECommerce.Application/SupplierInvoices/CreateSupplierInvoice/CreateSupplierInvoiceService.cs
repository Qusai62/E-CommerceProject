using ECommerce.Application.Common.Results;
using ECommerce.Domain.Supplier_Invoice___Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.SupplierInvoices.CreateSupplierInvoice
{
    public sealed class CreateSupplierInvoiceService
    {
        private const decimal MaxAmount = 9999999999999999.99m;

        private readonly ICreateSupplierInvoiceRepository _repository;
        private readonly TimeProvider _timeProvider;

        public CreateSupplierInvoiceService(
            ICreateSupplierInvoiceRepository repository,
            TimeProvider timeProvider)
        {
            _repository = repository;
            _timeProvider = timeProvider;
        }

        public async Task<Result<CreateSupplierInvoiceResponse>> ExecuteAsync(
            CreateSupplierInvoiceRequest request,
            CancellationToken cancellationToken = default)
        {
            // Step 1: Validate invoice information.
            if (request is null ||
                request.SupplierId <= 0 ||
                string.IsNullOrWhiteSpace(request.InvoiceNumber) ||
                request.InvoiceNumber.Trim().Length > 100 ||
                request.InvoiceDateUtc.Kind != DateTimeKind.Utc)
            {
                return Failure(
                    "SupplierInvoice.InvalidRequest",
                    "Supplier invoice information is invalid.");
            }

            // Step 2: Validate the item collection.
            if (request.Items is null || request.Items.Count == 0)
            {
                return Failure(
                    "SupplierInvoice.EmptyItems",
                    "Invoice must contain at least one item.");
            }

            // Step 3: Validate each item and total amount.
            decimal total = 0m;

            foreach (var item in request.Items)
            {
                if (item is null ||
                    string.IsNullOrWhiteSpace(item.ProductName) ||
                    item.ProductName.Trim().Length > 200 ||
                    item.Quantity <= 0 ||
                    item.UnitPrice <= 0 ||
                    decimal.Round(item.UnitPrice, 2) != item.UnitPrice)
                {
                    return Failure(
                        "SupplierInvoice.InvalidItem",
                        "One or more invoice items are invalid.");
                }

                // Avoid numeric overflow and exceeding SQL precision.
                if (item.UnitPrice > MaxAmount / item.Quantity)
                {
                    return Failure(
                        "SupplierInvoice.InvalidTotal",
                        "Invoice amount exceeds the supported limit.");
                }

                var lineTotal = item.Quantity * item.UnitPrice;

                if (lineTotal > MaxAmount - total)
                {
                    return Failure(
                        "SupplierInvoice.InvalidTotal",
                        "Invoice amount exceeds the supported limit.");
                }

                total += lineTotal;
            }

            // Step 4: Verify that the supplier exists.
            var supplierExists = await _repository.SupplierExistsAsync(
                request.SupplierId,
                cancellationToken);

            if (!supplierExists)
            {
                return Failure(
                    "Supplier.NotFound",
                    "The selected supplier does not exist.");
            }

            // Step 5: Check duplicate invoice number.
            var invoiceNumber = request.InvoiceNumber.Trim();

            var duplicate = await _repository.InvoiceNumberExistsAsync(
                request.SupplierId,
                invoiceNumber,
                cancellationToken);

            if (duplicate)
            {
                return Failure(
                    "SupplierInvoice.DuplicateNumber",
                    "This supplier already has an invoice with this number.");
            }

            // Step 6: Convert request items into Domain inputs.
            var domainItems = request.Items
                .Select(i => new SupplierInvoiceItemInput(
                    i.ProductName,
                    i.Quantity,
                    i.UnitPrice))
                .ToList();

            // Step 7: Generate the current UTC timestamp.
            var utcNow = _timeProvider.GetUtcNow().UtcDateTime;

            // Step 8: Create invoice through the Domain constructor.
            var invoice = new SupplierInvoice(
                request.SupplierId,
                invoiceNumber,
                domainItems,
                request.InvoiceDateUtc,
                utcNow);

            // Step 9: Track and save the invoice aggregate.
            try
            {
                await _repository.AddAsync(
                    invoice,
                    cancellationToken);

                await _repository.SaveChangesAsync(
                    cancellationToken);
            }
            catch (DuplicateSupplierInvoiceException)
            {
                return Failure(
                    "SupplierInvoice.DuplicateNumber",
                    "This supplier already has an invoice with this number.");
            }

            // Step 10: Map purchased items to the response.
            var itemResponses = invoice.Items
                .Select(i => new SupplierInvoiceItemResponse(
                    i.SupplierInvoiceItemId,
                    i.ProductName,
                    i.Quantity,
                    i.UnitPrice,
                    i.LineTotal))
                .ToList();

            // Step 11: Build response.
            var response = new CreateSupplierInvoiceResponse(
                invoice.SupplierInvoiceId,
                invoice.SupplierId,
                invoice.InvoiceNumber,
                invoice.InvoiceDateUtc,
                invoice.TotalAmount,
                invoice.PaidAmount,
                invoice.RemainingAmount,
                invoice.PaymentStatus,
                itemResponses,
                invoice.CreatedAtUtc);

            // Step 12: Return success.
            return Result<CreateSupplierInvoiceResponse>.Success(
                response);
        }

        private static Result<CreateSupplierInvoiceResponse> Failure(
            string code,
            string message)
        {
            return Result<CreateSupplierInvoiceResponse>.Failure(
                new[] { new Error(code, message) });
        }
    }
}
