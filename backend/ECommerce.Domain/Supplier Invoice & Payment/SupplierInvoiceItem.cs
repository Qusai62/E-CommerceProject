using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Supplier_Invoice___Payment
{
    public class SupplierInvoiceItem
    {
        public long SupplierInvoiceItemId { get; private set; }

        public long SupplierInvoiceId { get; private set; }

        public string ProductName { get; private set; } = null!;

        public int Quantity { get; private set; }

        public decimal UnitPrice { get; private set; }

        public decimal LineTotal => Quantity * UnitPrice;

        public SupplierInvoice SupplierInvoice { get; private set; } = null!;

        private SupplierInvoiceItem() { }

        internal SupplierInvoiceItem(
            string productName,
            int quantity,
            decimal unitPrice)
        {
            if (string.IsNullOrWhiteSpace(productName) ||
                productName.Trim().Length > 200)
            {
                throw new ArgumentException(
                    "Product name must be between 1 and 200 characters.");
            }

            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(quantity),
                    "Quantity must be greater than zero.");
            }

            if (unitPrice <= 0 ||
                decimal.Round(unitPrice, 2) != unitPrice)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(unitPrice),
                    "Unit price must be positive with at most two decimal places.");
            }

            ProductName = productName.Trim();
            Quantity = quantity;
            UnitPrice = unitPrice;
        }
    }
}
