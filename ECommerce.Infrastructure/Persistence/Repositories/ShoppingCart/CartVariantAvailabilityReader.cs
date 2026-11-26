using ECommerce.Application.Inventory.Abstractions;
using ECommerce.Application.ShoppingCart.AddCartItem;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Repositories.ShoppingCart
{
    public sealed class CartVariantAvailabilityReader: ICartVariantAvailabilityReader
    {
        private readonly ApplicationDbContext _context;

        private readonly IInventoryAvailabilityReadRepository
            _inventoryRepository;

        public CartVariantAvailabilityReader(
            ApplicationDbContext context,
            IInventoryAvailabilityReadRepository inventoryRepository)
        {
            _context = context;
            _inventoryRepository = inventoryRepository;
        }

        public async Task<CartVariantAvailability?> GetAsync(
            long productVariantId,
            CancellationToken cancellationToken = default)
        {
            // Step 1: Retrieve product and variant activation states
            var variant = await _context.ProductVariants
                .AsNoTracking()
                .Where(v => v.ProductVariantId == productVariantId)
                .Select(v => new
                {
                    IsVariantActive = v.IsActive,
                    IsProductActive = v.Product.IsActive
                })
                .SingleOrDefaultAsync(cancellationToken);

            // Step 2: Variant does not exist
            if (variant is null)
            {
                return null;
            }

            // Step 3: Do not allow inactive products or variants
            if (!variant.IsProductActive || !variant.IsVariantActive)
            {
                return new CartVariantAvailability(
                    variant.IsProductActive,
                    variant.IsVariantActive,
                    0);
            }

            // Step 4: Reuse the Inventory module
            
            var stock = await _inventoryRepository.GetAsync(
                productVariantId,
                cancellationToken);

            if (stock is null)
            {
                return new CartVariantAvailability(
                    variant.IsProductActive,
                    variant.IsVariantActive,
                    0);
            }

            long availableQuantity = Math.Max(
    0L,
    (long)stock.OnHandQuantity
    - stock.ReservedQuantity
    - stock.SecuredCODQuantity);

            // Step 5: Return combined availability
            return new CartVariantAvailability(
                variant.IsProductActive,
                variant.IsVariantActive,
                availableQuantity);
        }
    }
}
