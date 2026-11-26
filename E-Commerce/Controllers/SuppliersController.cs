using ECommerce.Application.Common.Security;
using ECommerce.Application.Suppliers.CreateSupplier;
using ECommerce.Application.Suppliers.GetSupplierById;
using ECommerce.Application.Suppliers.ListSuppliers;
using ECommerce.Application.Suppliers.UpdateSupplier;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    [ApiController]
    [Route("api/suppliers")]
    [Authorize(Policy = AuthorizationPolicies.StoreOwnerOnly)]
    public sealed class SuppliersController : ControllerBase
    {
        private readonly CreateSupplierService _createSupplierService;
        private readonly GetSupplierByIdService _getSupplierByIdService;
        private readonly ListSuppliersService _listSuppliersService;
        private readonly UpdateSupplierService _updateSupplierService;
        public SuppliersController(
            CreateSupplierService createSupplierService,
            GetSupplierByIdService getSupplierByIdService,
            ListSuppliersService listSuppliersService,
            UpdateSupplierService updateSupplierService)
        {
            _createSupplierService = createSupplierService;
            _getSupplierByIdService = getSupplierByIdService;
            _listSuppliersService = listSuppliersService;
            _updateSupplierService = updateSupplierService;
        }

        [HttpPost]
        [ProducesResponseType(
            typeof(CreateSupplierResponse),
            StatusCodes.Status201Created)]
        [ProducesResponseType(
            StatusCodes.Status400BadRequest)]
        [ProducesResponseType(
            StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(
            StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<CreateSupplierResponse>> Create(
            [FromBody] CreateSupplierRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var response = await _createSupplierService.ExecuteAsync(
                    request,
                    cancellationToken);

                return Created(
                    $"/api/suppliers/{response.SupplierId}",
                    response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Invalid supplier information",
                    Detail = ex.Message
                });
            }
        }


        [HttpGet("{id:long}")]
        [ProducesResponseType(
    typeof(SupplierResponse),
    StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetById(
    long id,
    CancellationToken cancellationToken)
        {
            var result = await _getSupplierByIdService.ExecuteAsync(
                id,
                cancellationToken);

            if (result.IsFailure)
            {
                if (result.Errors.Any(
                    e => e.Code == "Supplier.NotFound"))
                {
                    return NotFound(result.Errors);
                }

                return BadRequest(result.Errors);
            }

            return Ok(result.Value);
        }

        [HttpGet]
        [ProducesResponseType(
    typeof(PagedSuppliersResponse),
    StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAll(
    [FromQuery] ListSuppliersRequest request,
    CancellationToken cancellationToken)
        {
            var result = await _listSuppliersService.ExecuteAsync(
                request,
                cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(result.Errors);
            }

            return Ok(result.Value);
        }


        [HttpPut("{id:long}")]
        [ProducesResponseType(
    typeof(SupplierResponse),
    StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Update(
    long id,
    [FromBody] UpdateSupplierRequest request,
    CancellationToken cancellationToken)
        {
            var result = await _updateSupplierService.ExecuteAsync(
                id,
                request,
                cancellationToken);

            if (result.IsFailure)
            {
                if (result.Errors.Any(
                    e => e.Code == "Supplier.NotFound"))
                {
                    return NotFound(result.Errors);
                }

                return BadRequest(result.Errors);
            }

            return Ok(result.Value);
        }
    }
}
