using ECommerce.Application.ShoppingCart.ClearCart;
using Microsoft.EntityFrameworkCore;
using System;

namespace ECommerce.Infrastructure.Persistence.Repositories.ShoppingCart
{
    public class ClearCartRepository: IClearCartRepository
    {
        private readonly ApplicationDbContext _context;

        public ClearCartRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ClearAsync(
            Guid applicationUserId,
            CancellationToken cancellationToken = default)
        {
            // Step 1: Find the authenticated customer's cart
            var cart = await _context.ShoppingCarts
                .Include(c => c.CartItems)
                .SingleOrDefaultAsync(
                    c => c.Customer.ApplicationUserId
                        == applicationUserId,
                    cancellationToken);

            // Step 2: Handle a missing cart
            if (cart is null)
            {
                return false;
            }

            // Step 3: Remove all cart items
            _context.CartItems.RemoveRange(cart.CartItems);

            // Step 4: Update the parent cart timestamp
            cart.UpdatedAtUtc = DateTime.UtcNow;

            // Step 5: Persist changes
            await _context.SaveChangesAsync(cancellationToken);

            // Step 6: Return success
            return true;
        }
    }
}
