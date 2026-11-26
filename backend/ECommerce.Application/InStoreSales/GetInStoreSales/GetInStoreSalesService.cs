using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.InStoreSales.GetInStoreSales
{
    public sealed class GetInStoreSalesService
    {
        private const int MaxPageSize = 100;

        private readonly IGetInStoreSalesRepository _repository;

        public GetInStoreSalesService(
            IGetInStoreSalesRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetInStoreSalesResponse> ExecuteAsync(
            GetInStoreSalesRequest request,
            CancellationToken cancellationToken = default)
        {
            // 1. Validate pagination
            if (request.PageNumber < 1)
            {
                throw new ArgumentException(
                    "PageNumber must be greater than zero.");
            }

            if (request.PageSize < 1 ||
                request.PageSize > MaxPageSize)
            {
                throw new ArgumentException(
                    $"PageSize must be between 1 and {MaxPageSize}.");
            }

            // 2. Validate date range
            if (request.FromUtc.HasValue &&
                request.ToUtc.HasValue &&
                request.FromUtc.Value >= request.ToUtc.Value)
            {
                throw new ArgumentException(
                    "FromUtc must be earlier than ToUtc.");
            }

            // 3. Validate status
            if (request.Status.HasValue &&
                !Enum.IsDefined(request.Status.Value))
            {
                throw new ArgumentException(
                    "Invalid in-store sale status.");
            }

            // 4. Validate pagination offset
            long skip =
                ((long)request.PageNumber - 1) * request.PageSize;

            if (skip > int.MaxValue)
            {
                throw new ArgumentException(
                    "Requested page is too large.");
            }

            // 5. Retrieve sales
            var (items, totalCount) =
                await _repository.GetPagedAsync(
                    request,
                    cancellationToken);

            // 6. Calculate total pages
            int totalPages = totalCount / request.PageSize;

            if (totalCount % request.PageSize != 0)
            {
                totalPages++;
            }

            // 7. Return response
            return new GetInStoreSalesResponse(
                Items: items,
                PageNumber: request.PageNumber,
                PageSize: request.PageSize,
                TotalCount: totalCount,
                TotalPages: totalPages);
        }
    }
}
