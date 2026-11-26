using E_Commerce.Contracts.Categories;
using ECommerce.Application.Categories.CreateCategory;
using ECommerce.Application.Categories.DeactivateCategory;
using ECommerce.Application.Categories.ManageHierarchy;
using ECommerce.Application.Categories.ReactivateCategory;
using ECommerce.Application.Categories.UpdateCategory;
using ECommerce.Application.Categories.ViewCategories;
using ECommerce.Application.Categories.ViewHierarchy;
using ECommerce.Application.Common.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    [ApiController]
    [Route("api/store/categories")]
  
    public sealed class StoreCategoriesController : ControllerBase
    {
        private readonly CreateCategoryService _createCategoryService;
        private readonly ViewCategoriesService _viewCategoriesService;
        private readonly UpdateCategoryService _updateCategoryService;
        private readonly MoveCategoryService _moveCategoryService;
        private readonly GetCategoryHierarchyService _hierarchyService;
        private readonly DeactivateCategoryService _deactivateCategoryService;
        private readonly ReactivateCategoryService _reactivateCategoryService;
        public StoreCategoriesController(
            CreateCategoryService createCategoryService,
            ViewCategoriesService viewCategoriesService,
            UpdateCategoryService updateCategoryService,
            MoveCategoryService moveCategoryService,
            GetCategoryHierarchyService getCategoryHierarchyService,
            DeactivateCategoryService deactivateCategoryService,
            ReactivateCategoryService  reactivateCategoryService)
        {
            _createCategoryService = createCategoryService;
            _viewCategoriesService = viewCategoriesService;
            _updateCategoryService = updateCategoryService;
            _moveCategoryService = moveCategoryService;
            _hierarchyService = getCategoryHierarchyService;
            _deactivateCategoryService = deactivateCategoryService;
            _reactivateCategoryService = reactivateCategoryService;
            
        }

        // POST /api/store/categories
        [HttpPost]
        [ProducesResponseType(
            typeof(CreateCategoryResponse),
            StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create(
            [FromBody] CreateCategoryCommand command,
            CancellationToken cancellationToken)
        {
            // STEP 1: Execute the Application use case.
            var result = await _createCategoryService.ExecuteAsync(
                command,
                cancellationToken);

            // STEP 2: Handle business errors.
            if (result.IsFailure)
            {
                if (result.Errors.Any(error =>
                    error.Code == "Category.ParentNotFound"))
                {
                    return NotFound(result.Errors);
                }

                if (result.Errors.Any(error =>
                    error.Code == "Category.DuplicateName"))
                {
                    return Conflict(result.Errors);
                }

                return BadRequest(result.Errors);
            }

            // STEP 3: Return the newly created category.
            return StatusCode(
                StatusCodes.Status201Created,
                result.Value);
        }

        // GET /api/store/categories
        [HttpGet]
        [ProducesResponseType(
            typeof(IReadOnlyList<CategoryListItemResponse>),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<
            ActionResult<IReadOnlyList<CategoryListItemResponse>>> GetAll(
            CancellationToken cancellationToken)
        {
            var categories = await _viewCategoriesService.ExecuteAsync(
                cancellationToken);

            return Ok(categories);
        }

        // PUT /api/store/categories/{categoryId}
        [HttpPut("{categoryId:long}")]
        [ProducesResponseType(
            typeof(UpdateCategoryResponse),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Update(
            [FromRoute] long categoryId,
            [FromBody] UpdateCategoryRequest request,
            CancellationToken cancellationToken)
        {
            var command = new UpdateCategoryCommand(
                categoryId,
                request.Name,
                request.Description,
                request.SortOrder);

            var result = await _updateCategoryService.ExecuteAsync(
                command,
                cancellationToken);

            if (result.IsFailure)
            {
                if (result.Errors.Any(error =>
                    error.Code == "Category.NotFound"))
                {
                    return NotFound(result.Errors);
                }

                if (result.Errors.Any(error =>
                    error.Code == "Category.DuplicateName"))
                {
                    return Conflict(result.Errors);
                }

                return BadRequest(result.Errors);
            }

            return Ok(result.Value);
        }


        // PUT /api/store/categories/{categoryId}/parent
        [HttpPut("{categoryId:long}/parent")]
        [ProducesResponseType(
            typeof(MoveCategoryResponse),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> MoveCategory(
            [FromRoute] long categoryId,
            [FromBody] MoveCategoryRequest request,
            CancellationToken cancellationToken)
        {
            var command = new MoveCategoryCommand(
                categoryId,
                request.ParentCategoryId);

            var result = await _moveCategoryService.ExecuteAsync(
                command,
                cancellationToken);

            if (result.IsFailure)
            {
                if (result.Errors.Any(error =>
                    error.Code is "Category.NotFound"
                        or "Category.ParentNotFound"))
                {
                    return NotFound(result.Errors);
                }

                if (result.Errors.Any(error =>
                    error.Code is "Category.DuplicateName"
                        or "Category.SelfParent"
                        or "Category.CircularHierarchy"))
                {
                    return Conflict(result.Errors);
                }

                return BadRequest(result.Errors);
            }

            return Ok(result.Value);
        }


        // GET /api/store/categories/hierarchy
        [HttpGet("hierarchy")]
        [ProducesResponseType(
            typeof(IReadOnlyList<CategoryHierarchyResponse>),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IReadOnlyList<CategoryHierarchyResponse>>>
            GetHierarchy(CancellationToken cancellationToken)
        {
            var hierarchy = await _hierarchyService.ExecuteAsync(
                cancellationToken);

            return Ok(hierarchy);
        }



        // PATCH /api/store/categories/{categoryId}/deactivate
        [HttpPatch("{categoryId:long}/deactivate")]
        [ProducesResponseType(
            typeof(DeactivateCategoryResponse),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Deactivate(
            [FromRoute] long categoryId,
            CancellationToken cancellationToken)
        {
            var command = new DeactivateCategoryCommand(categoryId);

            var result = await _deactivateCategoryService.ExecuteAsync(
                command,
                cancellationToken);

            if (result.IsFailure)
            {
                if (result.Errors.Any(error =>
                    error.Code == "Category.NotFound"))
                {
                    return NotFound(result.Errors);
                }

                if (result.Errors.Any(error =>
                    error.Code == "Category.AlreadyInactive"))
                {
                    return Conflict(result.Errors);
                }

                return BadRequest(result.Errors);
            }

            return Ok(result.Value);
        }



        // PATCH /api/store/categories/{categoryId}/reactivate
        [HttpPatch("{categoryId:long}/reactivate")]
        [ProducesResponseType(
            typeof(ReactivateCategoryResponse),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Reactivate(
            [FromRoute] long categoryId,
            CancellationToken cancellationToken)
        {
            var command = new ReactivateCategoryCommand(categoryId);

            var result = await _reactivateCategoryService.ExecuteAsync(
                command,
                cancellationToken);

            if (result.IsFailure)
            {
                if (result.Errors.Any(error =>
                    error.Code == "Category.NotFound"))
                {
                    return NotFound(result.Errors);
                }

                if (result.Errors.Any(error =>
                    error.Code == "Category.AlreadyActive"))
                {
                    return Conflict(result.Errors);
                }

                return BadRequest(result.Errors);
            }

            return Ok(result.Value);
        }

    }
}
