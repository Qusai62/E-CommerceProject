using ECommerce.Application.ShoppingCart.GetShoppingCart;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Repositories.ShoppingCart
{
    public class ShoppingCartReadRepository: IShoppingCartReadRepository
    {
        private readonly ApplicationDbContext _context;

        public ShoppingCartReadRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ShoppingCartResponse?>
            GetByApplicationUserIdAsync(
                Guid applicationUserId,
                CancellationToken cancellationToken = default)
        {
            var cart = await _context.ShoppingCarts
                .AsNoTracking()
                .Where(cart =>
                    cart.Customer.ApplicationUserId
                        == applicationUserId)
                .Select(cart => new
                {
                    cart.ShoppingCartId,

                    Items = cart.CartItems
                        .OrderBy(item => item.CartItemId)
                        .Select(item => new ShoppingCartItemResponse(
                            item.CartItemId,
                            item.ProductVariantId,
                            item.ProductVariant.Product.Name,
                            item.ProductVariant.Sku,
                            item.Quantity,
                            item.ProductVariant.Price,
                            item.ProductVariant.Price * item.Quantity
                        ))
                        .ToList()
                })
                .SingleOrDefaultAsync(cancellationToken);

            if (cart is null)
            {
                return null;
            }

            return new ShoppingCartResponse(
                cart.ShoppingCartId,
                cart.Items,
                cart.Items.Sum(item => item.Quantity),
                cart.Items.Sum(item => item.Subtotal)
            );
        }
    }
}
