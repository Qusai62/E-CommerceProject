using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    using Microsoft.AspNetCore.Antiforgery;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/security")]
    public sealed class SecurityController : ControllerBase
    {
        private readonly IAntiforgery _antiforgery;

        public SecurityController(IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
        }

        [HttpGet("csrf")]
        [AllowAnonymous]
        public IActionResult GetCsrfToken()
        {
            //This generates the request token and ensures the associated antiforgery cookie token is stored in the HTTP response when necessary.
            var tokens = _antiforgery.GetAndStoreTokens(
                HttpContext);

            Response.Headers.CacheControl = "no-store";

            return Ok(new
            {
                RequestToken = tokens.RequestToken
            });
        }
    }
}
