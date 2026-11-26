using E_Commerce.Contracts.Checkout;
using ECommerce.Application.Checkout.DetermineShippingCharge;
using ECommerce.Application.Checkout.ReviewOrder;
using ECommerce.Application.Checkout.SelectSavedAddress;
using ECommerce.Application.Checkout.StartCheckout;
using ECommerce.Application.Common.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_Commerce.Controllers
{
    [ApiController]
    [Route("api/checkout")]
   
    public sealed class CheckoutController : ControllerBase
    {
        private readonly GetCheckoutSummaryService _getCheckoutSummaryService;
        private readonly SelectCheckoutAddressService _selectAddressService;
        private readonly DetermineShippingChargeService _shippingService;
        private readonly ReviewCheckoutService _reviewCheckoutService;
        public CheckoutController(GetCheckoutSummaryService getCheckoutSummaryService,
            SelectCheckoutAddressService selectCheckoutAddressService,
            DetermineShippingChargeService determineShippingChargeService,
            ReviewCheckoutService reviewCheckoutService)
        {
            _getCheckoutSummaryService = getCheckoutSummaryService;
            _selectAddressService = selectCheckoutAddressService;
            _shippingService = determineShippingChargeService;
            _reviewCheckoutService = reviewCheckoutService;
        }

        [HttpGet]
        [ProducesResponseType(
            typeof(CheckoutSummaryResponse),
            StatusCodes.Status200OK)]
        [ProducesResponseType(
            StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(
            StatusCodes.Status403Forbidden)]
        [ProducesResponseType(
            StatusCodes.Status404NotFound)]
        [ProducesResponseType(
            StatusCodes.Status409Conflict)]
        public async Task<ActionResult<CheckoutSummaryResponse>> StartCheckout(
            CancellationToken cancellationToken)
        {
            // 1. Extract the authenticated user's identifier.
            var userIdValue = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(
                userIdValue,
                out var applicationUserId))
            {
                return Unauthorized();
            }

            // 2. Execute the Application use case.
            var result = await _getCheckoutSummaryService.ExecuteAsync(
                applicationUserId,
                cancellationToken);

            // 3. Map business errors to HTTP responses.
            if (result.IsFailure)
            {
                var errors = result.Errors;

                if (errors.Any(e =>
                    e.Code == "Checkout.InvalidUser"))
                {
                    return Unauthorized();
                }

                if (errors.Any(e =>
                    e.Code == "Checkout.CustomerNotFound" ||
                    e.Code == "Checkout.CartNotFound"))
                {
                    return NotFound(new
                    {
                        Errors = errors
                    });
                }

                if (errors.Any(e =>
                    e.Code == "Checkout.EmptyCart" ||
                    e.Code == "Checkout.InactiveVariant" ||
                    e.Code == "Checkout.InsufficientStock"))
                {
                    return Conflict(new
                    {
                        Errors = errors
                    });
                }

                return BadRequest(new
                {
                    Errors = errors
                });
            }

            // 4. Return the validated checkout summary.
            return Ok(result.Value);

        }

        [HttpGet("addresses/{savedAddressId:long}")]
        [ProducesResponseType(
       typeof(SelectCheckoutAddressResponse),
       StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<SelectCheckoutAddressResponse>>
       SelectSavedAddress(
           [FromRoute] long savedAddressId,
           CancellationToken cancellationToken)
        {
            // 1. Get the authenticated Identity user's ID.
            var userIdValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdValue, out var applicationUserId))
            {
                return Unauthorized();
            }

            // 2. Execute the Application service.
            var result = await _selectAddressService.ExecuteAsync(
                applicationUserId,
                savedAddressId,
                cancellationToken);

            // 3. Map business errors to HTTP responses.
            if (result.IsFailure)
            {
                if (result.Errors.Any(e =>
                    e.Code == "Checkout.InvalidAddressSelection"))
                {
                    return BadRequest(new
                    {
                        Errors = result.Errors
                    });
                }

                if (result.Errors.Any(e =>
                    e.Code == "Checkout.AddressNotFound"))
                {
                    return NotFound(new
                    {
                        Errors = result.Errors
                    });
                }

                if (result.Errors.Any(e =>
                    e.Code == "Checkout.InvalidAddress"))
                {
                    return UnprocessableEntity(new
                    {
                        Errors = result.Errors
                    });
                }

                return BadRequest(new
                {
                    Errors = result.Errors
                });
            }

            // 4. Return the validated address.
            return Ok(result.Value);
        }

        [HttpGet("shipping")]
        [ProducesResponseType(
            typeof(DetermineShippingChargeResponse),
            StatusCodes.Status200OK)]
        [ProducesResponseType(
            StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(
            StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<DetermineShippingChargeResponse>>
            GetShippingCharge(
                CancellationToken cancellationToken)
        {
            // 1. Execute the shipping business use case.
            var result = await _shippingService.ExecuteAsync(
                cancellationToken);

            // 2. Handle business errors.
            if (result.IsFailure)
            {
                if (result.Errors.Any(error =>
                    error.Code == "Checkout.ShippingConfigurationMissing" ||
                    error.Code == "Checkout.InvalidShippingMode" ||
                    error.Code == "Checkout.InvalidShippingCharge"))
                {
                    return StatusCode(
                        StatusCodes.Status503ServiceUnavailable,
                        new
                        {
                            Errors = result.Errors
                        });
                }

                // Unexpected application failure.
                return Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Unable to determine shipping charge.");
            }

            // 3. Return the effective shipping charge.
            return Ok(result.Value);
        }



        [HttpPost("review")]
        [ProducesResponseType(
            typeof(ReviewCheckoutResponse),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<ReviewCheckoutResponse>> ReviewOrder(
            [FromBody] ReviewCheckoutRequest request,
            CancellationToken cancellationToken)
        {
            // 1. Extract the authenticated Identity user's ID.
            var userIdValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdValue, out var applicationUserId))
            {
                return Unauthorized();
            }

            // 2. Map the HTTP request to the Application command.
            var command = new ReviewCheckoutCommand(
                request.SavedAddressId,
                request.PaymentMethod
            );

            // 3. Execute the Review Order use case.
            var result = await _reviewCheckoutService.ExecuteAsync(
                applicationUserId,
                command,
                cancellationToken);

            // 4. Handle business validation failures.
            if (result.IsFailure)
            {
                if (result.Errors.Any(e =>
                    e.Code == "Checkout.InvalidUser"))
                {
                    return Unauthorized();
                }

                if (result.Errors.Any(e =>
                    e.Code == "Checkout.InvalidAddressSelection" ||
                    e.Code == "Checkout.UnsupportedPaymentMethod"))
                {
                    return BadRequest(new
                    {
                        Errors = result.Errors
                    });
                }

                if (result.Errors.Any(e =>
                    e.Code == "Checkout.AddressNotFound"))
                {
                    return NotFound(new
                    {
                        Errors = result.Errors
                    });
                }

                if (result.Errors.Any(e =>
                    e.Code == "Checkout.EmptyCart" ||
                    e.Code == "Checkout.InactiveProduct" ||
                    e.Code == "Checkout.InactiveVariant" ||
                    e.Code == "Checkout.InvalidQuantity" ||
                    e.Code == "Checkout.InsufficientStock"))
                {
                    return Conflict(new
                    {
                        Errors = result.Errors
                    });
                }

                if (result.Errors.Any(e =>
                    e.Code == "Checkout.InvalidAddress"))
                {
                    return UnprocessableEntity(new
                    {
                        Errors = result.Errors
                    });
                }

                if (result.Errors.Any(e =>
                    e.Code == "Checkout.ShippingConfigurationMissing" ||
                    e.Code == "Checkout.InvalidShippingMode" ||
                    e.Code == "Checkout.InvalidShippingCharge"))
                {
                    return StatusCode(
                        StatusCodes.Status503ServiceUnavailable,
                        new
                        {
                            Errors = result.Errors
                        });
                }

                // Unknown errors are not silently treated as successful.
                return Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Unable to review checkout.");
            }

            // 5. Return the current checkout review.
            return Ok(result.Value);
        }

    }
}