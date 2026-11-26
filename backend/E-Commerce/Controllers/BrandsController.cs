using E_Commerce.Contracts.Brands;
using ECommerce.Application.Brands.CreateBrand;
using ECommerce.Application.Brands.DeactivateBrand;
using ECommerce.Application.Brands.GetBrands;
using ECommerce.Application.Brands.UpdateBrand;
using ECommerce.Application.Common.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    [ApiController]
    [Route("api/brands")]
    public sealed class BrandsController : ControllerBase
    {
        private readonly CreateBrandService _createBrandService;
        private readonly GetBrandsService _getBrandsService;
        private readonly UpdateBrandService _updateBrandService;
        private readonly DeactivateBrandService _deactivateBrandService;
        public BrandsController(
            CreateBrandService createBrandService,
            GetBrandsService  getBrandsService,
            UpdateBrandService updateBrandService,
            DeactivateBrandService deactivateBrandService)
        {
            _createBrandService = createBrandService;
            _getBrandsService = getBrandsService;
            _updateBrandService = updateBrandService;
            _deactivateBrandService = deactivateBrandService;
        }

        // POST /api/brands
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicies.StoreOwnerOnly)]
        [ProducesResponseType(
            typeof(CreateBrandResponse),
            StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<CreateBrandResponse>> Create(
            [FromBody] CreateBrandRequest request,
            CancellationToken cancellationToken)
        {
            // Map API Request to Application Command
            var command = new CreateBrandCommand(
                request.Name,
                request.Description,
                request.LogoStorageKey);

            // Execute business logic
            var result = await _createBrandService.ExecuteAsync(
                command,
                cancellationToken);

            // Handle business failures
            if (result.IsFailure)
            {
                bool isConflict = result.Errors.Any(error =>
                    error.Code is
                        "Brand.NameAlreadyExists" or
                        "Brand.SlugAlreadyExists");

                if (isConflict)
                {
                    return Conflict(new
                    {
                        errors = result.Errors
                    });
                }

                return BadRequest(new
                {
                    errors = result.Errors
                });
            }

            // Return HTTP 201 Created
            return StatusCode(
                StatusCodes.Status201Created,
                result.Value);
        }



        [HttpGet]
        [Authorize(Policy = AuthorizationPolicies.StoreOwnerOnly)]
        [ProducesResponseType(
            typeof(GetBrandsResponse),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<GetBrandsResponse>> GetBrands(
            [FromQuery] GetBrandsRequest request,
            CancellationToken cancellationToken)
        {
            // Step 1: Map API request to Application query
            var query = new GetBrandsQuery(
                request.PageNumber,
                request.PageSize,
                request.Search,
                request.IsActive);

            // Step 2: Execute Application service
            var result = await _getBrandsService.ExecuteAsync(
                query,
                cancellationToken);

            // Step 3: Handle validation failures
            if (result.IsFailure)
            {
                return BadRequest(new
                {
                    errors = result.Errors
                });
            }

            // Step 4: Return paginated brands
            return Ok(result.Value);
        }



        [HttpPut("{brandId:long}")]
        [Authorize(Policy = AuthorizationPolicies.StoreOwnerOnly)]
        [ProducesResponseType(
            typeof(UpdateBrandResponse),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<UpdateBrandResponse>> UpdateBrand(
            [FromRoute] long brandId,
            [FromBody] UpdateBrandRequest request,
            CancellationToken cancellationToken)
        {
            // Step 1: Map HTTP request to Application command
            var command = new UpdateBrandCommand(
                brandId,
                request.Name,
                request.Description,
                request.LogoStorageKey);

            // Step 2: Execute the update use case
            var result = await _updateBrandService.ExecuteAsync(
                command,
                cancellationToken);

            // Step 3: Translate application errors to HTTP
            if (result.IsFailure)
            {
                if (result.Errors.Any(e =>
                    e.Code == "Brand.NotFound"))
                {
                    return NotFound(new
                    {
                        errors = result.Errors
                    });
                }

                if (result.Errors.Any(e =>
                    e.Code is "Brand.NameAlreadyExists"
                        or "Brand.SlugAlreadyExists"))
                {
                    return Conflict(new
                    {
                        errors = result.Errors
                    });
                }

                return BadRequest(new
                {
                    errors = result.Errors
                });
            }

            // Step 4: Return updated Brand
            return Ok(result.Value);
        }


        [HttpPatch("{brandId:long}/deactivate")]
        [Authorize(Policy = AuthorizationPolicies.StoreOwnerOnly)]
        [ProducesResponseType(
            typeof(DeactivateBrandResponse),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<DeactivateBrandResponse>> DeactivateBrand(
            [FromRoute] long brandId,
            CancellationToken cancellationToken)
        {
            // Step 1: Create Application command
            var command = new DeactivateBrandCommand(brandId);

            // Step 2: Execute Application service
            var result = await _deactivateBrandService.ExecuteAsync(
                command,
                cancellationToken);

            // Step 3: Handle business errors
            if (result.IsFailure)
            {
                if (result.Errors.Any(e =>
                    e.Code == "Brand.NotFound"))
                {
                    return NotFound(new
                    {
                        errors = result.Errors
                    });
                }

                if (result.Errors.Any(e =>
                    e.Code == "Brand.AlreadyInactive"))
                {
                    return Conflict(new
                    {
                        errors = result.Errors
                    });
                }

                return BadRequest(new
                {
                    errors = result.Errors
                });
            }

            // Step 4: Return successful deactivation
            return Ok(result.Value);
        }


    }
}
