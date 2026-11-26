using ECommerce.Application.Checkout.DetermineShippingCharge;
using ECommerce.Application.Checkout.PlaceOrder;
using ECommerce.Application.Checkout.SelectSavedAddress;
using ECommerce.Application.Checkout.StartCheckout;
using ECommerce.Application.Checkout.ValidateAvailability;
using ECommerce.Application.Common.Results;
using ECommerce.Domain.InventoryManagement;
using ECommerce.Domain.Orders;
using ECommerce.Domain.Payments;
using ECommerce.Infrastructure.Persistence.Locking;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using InventoryEntity = ECommerce.Domain.InventoryManagement.Inventory;
namespace ECommerce.Infrastructure.Persistence.Repositories.Checkout
{
    public class PlaceOrderRepository : IPlaceOrderRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ShoppingCartLock _shoppingCartLock;
        private readonly ICheckoutCartReadRepository _cartReader;
        private readonly CheckoutAvailabilityValidator _availabilityValidator;
        private readonly SelectCheckoutAddressService _addressService;
        private readonly DetermineShippingChargeService _shippingService;

        public PlaceOrderRepository(
            ApplicationDbContext context,
            ShoppingCartLock shoppingCartLock,
            ICheckoutCartReadRepository cartReader,
            CheckoutAvailabilityValidator availabilityValidator,
            SelectCheckoutAddressService addressService,
            DetermineShippingChargeService shippingService)
        {
            _context = context;
            _shoppingCartLock = shoppingCartLock;
            _cartReader = cartReader;
            _availabilityValidator = availabilityValidator;
            _addressService = addressService;
            _shippingService = shippingService;
        }

        public async Task<Result<PlaceOrderResponse>> PlaceAtomicallyAsync(
            Guid applicationUserId,
            PlaceOrderCommand command,
            CancellationToken cancellationToken = default)
        {
            if (applicationUserId == Guid.Empty ||
                command.CheckoutSubmissionId == Guid.Empty ||
                command.SavedAddressId <= 0)
                return Fail("Order.InvalidRequest", "Invalid checkout request.");

            if (command.PaymentMethod is not
                (PaymentMethod.OnlinePayment or PaymentMethod.CashOnDelivery))
                return Fail("Order.InvalidPaymentMethod", "Unsupported payment method.");

            var fingerprint = GenerateRequestFingerprint(applicationUserId, command);
            var strategy = _context.Database.CreateExecutionStrategy();

            try
            {
                // The entire user-controlled transaction is one execution-strategy unit.
                // Rechecking the submission ID allows safe recovery after an uncertain commit.
                return await strategy.ExecuteAsync(async () =>
                {
                    // Only use this repository with a fresh, dedicated scoped context.
                    // Clearing prevents Added entities from a failed retry being inserted twice.
                    _context.ChangeTracker.Clear();

                    var previous = await FindExistingSubmissionAsync(
                        applicationUserId, command, fingerprint, cancellationToken);
                    if (previous is not null)
                        return previous;

                    return await CreateNewOrderAsync(
                        applicationUserId, command, fingerprint, cancellationToken);
                });
            }
            catch (DbUpdateException ex) when (IsDuplicateSubmission(ex))
            {
                // The transaction has been disposed before this recovery lookup.
                _context.ChangeTracker.Clear();
                var previous = await FindExistingSubmissionAsync(
                    applicationUserId, command, fingerprint, cancellationToken);
                if (previous is not null)
                    return previous;

                throw; // Do not hide an unresolved uniqueness failure.
            }
        }

        private async Task<Result<PlaceOrderResponse>> CreateNewOrderAsync(
            Guid applicationUserId,
            PlaceOrderCommand command,
            string fingerprint,
            CancellationToken cancellationToken)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(
                IsolationLevel.ReadCommitted, cancellationToken);

            // 1. Acquire the customer's cart lock before reading its items.
            long? cartId = await _shoppingCartLock.LockAsync(
                applicationUserId, cancellationToken);
            if (cartId is null)
                return Fail("Order.EmptyCart", "The customer has no shopping cart.");

