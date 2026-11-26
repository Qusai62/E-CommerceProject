using ECommerce.Application.Checkout.DetermineShippingCharge;
using ECommerce.Application.Checkout.SelectPaymentMethod;
using ECommerce.Application.Checkout.SelectSavedAddress;
using ECommerce.Application.Checkout.StartCheckout;
using ECommerce.Application.Checkout.ValidateAvailability;
using ECommerce.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Checkout.ReviewOrder
{
    public sealed class ReviewCheckoutService
    {
        private readonly ICheckoutCartReadRepository _cartRepository;
        private readonly CheckoutAvailabilityValidator _availabilityValidator;
        private readonly SelectCheckoutAddressService _addressService;
        private readonly DetermineShippingChargeService _shippingService;
        private readonly IPaymentMethodValidator _paymentMethodValidator;

        public ReviewCheckoutService(
            ICheckoutCartReadRepository cartRepository,
            CheckoutAvailabilityValidator availabilityValidator,
            SelectCheckoutAddressService addressService,
            DetermineShippingChargeService shippingService,
            IPaymentMethodValidator paymentMethodValidator)
        {
            _cartRepository = cartRepository;
            _availabilityValidator = availabilityValidator;
            _addressService = addressService;
            _shippingService = shippingService;
            _paymentMethodValidator = paymentMethodValidator;
        }

        public async Task<Result<ReviewCheckoutResponse>> ExecuteAsync(
            Guid applicationUserId,
            ReviewCheckoutCommand command,
            CancellationToken cancellationToken = default)
        {
            // Step 1: Validate the authenticated customer's identity.
            if (applicationUserId == Guid.Empty)
            {
                return Result<ReviewCheckoutResponse>.Failure(
                    new[]
                    {
                    new Error(
                        "Checkout.InvalidUser",
                        "A valid customer identity is required.")
                    });
            }

            // Step 2: Validate the selected payment method.
            if (!_paymentMethodValidator.IsSupported(command.PaymentMethod))
            {
                return Result<ReviewCheckoutResponse>.Failure(
                    new[]
                    {
                    new Error(
                        "Checkout.UnsupportedPaymentMethod",
                        "The selected payment method is not supported.")
                    });
            }

            // Step 3: Retrieve the customer's current shopping cart.
            var cart = await _cartRepository.GetAsync(
                applicationUserId,
                cancellationToken);

            // Step 4: Validate current product and stock availability.
            var availabilityErrors = _availabilityValidator.Validate(cart);

            if (availabilityErrors.Count > 0)
            {
                return Result<ReviewCheckoutResponse>.Failure(
                    availabilityErrors);
            }

            // Validation guarantees a non-null and non-empty cart.
            var validCart = cart!;

            // Step 5: Validate the selected delivery address.
            var addressResult = await _addressService.ExecuteAsync(
                applicationUserId,
                command.SavedAddressId,
                cancellationToken);

            if (addressResult.IsFailure)
            {
                return Result<ReviewCheckoutResponse>.Failure(
                    addressResult.Errors);
            }

            // Step 6: Determine the current shipping charge.
            var shippingResult = await _shippingService.ExecuteAsync(
                cancellationToken);

            if (shippingResult.IsFailure)
            {
                return Result<ReviewCheckoutResponse>.Failure(
                    shippingResult.Errors);
            }

            // Step 7: Calculate each cart item's subtotal.
            var items = validCart.Items
                .Select(item => new CheckoutItemResponse(
                    item.CartItemId,
                    item.ProductVariantId,
                    item.ProductName,
                    item.Sku,
                    item.Quantity,
                    item.UnitPrice,
                    item.UnitPrice * item.Quantity
                ))
                .ToList();

            // Step 8: Calculate the total price of all products.
            decimal productsTotal = items.Sum(
                item => item.Subtotal);

            // Step 9: Retrieve the effective shipping charge.
            decimal shippingCharge =
                shippingResult.Value!.ShippingCharge;

            // Step 10: Calculate the final checkout total.
            decimal finalTotal =
                productsTotal + shippingCharge;

            // Step 11: Construct the complete review response.
            var response = new ReviewCheckoutResponse(
                validCart.ShoppingCartId,
                items,
                addressResult.Value!,
                command.PaymentMethod,
                productsTotal,
                shippingCharge,
                finalTotal
            );

            // Step 12: Return the successful review.
            return Result<ReviewCheckoutResponse>.Success(response);
        }
    }
}
