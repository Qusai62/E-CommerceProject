using ECommerce.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.SupplierInvoices.ListSupplierInvoices
{
    public sealed class ListSupplierInvoicesService
    {
        private readonly IListSupplierInvoicesRepository _repository;

        public ListSupplierInvoicesService(
            IListSupplierInvoicesRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<PagedSupplierInvoicesResponse>> ExecuteAsync(
            ListSupplierInvoicesRequest request,
            CancellationToken cancellationToken = default)
        {
            if (request is null)
            {
                return Failure(
                    "SupplierInvoice.InvalidRequest",
                    "Request is required.");
            }

            // Step 1: Validate optional supplier filter.
            if (request.SupplierId.HasValue &&
                request.SupplierId.Value <= 0)
            {
                return Failure(
                    "SupplierInvoice.InvalidSupplierId",
                    "Supplier ID must be greater than zero.");
            }

            // Step 2: Validate pagination.
            if (request.PageNumber < 1 ||
                request.PageSize is < 1 or > 100)
            {
                return Failure(
                    "SupplierInvoice.InvalidPagination",
                    "Invalid page number or page size.");
            }

            // Step 3: Validate search text.
            if (request.Search?.Length > 100)
            {
                return Failure(
                    "SupplierInvoice.InvalidSearch",
                    "Search cannot exceed 100 characters.");
            }

            // Step 4: Normalize payment status.
            var status = request.PaymentStatus?
                .Trim()
                .ToLowerInvariant();

            if (!string.IsNullOrEmpty(status) &&
                status is not ("unpaid" or "partiallypaid" or "paid"))
            {
                return Failure(
                    "SupplierInvoice.InvalidPaymentStatus",
                    "Payment status must be Unpaid, PartiallyPaid, or Paid.");
            }

            // Step 5: Validate sort field.
            var sortBy = request.SortBy?
                .Trim()
                .ToLowerInvariant();

            if (sortBy is not
                ("invoicedate" or "totalamount" or "remainingamount"))
            {
                return Failure(
                    "SupplierInvoice.InvalidSort",
                    "Invalid invoice sort field.");
            }

            // Step 6: Prevent pagination offset overflow.
            var offset =
                ((long)request.PageNumber - 1) * request.PageSize;

            if (offset > int.MaxValue)
            {
                return Failure(
                    "SupplierInvoice.InvalidPagination",
                    "Requested page exceeds the supported range.");
            }

            // Step 7: Normalize the request.
            var normalizedRequest = request with
            {
                Search = request.Search?.Trim(),
                PaymentStatus = status,
                SortBy = sortBy
            };

            // Step 8: Execute repository query.
            var response = await _repository.GetPagedAsync(
                normalizedRequest,
                cancellationToken);

            return Result<PagedSupplierInvoicesResponse>.Success(
                response);
        }

        private static Result<PagedSupplierInvoicesResponse> Failure(
            string code,
            string message)
        {
            return Result<PagedSupplierInvoicesResponse>.Failure(
                new[] { new Error(code, message) });
        }
    }
}
