using E_Commerce.Contracts.Auth;
using ECommerce.Application.Authentication.Login;
using ECommerce.Application.Authentication.Logout;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_Commerce.Controllers
{
    [ApiController]
    [Route("api/auth")]

    public sealed class AuthController : ControllerBase
    {
        private readonly LoginService _loginService;
        private readonly LogoutService _logoutService;
      
        public AuthController(LoginService loginService, LogoutService logoutService)
        {
            _loginService = loginService;
            _logoutService = logoutService;
       
        }

        // POST /api/auth/login
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(
            typeof(LoginResponse),
            StatusCodes.Status200OK)]
        [ProducesResponseType(
            StatusCodes.Status400BadRequest)]
        [ProducesResponseType(
            StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<LoginResponse>> Login(
            [FromBody] LoginRequest request,
            CancellationToken cancellationToken)
        {
            // 1. Map the HTTP request to an Application command.
            var command = new LoginCommand
            {
                Email = request.Email,
                Password = request.Password
            };

            // 2. Execute the shared login use case.
            var result = await _loginService.ExecuteAsync(
                command,
                cancellationToken);

            // 3. Handle unsuccessful login.
            if (result.IsFailure)
            {
                // Invalid input: missing email or password.
                bool validationFailed = result.Errors.Any(
                    error =>
                        error.Code == "Authentication.EmailRequired" ||
                        error.Code == "Authentication.PasswordRequired");

                if (validationFailed)
                {
                    return BadRequest(new
                    {
                        Errors = result.Errors
                    });
                }

                // Invalid credentials, lockout, or other
                // authentication failure.
                return Unauthorized(new
                {
                    Errors = result.Errors
                });
            }

            // 4. Identity has already issued the auth cookie.
            return Ok(result.Value);
        }

        [HttpPost("logout")]

        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Logout()
        {
            await _logoutService.ExecuteAsync();

            return NoContent();
        }

        // GET /api/auth/me
        [HttpGet("me")]

        [Authorize]
        [ProducesResponseType(
            typeof(CurrentUserResponse),
            StatusCodes.Status200OK)]
        [ProducesResponseType(
            StatusCodes.Status401Unauthorized)]
        public ActionResult<CurrentUserResponse> GetCurrentUser()
        {
            var userIdValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdValue, out var userId))
            {
                return Unauthorized();
            }

            var email =
                User.FindFirstValue(ClaimTypes.Email);

            var roles = User.FindAll(ClaimTypes.Role)
                .Select(claim => claim.Value)
                .Distinct()
                .ToArray();

            return Ok(new CurrentUserResponse(
                userId,
                email,
                roles));
        }

       
    }




    public sealed record CurrentUserResponse(
        Guid UserId,
        string? Email,
        IReadOnlyList<string> Roles);
}
