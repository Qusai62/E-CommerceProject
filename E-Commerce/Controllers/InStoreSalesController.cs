using ECommerce.Application.Common.Security;
using ECommerce.Application.InStoreSales.CreateInStoreSale;
using ECommerce.Application.InStoreSales.GetInStoreSaleDetails;
using ECommerce.Application.InStoreSales.GetInStoreSales;
using ECommerce.Application.InStoreSales.UpdateInStoreSale;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    [ApiController]
    [Route("api/in-store-sales")]
    [Authorize(Policy = AuthorizationPolicies.StoreOwnerOnly)]
    public sealed class InStoreSalesController : ControllerBase
    {
        private readonly CreateInStoreSaleService _createSaleService;
        private readonly GetInStoreSalesService _getSalesService;
        private readonly GetInStoreSaleDetailsService _getSaleDetailsService;
        private readonly UpdateInStoreSaleService _updateInStoreSaleService;


        public InStoreSalesController(
            CreateInStoreSaleService createSaleService,
            GetInStoreSalesService getInStoreSalesService,
            GetInStoreSaleDetailsService getInStoreSaleDetailsService,
            UpdateInStoreSaleService updateInStoreSaleService)
        {
            _createSaleService = createSaleService;
            _getSalesService = getInStoreSalesService;
            _getSaleDetailsService = getInStoreSaleDetailsService;
            _updateInStoreSaleService = updateInStoreSaleService;
        }
        [HttpPost]
        [ProducesResponseType(
            typeof(CreateInStoreSaleResponse),
            StatusCodes.Status201Created)]
        [ProducesResponseType(
            typeof(CreateInStoreSaleResponse),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<CreateInStoreSaleResponse>> Create(
            [FromBody] CreateInStoreSaleRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _createSaleService.ExecuteAsync(
                    request,
                    cancellationToken);

                if (!result.Created)
                {
                    return Ok(result.Sale);
                }

                return StatusCode(
                    StatusCodes.Status201Created,
                    result.Sale);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Invalid in-store sale",
                    Detail = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "Sale submission conflict",
                    Detail = ex.Message
                });
            }
        }


        [HttpGet]
        [ProducesResponseType(
            typeof(GetInStoreSalesResponse),
            StatusCodes.Status200OK)]
        [ProducesResponseType(
            StatusCodes.Status400BadRequest)]
        [ProducesResponseType(
            StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(
            StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<GetInStoreSalesResponse>> GetSales(
            [FromQuery] GetInStoreSalesRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var response = await _getSalesService.ExecuteAsync(
                    request,
                    cancellationToken);

                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Invalid sales history request",
                    Detail = ex.Message
                });
            }
        }



        [HttpGet("{id:long}")]
        [ProducesResponseType(
            typeof(GetInStoreSaleDetailsResponse),
            StatusCodes.Status200OK)]
        [ProducesResponseType(
            StatusCodes.Status400BadRequest)]
        [ProducesResponseType(
            StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(
            StatusCodes.Status403Forbidden)]
        [ProducesResponseType(
            StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GetInStoreSaleDetailsResponse>>
            GetSaleDetails(
                [FromRoute] long id,
                CancellationToken cancellationToken)
        {
            // 1. Validate sale identifier
            if (id <= 0)
            {
                return BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Invalid in-store sale ID",
                    Detail = "The sale ID must be greater than zero."
                });
            }

            // 2. Execute Application Service
            var response = await _getSaleDetailsService.ExecuteAsync(
                id,
                cancellationToken);

            // 3. Sale not found
            if (response is null)
            {
                return NotFound(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "In-store sale not found",
                    Detail = $"No in-store sale exists with ID {id}."
                });
            }

            // 4. Return sale details
            return Ok(response);
        }


        [HttpPut("{id:long}")]
        [ProducesResponseType(
            typeof(UpdateInStoreSaleResponse),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<UpdateInStoreSaleResponse>>
            UpdateInStoreSale(
                [FromRoute] long id,
                [FromBody] UpdateInStoreSaleRequest request,
                CancellationToken cancellationToken)
        {
            if (id <= 0)
            {
                return BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Invalid sale ID"
                });
            }

            try
            {
                var result = await _updateInStoreSaleService.ExecuteAsync(
                    id,
                    request,
                    cancellationToken);

                return result.Outcome switch
                {
                    UpdateInStoreSaleOutcome.Updated =>
                        Ok(result.Response),

                    UpdateInStoreSaleOutcome.NotFound =>
                        NotFound(new ProblemDetails
                        {
                            Status = StatusCodes.Status404NotFound,
                            Title = "In-store sale not found"
                        }),

                    UpdateInStoreSaleOutcome.NotEditable =>
                        Conflict(new ProblemDetails
                        {
                            Status = StatusCodes.Status409Conflict,
                            Title = "Sale cannot be edited",
                            Detail = "Only completed sales can be updated."
                        }),

                    UpdateInStoreSaleOutcome.ConcurrencyConflict =>
                        Conflict(new ProblemDetails
                        {
                            Status = StatusCodes.Status409Conflict,
                            Title = "Sale was modified",
                            Detail = "Reload the sale before updating it."
                        }),

                    _ => throw new InvalidOperationException(
                        "Unexpected update result.")
                };
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Invalid update request",
                    Detail = ex.Message
                });
            }
        }

    }
}
