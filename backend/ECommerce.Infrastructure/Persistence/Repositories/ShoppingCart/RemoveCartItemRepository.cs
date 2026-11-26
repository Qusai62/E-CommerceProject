using ECommerce.Application.ShoppingCart.RemoveCartItem;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Repositories.ShoppingCart
{
    public class RemoveCartItemRepository : IRemoveCartItemRepository
    {
        private readonly ApplicationDbContext _context;

        public RemoveCartItemRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> RemoveAsync(
            Guid applicationUserId,
            long cartItemId,
            CancellationToken cancellationToken = default)
        {
            // Step 1: Find the item belonging to this customer
            var item = await _context.CartItems
                .Include(x => x.ShoppingCart)
                .SingleOrDefaultAsync(
                    x => x.CartItemId == cartItemId &&
                         x.ShoppingCart.Customer.ApplicationUserId
                             == applicationUserId,
                    cancellationToken);

            // Step 2: No item found or item belongs to another customer
            if (item is null)
            {
                return false;
            }

            // Step 3: Mark the CartItem for deletion
            _context.CartItems.Remove(item);

            // Step 4: Update the shopping cart timestamp
            item.ShoppingCart.UpdatedAtUtc = DateTime.UtcNow;

            // Step 5: Save both changes
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
