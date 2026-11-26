using E_Commerce.Contracts.Products;
using ECommerce.Application.Catalog.ProductVariant.CreateProductVariant;
using ECommerce.Application.Catalog.ProductVariant.UpdateProductVariant;
using ECommerce.Application.Common.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    [ApiController]
    [Route("api/products/{productId:long}/variants")]

    public class ProductVariantsController : ControllerBase
    {
        private readonly CreateProductVariantService _createProductVariantService;

        public ProductVariantsController(CreateProductVariantService createProductVariantService)
        {
            _createProductVariantService = createProductVariantService;
        }


        [HttpPost]
       
        [ProducesResponseType(
    typeof(CreateProductVariantResponse),
    StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateVariant(
    [FromRoute] long productId,
    [FromBody] CreateProductVariantRequest request,
    CancellationToken cancellationToken)
        {
            var command = new CreateProductVariantCommand
            {
                ProductId = productId,
                Sku = request.Sku,
                Price = request.Price,
                Attributes = request.Attributes?
                    .Select(a => a is null
                        ? null!
                        : new VariantAttributeCommand
                        {
                            Name = a.Name,
                            Value = a.Value,
                            SortOrder = a.SortOrder
                        })
                    .ToList()!
            };

            var result = await _createProductVariantService.ExecuteAsync(
                command, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Errors.Any(e =>
                        e.Code == "Product.NotFound"))
                {
                    return NotFound(result.Errors);
                }

                if (result.Errors.Any(e =>
                        e.Code == "Variant.DuplicateSku"))
                {
                    return Conflict(result.Errors);
                }

                return BadRequest(result.Errors);
            }

            return StatusCode(
                StatusCodes.Status201Created,
                result.Value);
        }

        [HttpPut("{variantId:long}")]
        [Authorize(Policy = AuthorizationPolicies.StoreOwnerOnly)]
        [ProducesResponseType(
           typeof(UpdateProductVariantResponse),
           StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<UpdateProductVariantResponse>>
           UpdateProductVariant(
               [FromRoute] long productId,
               [FromRoute] long variantId,
               [FromBody] UpdateProductVariantRequest request,
               [FromServices] UpdateProductVariantService service,
               CancellationToken cancellationToken)
        {
            var command = new UpdateProductVariantCommand
            {
                ProductId = productId,
                ProductVariantId = variantId,
                Sku = request.Sku,
                Price = request.Price,
                IsActive = request.IsActive
            };

            var result = await service.ExecuteAsync(
                command,
                cancellationToken);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            var errors = result.Errors;

            if (errors.Any(e => e.Code == "VariantNotFound"))
            {
                return NotFound(errors);
            }

            if (errors.Any(e => e.Code == "Variant.DuplicateSku"))
            {
                return Conflict(errors);
            }

            return BadRequest(errors);
        }
    }
}
