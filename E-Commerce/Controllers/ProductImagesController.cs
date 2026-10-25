using E_Commerce.Contracts.Products;
using ECommerce.Application.Catalog.ProductImage.AddProductImage;
using ECommerce.Application.Catalog.ProductImage.UpdateProductImage;
using ECommerce.Application.Catalog.ProductImageApplication.DeleteProductImage;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    [ApiController]
    [Route("api/products/{productId:long}/images")]
    public class ProductImagesController : ControllerBase
    {
        private readonly AddProductImageService _addProductImageService;
        private readonly UpdateProductImageService _updateProductImageService;
        private readonly DeleteProductImageService _deleteProductImageService;
        public ProductImagesController(AddProductImageService addProductImageService,
            DeleteProductImageService deleteProductImageService,
            UpdateProductImageService updateProductImageService)
        {
            _addProductImageService = addProductImageService;
            _deleteProductImageService = deleteProductImageService;
            _updateProductImageService = updateProductImageService;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(6 * 1024 * 1024)]
        [ProducesResponseType(
    typeof(AddProductImageResponse),
    StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddImage(
    [FromRoute] long productId,
    [FromForm] AddProductImageRequest request,
    CancellationToken cancellationToken)
        {
            if (request.File is null || request.File.Length ==0)
            {
                return BadRequest(new
                {
                    Code = "Image.FileRequired",
                    Message = "An image file is required."
                });
            }

            await using var stream = request.File.OpenReadStream();

            var command = new AddProductImageCommand
            {
                ProductId = productId,
                FileContent = stream,
                FileName = request.File.FileName,
                ContentType = request.File.ContentType,
                FileSize = request.File.Length,
                AltText = request.AltText,
                SortOrder = request.SortOrder
            };

            var result = await _addProductImageService.ExecuteAsync(
                command,
                cancellationToken);

            if (result.IsFailure)
            {
                if (result.Errors.Any(e => e.Code == "Product.NotFound"))
                {
                    return NotFound(result.Errors);
                }

                return BadRequest(result.Errors);
            }

            return StatusCode(
                StatusCodes.Status201Created,
                result.Value);
        }


        [HttpPut("{imageId:long}")]
        [ProducesResponseType(
            typeof(UpdateProductImageResponse),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UpdateProductImageResponse>>
            UpdateProductImage(
                [FromRoute] long productId,
                [FromRoute] long imageId,
                [FromBody] UpdateProductImageRequest request,
                CancellationToken cancellationToken)
        {
            // 1. Map Request to Command
            var command = new UpdateProductImageCommand
            {
                ProductId = productId,
                ProductImageId = imageId,
                AltText = request.AltText,
                SortOrder = request.SortOrder,
                IsPrimary = request.IsPrimary
            };

            // 2. Execute Application Service
            var result = await _updateProductImageService.ExecuteAsync(
                command,
                cancellationToken);

            // 3. Handle errors
            if (result.IsFailure)
            {
                if (result.Errors.Any(e =>
                    e.Code == "Image.NotFound"))
                {
                    return NotFound(result.Errors);
                }

                return BadRequest(result.Errors);
            }

            // 4. Return updated image
            return Ok(result.Value);
        }


        [HttpDelete("{imageId:long}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteImage(
            [FromRoute] long productId,
            [FromRoute] long imageId,
            CancellationToken cancellationToken)
        {
            var command = new DeleteProductImageCommand
            {
                ProductId = productId,
                ProductImageId = imageId
            };

            var result = await _deleteProductImageService.ExecuteAsync(
                command,
                cancellationToken);

            if (result.IsFailure)
            {
                if (result.Errors.Any(e =>
                    e.Code == "Image.NotFound"))
                {
                    return NotFound(result.Errors);
                }

                return BadRequest(result.Errors);
            }

            return NoContent();
        }

    }
}
