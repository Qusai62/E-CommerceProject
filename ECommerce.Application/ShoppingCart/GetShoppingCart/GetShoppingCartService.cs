using ECommerce.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.ShoppingCart.GetShoppingCart
{
    public class GetShoppingCartService
    {
        private readonly IShoppingCartReadRepository _repository;

        public GetShoppingCartService(
            IShoppingCartReadRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<ShoppingCartResponse>> ExecuteAsync(
            Guid applicationUserId,
            CancellationToken cancellationToken = default)
        {
            var cart = await _repository
                .GetByApplicationUserIdAsync(
                    applicationUserId,
                    cancellationToken);

            if (cart is null)
            {
                return Result<ShoppingCartResponse>.Failure(
                    new[]
                    {
                    new Error(
                        "ShoppingCart.NotFound",
                        "Shopping cart was not found.")
                    });
            }

            return Result<ShoppingCartResponse>.Success(cart);
        }
    }
}
