using ECommerce.Application.Common.Pagination;
using ECommerce.Application.Inventory.GetVariantAvailability;
using ECommerce.Application.Inventory.StoreOwnerViewStock;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    [ApiController]
    [Route("api/variants")]
    public class VariantAvailabilityController : ControllerBase
    {
        private readonly GetVariantAvailabilityService _getVariantAvailabilityService;
       

        public VariantAvailabilityController(
            GetVariantAvailabilityService service)
        {
            _getVariantAvailabilityService = service;
          

        }

        [HttpGet("{productVariantId:long}/availability")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GetVariantAvailabilityResponse>> GetAsync(
       [FromRoute] long productVariantId,
       CancellationToken cancellationToken)
        {
            var result = await _getVariantAvailabilityService.ExecuteAsync(
                productVariantId,
                cancellationToken);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            if (result.Errors.Any(e =>
                e.Code == "Inventory.VariantNotFound"))
            {
                return NotFound(result.Errors);
            }

            return BadRequest(result.Errors);
        }

       
    }
}
