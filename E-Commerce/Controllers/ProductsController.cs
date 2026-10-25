using E_Commerce.Contracts.Products;
using ECommerce.Application.Catalog.GetProducts;
using ECommerce.Application.Catalog.Product.CreateProduct;
using ECommerce.Application.Catalog.Product.GetProductById;
using ECommerce.Application.Catalog.Product.UpdateProduct;
using ECommerce.Application.Catalog.ProductVariant.UpdateProductVariant;
using ECommerce.Application.Common.Pagination;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly GetProductByIdService _getProductByIdService;
        private readonly CreateProductService _createProductService;
        private readonly UpdateProductService _updateProductService;
        private readonly GetProductsService _getProductsService;

        public ProductsController(GetProductByIdService getProductByIdService, CreateProductService createProductService,
            UpdateProductService updateProductService,
             UpdateProductVariantService updateProductVariantService,
             GetProductsService getProductsService)
        {
            _getProductByIdService = getProductByIdService;
            _createProductService = createProductService;
            _updateProductService = updateProductService;
            _getProductsService = getProductsService;
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id,CancellationToken cancellationToken)
        {
           var product=await _getProductByIdService.ExecuteAsync(id,cancellationToken);

            if(product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        //These attributes describe the HTTP responses the endpoint is intended to return.
        public async Task<IActionResult> Create(
    [FromBody] CreateProductCommand command,
    CancellationToken cancellationToken)
        {
            var result = await _createProductService.ExecuteAsync(
                command,
                cancellationToken);
           
            if (result.IsFailure)
            {
                var hasDuplicateConflict = result.Errors.Any(error =>
                    error.Code == "Product.DuplicateSlug");

                if (hasDuplicateConflict)
                    return Conflict(result.Errors);

                return BadRequest(result.Errors);
            }

            var response = result.Value!;//!: It tells the C# compiler not to warn about a possible null value for that expression.
            
            return CreatedAtAction(
                nameof(GetById),
                new { id = response.ProductId },
                response);
        }


        [HttpPut("{id:long}")]
        [ProducesResponseType(
            typeof(UpdateProductResponse),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(
            [FromRoute] long id,
            [FromBody] UpdateProductRequest request,
            CancellationToken cancellationToken)
        {
            // 1. Map API Request to Application Command
            var command = new UpdateProductCommand
            {
                ProductId = id,
                Name = request.Name,
                Description = request.Description,
                BrandId = request.BrandId,
                CategoryId = request.CategoryId
            };

            // 2. Execute application business logic
            var result = await _updateProductService.ExecuteAsync(
                command,
                cancellationToken);

            // 3. Handle expected failures
            if (result.IsFailure)
            {
                if (result.Errors.Any(e =>
                    e.Code == "Product.NotFound"))
                {
                    return NotFound(result.Errors);
                }

                return BadRequest(result.Errors);
            }

            // 4. Return updated product
            return Ok(result.Value);
        }


        [HttpGet]
        [ProducesResponseType(
            typeof(PagedResult<ProductListItemResponse>),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetProducts(
            [FromQuery] GetProductsRequest request,
            CancellationToken cancellationToken)
        {
            var query = new GetProductsQuery
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };

            var result = await _getProductsService.ExecuteAsync(
                query,
                cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Errors);
            }

            return Ok(result.Value);
        }

    }
}
