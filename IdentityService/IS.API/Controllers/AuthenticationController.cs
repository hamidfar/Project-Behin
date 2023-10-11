using IS.API.Application.DTO;
using IS.API.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IS.API.Application
{
    [ApiController]
    [Route("api/authentication")]
    [AllowAnonymous]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticateService _authenticateService;


        public AuthenticationController(IAuthenticateService authenticateService)
        {
            _authenticateService = authenticateService;

        }

        [HttpPost("token")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticationRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Username and password are required.");
            }

            var token = await _authenticateService.AuthenticateAsync(request.Username, request.Password);

            if (token == null)
            {
                return Unauthorized("Invalid credentials.");
            }

            return Ok(new { Token = token });
        }

        [HttpPost("revoke")]
        public async Task<IActionResult> RevokeTokenAsync([FromBody] RevokeTokenRequest request)
        {
            if (string.IsNullOrEmpty(request.Token))
            {
                return BadRequest("Invalid token.");
            }

            if (await _authenticateService.RevokeToken(request.Token))
            {
                return Ok("Token revoked successfully.");
            }

            return BadRequest("Token is not valid.");
        }
    }

}
