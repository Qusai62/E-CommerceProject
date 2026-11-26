using E_Commerce.Contracts.CartRequest;
using ECommerce.Application.Common.Security;
using ECommerce.Application.ShoppingCart.AddCartItem;
using ECommerce.Application.ShoppingCart.ClearCart;
using ECommerce.Application.ShoppingCart.GetShoppingCart;
using ECommerce.Application.ShoppingCart.RemoveCartItem;
using ECommerce.Application.ShoppingCart.UpdateCartItemQuantity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_Commerce.Controllers
{
    [ApiController]
    [Route("api/cart")]
    [AllowAnonymous]
    public sealed class ShoppingCartController : ControllerBase
    {
        private readonly GetShoppingCartService _getShoppingCartService;
        private readonly AddCartItemService _addCartItemService;
        private readonly UpdateCartItemQuantityService _updateQuantityService;
        private readonly RemoveCartItemService _removeCartItemService;
        private readonly ClearCartService _clearCartService;

        public ShoppingCartController(
            GetShoppingCartService service,AddCartItemService addCartItemService, 
            UpdateCartItemQuantityService updateCartItemQuantityService,
            RemoveCartItemService removeCartItemService, ClearCartService clearCartService)
        {
            _getShoppingCartService = service;
            _addCartItemService = addCartItemService;
            _updateQuantityService = updateCartItemQuantityService;
            _removeCartItemService = removeCartItemService;
            _clearCartService = clearCartService;
        }

        [HttpGet]
        [ProducesResponseType(
            typeof(ShoppingCartResponse),
            StatusCodes.Status200OK)]
        [ProducesResponseType(
            StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(
            StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetShoppingCart(
            CancellationToken cancellationToken)
        {
            // Step 1: Identify authenticated user
            var userIdValue = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(
                userIdValue,
                out var applicationUserId))
            {
                return Unauthorized();
            }

            // Step 2: Execute application use case
            var result = await _getShoppingCartService.ExecuteAsync(
                applicationUserId,
                cancellationToken);

            // Step 3: Handle application failure
            if (result.IsFailure)
            {
                return NotFound(new
                {
                    Code = "ShoppingCart.NotFound",
                    Message = "Shopping cart was not found."
                });
            }

            // Step 4: Return successful response
            return Ok(result.Value);
        }

        [HttpPost("items")]
        [ProducesResponseType(
    typeof(AddCartItemResponse),
    StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AddItem(
    [FromBody] AddCartItemRequest request,
    CancellationToken cancellationToken)
        {
            // 1. Get authenticated user
            var userIdValue = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdValue, out var applicationUserId))
            {
                return Unauthorized();
            }

            // 2. Map API request to Application command
            var command = new AddCartItemCommand(
                request.ProductVariantId,
                request.Quantity);

            // 3. Execute the use case
            var result = await _addCartItemService.ExecuteAsync(
                applicationUserId,
                command,
                cancellationToken);

            // 4. Map application errors to HTTP responses
            if (result.IsFailure)
            {
                var error = result.Errors[0];

                return error.Code switch
                {
                    "Cart.NotFound" or "Cart.VariantNotFound" =>
                        NotFound(result.Errors),

                    "Cart.InsufficientStock" =>
                        Conflict(result.Errors),

                    "Cart.InvalidQuantity" or
                    "Cart.InvalidVariant" or
                    "Cart.VariantInactive" =>
                        BadRequest(result.Errors),

                    _ => StatusCode(
                        StatusCodes.Status500InternalServerError)
                };
            }

            // 5. Return resulting item
            return Ok(result.Value);
        }

        [HttpPut("items/{cartItemId:long}")]
        [ProducesResponseType(
    typeof(UpdateCartItemQuantityResponse),
    StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateCartItemQuantity(
    [FromRoute] long cartItemId,
    [FromBody] UpdateCartItemQuantityRequest request,
    CancellationToken cancellationToken)
        {
            // 1. Get authenticated user
            var userIdValue = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(
                userIdValue,
                out var applicationUserId))
            {
                return Unauthorized();
            }

            // 2. Create application command
            var command = new UpdateCartItemQuantityCommand(
                cartItemId,
                request.Quantity);

            // 3. Execute business operation
            var result = await _updateQuantityService.ExecuteAsync(
                applicationUserId,
                command,
                cancellationToken);

            // 4. Map errors to HTTP responses
            if (result.IsFailure)
            {
                var error = result.Errors[0];

                return error.Code switch
                {
                    "Cart.ItemNotFound" or
                    "Cart.VariantNotFound" =>
                        NotFound(result.Errors),

                    "Cart.InsufficientStock" =>
                        Conflict(result.Errors),

                    "Cart.InvalidQuantity" or
                    "Cart.VariantInactive" =>
                        BadRequest(result.Errors),

                    _ => StatusCode(
                        StatusCodes.Status500InternalServerError)
                };
            }

            // 5. Return updated item
            return Ok(result.Value);
        }

        [HttpDelete("items/{cartItemId:long}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveCartItem(
    [FromRoute] long cartItemId,
    CancellationToken cancellationToken)
        {
            // Step 1: Get authenticated user identifier
            var userIdValue = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(
                userIdValue,
                out var applicationUserId))
            {
                return Unauthorized();
            }

            // Step 2: Create application command
            var command = new RemoveCartItemCommand(
                cartItemId);

            // Step 3: Execute use case
            var result = await _removeCartItemService.ExecuteAsync(
                applicationUserId,
                command,
                cancellationToken);

            // Step 4: Handle errors
            if (result.IsFailure)
            {
                var error = result.Errors[0];

                return error.Code switch
                {
                    "Cart.InvalidItemId" =>
                        BadRequest(result.Errors),

                    "Cart.ItemNotFound" =>
                        NotFound(result.Errors),

                    _ => StatusCode(
                        StatusCodes.Status500InternalServerError)
                };
            }

            // Step 5: Return 204
            return NoContent();
        }

        [HttpDelete("items")]
        [ProducesResponseType(
    StatusCodes.Status204NoContent)]
        [ProducesResponseType(
    StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(
    StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ClearCart(
    CancellationToken cancellationToken)
        {
            // Step 1: Get authenticated user
            var userIdValue = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(
                userIdValue,
                out var applicationUserId))
            {
                return Unauthorized();
            }

            // Step 2: Create command
            var command = new ClearCartCommand();

            // Step 3: Execute application service
            var result = await _clearCartService.ExecuteAsync(
                applicationUserId,
                command,
                cancellationToken);

            // Step 4: Handle failure
            if (result.IsFailure)
            {
                var error = result.Errors[0];

                return error.Code switch
                {
                    "Cart.NotFound" =>
                        NotFound(result.Errors),

                    _ => StatusCode(
                        StatusCodes.Status500InternalServerError)
                };
            }

            // Step 5: Return success
            return NoContent();
        }
    }

}