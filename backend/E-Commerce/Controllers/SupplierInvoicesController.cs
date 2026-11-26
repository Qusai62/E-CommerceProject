using ECommerce.Application.Common.Security;
using ECommerce.Application.SupplierInvoices.CreateSupplierInvoice;
using ECommerce.Application.SupplierInvoices.GetSupplierInvoiceById;
using ECommerce.Application.SupplierInvoices.GetSupplierPaymentHistory;
using ECommerce.Application.SupplierInvoices.ListSupplierInvoices;
using ECommerce.Application.SupplierInvoices.RecordSupplierPayment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    [ApiController]
    [Route("api/supplier-invoices")]
    [Authorize(Policy = AuthorizationPolicies.StoreOwnerOnly)]
    public sealed class SupplierInvoicesController : ControllerBase
    {
        private readonly CreateSupplierInvoiceService _createInvoiceService;
        private readonly GetSupplierInvoiceByIdService _getInvoiceByIdService;
        private readonly ListSupplierInvoicesService _listSupplierInvoicesService;
        private readonly RecordSupplierPaymentService _recordSupplierPaymentService;
        private readonly GetSupplierPaymentHistoryService _getSupplierPaymentHistoryService;

        public SupplierInvoicesController(
            CreateSupplierInvoiceService createInvoiceService,
            GetSupplierInvoiceByIdService getSupplierInvoiceByIdService,
            ListSupplierInvoicesService listSupplierInvoicesService,
            RecordSupplierPaymentService recordSupplierPaymentService,
            GetSupplierPaymentHistoryService getSupplierPaymentHistoryService)
        {
            _createInvoiceService = createInvoiceService;
            _getInvoiceByIdService = getSupplierInvoiceByIdService;
            _listSupplierInvoicesService = listSupplierInvoicesService;
            _recordSupplierPaymentService = recordSupplierPaymentService;
            _getSupplierPaymentHistoryService = getSupplierPaymentHistoryService;
        }

        [HttpPost]
        [ProducesResponseType(
            typeof(CreateSupplierInvoiceResponse),
            StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create(
            [FromBody] CreateSupplierInvoiceRequest request,
            CancellationToken cancellationToken)
        {
            // Step 1: Execute the Application Service.
            var result = await _createInvoiceService.ExecuteAsync(
                request,
                cancellationToken);

            // Step 2: Handle business failures.
            if (result.IsFailure)
            {
                if (result.Errors.Any(e =>
                    e.Code == "Supplier.NotFound"))
                {
                    return NotFound(result.Errors);
                }

                if (result.Errors.Any(e =>
                    e.Code == "SupplierInvoice.DuplicateNumber"))
                {
                    return Conflict(result.Errors);
                }

                return BadRequest(result.Errors);
            }

            // Step 3: Return HTTP 201 Created.
            var response = result.Value;

            return Created(
                $"/api/supplier-invoices/{response.SupplierInvoiceId}",
                response);
        }

        [HttpGet("{id:long}")]
        [ProducesResponseType(
    typeof(SupplierInvoiceDetailsResponse),
    StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetById(
    long id,
    CancellationToken cancellationToken)
        {
            // Step 1: Execute Application Service.
            var result = await _getInvoiceByIdService.ExecuteAsync(
                id,
                cancellationToken);

            // Step 2: Map business errors to HTTP status codes.
            if (result.IsFailure)
            {
                if (result.Errors.Any(
                    e => e.Code == "SupplierInvoice.NotFound"))
                {
                    return NotFound(result.Errors);
                }

                return BadRequest(result.Errors);
            }

            // Step 3: Return invoice details.
            return Ok(result.Value);
        }


        [HttpGet]
        [ProducesResponseType(
    typeof(PagedSupplierInvoicesResponse),
    StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAll(
    [FromQuery] ListSupplierInvoicesRequest request,
    CancellationToken cancellationToken)
        {
            // Execute the Application Service.
            var result = await _listSupplierInvoicesService.ExecuteAsync(
                request,
                cancellationToken);

            // Handle invalid filters or pagination.
            if (result.IsFailure)
            {
                return BadRequest(result.Errors);
            }

            // Return the paginated invoices.
            return Ok(result.Value);
        }

        [HttpPost("{id:long}/payments")]
        [ProducesResponseType(
    typeof(RecordSupplierPaymentResponse),
    StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> RecordPayment(
    long id,
    [FromBody] RecordSupplierPaymentRequest request,
    CancellationToken cancellationToken)
        {
            // Step 1: Execute application service.
            var result = await _recordSupplierPaymentService.ExecuteAsync(
                id,
                request,
                cancellationToken);

            // Step 2: Handle business failures.
            if (result.IsFailure)
            {
                if (result.Errors.Any(e =>
                    e.Code == "SupplierInvoice.NotFound"))
                {
                    return NotFound(result.Errors);
                }

                if (result.Errors.Any(e =>
                    e.Code == "SupplierPayment.SubmissionConflict" ||
                    e.Code == "SupplierPayment.ConcurrencyConflict"))
                {
                    return Conflict(result.Errors);
                }

                return BadRequest(result.Errors);
            }

            // Step 3: Return payment and updated balance.
            return Ok(result.Value);
        }

        [HttpGet("{id:long}/payments")]
        [ProducesResponseType(
    typeof(SupplierPaymentHistoryResponse),
    StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetPaymentHistory(
    long id,
    CancellationToken cancellationToken)
        {
            // Step 1: Execute the Application Service.
            var result =
                await _getSupplierPaymentHistoryService.ExecuteAsync(
                    id,
                    cancellationToken);

            // Step 2: Map failures to HTTP responses.
            if (result.IsFailure)
            {
                if (result.Errors.Any(e =>
                    e.Code == "SupplierInvoice.NotFound"))
                {
                    return NotFound(result.Errors);
                }

                return BadRequest(result.Errors);
            }

            // Step 3: Return payment history.
            return Ok(result.Value);
        }
    }
}
