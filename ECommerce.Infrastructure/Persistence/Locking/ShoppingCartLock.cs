using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Locking
{
    public sealed class ShoppingCartLock
    {
        private readonly ApplicationDbContext _context;

        public ShoppingCartLock(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<long?> LockAsync(
            Guid applicationUserId,
            CancellationToken cancellationToken = default)
        {
            // Step 1: Verify that a transaction has started.
            if (_context.Database.CurrentTransaction is null)
            {
                throw new InvalidOperationException(
                    "Shopping cart locking requires an active transaction.");
            }

            // Step 2: Find the customer's shopping cart.
            long? shoppingCartId = await _context.ShoppingCarts
                .AsNoTracking()
                .Where(cart =>
                    cart.Customer.ApplicationUserId == applicationUserId)
                .Select(cart => (long?)cart.ShoppingCartId)
                .SingleOrDefaultAsync(cancellationToken);

            if (shoppingCartId is null)
            {
                return null;
            }

            // Step 3: Acquire the database lock.
            var lockedCarts = await _context.ShoppingCarts
                .FromSqlInterpolated(
                    $"""
                SELECT *
                FROM dbo.ShoppingCart WITH (UPDLOCK, HOLDLOCK)
                WHERE ShoppingCartId = {shoppingCartId.Value}
                """)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return lockedCarts.SingleOrDefault()?.ShoppingCartId;
        }
    }
}
