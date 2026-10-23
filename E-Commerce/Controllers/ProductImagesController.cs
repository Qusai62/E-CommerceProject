using E_Commerce.Contracts.Products;
using ECommerce.Application.Catalog.ProductImage.AddProductImage;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    [ApiController]
    [Route("api/products/{productId:long}/images")]
    public class ProductImagesController : ControllerBase
    {
        private readonly AddProductImageService _addProductImageService;

        public ProductImagesController(AddProductImageService addProductImageService)
        {
            _addProductImageService = addProductImageService;
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
    }
}