            // 2. A request for the same cart may have finished while we waited.
            var existing = await FindExistingSubmissionAsync(
                applicationUserId, command, fingerprint, cancellationToken);
            if (existing is not null)
                return existing;

            // 3. Discover all variants and their inventory rows.
            var map = await GetCartInventoryMapAsync(cartId.Value, cancellationToken);
            if (map.Count == 0)
                return Fail("Order.EmptyCart", "The shopping cart is empty.");

            if (map.Any(x => x.InventoryId is null) ||
                map.Select(x => x.ProductVariantId).Distinct().Count() != map.Count)
                return Fail("Order.InvalidCart", "The cart has invalid or duplicate variants.");

            // Price updates on ProductVariant rows are blocked until transaction finishes.
            await LockVariantsAsync(map.Select(x => x.ProductVariantId), cancellationToken);

            // 4. Lock affected inventory rows in deterministic ascending ID order.
            await LockInventoriesAsync(
                map.Select(x => x.InventoryId!.Value), cancellationToken);

            // 5. Re-read stock AFTER locking. Do not reuse a pre-checkout result.
            var cart = await _cartReader.GetAsync(applicationUserId, cancellationToken);
            var errors = _availabilityValidator.Validate(cart);
            if (errors.Count > 0)
                return Result<PlaceOrderResponse>.Failure(errors);

            var validCart = cart!;
            if (validCart.ShoppingCartId != cartId.Value ||
                validCart.Items.Count != map.Count ||
                !validCart.Items.Select(x => x.ProductVariantId).OrderBy(x => x)
                    .SequenceEqual(map.Select(x => x.ProductVariantId).OrderBy(x => x)))
                return Fail("Order.CartChanged", "The cart changed during order placement.");

            if (validCart.Items.Any(x => x.Quantity <= 0 || x.UnitPrice <= 0m ||
                                         x.ProductName.Length > 200 || x.Sku.Length > 100 ||
                                         !HasScaleAtMost3(x.UnitPrice)))
                return Fail("Order.InvalidItem", "An item has invalid purchase details.");

            // 6. Read the current owned address and effective shipping charge.
            var addressResult = await _addressService.ExecuteAsync(
                applicationUserId, command.SavedAddressId, cancellationToken);
            if (addressResult.IsFailure)
                return Result<PlaceOrderResponse>.Failure(addressResult.Errors);

            var shippingResult = await _shippingService.ExecuteAsync(cancellationToken);
            if (shippingResult.IsFailure)
                return Result<PlaceOrderResponse>.Failure(shippingResult.Errors);

            var shippingCharge = shippingResult.Value!.ShippingCharge;
            if (shippingCharge < 0m || !HasScaleAtMost3(shippingCharge))
                return Fail("Order.InvalidShipping", "Invalid shipping configuration.");

            // 7. Recalculate from authoritative prices; do not accept client totals.
            decimal productsTotal = 0m;
            foreach (var item in validCart.Items)
                productsTotal = checked(productsTotal + item.UnitPrice * item.Quantity);

            decimal finalTotal = checked(productsTotal + shippingCharge);
            if (finalTotal <= 0m || !FitsDecimal18_3(finalTotal) ||
                !FitsDecimal18_3(shippingCharge))
                return Fail("Order.InvalidTotal", "The amount cannot be stored safely.");

            // 8. Resolve internal CustomerId from the locked cart.
            long? customerId = await _context.ShoppingCarts.AsNoTracking()
                .Where(x => x.ShoppingCartId == cartId.Value)
                .Select(x => (long?)x.CustomerId)
                .SingleOrDefaultAsync(cancellationToken);
            if (customerId is null)
                return Fail("Order.CustomerNotFound", "The customer does not exist.");

            // 9. Snapshot mutable attributes. Confirm attributes are controlled by
            // the same ProductVariant locking protocol in their write operations.
            var attributeSnapshots = await BuildVariantAttributesSnapshotsAsync(
                map.Select(x => x.ProductVariantId).ToArray(), cancellationToken);
            if (attributeSnapshots.Values.Any(x => x.Length > 2000))
                return Fail("Order.AttributesTooLong", "Variant attributes exceed storage limits.");

