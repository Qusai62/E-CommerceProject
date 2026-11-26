using ECommerce.Application.Checkout.StartCheckout;
using ECommerce.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Checkout.ValidateAvailability
{
    public class ValidateCheckoutAvailabilityService
    {
        private readonly ICheckoutCartReadRepository _cartRepository;
        private readonly CheckoutAvailabilityValidator _validator;

        public ValidateCheckoutAvailabilityService(
            ICheckoutCartReadRepository cartRepository,
            CheckoutAvailabilityValidator validator)
        {
            _cartRepository = cartRepository;
            _validator = validator;
        }

        public async Task<Result<ValidateCheckoutAvailabilityResponse>>
            ExecuteAsync(
                Guid applicationUserId,
                CancellationToken cancellationToken = default)
        {
            // 1. Validate authenticated user identity.
            if (applicationUserId == Guid.Empty)
            {
                return Result<ValidateCheckoutAvailabilityResponse>.Failure(
                    new[]
                    {
                    new Error(
                        "Checkout.InvalidUser",
                        "A valid customer identity is required.")
                    });
            }

            // 2. Retrieve current cart and inventory information.
            var cart = await _cartRepository.GetAsync(
                applicationUserId,
                cancellationToken);

            // 3. Validate product, variant, and stock availability.
            var errors = _validator.Validate(cart);

            // 4. Reject the checkout if any validation fails.
            if (errors.Count > 0)
            {
                return Result<ValidateCheckoutAvailabilityResponse>.Failure(
                    errors);
            }

            // 5. Return successful validation.
            return Result<ValidateCheckoutAvailabilityResponse>.Success(
                new ValidateCheckoutAvailabilityResponse(
                    IsAvailable: true,
                    TotalItems: cart!.Items.Count
                ));
        }
    }
}
