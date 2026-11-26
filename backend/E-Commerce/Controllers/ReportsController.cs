using ECommerce.Application.Common.Security;
using ECommerce.Application.Reports.OrdersByStatus;
using ECommerce.Application.Reports.RevenueTrends;
using ECommerce.Application.Reports.SalesSummary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    [ApiController]
    [Route("api/reports")]
    [Authorize(Policy = AuthorizationPolicies.StoreOwnerOnly)]
    public sealed class ReportsController : ControllerBase
    {
        private readonly GetSalesSummaryService _salesSummaryService;
        private readonly GetRevenueTrendsService _revenueTrendsService;
        private readonly GetOrdersByStatusService _ordersByStatusService;
        public ReportsController(
            GetSalesSummaryService salesSummaryService,
            GetRevenueTrendsService getRevenueTrendsService,
            GetOrdersByStatusService getOrdersByStatusService)
        {
            _salesSummaryService = salesSummaryService;
            _revenueTrendsService = getRevenueTrendsService;
            _ordersByStatusService = getOrdersByStatusService;
        }

        [HttpGet("sales-summary")]
        [ProducesResponseType(
            typeof(GetSalesSummaryResponse),
            StatusCodes.Status200OK)]
        [ProducesResponseType(
            StatusCodes.Status400BadRequest)]
        [ProducesResponseType(
            StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(
            StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<GetSalesSummaryResponse>>
            GetSalesSummary(
                [FromQuery] DateTime? fromUtc,
                [FromQuery] DateTime? toUtc,
                CancellationToken cancellationToken)
        {
            // Step 1: Validate required query parameters
            if (fromUtc is null || toUtc is null)
            {
                return BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Missing reporting dates",
                    Detail = "Both fromUtc and toUtc are required."
                });
            }

            // Step 2: Create Application request
            var request = new GetSalesSummaryRequest(
                fromUtc.Value,
                toUtc.Value);

            try
            {
                // Step 3: Execute Application Service
                var response = await _salesSummaryService.ExecuteAsync(
                    request,
                    cancellationToken);

                // Step 4: Return summary
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Invalid reporting date range",
                    Detail = ex.Message
                });
            }
        }


        [HttpGet("revenue-trends")]
        [ProducesResponseType(
            typeof(GetRevenueTrendsResponse),
            StatusCodes.Status200OK)]
        [ProducesResponseType(
            StatusCodes.Status400BadRequest)]
        [ProducesResponseType(
            StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(
            StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<GetRevenueTrendsResponse>>
            GetRevenueTrends(
                [FromQuery] DateTime? fromUtc,
                [FromQuery] DateTime? toUtc,
                [FromQuery] RevenueGrouping? groupBy,
                CancellationToken cancellationToken)
        {
            // Step 1: Validate required query parameters
            if (fromUtc is null ||
                toUtc is null ||
                groupBy is null)
            {
                return BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Missing reporting parameters",
                    Detail = "fromUtc, toUtc and groupBy are required."
                });
            }

            // Step 2: Create Application request
            var request = new GetRevenueTrendsRequest(
                FromUtc: fromUtc.Value,
                ToUtc: toUtc.Value,
                GroupBy: groupBy.Value);

            try
            {
                // Step 3: Execute Application Service
                var response = await _revenueTrendsService.ExecuteAsync(
                    request,
                    cancellationToken);

                // Step 4: Return grouped revenue
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Invalid reporting parameters",
                    Detail = ex.Message
                });
            }
        }



        [HttpGet("orders-by-status")]
        [ProducesResponseType(
            typeof(GetOrdersByStatusResponse),
            StatusCodes.Status200OK)]
        [ProducesResponseType(
            StatusCodes.Status400BadRequest)]
        [ProducesResponseType(
            StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(
            StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<GetOrdersByStatusResponse>>
            GetOrdersByStatus(
                [FromQuery] DateTime? fromUtc,
                [FromQuery] DateTime? toUtc,
                CancellationToken cancellationToken)
        {
            // Step 1: Validate required query parameters
            if (fromUtc is null || toUtc is null)
            {
                return BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Missing reporting dates",
                    Detail = "Both fromUtc and toUtc are required."
                });
            }

            // Step 2: Create Application request
            var request = new GetOrdersByStatusRequest(
                FromUtc: fromUtc.Value,
                ToUtc: toUtc.Value);

            try
            {
                // Step 3: Execute Application Service
                var response = await _ordersByStatusService.ExecuteAsync(
                    request,
                    cancellationToken);

                // Step 4: Return the grouped order counts
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Invalid reporting date range",
                    Detail = ex.Message
                });
            }
        }

    }
}