            var nowUtc = DateTime.UtcNow;
            var order = BuildOrder(
                customerId.Value, command, fingerprint, addressResult.Value!,
                shippingCharge, finalTotal, nowUtc);

            var orderItems = BuildOrderItems(order, validCart, attributeSnapshots, nowUtc);
            var payment = BuildPayment(order, command.PaymentMethod, finalTotal, nowUtc);

            _context.Orders.Add(order);
            _context.OrderItems.AddRange(orderItems);
            _context.Payments.Add(payment);

            if (command.PaymentMethod == PaymentMethod.CashOnDelivery)
            {
                var inventoryByVariant = map.ToDictionary(
                    x => x.ProductVariantId, x => x.InventoryId!.Value);
                var allocations = BuildCodAllocations(
                    order, validCart, inventoryByVariant, nowUtc);
                _context.CODStockAllocations.AddRange(allocations);
            }

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result<PlaceOrderResponse>.Success(new PlaceOrderResponse(
                order.OrderId, order.OrderNumber, order.TotalAmount,
                payment.Method, payment.Status));
        }

        private static bool HasScaleAtMost3(decimal value) =>
            decimal.Round(value, 3) == value;

        private static bool FitsDecimal18_3(decimal value) =>
            HasScaleAtMost3(value) && Math.Abs(value) < 1_000_000_000_000_000m;

        private sealed record CartInventoryEntry(long ProductVariantId, long? InventoryId);

        private Task<List<CartInventoryEntry>> GetCartInventoryMapAsync(
            long shoppingCartId, CancellationToken ct) =>
            _context.CartItems.AsNoTracking()
                .Where(x => x.ShoppingCartId == shoppingCartId)
                .Select(x => new CartInventoryEntry(
                    x.ProductVariantId,
                    x.ProductVariant.Inventory == null
                        ? null : x.ProductVariant.Inventory.InventoryId))
                .ToListAsync(ct);

        private async Task LockVariantsAsync(
            IEnumerable<long> variantIds, CancellationToken ct)
        {
            RequireTransaction();
            foreach (long id in variantIds.Distinct().OrderBy(x => x))
            {
                // Verify dbo.ProductVariant matches your actual EF mapping.
                var rows = await _context.ProductVariants
                    .FromSqlInterpolated($"""
                    SELECT * FROM dbo.ProductVariant WITH (UPDLOCK, HOLDLOCK)
                    WHERE ProductVariantId = {id}
                    """)
                    .AsNoTracking()
                    .ToListAsync(ct);
                if (rows.Count != 1)
                    throw new InvalidOperationException($"Product variant {id} was not found.");
            }
        }

        private async Task LockInventoriesAsync(
            IEnumerable<long> inventoryIds, CancellationToken ct)
        {
            RequireTransaction();
            foreach (long id in inventoryIds.Distinct().OrderBy(x => x))
            {
                var rows = await _context.Inventories
                    .FromSqlInterpolated($"""
                    SELECT * FROM dbo.Inventory WITH (UPDLOCK, HOLDLOCK)
                    WHERE InventoryId = {id}
                    """)
                    .AsNoTracking()
                    .ToListAsync(ct);
                if (rows.Count != 1)
                    throw new InvalidOperationException($"Inventory {id} was not found.");
            }
        }

        private void RequireTransaction()
        {
            if (_context.Database.CurrentTransaction is null)
                throw new InvalidOperationException("An active transaction is required.");
        }

        private static string GenerateRequestFingerprint(
            Guid userId, PlaceOrderCommand command)
        {
            string input = FormattableString.Invariant(
                $"v1|{userId:N}|{command.SavedAddressId}|{(int)command.PaymentMethod}");
            return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(input)));
        }

        private async Task<Result<PlaceOrderResponse>?> FindExistingSubmissionAsync(
            Guid userId, PlaceOrderCommand command, string fingerprint,
            CancellationToken ct)
        {
            var existing = await _context.Orders.AsNoTracking()
                .Where(x => x.CheckoutSubmissionId == command.CheckoutSubmissionId)
                .Select(x => new
                {
                    x.OrderId,
                    x.OrderNumber,
                    x.TotalAmount,
                    x.CheckoutRequestFingerprint,
                    OwnerId = x.Customer.ApplicationUserId,
                    Method = x.Payment.Method,
                    Status = x.Payment.Status
                })
                .SingleOrDefaultAsync(ct);

            if (existing is null)
                return null;

            if (existing.OwnerId != userId ||
                existing.CheckoutRequestFingerprint != fingerprint)
                return Fail("Order.SubmissionConflict", "Submission ID already belongs to a different request.");

            return Result<PlaceOrderResponse>.Success(new PlaceOrderResponse(
                existing.OrderId, existing.OrderNumber, existing.TotalAmount,
                existing.Method, existing.Status));
        }

        private static bool IsDuplicateSubmission(DbUpdateException ex) =>
            ex.InnerException is SqlException sql &&
            sql.Number is 2601 or 2627 &&
            sql.Message.Contains("UX_Order_CheckoutSubmissionId", StringComparison.OrdinalIgnoreCase);

        private async Task<Dictionary<long, string>> BuildVariantAttributesSnapshotsAsync(
            IReadOnlyCollection<long> variantIds, CancellationToken ct)
        {
            var attributes = await _context.VariantAttributes.AsNoTracking()
                .Where(x => variantIds.Contains(x.ProductVariantId))
                .Select(x => new { x.ProductVariantId, x.Name, x.Value })
                .ToListAsync(ct);

            return variantIds.ToDictionary(
                id => id,
                id => JsonSerializer.Serialize(attributes
                    .Where(x => x.ProductVariantId == id)
                    .OrderBy(x => x.Name).ThenBy(x => x.Value)
                    .Select(x => new { x.Name, x.Value }).ToArray()));
        }

        private static Order BuildOrder(long customerId, PlaceOrderCommand command,
            string fingerprint, SelectCheckoutAddressResponse address,
            decimal shippingCharge, decimal total, DateTime now)
            => new()
            {
                CheckoutSubmissionId = command.CheckoutSubmissionId,
                CheckoutRequestFingerprint = fingerprint,
                OrderNumber = $"ORD-{Guid.NewGuid():N}".ToUpperInvariant(),
                CustomerId = customerId,
                
                ShippingCharge = shippingCharge,
                TotalAmount = total,
                DeliveryRecipientName = address.RecipientName,
                DeliveryContactNumber = address.ContactNumber,
                DeliveryCity = address.City,
                DeliveryNeighborhood = address.Neighborhood,
                DeliveryStreet = address.Street,
                DeliveryBuildingInfo = address.BuildingInfo,
                DeliveryAdditionalInfo = address.AdditionalAddressInfo,
                PlacedAtUtc = now,
                UpdatedAtUtc = now
            };

        private static List<OrderItem> BuildOrderItems(Order order,
            CheckoutCartSnapshot cart, IReadOnlyDictionary<long, string> attributes,
            DateTime now)
            => cart.Items.Select(item => new OrderItem
            {
                Order = order,
                ProductVariantId = item.ProductVariantId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                ProductNameSnapshot = item.ProductName,
                SkuSnapshot = item.Sku,
                VariantAttributesSnapshot = attributes[item.ProductVariantId],
                CreatedAtUtc = now
            }).ToList();

        private static Payment BuildPayment(Order order, PaymentMethod method,
            decimal total, DateTime now)
            => new()
            {
                Order = order,
                Method = method,
                Status = PaymentStatus.Unpaid,
                ExpectedAmount = total,
                PaidAtUtc = null,
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            };

        private static List<CODStockAllocation> BuildCodAllocations(Order order,
            CheckoutCartSnapshot cart, IReadOnlyDictionary<long, long> inventoryByVariant,
            DateTime now)
            => cart.Items.Select(item => new CODStockAllocation
            {
                Order = order,
                InventoryId = inventoryByVariant[item.ProductVariantId],
                Quantity = item.Quantity,
                Status = CODStockAllocationStatus.Secured,
                SecuredAtUtc = now,
                ReleasedAtUtc = null,
                FinalizedAtUtc = null,
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            }).ToList();

        private static Result<PlaceOrderResponse> Fail(string code, string message) =>
            Result<PlaceOrderResponse>.Failure(new[] { new Error(code, message) });

    }
}  

