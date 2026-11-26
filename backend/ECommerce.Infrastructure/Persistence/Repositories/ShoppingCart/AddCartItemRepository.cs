using ECommerce.Application.ShoppingCart.AddCartItem;
using ECommerce.Domain.Cart;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Repositories.ShoppingCart
{
    public class AddCartItemRepository: IAddCartItemRepository
    {
        private readonly ApplicationDbContext _context;

        public AddCartItemRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AddCartItemResult> AddAsync(
            Guid applicationUserId,
            long productVariantId,
            int quantity,
            long availableQuantity,
            CancellationToken cancellationToken = default)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable,
                    cancellationToken);

            // 1. Find the authenticated customer's cart
            var cart = await _context.ShoppingCarts
                .SingleOrDefaultAsync(
                    x => x.Customer.ApplicationUserId
                        == applicationUserId,
                    cancellationToken);

            if (cart is null)
            {
                return new AddCartItemResult(
                    AddCartItemStatus.CartNotFound,
                    null);
            }

            // Verify current catalog eligibility
            bool isProductAvailable = await _context.ProductVariants
                .AnyAsync(v =>
                    v.ProductVariantId == productVariantId &&
                    v.IsActive &&
                    v.Product.IsActive &&
                    v.Product.BrandId != null &&
                    v.Product.Brand != null &&
                    v.Product.Brand.IsActive &&
                    v.Product.CategoryId != null &&
                    v.Product.Category != null &&
                    v.Product.Category.IsActive,
                    cancellationToken);

            if (!isProductAvailable)
            {
                return new AddCartItemResult(
                    AddCartItemStatus.ProductUnavailable,
                    null);
            }

            // 2. Find existing variant in this cart
            var existingItem = await _context.CartItems
                .SingleOrDefaultAsync(
                    x => x.ShoppingCartId == cart.ShoppingCartId &&
                         x.ProductVariantId == productVariantId,
                    cancellationToken);

            // 3. Calculate the resulting quantity
            long totalQuantity =
                (long)(existingItem?.Quantity ?? 0) + quantity;

            if (totalQuantity > availableQuantity)
            {
                return new AddCartItemResult(
                    AddCartItemStatus.InsufficientStock,
                    null);
            }

            // 4. Create or update
            var now = DateTime.UtcNow;

            CartItem item;

            if (existingItem is null)
            {
                item = new CartItem
                {
                    ShoppingCartId = cart.ShoppingCartId,
                    ProductVariantId = productVariantId,
                    Quantity = (int)totalQuantity,
                    CreatedAtUtc = now,
                    UpdatedAtUtc = now
                };

                _context.CartItems.Add(item);
            }
            else
            {
                existingItem.Quantity = (int)totalQuantity;
                existingItem.UpdatedAtUtc = now;

                item = existingItem;
            }

            cart.UpdatedAtUtc = now;

            // 5. Persist changes
            await _context.SaveChangesAsync(cancellationToken);

            // 6. Commit transaction
            await transaction.CommitAsync(cancellationToken);

            return new AddCartItemResult(
                AddCartItemStatus.Added,
                new AddCartItemResponse(
                    item.CartItemId,
                    item.ProductVariantId,
                    item.Quantity));
        }
    }
}
