using E_Commerce.Contracts.Products;
using ECommerce.Application.Catalog.TechnicalSpecification.AddTechnicalSpecification;
using ECommerce.Application.Catalog.TechnicalSpecification.DeleteTechnicalSpecification;
using ECommerce.Application.Catalog.TechnicalSpecification.UpdateTechnicalSpecification;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class TechnicalSpecificationsController : ControllerBase
    {
        private readonly AddTechnicalSpecificationService _addTechnicalSpecificationService;
        private readonly UpdateTechnicalSpecificationService _updateTechnicalSpecificationService;
        private readonly DeleteTechnicalSpecificationService _deleteTechnicalSpecificationService;
        public TechnicalSpecificationsController(AddTechnicalSpecificationService addTechnicalSpecificationService,
            UpdateTechnicalSpecificationService updateTechnicalSpecificationService, DeleteTechnicalSpecificationService deleteTechnicalSpecificationService)
        {
            _addTechnicalSpecificationService = addTechnicalSpecificationService;
            _updateTechnicalSpecificationService = updateTechnicalSpecificationService;
            _deleteTechnicalSpecificationService = deleteTechnicalSpecificationService;
        }

        [HttpPost("{productId:long}/specifications")]
        [ProducesResponseType(
            typeof(AddTechnicalSpecificationResponse),
            StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AddSpecification(
            [FromRoute] long productId,
            [FromBody] AddTechnicalSpecificationRequest request,
            CancellationToken cancellationToken)
        {
            // 1. Map API Request to Application Command
            var command = new AddTechnicalSpecificationCommand
            {
                ProductId = productId,
                Name = request.Name,
                Value = request.Value,
                SortOrder = request.SortOrder
            };

            // 2. Execute Application business logic
            var result = await _addTechnicalSpecificationService
                .ExecuteAsync(command, cancellationToken);

            // 3. Handle failure
            if (result.IsFailure)
            {
                if (result.Errors.Any(e =>
                    e.Code == "Product.NotFound"))
                {
                    return NotFound(result.Errors);
                }

                if (result.Errors.Any(e =>
                    e.Code == "Specification.DuplicateName"))
                {
                    return Conflict(result.Errors);
                }

                return BadRequest(result.Errors);
            }

            // 4. Return success
            return StatusCode(
                StatusCodes.Status201Created,
                result.Value);
        }

        [HttpPut("{productId:long}/specifications/{specificationId:long}")]
        [ProducesResponseType(
    typeof(UpdateTechnicalSpecificationResponse),
    StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateSpecification(
    [FromRoute] long productId,
    [FromRoute] long specificationId,
    [FromBody] UpdateTechnicalSpecificationRequest request,
    CancellationToken cancellationToken)
        {
            // 1. Convert API request to Application command
            var command = new UpdateTechnicalSpecificationCommand
            {
                ProductId = productId,
                SpecificationId = specificationId,
                Name = request.Name,
                Value = request.Value,
                SortOrder = request.SortOrder
            };

            // 2. Execute the Application service
            var result = await _updateTechnicalSpecificationService
                .ExecuteAsync(command, cancellationToken);

            // 3. Handle business errors
            if (result.IsFailure)
            {
                if (result.Errors.Any(e =>
                    e.Code == "Specification.NotFound"))
                {
                    return NotFound(result.Errors);
                }

                if (result.Errors.Any(e =>
                    e.Code == "Specification.DuplicateName"))
                {
                    return Conflict(result.Errors);
                }

                return BadRequest(result.Errors);
            }

            // 4. Return updated specification
            return Ok(result.Value);
        }

        [HttpDelete("{productId:long}/specifications/{specificationId:long}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteSpecification(
            [FromRoute] long productId,
            [FromRoute] long specificationId,
            CancellationToken cancellationToken)
        {
            var command = new DeleteTechnicalSpecificationCommand
            {
                ProductId = productId,
                SpecificationId = specificationId
            };

            var result = await _deleteTechnicalSpecificationService
                .ExecuteAsync(command, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Errors.Any(e =>
                    e.Code == "Specification.NotFound"))
                {
                    return NotFound(result.Errors);
                }

                return BadRequest(result.Errors);
            }

            return NoContent();
        }
    }
}