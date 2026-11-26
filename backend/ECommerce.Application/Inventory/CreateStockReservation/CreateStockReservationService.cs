using ECommerce.Application.Common.Results;
using ECommerce.Application.Inventory.Abstractions;
using System;
using Microsoft.Extensions.Options;

namespace ECommerce.Application.Inventory.CreateStockReservation
{
    public class CreateStockReservationService
    {
        private readonly IStockReservationWriteRepository _repository;
        private readonly TimeProvider _timeProvider;
        private readonly StockReservationOptions _options;

        public CreateStockReservationService(
            IStockReservationWriteRepository repository,
            TimeProvider timeProvider,
            IOptions<StockReservationOptions> options)
        {
            _repository = repository;
            _timeProvider = timeProvider;
            _options = options.Value;
        }

        public async Task<Result<CreateStockReservationResponse>> ExecuteAsync(
            CreateStockReservationCommand command,
            CancellationToken cancellationToken = default)
        {
            // 1. Validate the command
            ArgumentNullException.ThrowIfNull(command);

            if (command.PaymentAttemptId <= 0)
            {
                return Failure(
                    "Inventory.InvalidPaymentAttemptId",
                    "PaymentAttemptId must be greater than zero.",
                    nameof(command.PaymentAttemptId));
            }

            // 2. Get the current UTC time
            DateTime utcNow =
                _timeProvider.GetUtcNow().UtcDateTime;

            // 3. Calculate the expiration timestamp
            DateTime expiresAtUtc =
                utcNow.AddMinutes(_options.DurationMinutes);

            // 4. Execute the reservation operation
            var outcome = await _repository.CreateAsync(
                command.PaymentAttemptId,
                utcNow,
                expiresAtUtc,
                cancellationToken);

            // 5. Convert repository outcome into Application Result
            return outcome switch
            {
                CreateStockReservationOutcome.Created =>
                    Result<CreateStockReservationResponse>.Success(
                        new CreateStockReservationResponse(
                            command.PaymentAttemptId,
                            expiresAtUtc)),

                CreateStockReservationOutcome.PaymentAttemptNotFound =>
                    Failure(
                        "Inventory.PaymentAttemptNotFound",
                        "The payment attempt was not found."),

                CreateStockReservationOutcome.PaymentNotEligible =>
                    Failure(
                        "Inventory.PaymentNotEligible",
                        "The payment or payment attempt is not eligible for reservation."),

                CreateStockReservationOutcome.OrderNotEligible =>
                    Failure(
                        "Inventory.OrderNotEligible",
                        "The order is not eligible for stock reservation."),

                CreateStockReservationOutcome.OrderHasNoItems =>
                    Failure(
                        "Inventory.OrderHasNoItems",
                        "The order contains no items."),

                CreateStockReservationOutcome.AlreadyReserved =>
                    Failure(
                        "Inventory.AlreadyReserved",
                        "A conflicting stock reservation already exists."),

                CreateStockReservationOutcome.InsufficientStock =>
                    Failure(
                        "Inventory.InsufficientStock",
                        "There is insufficient available stock for one or more variants."),

                _ => throw new InvalidOperationException(
                    $"Unexpected reservation outcome: {outcome}")
            };
        }

        private static Result<CreateStockReservationResponse> Failure(
            string code,
            string message,
            string? field = null)
        {
            return Result<CreateStockReservationResponse>.Failure(
                new[]
                {
                new Error(code, message, field)
                });
        }
    }
}
