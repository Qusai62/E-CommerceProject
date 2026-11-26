using ECommerce.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.SupplierInvoices.GetSupplierInvoiceById
{
    public sealed class GetSupplierInvoiceByIdService
    {
        private readonly IGetSupplierInvoiceByIdRepository _repository;

        public GetSupplierInvoiceByIdService(
            IGetSupplierInvoiceByIdRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<SupplierInvoiceDetailsResponse>> ExecuteAsync(
            long supplierInvoiceId,
            CancellationToken cancellationToken = default)
        {
            // Step 1: Validate invoice ID.
            if (supplierInvoiceId <= 0)
            {
                return Result<SupplierInvoiceDetailsResponse>.Failure(
                    new[]
                    {
                    new Error(
                        "SupplierInvoice.InvalidId",
                        "Supplier invoice ID must be greater than zero.")
                    });
            }

            // Step 2: Retrieve invoice details.
            var invoice = await _repository.GetByIdAsync(
                supplierInvoiceId,
                cancellationToken);

            // Step 3: Handle a missing invoice.
            if (invoice is null)
            {
                return Result<SupplierInvoiceDetailsResponse>.Failure(
                    new[]
                    {
                    new Error(
                        "SupplierInvoice.NotFound",
                        "The requested supplier invoice was not found.")
                    });
            }

            // Step 4: Return the invoice.
            return Result<SupplierInvoiceDetailsResponse>.Success(
                invoice);
        }
    }
}
