using ECommerce.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Checkout.PlaceOrder
{
    public interface IPlaceOrderRepository
    {
        Task<Result<PlaceOrderResponse>> PlaceAtomicallyAsync(
            Guid applicationUserId,
            PlaceOrderCommand command,
            CancellationToken cancellationToken = default);
    }
}
