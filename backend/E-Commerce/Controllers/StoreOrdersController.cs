using ECommerce.Application.Common.Pagination;
using ECommerce.Application.Common.Security;
using ECommerce.Application.StoreOrders.CancelOrder;
using ECommerce.Application.StoreOrders.SearchOrders;
using ECommerce.Application.StoreOrders.UpdateFulfillmentStatus;
using ECommerce.Application.StoreOrders.UpdatePaymentStatus;
using ECommerce.Application.StoreOrders.ViewFulfillmentHistory;
using ECommerce.Application.StoreOrders.ViewOrderDetails;
using ECommerce.Application.StoreOrders.ViewOrders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_Commerce.Controllers
{
    [ApiController]
    [Route("api/store/orders")]
    [Authorize(Policy = AuthorizationPolicies.StoreOwnerOnly)]
    public sealed class StoreOrdersController : ControllerBase
    {
        private readonly GetStoreOrdersService _getStoreOrdersService;
        private readonly GetStoreOrderDetailsService _getStoreOrderDetailsService;
        private readonly SearchStoreOrdersService _searchStoreOrdersService;
        private readonly UpdateOrderFulfillmentStatusService _updateFulfillmentStatusService;
        private readonly GetOrderFulfillmentHistoryService _getOrderFulfillmentHistoryService;
        private readonly UpdatePaymentStatusService _updatePaymentStatusService;
        private readonly CancelOrderService _cancelOrderService;
        public StoreOrdersController(
            GetStoreOrdersService getStoreOrdersService,
            GetStoreOrderDetailsService getStoreOrderDetailsService,
            SearchStoreOrdersService searchStoreOrdersService,
            UpdateOrderFulfillmentStatusService updateOrderFulfillmentStatusService,
            GetOrderFulfillmentHistoryService getOrderFulfillmentHistoryService,
            UpdatePaymentStatusService updatePaymentStatusService,
            CancelOrderService cancelOrderService)
        {
            _getStoreOrdersService = getStoreOrdersService;
            _getStoreOrderDetailsService = getStoreOrderDetailsService;
            _searchStoreOrdersService = searchStoreOrdersService;
            _updateFulfillmentStatusService = updateOrderFulfillmentStatusService;
            _getOrderFulfillmentHistoryService = getOrderFulfillmentHistoryService;
            _updatePaymentStatusService = updatePaymentStatusService;
            _cancelOrderService = cancelOrderService;
        }

        // GET /api/store/orders

        [HttpGet]
        [ProducesResponseType(
            typeof(PagedResult<StoreOrderResponse>),
            StatusCodes.Status200OK)]
        [ProducesResponseType(
            StatusCodes.Status400BadRequest)]
        [ProducesResponseType(
            StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(
            StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<PagedResult<StoreOrderResponse>>>
            GetOrders(
                [FromQuery] GetStoreOrdersRequest request,
                CancellationToken cancellationToken)
        {
            // Step 1: Execute the Application service.
            var result = await _getStoreOrdersService.ExecuteAsync(
                request,
                cancellationToken);

            // Step 2: Handle validation errors.
            if (result.IsFailure)
            {
                return BadRequest(result.Errors);
            }

            // Step 3: Return filtered and paginated orders.
            return Ok(result.Value);
        }



        [HttpGet("{orderId:long}")]
        [ProducesResponseType(
            typeof(StoreOrderDetailsResponse),
            StatusCodes.Status200OK)]
        [ProducesResponseType(
            StatusCodes.Status400BadRequest)]
        [ProducesResponseType(
            StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(
            StatusCodes.Status403Forbidden)]
        [ProducesResponseType(
            StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StoreOrderDetailsResponse>>
            GetOrderDetails(
                [FromRoute] long orderId,
                CancellationToken cancellationToken)
        {
            var result = await _getStoreOrderDetailsService.ExecuteAsync(
                orderId,
                cancellationToken);

            if (result.IsFailure)
            {
                if (result.Errors.Any(e =>
                    e.Code == "StoreOrders.OrderNotFound"))
                {
                    return NotFound(result.Errors);
                }

                return BadRequest(result.Errors);
            }

            return Ok(result.Value);
        }



        [HttpGet("by-number/{orderNumber}")]
        [ProducesResponseType(
            typeof(StoreOrderResponse),
            StatusCodes.Status200OK)]
        [ProducesResponseType(
            StatusCodes.Status400BadRequest)]
        [ProducesResponseType(
            StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(
            StatusCodes.Status403Forbidden)]
        [ProducesResponseType(
            StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StoreOrderResponse>>
            SearchOrder(
                [FromRoute] string orderNumber,
                CancellationToken cancellationToken)
        {
            // Step 1: Create the Application request.
            var request = new SearchStoreOrdersRequest
            {
                OrderNumber = orderNumber
            };

            // Step 2: Execute the Application service.
            var result = await _searchStoreOrdersService.ExecuteAsync(
                request,
                cancellationToken);

            // Step 3: Handle unsuccessful results.
            if (result.IsFailure)
            {
                if (result.Errors.Any(e =>
                    e.Code == "StoreOrders.OrderNotFound"))
                {
                    return NotFound(result.Errors);
                }

                return BadRequest(result.Errors);
            }

            // Step 4: Return the matching order.
            return Ok(result.Value);
        }

        [HttpPatch("{orderId:long}/fulfillment-status")]
        [Authorize(Policy = AuthorizationPolicies.StoreOwnerOnly)]
        [ProducesResponseType(
    typeof(UpdateOrderFulfillmentStatusResponse),
    StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<UpdateOrderFulfillmentStatusResponse>>
    UpdateFulfillmentStatus(
        [FromRoute] long orderId,
        [FromBody] UpdateOrderFulfillmentStatusRequest request,
        CancellationToken cancellationToken)
        {
            // Step 1: Retrieve the authenticated Store Owner ID.
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userId, out var storeOwnerId)
                || storeOwnerId == Guid.Empty)
            {
                return Unauthorized();
            }

            // Step 2: Execute the Application service.
            var result = await _updateFulfillmentStatusService.ExecuteAsync(
                orderId,
                request,
                storeOwnerId,
                cancellationToken);

            // Step 3: Return the successful response.
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            // Step 4: Translate Application errors into HTTP responses.
            var errors = result.Errors;

            if (errors.Any(e =>
                e.Code == "StoreOrders.OrderNotFound"))
            {
                return NotFound(new { Errors = errors });
            }

            if (errors.Any(e =>
                e.Code == "StoreOrders.ConcurrencyConflict" ||
                e.Code == "StoreOrders.TerminalOrder" ||
                e.Code == "StoreOrders.StatusUnchanged" ||
                e.Code == "StoreOrders.InvalidTransition" ||
                e.Code == "StoreOrders.PaymentRequired"))
            {
                return Conflict(new { Errors = errors });
            }

            // Remaining errors are request-validation failures.
            return BadRequest(new { Errors = errors });
        }


        [HttpGet("{orderId:long}/fulfillment-history")]
        [Authorize(Policy = AuthorizationPolicies.StoreOwnerOnly)]
        [ProducesResponseType(
            typeof(GetOrderFulfillmentHistoryResponse),
            StatusCodes.Status200OK)]
        [ProducesResponseType(
            StatusCodes.Status400BadRequest)]
        [ProducesResponseType(
            StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(
            StatusCodes.Status403Forbidden)]
        [ProducesResponseType(
            StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GetOrderFulfillmentHistoryResponse>>
            GetFulfillmentHistory(
                [FromRoute] long orderId,
                CancellationToken cancellationToken)
        {
            // Step 1: Execute the Application service.
            var result = await _getOrderFulfillmentHistoryService.ExecuteAsync(
                orderId,
                cancellationToken);

            // Step 2: Return the response if successful.
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            // Step 3: Handle an Order that does not exist.
            if (result.Errors.Any(e =>
                e.Code == "StoreOrders.OrderNotFound"))
            {
                return NotFound(new
                {
                    Errors = result.Errors
                });
            }

            // Step 4: Handle invalid request input.
            return BadRequest(new
            {
                Errors = result.Errors
            });
        }


        [HttpPatch("{orderId:long}/payment-status")]
        [Authorize(Policy = AuthorizationPolicies.StoreOwnerOnly)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdatePaymentStatus(
    [FromRoute] long orderId,
    CancellationToken cancellationToken)
        {
            if (orderId <= 0)
            {
                return BadRequest(new
                {
                    Message = "Order ID must be greater than zero."
                });
            }

            var result = await _updatePaymentStatusService.ExecuteAsync(
                orderId,
                cancellationToken);

            return result switch
            {
                UpdatePaymentStatusResult.Success =>
                    Ok(new
                    {
                        Message = "Cash on Delivery payment recorded successfully.",
                        OrderId = orderId,
                        PaymentStatus = "Paid"
                    }),

                UpdatePaymentStatusResult.PaymentNotFound =>
                    NotFound(new
                    {
                        Message = "Payment was not found for this order."
                    }),

                UpdatePaymentStatusResult.InvalidPaymentMethod =>
                    Conflict(new
                    {
                        Message = "Only Cash on Delivery payments can be marked as paid."
                    }),

                UpdatePaymentStatusResult.AlreadyPaid =>
                    Conflict(new
                    {
                        Message = "Payment is already paid or is not eligible for collection."
                    }),

                UpdatePaymentStatusResult.ConcurrencyConflict =>
                    Conflict(new
                    {
                        Message = "Payment was modified by another request. Refresh and try again."
                    }),

                _ => throw new InvalidOperationException(
                    "Unexpected payment update result.")
            };
        }

        [HttpPost("{orderId:long}/cancel")]
        [Authorize(Policy = AuthorizationPolicies.StoreOwnerOnly)]
        [ProducesResponseType(
    typeof(CancelOrderResponse),
    StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<CancelOrderResponse>> CancelOrder(
    [FromRoute] long orderId,
    [FromBody] CancelOrderRequest request,
    CancellationToken cancellationToken)
        {
            // Step 1: Get the authenticated Store Owner's ID.
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userId, out var storeOwnerId) ||
                storeOwnerId == Guid.Empty)
            {
                return Unauthorized();
            }

            // Step 2: Execute the cancellation service.
            var result = await _cancelOrderService.ExecuteAsync(
                orderId,
                request,
                storeOwnerId,
                cancellationToken);

            // Step 3: Return the committed cancellation result.
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            // Step 4: Handle a nonexistent order.
            if (result.Errors.Any(e =>
                e.Code == "StoreOrders.OrderNotFound"))
            {
                return NotFound(new
                {
                    Errors = result.Errors
                });
            }

            // Step 5: Handle cancellation conflicts.
            if (result.Errors.Any(e =>
                e.Code == "StoreOrders.OrderAlreadyPaid" ||
                e.Code == "StoreOrders.AlreadyCancelled" ||
                e.Code == "StoreOrders.OrderFulfilled" ||
                e.Code == "StoreOrders.InvalidCancellationStage" ||
                e.Code == "StoreOrders.InvalidAllocationState" ||
                e.Code == "StoreOrders.AllocationsMissing" ||
                e.Code == "StoreOrders.UnsupportedPaymentMethod" ||
                e.Code == "StoreOrders.PaymentMissing" ||
                e.Code == "StoreOrders.ConcurrencyConflict"))
            {
                return Conflict(new
                {
                    Errors = result.Errors
                });
            }

            // Step 6: Handle invalid request inputs.
            return BadRequest(new
            {
                Errors = result.Errors
            });
        }
    }
}
