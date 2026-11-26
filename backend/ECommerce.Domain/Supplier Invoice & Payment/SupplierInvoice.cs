using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Supplier_Invoice___Payment
{
    public class SupplierInvoice
    {
        public long SupplierInvoiceId { get; private set; }

        public long SupplierId { get; private set; }

        public string InvoiceNumber { get; private set; } = null!;

        public decimal TotalAmount { get; private set; }

        public decimal PaidAmount { get; private set; }

        public decimal RemainingAmount => TotalAmount - PaidAmount;

        public DateTime InvoiceDateUtc { get; private set; }

        public DateTime CreatedAtUtc { get; private set; }

        public DateTime UpdatedAtUtc { get; private set; }

        public byte[] RowVersion { get; private set; } = null!;

        public Supplier Supplier { get; private set; } = null!;

        private readonly List<SupplierPayment> _payments = new();

        public IReadOnlyCollection<SupplierPayment> Payments =>
            _payments.AsReadOnly();

        private readonly List<SupplierInvoiceItem> _items = new();

        public IReadOnlyCollection<SupplierInvoiceItem> Items =>
            _items.AsReadOnly();

        public string PaymentStatus =>
            PaidAmount == 0 ? "Unpaid" :
            RemainingAmount == 0 ? "Paid" :
            "PartiallyPaid";

        private SupplierInvoice() { }

        public SupplierInvoice(
     long supplierId,
     string invoiceNumber,
     IReadOnlyCollection<SupplierInvoiceItemInput> items,
     DateTime invoiceDateUtc,
     DateTime utcNow)
        {
            if (supplierId <= 0)
                throw new ArgumentOutOfRangeException(nameof(supplierId));

            if (string.IsNullOrWhiteSpace(invoiceNumber) ||
                invoiceNumber.Trim().Length > 100)
                throw new ArgumentException("Invalid invoice number.");

            ArgumentNullException.ThrowIfNull(items);

            if (items.Count == 0)
                throw new ArgumentException(
                    "Invoice must contain at least one item.");

            if (invoiceDateUtc.Kind != DateTimeKind.Utc ||
                utcNow.Kind != DateTimeKind.Utc)
                throw new ArgumentException(
                    "Timestamps must be UTC.");

            SupplierId = supplierId;
            InvoiceNumber = invoiceNumber.Trim();

            InvoiceDateUtc = invoiceDateUtc;
            CreatedAtUtc = utcNow;
            UpdatedAtUtc = utcNow;

            decimal totalAmount = 0m;

            foreach (var input in items)
            {
                var item = new SupplierInvoiceItem(
                    input.ProductName,
                    input.Quantity,
                    input.UnitPrice);

                totalAmount = checked(totalAmount + item.LineTotal);

                _items.Add(item);
            }

            if (totalAmount > 9999999999999999.99m)
                throw new ArgumentOutOfRangeException(
                    nameof(items),
                    "Invoice total exceeds the supported database precision.");

            TotalAmount = totalAmount;
            PaidAmount = 0m;
        }
        public SupplierPayment RecordPayment(
     Guid submissionId,
     decimal amount,
     DateTime paidAtUtc,
     DateTime recordedAtUtc)
        {
            // 1. Validate submission ID.
            if (submissionId == Guid.Empty)
            {
                throw new ArgumentException(
                    "Payment submission ID is required.",
                    nameof(submissionId));
            }

            // 2. Validate payment amount.
            if (amount <= 0 ||
                decimal.Round(amount, 2) != amount)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(amount),
                    "Payment amount must be positive with at most two decimal places.");
            }

            // 3. Validate UTC timestamps.
            if (paidAtUtc.Kind != DateTimeKind.Utc ||
                recordedAtUtc.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException(
                    "Payment timestamps must be UTC.");
            }

            // 4. Ensure payment does not exceed the balance.
            if (amount > RemainingAmount)
            {
                throw new InvalidOperationException(
                    "Payment exceeds the remaining invoice balance.");
            }

            // 5. Create payment history record.
            var payment = new SupplierPayment(
                submissionId,
                amount,
                paidAtUtc,
                recordedAtUtc);

            // 6. Add payment to invoice aggregate.
            _payments.Add(payment);

            // 7. Update invoice financial balance.
            PaidAmount += amount;

            // 8. Update timestamp.
            UpdatedAtUtc = recordedAtUtc;

            return payment;
        }
    }
}
