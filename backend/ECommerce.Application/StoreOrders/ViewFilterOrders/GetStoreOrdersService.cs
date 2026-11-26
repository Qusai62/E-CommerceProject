using ECommerce.Application.Common.Pagination;
using ECommerce.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.StoreOrders.ViewOrders
{
    public sealed class GetStoreOrdersService
    {
        private const int MaxPageSize = 100;

        private readonly IStoreOrderReadRepository _repository;

        public GetStoreOrdersService(
            IStoreOrderReadRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<PagedResult<StoreOrderResponse>>>
            ExecuteAsync(
                GetStoreOrdersRequest request,
                CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            var errors = new List<Error>();

            // Step 1: Validate pagination.
            if (request.PageNumber < 1)
            {
                errors.Add(new Error(
                    "StoreOrders.InvalidPageNumber",
                    "Page number must be greater than zero.",
                    nameof(request.PageNumber)));
            }

            if (request.PageSize < 1 ||
                request.PageSize > MaxPageSize)
            {
                errors.Add(new Error(
                    "StoreOrders.InvalidPageSize",
                    "Page size must be between 1 and 100.",
                    nameof(request.PageSize)));
            }

            // Step 2: Validate fulfillment status.
            if (request.FulfillmentStatus.HasValue &&
                !Enum.IsDefined(request.FulfillmentStatus.Value))
            {
                errors.Add(new Error(
                    "StoreOrders.InvalidFulfillmentStatus",
                    "Invalid fulfillment status.",
                    nameof(request.FulfillmentStatus)));
            }

            // Step 3: Validate payment status.
            if (request.PaymentStatus.HasValue &&
                !Enum.IsDefined(request.PaymentStatus.Value))
            {
                errors.Add(new Error(
                    "StoreOrders.InvalidPaymentStatus",
                    "Invalid payment status.",
                    nameof(request.PaymentStatus)));
            }

            // Step 4: Validate date range.
            if (request.StartDate.HasValue &&
                request.EndDate.HasValue &&
                request.StartDate.Value > request.EndDate.Value)
            {
                errors.Add(new Error(
                    "StoreOrders.InvalidDateRange",
                    "Start date cannot be later than end date.",
                    nameof(request.StartDate)));
            }

            // Step 5: Validate exclusive end-date conversion.
            if (request.EndDate == DateOnly.MaxValue)
            {
                errors.Add(new Error(
                    "StoreOrders.InvalidEndDate",
                    "End date is outside the supported range.",
                    nameof(request.EndDate)));
            }

            // Step 6: Prevent pagination offset overflow.
            if (request.PageNumber > 0 &&
                request.PageSize > 0 &&
                (long)(request.PageNumber - 1) *
                request.PageSize > int.MaxValue)
            {
                errors.Add(new Error(
                    "StoreOrders.InvalidPagination",
                    "Pagination offset is too large.",
                    nameof(request.PageNumber)));
            }

            // Step 7: Return validation errors.
            if (errors.Count > 0)
            {
                return Result<PagedResult<StoreOrderResponse>>
                    .Failure(errors);
            }

            // Step 8: Convert start date to UTC.
            DateTime? startUtc = request.StartDate?
                .ToDateTime(
                    TimeOnly.MinValue,
                    DateTimeKind.Utc);

            // Step 9: Convert end date to an exclusive UTC bound.
            DateTime? endExclusiveUtc = request.EndDate?
                .AddDays(1)
                .ToDateTime(
                    TimeOnly.MinValue,
                    DateTimeKind.Utc);

            // Step 10: Create validated repository criteria.
            var criteria = new StoreOrderQueryCriteria(
                request.PageNumber,
                request.PageSize,
                request.FulfillmentStatus,
                request.PaymentStatus,
                startUtc,
                endExclusiveUtc
            );

            // Step 11: Retrieve the filtered orders.
            var orders = await _repository.GetPagedAsync(
                criteria,
                cancellationToken);

            // Step 12: Return the successful result.
            return Result<PagedResult<StoreOrderResponse>>
                .Success(orders);

        }
    }
}