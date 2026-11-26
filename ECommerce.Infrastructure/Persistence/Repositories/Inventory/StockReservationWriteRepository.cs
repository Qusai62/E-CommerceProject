using ECommerce.Application.Inventory.Abstractions;
using ECommerce.Application.Inventory.CreateStockReservation;
using ECommerce.Domain.InventoryManagement;
using ECommerce.Domain.Orders;
using ECommerce.Domain.Payments;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Repositories.Inventory
{
    public class StockReservationWriteRepository : IStockReservationWriteRepository
    {
        private readonly ApplicationDbContext _context;

        public StockReservationWriteRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CreateStockReservationOutcome> CreateAsync(
            long paymentAttemptId,
            DateTime utcNow,//	Current UTC time supplied by the Application Service
            DateTime expiresAtUtc,//When this reservation is scheduled to expire
            CancellationToken cancellationToken = default)
        {
            // 1. Validate repository inputs
            if (paymentAttemptId <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(paymentAttemptId));

            if (utcNow.Kind != DateTimeKind.Utc ||
                expiresAtUtc.Kind != DateTimeKind.Utc ||
                expiresAtUtc <= utcNow)
            {
                throw new ArgumentException(
                    "Invalid UTC reservation timestamps.");
            }

            // 2. Begin the transaction
            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable,
                    cancellationToken);

            // 3. Find the payment attempt and its payment details
            var attempt = await _context.PaymentAttempts
                .AsNoTracking()
                .Where(x => x.PaymentAttemptId == paymentAttemptId)
                .Select(x => new
                {
                    OrderId = x.Payment.OrderId, //500
                    AttemptStatus = x.Status,    //Pending
                    AttemptAmount = x.Amount,    //710
                    x.CompletedAtUtc,            //null
                    PaymentMethod = x.Payment.Method,//OnlinePayment
                    PaymentStatus = x.Payment.Status,//Unpaid
                    ExpectedAmount = x.Payment.ExpectedAmount,//710
                    x.Payment.PaidAtUtc//null
                })
                .SingleOrDefaultAsync(cancellationToken);

            if (attempt is null)
                return CreateStockReservationOutcome.PaymentAttemptNotFound;

            // 4. Lock the associated order
            var order = await _context.Orders
                .FromSqlInterpolated($@"
                SELECT *
                FROM [Order] WITH (UPDLOCK, HOLDLOCK)
                WHERE [OrderId] = {attempt.OrderId}")
                .SingleOrDefaultAsync(cancellationToken);

            if (order is null ||
                order.FulfillmentStatus != OrderFulfillmentStatus.Pending)
            {
                return CreateStockReservationOutcome.OrderNotEligible;
            }

            // 5. Validate the payment and payment attempt
            if (attempt.PaymentMethod != PaymentMethod.OnlinePayment ||
                attempt.PaymentStatus != PaymentStatus.Unpaid ||
                attempt.AttemptStatus != PaymentAttemptStatus.Pending ||
                attempt.PaidAtUtc is not null ||
                attempt.CompletedAtUtc is not null ||
                attempt.AttemptAmount != attempt.ExpectedAmount ||
                order.TotalAmount != attempt.ExpectedAmount)
            {
                return CreateStockReservationOutcome.PaymentNotEligible;
            }

            // 6. Retrieve order items from the database
            var orderItems = await _context.OrderItems//here we will retrive all order items that the customer want to buy
                .AsNoTracking()
                .Where(x => x.OrderId == order.OrderId)
                .OrderBy(x => x.ProductVariantId)
                .Select(x => new
                { /////////////Example/////////////////
                    x.ProductVariantId,//10
                    x.Quantity,//3
                    VariantIsActive = x.ProductVariant.IsActive,//true
                    ProductIsActive = x.ProductVariant.Product.IsActive//true
                })
                .ToListAsync(cancellationToken);

            if (orderItems.Count == 0)
                return CreateStockReservationOutcome.OrderHasNoItems;

            if (orderItems.Any(x =>//here to check if any of items is not exist
                x.Quantity <= 0 ||
                !x.VariantIsActive ||
                !x.ProductIsActive))
            {
                return CreateStockReservationOutcome.OrderNotEligible;
            }

            // 7. Lock every required inventory record
            var inventoryItems =
                new List<(long InventoryId, int OnHandQuantity, int Quantity)>();
            //here to retrive how many items in the inventory and then check if the have enough to sell the coustomer 
            foreach (var item in orderItems)
            {
                var inventory = await _context.Inventories
                    .FromSqlInterpolated($@"
                    SELECT *
                    FROM [Inventory] WITH (UPDLOCK, HOLDLOCK)
                    WHERE [ProductVariantId] = {item.ProductVariantId}")
                    .AsNoTracking()
                    .SingleOrDefaultAsync(cancellationToken);

                if (inventory is null)
                {
                    throw new InvalidOperationException(
                        $"Inventory missing for variant {item.ProductVariantId}.");
                }

                inventoryItems.Add((
                    inventory.InventoryId,
                    inventory.OnHandQuantity,
                    item.Quantity));
            }

            // 8. Prevent duplicate/conflicting reservations
            var alreadyReserved = await _context.StockReservations
                .AsNoTracking()
                .AnyAsync(r =>
                    r.PaymentAttemptId == paymentAttemptId ||
                    (
                        r.PaymentAttempt.Payment.OrderId == order.OrderId &&
                        (
                            r.Status == StockReservationStatus.Active ||
                            r.Status == StockReservationStatus.Finalized
                        )
                    ),
                    cancellationToken);

            if (alreadyReserved)
                return CreateStockReservationOutcome.AlreadyReserved;

            // 9. Validate available stock for each variant
            var reservations = new List<StockReservation>();

            foreach (var item in inventoryItems)
            {
                //here to search if there is any another coustomer want to buy the item in online payment
                var reservedQuantity = await _context.StockReservations
                    .Where(r =>
                        r.InventoryId == item.InventoryId &&
                        r.Status == StockReservationStatus.Active)
                    .SumAsync(r => (long?)r.Quantity, cancellationToken)
                    ?? 0L;
                //here to check if any coustomer wnat to buy the item and on cash delivery 
                var securedCODQuantity = await _context.CODStockAllocations
                    .Where(a =>
                        a.InventoryId == item.InventoryId &&
                        a.Status == CODStockAllocationStatus.Secured)
                    .SumAsync(a => (long?)a.Quantity, cancellationToken)
                    ?? 0L;
                //here sub the value to check if it is available in the stock
                long availableQuantity =
                    (long)item.OnHandQuantity
                    - reservedQuantity
                    - securedCODQuantity;

                if (availableQuantity < 0)
                {
                    throw new InvalidOperationException(
                        $"Inventory integrity violation: {item.InventoryId}");
                }

                if (availableQuantity < item.Quantity)
                    return CreateStockReservationOutcome.InsufficientStock;

                // 10. Prepare a reservation (not saved yet)
                reservations.Add(new StockReservation
                {
                    InventoryId = item.InventoryId,
                    PaymentAttemptId = paymentAttemptId,
                    Quantity = item.Quantity,
                    Status = StockReservationStatus.Active,
                    ExpiresAtUtc = expiresAtUtc,
                    StatusChangedAtUtc = utcNow,
                    CreatedAtUtc = utcNow,
                    UpdatedAtUtc = utcNow
                });
            }

            // 11. Insert all reservations
            _context.StockReservations.AddRange(reservations);

            await _context.SaveChangesAsync(cancellationToken);

            // 12. Commit the transaction
            await transaction.CommitAsync(cancellationToken);

            return CreateStockReservationOutcome.Created;
        }
    }
}
