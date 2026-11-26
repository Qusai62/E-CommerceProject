using ECommerce.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.SupplierInvoices.GetSupplierPaymentHistory
{
    public sealed class GetSupplierPaymentHistoryService
    {
        private readonly IGetSupplierPaymentHistoryRepository _repository;

        public GetSupplierPaymentHistoryService(
            IGetSupplierPaymentHistoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<SupplierPaymentHistoryResponse>> ExecuteAsync(
            long supplierInvoiceId,
            CancellationToken cancellationToken = default)
        {
            // Step 1: Validate invoice ID.
            if (supplierInvoiceId <= 0)
            {
                return Result<SupplierPaymentHistoryResponse>.Failure(
                    new[]
                    {
                    new Error(
                        "SupplierInvoice.InvalidId",
                        "Supplier invoice ID must be greater than zero.")
                    });
            }

            // Step 2: Retrieve payment history.
            var history = await _repository.GetAsync(
                supplierInvoiceId,
                cancellationToken);

            // Step 3: Check whether the invoice exists.
            if (history is null)
            {
                return Result<SupplierPaymentHistoryResponse>.Failure(
                    new[]
                    {
                    new Error(
                        "SupplierInvoice.NotFound",
                        "The requested supplier invoice was not found.")
                    });
            }

            // Step 4: Return successful result.
            return Result<SupplierPaymentHistoryResponse>.Success(
                history);
        }
    }
}
