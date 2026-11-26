using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Supplier_Invoice___Payment
{
    public class Supplier
    {
        public long SupplierId { get; private set; }

        public string Name { get; private set; } = null!;

        public string? PhoneNumber { get; private set; }

        public string? Email { get; private set; }

        public DateTime CreatedAtUtc { get; private set; }

        public DateTime UpdatedAtUtc { get; private set; }

        public ICollection<SupplierInvoice> Invoices
        { get; private set; } = new List<SupplierInvoice>();

        private Supplier() { }

        public Supplier(
            string name,
            string? phoneNumber,
            string? email,
            DateTime utcNow)
        {
            if (string.IsNullOrWhiteSpace(name) ||
                name.Trim().Length > 200)
            {
                throw new ArgumentException(
                    "Supplier name must be between 1 and 200 characters.");
            }

            if (phoneNumber?.Length > 30)
                throw new ArgumentException("Phone number is too long.");

            if (email?.Length > 254)
                throw new ArgumentException("Email is too long.");

            if (utcNow.Kind != DateTimeKind.Utc)
                throw new ArgumentException("Timestamp must be UTC.");

            Name = name.Trim();
            PhoneNumber = phoneNumber?.Trim();
            Email = email?.Trim();

            CreatedAtUtc = utcNow;
            UpdatedAtUtc = utcNow;
        }

        public void UpdateDetails(
    string name,
    string? phoneNumber,
    string? email,
    DateTime updatedAtUtc)
        {
            if (string.IsNullOrWhiteSpace(name) ||
                name.Trim().Length > 200)
            {
                throw new ArgumentException(
                    "Supplier name must be between 1 and 200 characters.");
            }

            if (phoneNumber?.Length > 30)
            {
                throw new ArgumentException(
                    "Phone number cannot exceed 30 characters.");
            }

            if (email?.Length > 254)
            {
                throw new ArgumentException(
                    "Email cannot exceed 254 characters.");
            }

            if (updatedAtUtc.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException(
                    "Timestamp must be UTC.");
            }

            Name = name.Trim();
            PhoneNumber = phoneNumber?.Trim();
            Email = email?.Trim();
            UpdatedAtUtc = updatedAtUtc;
        }
    }
}
