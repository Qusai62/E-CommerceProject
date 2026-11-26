using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entities
{
    public class InStoreSale
    {
        public long InStoreSaleId { get; set; }
        public string SubmissionFingerprint { get; set; } = null!;

        public string SaleNumber { get; set; } = null!;

        public decimal TotalAmount { get; set; }

        public InStoreSaleStatus Status { get; set; }

        public Guid SubmissionId { get; set; }

        public DateTime SoldAtUtc { get; set; }

        public DateTime? VoidedAtUtc { get; set; }

        public DateTime UpdatedAtUtc { get; private set; }

        public byte[] RowVersion { get; private set; } = null!;


        public ICollection<InStoreSaleItem> Items { get; set; } = new List<InStoreSaleItem>();



        public void ReplaceItems(
            IReadOnlyCollection<InStoreSaleItemInput> newItems,
            DateTime updatedAtUtc)
        {
            // Step 1: Only completed sales can be edited.
            if (Status != InStoreSaleStatus.Completed)
            {
                throw new InvalidOperationException(
                    "Only completed in-store sales can be updated.");
            }

            // Step 2: Validate the timestamp.
            if (updatedAtUtc.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException(
                    "Update timestamp must be UTC.",
                    nameof(updatedAtUtc));
            }

            // Step 3: Validate the item collection.
            ArgumentNullException.ThrowIfNull(newItems);

            if (newItems.Count == 0)
            {
                throw new ArgumentException(
                    "An in-store sale must contain at least one item.",
                    nameof(newItems));
            }

            // Step 4: Validate and prepare new items first.
            var replacementItems = new List<InStoreSaleItem>();
            decimal newTotal = 0m;

            const decimal MaxMoney = 9999999999999999.99m;

            foreach (var input in newItems)
            {
                if (input is null ||
                    string.IsNullOrWhiteSpace(input.ItemName))
                {
                    throw new ArgumentException(
                        "Every item must have a valid name.");
                }

                string itemName = input.ItemName.Trim();

                if (itemName.Length > 200)
                {
                    throw new ArgumentException(
                        "Item name cannot exceed 200 characters.");
                }

                if (input.UnitPrice <= 0 ||
                    input.UnitPrice > MaxMoney ||
                    decimal.Round(input.UnitPrice, 2) != input.UnitPrice)
                {
                    throw new ArgumentException(
                        "Unit price must be positive and have at most two decimal places.");
                }

                if (input.Quantity <= 0)
                {
                    throw new ArgumentException(
                        "Item quantity must be greater than zero.");
                }

                decimal lineTotal = checked(
                    input.UnitPrice * input.Quantity);

                if (lineTotal > MaxMoney)
                {
                    throw new ArgumentException(
                        "Item line total exceeds the supported amount.");
                }

                newTotal = checked(newTotal + lineTotal);

                if (newTotal > MaxMoney)
                {
                    throw new ArgumentException(
                        "Sale total exceeds the supported amount.");
                }

                replacementItems.Add(new InStoreSaleItem
                {
                    ItemName = itemName,
                    UnitPrice = input.UnitPrice,
                    Quantity = input.Quantity,
                    LineTotal = lineTotal
                });
            }

            // Step 5: Replace items only after all validation succeeds.
            Items.Clear();

            foreach (var item in replacementItems)
            {
                Items.Add(item);
            }

            // Step 6: Update the total and modification date.
            TotalAmount = newTotal;
            UpdatedAtUtc = updatedAtUtc;
        }

    }
}
