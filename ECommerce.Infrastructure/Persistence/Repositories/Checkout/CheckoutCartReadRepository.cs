using ECommerce.Application.Checkout.StartCheckout;
using ECommerce.Domain.InventoryManagement;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Repositories.Checkout
{
    public class CheckoutCartReadRepository: ICheckoutCartReadRepository
    {
        private readonly ApplicationDbContext _context;

        public CheckoutCartReadRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CheckoutCartSnapshot?> GetAsync(
            Guid applicationUserId,
            CancellationToken cancellationToken = default)
        {
            // 1. Find the authenticated customer's cart.
            var cart = await _context.ShoppingCarts
                .AsNoTracking()
                .Where(c =>
                    c.Customer.ApplicationUserId == applicationUserId)
                .Select(c => new
                {
                    c.ShoppingCartId
                })
                .SingleOrDefaultAsync(cancellationToken);

            if (cart is null)
            {
                return null;
            }

            // 2. Retrieve the cart items and current variant data.
            var cartItems = await _context.CartItems
                .AsNoTracking()
                .Where(i =>
                    i.ShoppingCartId == cart.ShoppingCartId)
                .Select(i => new
                {
                    i.CartItemId,
                    i.ProductVariantId,
                    i.Quantity,
                    ProductName = i.ProductVariant.Product.Name,
                    i.ProductVariant.Sku,
                    UnitPrice = i.ProductVariant.Price,
                    IsProductActive = i.ProductVariant.Product.IsActive,
                    IsVariantActive = i.ProductVariant.IsActive
                })
                .ToListAsync(cancellationToken);

            // An empty cart is returned as an empty snapshot.
            if (cartItems.Count == 0)
            {
                return new CheckoutCartSnapshot(
                    cart.ShoppingCartId,
                    Array.Empty<CheckoutCartItemSnapshot>());
            }

            // 3. Find the inventory rows for all selected variants.
            var variantIds = cartItems
                .Select(i => i.ProductVariantId)
                .Distinct()
                .ToArray();

            var stockRows = await _context.Inventories
                .AsNoTracking()
                .Where(i => variantIds.Contains(i.ProductVariantId))
                .Select(i => new
                {
                    i.ProductVariantId,
                    i.OnHandQuantity,

                    ReservedQuantity = i.StockReservations
                        .Where(r =>
                            r.Status == StockReservationStatus.Active)
                        .Sum(r => (int?)r.Quantity) ?? 0,

                    SecuredCODQuantity = i.CODStockAllocations
                        .Where(a =>
                            a.Status == CODStockAllocationStatus.Secured)
                        .Sum(a => (int?)a.Quantity) ?? 0
                })
                .ToListAsync(cancellationToken);

            // 4. Calculate stock availability for each variant.
            var availabilityByVariant = stockRows.ToDictionary(
                s => s.ProductVariantId,
                s => (long)s.OnHandQuantity
                     - s.ReservedQuantity
                     - s.SecuredCODQuantity);

            // 5. Map database results to Application read models.
            var items = cartItems
                .Select(item => new CheckoutCartItemSnapshot(
                    item.CartItemId,
                    item.ProductVariantId,
                    item.ProductName,
                    item.Sku,
                    item.Quantity,
                    item.UnitPrice,
                    item.IsProductActive,
                    item.IsVariantActive,

                    availabilityByVariant.TryGetValue(
                        item.ProductVariantId,
                        out var available)
                        ? available
                        : 0
                ))
                .ToList();

            // 6. Return the complete checkout snapshot.
            return new CheckoutCartSnapshot(
                cart.ShoppingCartId,
                items);
        }
    }
}
