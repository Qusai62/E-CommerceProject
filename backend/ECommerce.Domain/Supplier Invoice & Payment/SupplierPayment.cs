using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Supplier_Invoice___Payment
{
    public class SupplierPayment
    {
        public long SupplierPaymentId { get; private set; }
        public Guid SubmissionId { get; private set; }

        public long SupplierInvoiceId { get; private set; }

        public decimal Amount { get; private set; }

        public DateTime PaidAtUtc { get; private set; }

        public DateTime RecordedAtUtc { get; private set; }

        public SupplierInvoice SupplierInvoice
        { get; private set; } = null!;

        private SupplierPayment() { }

        internal SupplierPayment(
      Guid submissionId,
      decimal amount,
      DateTime paidAtUtc,
      DateTime recordedAtUtc)
        {
            if (submissionId == Guid.Empty)
            {
                throw new ArgumentException(
                    "Payment submission ID is required.",
                    nameof(submissionId));
            }

            if (amount <= 0 ||
                decimal.Round(amount, 2) != amount)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(amount),
                    "Payment amount must be positive with at most two decimal places.");
            }

            if (paidAtUtc.Kind != DateTimeKind.Utc ||
                recordedAtUtc.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException(
                    "Payment timestamps must be UTC.");
            }

            SubmissionId = submissionId;
            Amount = amount;
            PaidAtUtc = paidAtUtc;
            RecordedAtUtc = recordedAtUtc;
        }
    }
}
