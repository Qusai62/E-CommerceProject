using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.InStoreSales.UpdateInStoreSale
{

    public sealed class UpdateInStoreSaleService
    {
        private readonly IUpdateInStoreSaleRepository _repository;
        private readonly TimeProvider _timeProvider;

        public UpdateInStoreSaleService(
            IUpdateInStoreSaleRepository repository,
            TimeProvider timeProvider)
        {
            _repository = repository;
            _timeProvider = timeProvider;
        }

        public async Task<UpdateInStoreSaleResult> ExecuteAsync(
            long inStoreSaleId,
            UpdateInStoreSaleRequest request,
            CancellationToken cancellationToken = default)
        {
            // Step 1: Validate request.
            ArgumentNullException.ThrowIfNull(request);

            if (inStoreSaleId <= 0)
            {
                throw new ArgumentException(
                    "Invalid in-store sale ID.");
            }

            if (request.Items is null || request.Items.Count == 0)
            {
                throw new ArgumentException(
                    "At least one sale item is required.");
            }

            // Step 2: Decode the concurrency token.
            if (string.IsNullOrWhiteSpace(request.RowVersion))
            {
                throw new ArgumentException(
                    "RowVersion is required.");
            }

            byte[] expectedRowVersion;

            try
            {
                expectedRowVersion =
                    Convert.FromBase64String(request.RowVersion);
            }
            catch (FormatException)
            {
                throw new ArgumentException(
                    "RowVersion must be a valid Base64 string.");
            }

            if (expectedRowVersion.Length != 8)
            {
                throw new ArgumentException(
                    "Invalid RowVersion length.");
            }

            // Step 3: Prepare Domain inputs.
            var newItems = request.Items
                .Select(item => item is null
                    ? throw new ArgumentException(
                        "Sale items cannot be null.")
                    : new InStoreSaleItemInput(
                        item.ItemName,
                        item.UnitPrice,
                        item.Quantity))
                .ToList();

            // Step 4: Load the existing sale and its items.
            var sale = await _repository.GetByIdForUpdateAsync(
                inStoreSaleId,
                cancellationToken);

            if (sale is null)
            {
                return new UpdateInStoreSaleResult(
                    UpdateInStoreSaleOutcome.NotFound,
                    null);
            }

            // Step 5: Check whether the sale is editable.
            if (sale.Status != InStoreSaleStatus.Completed)
            {
                return new UpdateInStoreSaleResult(
                    UpdateInStoreSaleOutcome.NotEditable,
                    null);
            }

            // Step 6: Check for an already outdated version.
            if (!sale.RowVersion.AsSpan()
                .SequenceEqual(expectedRowVersion))
            {
                return new UpdateInStoreSaleResult(
                    UpdateInStoreSaleOutcome.ConcurrencyConflict,
                    null);
            }

            // Step 7: Update the Domain aggregate.
            var utcNow = _timeProvider.GetUtcNow().UtcDateTime;

            sale.ReplaceItems(newItems, utcNow);

            // Step 8: Persist the update.
            bool saved = await _repository.TrySaveAsync(
                sale,
                expectedRowVersion,
                cancellationToken);

            if (!saved)
            {
                return new UpdateInStoreSaleResult(
                    UpdateInStoreSaleOutcome.ConcurrencyConflict,
                    null);
            }

            // Step 9: Return the updated sale.
            var response = new UpdateInStoreSaleResponse(
                InStoreSaleId: sale.InStoreSaleId,
                SaleNumber: sale.SaleNumber,
                TotalAmount: sale.TotalAmount,
                Status: sale.Status,
                SoldAtUtc: sale.SoldAtUtc,
                RowVersion: Convert.ToBase64String(sale.RowVersion));

            return new UpdateInStoreSaleResult(
                UpdateInStoreSaleOutcome.Updated,
                response);
        }
    }

}
