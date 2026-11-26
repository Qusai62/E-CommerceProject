using ECommerce.Application.Common.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Orders.GetCustomerOrders
{
    public class GetCustomerOrdersService
    {
        private readonly IOrderHistoryReadRepository _repository;

        public GetCustomerOrdersService(
            IOrderHistoryReadRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResult<CustomerOrderSummary>> ExecuteAsync(
            Guid applicationUserId,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            if (applicationUserId == Guid.Empty)
                throw new ArgumentException(
                    "Invalid application user ID.",
                    nameof(applicationUserId));

            if (pageNumber < 1)
                throw new ArgumentOutOfRangeException(
                    nameof(pageNumber));

            if (pageSize is < 1 or > 100)
                throw new ArgumentOutOfRangeException(
                    nameof(pageSize));

            return await _repository.GetPagedAsync(
                applicationUserId,
                pageNumber,
                pageSize,
                cancellationToken);
        }
    }
}
