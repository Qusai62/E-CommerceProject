using ECommerce.Application.Categories.PublicCategories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public sealed class CategoriesController : ControllerBase
    {
        private readonly GetPublicCategoriesService _getPublicCategoriesService;

        public CategoriesController(
            GetPublicCategoriesService getPublicCategoriesService)
        {
            _getPublicCategoriesService = getPublicCategoriesService;
        }

        // GET /api/categories
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(
            typeof(IReadOnlyList<PublicCategoryResponse>),
            StatusCodes.Status200OK)]
        public async Task<
            ActionResult<IReadOnlyList<PublicCategoryResponse>>> GetAll(
            CancellationToken cancellationToken)
        {
            var categories = await _getPublicCategoriesService.ExecuteAsync(
                cancellationToken);

            return Ok(categories);
        }
    }
}
