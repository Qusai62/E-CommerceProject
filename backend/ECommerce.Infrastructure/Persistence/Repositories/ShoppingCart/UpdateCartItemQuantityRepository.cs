using ECommerce.Application.ShoppingCart.UpdateCartItemQuantity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Repositories.ShoppingCart
{
    public class UpdateCartItemQuantityRepository
        : IUpdateCartItemQuantityRepository
    {
        private readonly ApplicationDbContext _context;

        public UpdateCartItemQuantityRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<long?> GetOwnedVariantIdAsync(
            Guid applicationUserId,
            long cartItemId,
            CancellationToken cancellationToken = default)
        {
            return await _context.CartItems
                .AsNoTracking()
                .Where(item =>
                    item.CartItemId == cartItemId &&
                    item.ShoppingCart.Customer.ApplicationUserId
                        == applicationUserId)
                .Select(item =>
                    (long?)item.ProductVariantId)
                .SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<UpdateCartItemResult> UpdateAsync(
    Guid applicationUserId,
    long cartItemId,
    int quantity,
    long availableQuantity,
    CancellationToken cancellationToken = default)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable,
                    cancellationToken);

            // 1. Find only an item owned by the current customer
            var item = await _context.CartItems
                .Include(x => x.ShoppingCart)
                .SingleOrDefaultAsync(
                    x => x.CartItemId == cartItemId &&
                         x.ShoppingCart.Customer.ApplicationUserId
                             == applicationUserId,
                    cancellationToken);

            if (item is null)
            {
                return new UpdateCartItemResult(
                    UpdateCartItemStatus.NotFound,
                    null);
            }

            // 2. Validate the quantity again before writing
            if (quantity <= 0 || quantity > availableQuantity)
            {
                return new UpdateCartItemResult(
                    UpdateCartItemStatus.InsufficientStock,
                    null);
            }

            // 3. Replace the quantity
            item.Quantity = quantity;

            // 4. Update timestamps
            var now = DateTime.UtcNow;

            item.UpdatedAtUtc = now;
            item.ShoppingCart.UpdatedAtUtc = now;

            // 5. Persist tracked changes
            await _context.SaveChangesAsync(cancellationToken);

            // 6. Commit the transaction
            await transaction.CommitAsync(cancellationToken);

            return new UpdateCartItemResult(
                UpdateCartItemStatus.Updated,
                new UpdateCartItemQuantityResponse(
                    item.CartItemId,
                    item.ProductVariantId,
                    item.Quantity));
        }
    }
}
