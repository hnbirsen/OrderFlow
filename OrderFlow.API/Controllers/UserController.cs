using Microsoft.AspNetCore.Mvc;
using OrderFlow.Application.DTOs;
using OrderFlow.Application.Interfaces.Abstract;

namespace OrderFlow.API.Controllers
{
    public class UserController : BaseApiControler
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("GetAll users requested.");
            var users = await _userService.GetAllUsersAsync();
            _logger.LogInformation("{Count} users returned.", users?.Count() ?? 0);
            return Ok(users);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
        {
            _logger.LogInformation("Create user requested for email: {Email}, role: {Role}", request.Email, request.Role);
            var result = await _userService.CreateUserAsync(request);
            if (result)
            {
                _logger.LogInformation("User created successfully for email: {Email}", request.Email);
                return Ok();
            }
            else
            {
                _logger.LogWarning("User creation failed for email: {Email}", request.Email);
                return BadRequest();
            }
        }
    }
}
