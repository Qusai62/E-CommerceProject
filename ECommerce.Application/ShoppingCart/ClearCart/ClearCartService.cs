using ECommerce.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.ShoppingCart.ClearCart
{
    public class ClearCartService
    {
        private readonly IClearCartRepository _repository;

        public ClearCartService(
            IClearCartRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<ClearCartResult>> ExecuteAsync(
            Guid applicationUserId,
            ClearCartCommand command,
            CancellationToken cancellationToken = default)
        {
            // 1. Clear the authenticated customer's cart
            var cleared = await _repository.ClearAsync(
                applicationUserId,
                cancellationToken);

            // 2. Handle missing shopping cart
            if (!cleared)
            {
                return Result<ClearCartResult>.Failure(
                    new[]
                    {
                    new Error(
                        "Cart.NotFound",
                        "Shopping cart was not found.")
                    });
            }

            // 3. Return success
            return Result<ClearCartResult>.Success(
                new ClearCartResult(true));
        }
    }
}
