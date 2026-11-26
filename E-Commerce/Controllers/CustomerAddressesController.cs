using E_Commerce.Contracts.CustomerAccount;
using ECommerce.Application.Common.Security;
using ECommerce.Application.Customers.Addresses.AddCustomerAddress;
using ECommerce.Application.Customers.Addresses.DeleteCustomerAddress;
using ECommerce.Application.Customers.Addresses.GetCustomerAddresses;
using ECommerce.Application.Customers.Addresses.SetDefaultCustomerAddress;
using ECommerce.Application.Customers.Addresses.UpdateCustomerAddress;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_Commerce.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/customer/addresses")]
    
    public sealed class CustomerAddressesController : ControllerBase
    {
        private readonly AddCustomerAddressService _addAddressService;
        private readonly GetCustomerAddressesService _getAddressesService;
        private readonly UpdateCustomerAddressService _updateAddressService;
        private readonly DeleteCustomerAddressService _deleteAddressService;
        private readonly SetDefaultCustomerAddressService _setDefaultAddressService;
        public CustomerAddressesController(
            AddCustomerAddressService service, GetCustomerAddressesService getCustomerAddressesService,
            UpdateCustomerAddressService updateCustomerAddressService, DeleteCustomerAddressService deleteCustomerAddressService,
            SetDefaultCustomerAddressService setDefaultCustomerAddressService)
        {
            _addAddressService = service;
            _getAddressesService = getCustomerAddressesService;
            _updateAddressService = updateCustomerAddressService;
            _deleteAddressService = deleteCustomerAddressService;
            _setDefaultAddressService = setDefaultCustomerAddressService;
        }

        [HttpPost]
        [ProducesResponseType(
            typeof(AddCustomerAddressResponse),
            StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddAddress(
            [FromBody] AddCustomerAddressRequest request,
            CancellationToken cancellationToken)
        {
            // Step 1: Extract the authenticated user's ID
            var userIdValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdValue, out var applicationUserId))
            {
                return Unauthorized();
            }

            // Step 2: Map HTTP request to Application command
            var command = new AddCustomerAddressCommand(
                request.RecipientName,
                request.ContactNumber,
                request.City,
                request.Neighborhood,
                request.Street,
                request.BuildingInfo,
                request.AdditionalAddressInfo
            );

            // Step 3: Execute the business use case
            var result = await _addAddressService.ExecuteAsync(
                applicationUserId,
                command,
                cancellationToken);

            // Step 4: Handle failure
            if (result.IsFailure)
            {
                if (result.Errors.Any(error =>
                    error.Code == "Customer.NotFound"))
                {
                    return NotFound(new
                    {
                        Errors = result.Errors
                    });
                }

                return BadRequest(new
                {
                    Errors = result.Errors
                });
            }

            // Step 5: Return the created address
            return StatusCode(
                StatusCodes.Status201Created,
                result.Value);
        }

        [HttpGet]
        [ProducesResponseType(
    typeof(IReadOnlyList<CustomerAddressResponse>),
    StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAddresses(
    CancellationToken cancellationToken)
        {
            // Step 1: Retrieve the authenticated user's ID
            var userIdValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdValue, out var applicationUserId))
            {
                return Unauthorized();
            }

            // Step 2: Execute the Application use case
            var result = await _getAddressesService.ExecuteAsync(
                applicationUserId,
                cancellationToken);

            // Step 3: Handle failure
            if (result.IsFailure)
            {
                if (result.Errors.Any(error =>
                    error.Code == "Customer.NotFound"))
                {
                    return NotFound(new
                    {
                        Errors = result.Errors
                    });
                }

                return BadRequest(new
                {
                    Errors = result.Errors
                });
            }

            // Step 4: Return the customer's addresses
            return Ok(result.Value);
        }

        [HttpPut("{addressId:long}")]
        [ProducesResponseType(
    typeof(UpdateCustomerAddressResponse),
    StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAddress(
    [FromRoute] long addressId,
    [FromBody] UpdateCustomerAddressRequest request,
    CancellationToken cancellationToken)
        {
            // Step 1: Get authenticated user's ID
            var userIdValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdValue, out var applicationUserId))
            {
                return Unauthorized();
            }

            // Step 2: Map request to Application command
            var command = new UpdateCustomerAddressCommand(
                request.RecipientName,
                request.ContactNumber,
                request.City,
                request.Neighborhood,
                request.Street,
                request.BuildingInfo,
                request.AdditionalAddressInfo
            );

            // Step 3: Execute use case
            var result = await _updateAddressService.ExecuteAsync(
                applicationUserId,
                addressId,
                command,
                cancellationToken);

            // Step 4: Handle errors
            if (result.IsFailure)
            {
                if (result.Errors.Any(error =>
                    error.Code == "Customer.NotFound" ||
                    error.Code == "Address.NotFound"))
                {
                    return NotFound(new
                    {
                        Errors = result.Errors
                    });
                }

                return BadRequest(new
                {
                    Errors = result.Errors
                });
            }

            // Step 5: Return updated address
            return Ok(result.Value);
        }

        [HttpDelete("{addressId:long}")]
        [ProducesResponseType(
    StatusCodes.Status204NoContent)]
        [ProducesResponseType(
    StatusCodes.Status400BadRequest)]
        [ProducesResponseType(
    StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(
    StatusCodes.Status404NotFound)]
        [ProducesResponseType(
    StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteAddress(
    [FromRoute] long addressId,
    CancellationToken cancellationToken)
        {
            // Step 1: Extract the authenticated user's ID
            var userIdValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(
                userIdValue,
                out var applicationUserId))
            {
                return Unauthorized();
            }

            // Step 2: Execute the Application use case
            var result = await _deleteAddressService.ExecuteAsync(
                applicationUserId,
                addressId,
                cancellationToken);

            // Step 3: Map business errors to HTTP responses
            if (result.IsFailure)
            {
                if (result.Errors.Any(error =>
                    error.Code == "Address.InvalidId"))
                {
                    return BadRequest(new
                    {
                        Errors = result.Errors
                    });
                }

                if (result.Errors.Any(error =>
                    error.Code == "Customer.NotFound" ||
                    error.Code == "Address.NotFound"))
                {
                    return NotFound(new
                    {
                        Errors = result.Errors
                    });
                }

                if (result.Errors.Any(error =>
                    error.Code == "Address.DefaultDeletionNotAllowed"))
                {
                    return Conflict(new
                    {
                        Errors = result.Errors
                    });
                }

                return BadRequest(new
                {
                    Errors = result.Errors
                });
            }

            // Step 4: Return HTTP 204
            return NoContent();
        }

        [HttpPut("{addressId:long}/default")]
        [ProducesResponseType(
    typeof(SetDefaultCustomerAddressResponse),
    StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SetDefaultAddress(
    [FromRoute] long addressId,
    CancellationToken cancellationToken)
        {
            // Step 1: Read authenticated user ID
            var userIdValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdValue, out var applicationUserId))
            {
                return Unauthorized();
            }

            // Step 2: Call Application service
            var result = await _setDefaultAddressService.ExecuteAsync(
                applicationUserId,
                addressId,
                cancellationToken);

            // Step 3: Handle failures
            if (result.IsFailure)
            {
                if (result.Errors.Any(error =>
                    error.Code == "Address.InvalidId"))
                {
                    return BadRequest(new
                    {
                        Errors = result.Errors
                    });
                }

                if (result.Errors.Any(error =>
                    error.Code == "Customer.NotFound" ||
                    error.Code == "Address.NotFound"))
                {
                    return NotFound(new
                    {
                        Errors = result.Errors
                    });
                }

                return BadRequest(new
                {
                    Errors = result.Errors
                });
            }

            // Step 4: Return success
            return Ok(result.Value);
        }
    }
}
