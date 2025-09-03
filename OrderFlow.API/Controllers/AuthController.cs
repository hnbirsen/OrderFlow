using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderFlow.Application.DTOs;
using OrderFlow.Application.Interfaces.Abstract;

namespace OrderFlow.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            _logger.LogInformation("Login attempt for user: {Email}", request.Email);
            var result = await _authService.LoginAsync(request);

            if (result == null)
            {
                _logger.LogWarning("Login failed for user: {Email}", request.Email);
                return Unauthorized();
            }

            _logger.LogInformation("User {Email} logged in successfully.", request.Email);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            _logger.LogInformation("Refresh token attempt for user: {Email}", request.Email);
            var result = await _authService.RefreshTokenAsync(request);

            if (result == null)
            {
                _logger.LogWarning("Refresh token failed for user: {Email}", request.Email);
                return Unauthorized();
            }

            _logger.LogInformation("Refresh token succeeded for user: {Email}", request.Email);
            return Ok(result);
        }
    }
}
