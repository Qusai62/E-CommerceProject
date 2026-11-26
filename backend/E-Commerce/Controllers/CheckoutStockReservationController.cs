using ECommerce.Application.Inventory.CreateStockReservation;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    [ApiController]
    [Route("api/checkout/payment-attempts")]
    public sealed class CheckoutStockReservationController
    : ControllerBase
    {
        private readonly CreateStockReservationService _service;
        private readonly IWebHostEnvironment _environment;

        public CheckoutStockReservationController(
            CreateStockReservationService service,
            IWebHostEnvironment environment)
        {
            _service = service;
            _environment = environment;
        }

        // TODO: Add authorization and verify customer owns the Order.
        // [Authorize]

        [HttpPost("{paymentAttemptId:long}/stock-reservation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<CreateStockReservationResponse>>
            CreateAsync(
                [FromRoute] long paymentAttemptId,
                CancellationToken cancellationToken)
        {
            // Temporary restriction for local development.
            if (!_environment.IsDevelopment())
                return NotFound();

            var command = new CreateStockReservationCommand(
                paymentAttemptId);

            var result = await _service.ExecuteAsync(
                command,
                cancellationToken);

            if (result.IsSuccess)
                return Ok(result.Value);

            if (result.Errors.Any(e =>
                e.Code == "Inventory.InvalidPaymentAttemptId"))
            {
                return BadRequest(result.Errors);
            }

            if (result.Errors.Any(e =>
                e.Code == "Inventory.PaymentAttemptNotFound"))
            {
                return NotFound(result.Errors);
            }

            return Conflict(result.Errors);
        }
    }
}