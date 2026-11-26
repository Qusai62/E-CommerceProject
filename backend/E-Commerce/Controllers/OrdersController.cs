using E_Commerce.Contracts.Checkout;
using ECommerce.Application.Checkout.PlaceOrder;
using ECommerce.Application.Common.Security;
using ECommerce.Application.Orders.GetCustomerOrderDetails;
using ECommerce.Application.Orders.GetCustomerOrders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_Commerce.Controllers
{

    [ApiController]
    [Route("api/orders")]
   
    public sealed class OrdersController : ControllerBase
    {
        private readonly PlaceOrderService _placeOrderService;
        private readonly GetCustomerOrdersService _getCustomerOrdersService;
        private readonly GetCustomerOrderDetailsService _detailsService;
        public OrdersController(
            PlaceOrderService placeOrderService,GetCustomerOrdersService getCustomerOrdersService, GetCustomerOrderDetailsService detailsService)
        {
            _placeOrderService = placeOrderService;
            _getCustomerOrdersService = getCustomerOrdersService;
            _detailsService = detailsService;
        }

        [HttpPost]
        [ProducesResponseType(
            typeof(PlaceOrderResponse),
            StatusCodes.Status201Created)]
        [ProducesResponseType(
            StatusCodes.Status400BadRequest)]
        [ProducesResponseType(
            StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(
            StatusCodes.Status404NotFound)]
        [ProducesResponseType(
            StatusCodes.Status409Conflict)]
        [ProducesResponseType(
            StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(
            StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<PlaceOrderResponse>> PlaceOrder(
            [FromBody] PlaceOrderRequest request,
            CancellationToken cancellationToken)
        {
            // 1. Retrieve the authenticated ASP.NET Identity user.
            string? userIdValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(
                userIdValue,
                out Guid applicationUserId))
            {
                return Unauthorized();
            }

            // 2. Convert the HTTP request to an Application command.
            var command = new PlaceOrderCommand(
                request.CheckoutSubmissionId,
                request.SavedAddressId,
                request.PaymentMethod);

            // 3. Execute the Place Order use case.
            var result = await _placeOrderService.ExecuteAsync(
                applicationUserId,
                command,
                cancellationToken);

            // 4. Translate business failures into HTTP responses.
            if (result.IsFailure)
            {
                var errors = result.Errors;

                int statusCode;

                if (errors.Any(e =>
                    e.Code is
                        "Order.InvalidRequest" or
                        "Order.InvalidCustomer" or
                        "Order.InvalidSubmissionId" or
                        "Order.InvalidAddress" or
                        "Order.InvalidPaymentMethod" or
                        "Order.UnsupportedPaymentMethod"))
                {
                    statusCode = StatusCodes.Status400BadRequest;
                }
                else if (errors.Any(e =>
                    e.Code is
                        "Order.SubmissionConflict" or
                        "Order.EmptyCart" or
                        "Order.InvalidCart" or
                        "Order.MissingInventory" or
                        "Order.CartChanged" or
                        "Order.InvalidItem"))
                {
                    statusCode = StatusCodes.Status409Conflict;
                }
                else if (errors.Any(e =>
                    e.Code.Contains(
                        "NotFound",
                        StringComparison.OrdinalIgnoreCase)))
                {
                    statusCode = StatusCodes.Status404NotFound;
                }
                else if (errors.Any(e =>
                    e.Code.StartsWith(
                        "Shipping.",
                        StringComparison.OrdinalIgnoreCase)))
                {
                    statusCode = StatusCodes.Status503ServiceUnavailable;
                }
                else
                {
                    statusCode =
                        StatusCodes.Status422UnprocessableEntity;
                }

                return StatusCode(
                    statusCode,
                    new { Errors = errors });
            }

            // 5. Return the persisted order confirmation.
            return StatusCode(
                StatusCodes.Status201Created,
                result.Value);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCustomerOrders(
       [FromQuery] int pageNumber = 1,
       [FromQuery] int pageSize = 10,
       CancellationToken cancellationToken = default)
        {
            // 1. Read the authenticated user's identity.
            var userIdValue = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdValue, out var applicationUserId))
            {
                return Unauthorized();
            }

            // 2. Validate pagination input.
            if (pageNumber < 1)
            {
                return BadRequest(
                    "PageNumber must be greater than zero.");
            }

            if (pageSize is < 1 or > 100)
            {
                return BadRequest(
                    "PageSize must be between 1 and 100.");
            }

            // 3. Execute the Application use case.
            var result = await _getCustomerOrdersService.ExecuteAsync(
                applicationUserId,
                pageNumber,
                pageSize,
                cancellationToken);

            // 4. Return the paginated orders.
            return Ok(result);
        }


        [HttpGet("{orderId:long}")]
        [ProducesResponseType(
            typeof(CustomerOrderDetailsResponse),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrderDetails(
            [FromRoute] long orderId,
            CancellationToken cancellationToken)
        {
            var userIdValue = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdValue, out var applicationUserId))
            {
                return Unauthorized();
            }

            if (orderId <= 0)
            {
                return BadRequest("Invalid order ID.");
            }

            var result = await _detailsService.ExecuteAsync(
                applicationUserId,
                orderId,
                cancellationToken);

            if (result is null)
            {
                return NotFound();
            }

            return Ok(result);
        }

    }
}
