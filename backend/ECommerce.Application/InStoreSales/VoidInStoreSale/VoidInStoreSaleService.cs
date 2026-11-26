using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.InStoreSales.VoidInStoreSale
{
    public sealed class VoidInStoreSaleService
    {
        private readonly IVoidInStoreSaleRepository _repository;
        private readonly TimeProvider _timeProvider;

        public VoidInStoreSaleService(
            IVoidInStoreSaleRepository repository,
            TimeProvider timeProvider)
        {
            _repository = repository;
            _timeProvider = timeProvider;
        }

        public async Task<VoidInStoreSaleResult> ExecuteAsync(
            VoidInStoreSaleRequest request,
            CancellationToken cancellationToken = default)
        {
            // Step 1: Validate request
            ArgumentNullException.ThrowIfNull(request);

            if (request.InStoreSaleId <= 0)
            {
                throw new ArgumentException(
                    "InStoreSaleId must be greater than zero.");
            }

            // Step 2: Get current UTC time
            DateTime utcNow =
                _timeProvider.GetUtcNow().UtcDateTime;

            // Step 3: Attempt atomic status transition
            bool voided = await _repository.TryVoidAsync(
                request.InStoreSaleId,
                utcNow,
                cancellationToken);

            // Step 4: Successfully voided
            if (voided)
            {
                var response = new VoidInStoreSaleResponse(
                    request.InStoreSaleId,
                    InStoreSaleStatus.Voided,
                    utcNow);

                return new VoidInStoreSaleResult(
                    VoidInStoreSaleOutcome.Voided,
                    response);
            }

            // Step 5: No row was updated.
            // Determine the current database state.
            var status = await _repository.GetStatusAsync(
                request.InStoreSaleId,
                cancellationToken);

            // Step 6: Sale doesn't exist
            if (status is null)
            {
                return new VoidInStoreSaleResult(
                    VoidInStoreSaleOutcome.NotFound,
                    null);
            }

            // Step 7: Sale already voided
            if (status == InStoreSaleStatus.Voided)
            {
                return new VoidInStoreSaleResult(
                    VoidInStoreSaleOutcome.AlreadyVoided,
                    null);
            }

            // Step 8: Unexpected state
            throw new InvalidOperationException(
                "The sale could not be voided. " +
                "Its current state must be rechecked.");
        }
    }
}
