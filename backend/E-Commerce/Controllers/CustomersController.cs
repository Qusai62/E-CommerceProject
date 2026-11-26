using E_Commerce.Contracts.CustomerAccount;
using ECommerce.Application.Common.Security;
using ECommerce.Application.Customers.CustomerAccountManag.ChangeCustomerPassword;
using ECommerce.Application.Customers.CustomerAccountManag.GetCustomerProfile;
using ECommerce.Application.Customers.CustomerAccountManag.RegisterCustomer;
using ECommerce.Application.Customers.CustomerAccountManag.UpdateCustomerProfile;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_Commerce.Controllers
{
    [ApiController]
    [Route("api/customers")]

    public sealed class CustomersController : ControllerBase
    {
        private readonly RegisterCustomerService _registerService;
        private readonly IAntiforgery _antiforgery; 
        private readonly GetCustomerProfileService _profileService;
        private readonly UpdateCustomerProfileService _updateProfileService;
        private readonly ChangeCustomerPasswordService _changePasswordService;
        public CustomersController(
            RegisterCustomerService registerService,IAntiforgery antiforgery
            ,
            GetCustomerProfileService profileService,
            UpdateCustomerProfileService updateCustomerProfileService,
            ChangeCustomerPasswordService changeCustomerPasswordService)
        {
            _registerService = registerService;
       
            _antiforgery = antiforgery;
       
            _profileService = profileService;
            _updateProfileService = updateCustomerProfileService;
            _changePasswordService = changeCustomerPasswordService;
        }

        [HttpPost("register")]//Our endpoint become: POST /api/customers/register
        [AllowAnonymous]
        [ProducesResponseType(
            typeof(RegisterCustomerResponse),
            StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register(
            [FromBody] RegisterCustomerRequest request,
            CancellationToken cancellationToken)
        {
            // 1. Convert API request into Application command
            var command = new RegisterCustomerCommand
            {
                FirstName = request.FirstName ?? string.Empty,
                LastName = request.LastName ?? string.Empty,
                Email = request.Email ?? string.Empty,
                PhoneNumber = request.PhoneNumber ?? string.Empty,
                Password = request.Password ?? string.Empty
            };

            // 2. Execute the registration use case
            var result = await _registerService.ExecuteAsync(
                command,
                cancellationToken);

            // 3. Handle business errors
            if (result.IsFailure)
            {
                bool emailConflict = result.Errors.Any(
                    error => error.Code is
                        "Customer.DuplicateEmail" or
                        "Customer.DuplicateUserName" or
                        "Customer.EmailAlreadyExists");

                if (emailConflict)
                {
                    return Conflict(result.Errors);
                }

                return BadRequest(result.Errors);
            }

            // 4. Return created customer
            return StatusCode(
                StatusCodes.Status201Created,
                result.Value);
        }

 
        [HttpGet("me")]
        [Authorize]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        [ProducesResponseType(
    typeof(GetCustomerProfileResponse),
    StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMyProfile(
    CancellationToken cancellationToken)
        {
            // 1. Read the Identity user ID from the authenticated principal
            var identityUserId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            // 2. Validate that the claim contains a GUID
            if (!Guid.TryParse(identityUserId, out var applicationUserId))
            {
                return Unauthorized();
            }

            // 3. Retrieve the authenticated customer's profile
            var result = await _profileService.ExecuteAsync(
                applicationUserId,
                cancellationToken);

            // 4. Handle missing customer profile
            if (result.IsFailure)
            {
                return NotFound(result.Errors);
            }

            // 5. Return the profile
            return Ok(result.Value);
        }

        [HttpPut("me")]
        [Authorize]
        
        [ProducesResponseType(
            typeof(UpdateCustomerProfileResponse),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateMyProfile(
            [FromBody] UpdateCustomerProfileRequest request,
            CancellationToken cancellationToken)
        {
            // 1. Identify the authenticated user
            var identityUserId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(identityUserId, out var applicationUserId))
            {
                return Unauthorized();
            }

            // 2. Create the Application command
            var command = new UpdateCustomerProfileCommand
            {
                FirstName = request.FirstName ?? string.Empty,
                LastName = request.LastName ?? string.Empty,
                Email = request.Email ?? string.Empty,
                PhoneNumber = request.PhoneNumber ?? string.Empty,
                CurrentPassword = request.CurrentPassword
            };

            // 3. Execute the use case
            var result = await _updateProfileService.ExecuteAsync(
                applicationUserId,
                command,
                cancellationToken);

            // 4. Handle errors
            if (result.IsFailure)
            {
                if (result.Errors.Any(e =>
                    e.Code == "Customer.ProfileNotFound"))
                {
                    return NotFound(result.Errors);
                }

                if (result.Errors.Any(e =>
                    e.Code is
                        "Customer.DuplicateEmail" or
                        "Customer.DuplicateUserName" or
                        "Customer.EmailAlreadyExists" or
                        "Customer.ConcurrencyFailure" or
                        "Customer.ConcurrentUpdate"))
                {
                    return Conflict(result.Errors);
                }

                return BadRequest(result.Errors);
            }

            // 5. Return updated profile
            return Ok(result.Value);
        }


        [HttpPost("me/change-password")]
        [Authorize]
       
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> ChangePassword(
            [FromBody] ChangeCustomerPasswordRequest request,
            [FromHeader(Name = "X-CSRF-TOKEN")] string? csrfToken,
            CancellationToken cancellationToken)
        {
            // 1. Identify the authenticated user.
            var identityUserId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(
                identityUserId,
                out var applicationUserId))
            {
                return Unauthorized();
            }

            // 2. Map API request to Application command.
            var command = new ChangeCustomerPasswordCommand
            {
                CurrentPassword = request.CurrentPassword
                    ?? string.Empty,

                NewPassword = request.NewPassword
                    ?? string.Empty
            };

            // 3. Execute the use case.
            var result = await _changePasswordService.ExecuteAsync(
                applicationUserId,
                command,
                cancellationToken);

            // 4. Handle failures.
            if (result.IsFailure)
            {
                if (result.Errors.Any(e =>
                    e.Code == "Customer.ProfileNotFound"))
                {
                    return NotFound(result.Errors);
                }

                if (result.Errors.Any(e =>
                    e.Code == "Customer.ConcurrencyFailure"))
                {
                    return Conflict(result.Errors);
                }

                return BadRequest(result.Errors);
            }

            // 5. Password changed successfully.
            return NoContent();
        }

    }
}