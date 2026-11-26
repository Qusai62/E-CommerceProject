using ECommerce.Application.Orders.GetCustomerOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Orders.GetCustomerOrderDetails
{
    public class GetCustomerOrderDetailsService
    {
        private readonly IOrderHistoryReadRepository _repository;

        public GetCustomerOrderDetailsService(
            IOrderHistoryReadRepository repository)
        {
            _repository = repository;
        }

        public async Task<CustomerOrderDetailsResponse?> ExecuteAsync(
            Guid applicationUserId,
            long orderId,
            CancellationToken cancellationToken = default)
        {
            if (applicationUserId == Guid.Empty)
            {
                throw new ArgumentException(
                    "Invalid application user ID.",
                    nameof(applicationUserId));
            }

            if (orderId <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(orderId));
            }

            return await _repository.GetByIdAsync(
                applicationUserId,
                orderId,
                cancellationToken);
        }
    }
}
