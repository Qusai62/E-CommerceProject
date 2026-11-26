using E_Commerce.Contracts.Inventory;
using ECommerce.Application.Common.Pagination;
using ECommerce.Application.Inventory.DecreaseStock;
using ECommerce.Application.Inventory.IncreaseStock;
using ECommerce.Application.Inventory.StoreOwnerViewStock;
using ECommerce.Application.Inventory.ViewOutOfStock;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    [ApiController]
    [Route("api/owner/inventory")]
    //[Authorize(Policy = "StoreOwnerOnly")]
    // TODO: Enable before production deployment.
    public sealed class StoreOwnerStockController : ControllerBase
    {
        private readonly StoreOwnerViewStockService _storeOwnerViewStockService;
        private readonly IncreaseStockService _increaseStockService;
        private readonly DecreaseStockService _decreaseStockService;
        private readonly ViewOutOfStockService _viewOutOfStockService;

        public StoreOwnerStockController(StoreOwnerViewStockService storeOwnerViewStockService,IncreaseStockService increaseStockService,
               DecreaseStockService decreaseStockService, ViewOutOfStockService viewOutOfStockService)
        {
            _storeOwnerViewStockService = storeOwnerViewStockService;
            _increaseStockService = increaseStockService;
            _decreaseStockService = decreaseStockService;
            _viewOutOfStockService = viewOutOfStockService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<PagedResult<StoreOwnerStockResponse>>> GetAsync(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var result = await _storeOwnerViewStockService.ExecuteAsync(
                pageNumber,
                pageSize,
                cancellationToken);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(result.Errors);
        }
      
        // Feature 2 — Increase Stock
        [HttpPost("variants/{productVariantId:long}/increase")]
        public async Task<ActionResult<IncreaseStockResponse>> IncreaseAsync(
            [FromRoute] long productVariantId,
            [FromBody] IncreaseStockRequest request,
            CancellationToken cancellationToken)
        {

            var command = new IncreaseStockCommand(
                productVariantId,
                request.Quantity);

            var result = await _increaseStockService.ExecuteAsync(
                command,
                cancellationToken);

            if (result.IsSuccess)
                return Ok(result.Value);

            if (result.Errors.Any(e =>
                e.Code == "Inventory.VariantNotFound"))
            {
                return NotFound(result.Errors);
            }

            if (result.Errors.Any(e =>
                e.Code == "Inventory.QuantityLimitExceeded"))
            {
                return Conflict(result.Errors);
            }

            if (result.Errors.Any(e =>
                e.Code is "Inventory.InvalidVariantId"
                    or "Inventory.InvalidQuantity"))
            {
                return BadRequest(result.Errors);
            }

            return Problem(statusCode: 500);
        }
        // Feature 4 — Decrease Stock
        [HttpPost("variants/{productVariantId:long}/decrease")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<DecreaseStockResponse>> DecreaseAsync(
            [FromRoute] long productVariantId,
            [FromBody] DecreaseStockRequest request,
            CancellationToken cancellationToken)
        {           

            // 1. Create the Application Command
            var command = new DecreaseStockCommand(
                productVariantId,
                request.Quantity);

            // 2. Execute the Application Service
            var result = await _decreaseStockService.ExecuteAsync(
                command,
                cancellationToken);

            // 3. Handle Success
            if (result.IsSuccess)
                return Ok(result.Value);

            // 4. Handle Variant Not Found
            if (result.Errors.Any(e =>
                e.Code == "Inventory.VariantNotFound"))
            {
                return NotFound(result.Errors);
            }

            // 5. Handle Insufficient Available Stock
            if (result.Errors.Any(e =>
                e.Code == "Inventory.InsufficientAvailableStock"))
            {
                return Conflict(result.Errors);
            }

            // 6. Handle Validation Errors
            if (result.Errors.Any(e =>
                e.Code is "Inventory.InvalidVariantId"
                    or "Inventory.InvalidQuantity"))
            {
                return BadRequest(result.Errors);
            }

            // 7. Handle Unexpected Application Errors
            return Problem(
                title: "Unexpected inventory decrease failure.",
                statusCode: StatusCodes.Status500InternalServerError);
        }

        // Feature 6 — View Out-of-Stock Variants
        [HttpGet("out-of-stock/variants")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResult<StoreOwnerStockResponse>>>
            GetOutOfStockVariantsAsync(
                [FromQuery] int pageNumber = 1,
                [FromQuery] int pageSize = 10,
                CancellationToken cancellationToken = default)
        {
            // Temporary local-development restriction
           

            // 1. Call Application Service
            var result = await _viewOutOfStockService.GetVariantsAsync(
                pageNumber,
                pageSize,
                cancellationToken);

            // 2. Handle success
            if (result.IsSuccess)
                return Ok(result.Value);

            // 3. Handle pagination validation errors
            return BadRequest(result.Errors);
        }

        // Feature 6 — View Out-of-Stock Products
        [HttpGet("out-of-stock/products")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResult<OutOfStockProductResponse>>>
            GetOutOfStockProductsAsync(
                [FromQuery] int pageNumber = 1,
                [FromQuery] int pageSize = 10,
                CancellationToken cancellationToken = default)
        {
           

            // 1. Call Application Service
            var result = await _viewOutOfStockService.GetProductsAsync(
                pageNumber,
                pageSize,
                cancellationToken);

            // 2. Handle success
            if (result.IsSuccess)
                return Ok(result.Value);

            // 3. Handle pagination validation errors
            return BadRequest(result.Errors);
        }
    }
}
