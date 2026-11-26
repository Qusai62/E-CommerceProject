using ECommerce.Application.Common.Results;
using ECommerce.Domain.Supplier_Invoice___Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.SupplierInvoices.RecordSupplierPayment
{
    public sealed class RecordSupplierPaymentService
    {
        private readonly IRecordSupplierPaymentRepository _repository;
        private readonly TimeProvider _timeProvider;

        public RecordSupplierPaymentService(
            IRecordSupplierPaymentRepository repository,
            TimeProvider timeProvider)
        {
            _repository = repository;
            _timeProvider = timeProvider;
        }

        public async Task<Result<RecordSupplierPaymentResponse>> ExecuteAsync(
            long invoiceId,
            RecordSupplierPaymentRequest request,
            CancellationToken cancellationToken = default)
        {
            // 1. Validate request.
            if (invoiceId <= 0 ||
                request is null ||
                request.SubmissionId == Guid.Empty ||
                request.Amount <= 0 ||
                decimal.Round(request.Amount, 2) != request.Amount ||
                request.PaidAtUtc.Kind != DateTimeKind.Utc)
            {
                return Failure(
                    "SupplierPayment.InvalidRequest",
                    "Invalid supplier payment information.");
            }

            // 2. Check whether submission already exists.
            var existingPayment =
                await _repository.GetBySubmissionIdAsync(
                    request.SubmissionId,
                    cancellationToken);

            if (existingPayment is not null)
            {
                return ResolveExistingPayment(
                    existingPayment,
                    invoiceId,
                    request);
            }

            // 3. Retrieve the tracked invoice.
            var invoice = await _repository.GetInvoiceForUpdateAsync(
                invoiceId,
                cancellationToken);

            if (invoice is null)
            {
                return Failure(
                    "SupplierInvoice.NotFound",
                    "Supplier invoice was not found.");
            }

            // 4. Check remaining balance.
            if (request.Amount > invoice.RemainingAmount)
            {
                return Failure(
                    "SupplierPayment.ExceedsBalance",
                    "Payment exceeds the remaining invoice balance.");
            }

            // 5. Get current UTC time.
            var utcNow = _timeProvider.GetUtcNow().UtcDateTime;

            // 6. Record payment through Domain entity.
            var payment = invoice.RecordPayment(
                request.SubmissionId,
                request.Amount,
                request.PaidAtUtc,
                utcNow);

            // 7. Save invoice and payment atomically.
            try
            {
                await _repository.SaveChangesAsync(
                    cancellationToken);
            }
            catch (DuplicateSupplierPaymentSubmissionException)
            {
                // Another request may have saved the same submission.
                var savedPayment =
                    await _repository.GetBySubmissionIdAsync(
                        request.SubmissionId,
                        cancellationToken);

                if (savedPayment is not null)
                {
                    return ResolveExistingPayment(
                        savedPayment,
                        invoiceId,
                        request);
                }

                return Failure(
                    "SupplierPayment.SubmissionConflict",
                    "Payment submission conflict. Refresh and try again.");
            }
            catch (SupplierPaymentConcurrencyException)
            {
                return Failure(
                    "SupplierPayment.ConcurrencyConflict",
                    "Invoice balance changed. Refresh and try again.");
            }

            // 8. Return the newly recorded payment.
            return Result<RecordSupplierPaymentResponse>.Success(
                MapResponse(payment, invoice));
        }

        private static Result<RecordSupplierPaymentResponse>
            ResolveExistingPayment(
                SupplierPayment payment,
                long invoiceId,
                RecordSupplierPaymentRequest request)
        {
            // The same submission ID must refer to the same operation.
            if (payment.SupplierInvoiceId != invoiceId ||
                payment.Amount != request.Amount ||
                payment.PaidAtUtc != request.PaidAtUtc)
            {
                return Failure(
                    "SupplierPayment.SubmissionConflict",
                    "Submission ID was already used for different payment details.");
            }

            return Result<RecordSupplierPaymentResponse>.Success(
                MapResponse(payment, payment.SupplierInvoice));
        }

        private static RecordSupplierPaymentResponse MapResponse(
            SupplierPayment payment,
            SupplierInvoice invoice)
        {
            return new RecordSupplierPaymentResponse(
                payment.SupplierPaymentId,
                invoice.SupplierInvoiceId,
                payment.SubmissionId,
                payment.Amount,
                payment.PaidAtUtc,
                payment.RecordedAtUtc,
                invoice.TotalAmount,
                invoice.PaidAmount,
                invoice.RemainingAmount,
                invoice.PaymentStatus);
        }

        private static Result<RecordSupplierPaymentResponse> Failure(
            string code,
            string message)
        {
            return Result<RecordSupplierPaymentResponse>.Failure(
                new[] { new Error(code, message) });
        }
    }
}
