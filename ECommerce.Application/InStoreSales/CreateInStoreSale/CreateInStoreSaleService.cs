using ECommerce.Application.Common.Results;
using ECommerce.Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.InStoreSales.CreateInStoreSale
{
    public sealed class CreateInStoreSaleService
    {
        private const decimal MaxAmount =
            9999999999999999.99m;

        private readonly ICreateInStoreSaleRepository _repository;
        private readonly TimeProvider _timeProvider;
        private readonly ILogger<CreateInStoreSaleService> _logger;
        public CreateInStoreSaleService(
            ICreateInStoreSaleRepository repository,
            TimeProvider timeProvider,
            ILogger<CreateInStoreSaleService> logger)
        {
            _repository = repository;
            _timeProvider = timeProvider;
            _logger = logger;
        }

        public async Task<CreateInStoreSaleResult> ExecuteAsync(
            CreateInStoreSaleRequest request,
            CancellationToken cancellationToken = default)
        {
            // 1. Validate request
            if (request is null)
                throw new ArgumentException("Request is required.");

            if (request.SubmissionId == Guid.Empty)
                throw new ArgumentException(
                    "SubmissionId is required.");

            if (request.Items is null || request.Items.Count == 0)
                throw new ArgumentException(
                    "At least one item is required.");

            // 2. Validate and normalize items
            var items = new List<InStoreSaleItem>();

            foreach (var item in request.Items)
            {
                if (item is null)
                    throw new ArgumentException(
                        "Sale item cannot be null.");

                var name = item.ItemName?.Trim();

                if (string.IsNullOrWhiteSpace(name) ||
                    name.Length > 200)
                {
                    throw new ArgumentException(
                        "Item name must contain 1-200 characters.");
                }

                if (item.UnitPrice <= 0 ||
                    item.UnitPrice > MaxAmount ||
                    decimal.Round(item.UnitPrice, 2) != item.UnitPrice)
                {
                    throw new ArgumentException(
                        "Invalid unit price.");
                }

                if (item.Quantity <= 0)
                    throw new ArgumentException(
                        "Quantity must be greater than zero.");

                decimal lineTotal;

                try
                {
                    lineTotal = checked(
                        item.UnitPrice * item.Quantity);
                }
                catch (OverflowException)
                {
                    throw new ArgumentException(
                        "Line total is too large.");
                }

                if (lineTotal > MaxAmount)
                    throw new ArgumentException(
                        "Line total exceeds the allowed amount.");

                items.Add(new InStoreSaleItem
                {
                    ItemName = name,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity,
                    LineTotal = lineTotal
                });
            }

            // 3. Calculate total
            decimal totalAmount;

            try
            {
                totalAmount = checked(
                    items.Sum(x => x.LineTotal));
            }
            catch (OverflowException)
            {
                throw new ArgumentException(
                    "Sale total is too large.");
            }

            if (totalAmount > MaxAmount)
                throw new ArgumentException(
                    "Sale total exceeds the allowed amount.");

            // 4. Calculate fingerprint
            string fingerprint =
                CreateInStoreSaleFingerprint.Compute(items);

            // 5. Check existing submission
            var existingSale =
                await _repository.GetBySubmissionIdAsync(
                    request.SubmissionId,
                    cancellationToken);

            if (existingSale is not null)
            {
                return ResolveExisting(
                    existingSale,
                    fingerprint);
            }

            // 6. Create new sale
            var sale = new InStoreSale
            {
                SubmissionId = request.SubmissionId,
                SubmissionFingerprint = fingerprint,
                SaleNumber = $"POS-{Guid.NewGuid():N}",
                TotalAmount = totalAmount,
                Status = InStoreSaleStatus.Completed,
                SoldAtUtc = _timeProvider.GetUtcNow().UtcDateTime,
                Items = items
            };

            // 7. Attempt insertion
            bool created = await _repository.TryAddAsync(
                sale,
                cancellationToken);

            if (created)
            {
                return new CreateInStoreSaleResult(
                    ToResponse(sale),
                    Created: true);
            }

            // 8. Concurrent request inserted first
            var savedSale =
                await _repository.GetBySubmissionIdAsync(
                    request.SubmissionId,
                    cancellationToken);
            
            
                _logger.LogInformation(
                    "In-store sale completed successfully. SaleId: {SaleId}",
                    savedSale.SaleNumber);
            
            if (savedSale is null)
            {
                throw new InvalidOperationException(
                    "Could not resolve the duplicate submission.");
            }

            return ResolveExisting(savedSale, fingerprint);
        }

        private static CreateInStoreSaleResult ResolveExisting(
            InStoreSale existing,
            string fingerprint)
        {
            if (existing.SubmissionFingerprint != fingerprint)
            {
                throw new InvalidOperationException(
                    "SubmissionId was already used for " +
                    "a different sale.");
            }

            return new CreateInStoreSaleResult(
                ToResponse(existing),
                Created: false);
        }

        private static CreateInStoreSaleResponse ToResponse(
            InStoreSale sale)
        {
            return new CreateInStoreSaleResponse(
                sale.InStoreSaleId,
                sale.SaleNumber,
                sale.TotalAmount,
                sale.Status,
                sale.SoldAtUtc);
        }
    }
}
