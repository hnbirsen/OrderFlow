using Microsoft.AspNetCore.Mvc;
using OrderFlow.Web.Helpers.Abstract;
using OrderFlow.Web.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace OrderFlow.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IApiRequestHelper _apiRequestHelper;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            IConfiguration configuration,
            IApiRequestHelper apiRequestHelper,
            ILogger<AccountController> logger)
        {
            _configuration = configuration;
            _apiRequestHelper = apiRequestHelper;
            _logger = logger;
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            _logger.LogInformation("Login page requested.");
            return View();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest model)
        {
            _logger.LogInformation("Login attempt for user: {Email}", model.Email);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid login data for user: {Email}", model.Email);
                ViewBag.Error = "Invalid login data.";
                return View(model);
            }

            try
            {
                var response = await _apiRequestHelper.SendAsync(
                    "/api/auth/login",
                    HttpMethod.Post,
                    model
                );

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Login failed for user: {Email}. API response: {ErrorContent}", model.Email, errorContent);
                    ViewBag.Error = !string.IsNullOrWhiteSpace(errorContent)
                        ? errorContent
                        : "Login failed. Please check your credentials.";
                    return View(model);
                }

                var json = await response.Content.ReadAsStringAsync();
                var loginResponse = JsonSerializer.Deserialize<LoginResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (loginResponse == null || string.IsNullOrEmpty(loginResponse.AccessToken))
                {
                    _logger.LogError("Invalid response from authentication service for user: {Email}", model.Email);
                    ViewBag.Error = "Invalid response from authentication service.";
                    return View(model);
                }

                var role = ParseRoleFromJwt(loginResponse.AccessToken);
                if (string.IsNullOrEmpty(role))
                {
                    _logger.LogError("Unable to determine user role for user: {Email}", model.Email);
                    ViewBag.Error = "Unable to determine user role.";
                    return View(model);
                }

                HttpContext.Session.SetString("access_token", loginResponse.AccessToken);
                HttpContext.Session.SetString("role", role);

                _logger.LogInformation("User {Email} logged in successfully with role {Role}.", model.Email, role);

                return role switch
                {
                    "Admin" => RedirectToAction("Index", "Home"),
                    "Customer" => RedirectToAction("Create", "Orders"),
                    "Courier" => RedirectToAction("MyDeliveries", "Orders"),
                    _ => RedirectToAction("Login")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during login for user: {Email}", model.Email);
                ViewBag.Error = "An unexpected error occurred. Please try again later.";
                return View(model);
            }
        }

        [HttpGet("forgot-password")]
        public IActionResult ForgotPassword()
        {
            _logger.LogInformation("Forgot password page requested.");
            return View();
        }

        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword(string email)
        {
            _logger.LogInformation("Forgot password requested for email: {Email}", email);
            // TODO: call API to send OTP
            ViewBag.Info = "If the email exists, an OTP has been sent.";
            return View();
        }

        [HttpGet("reset-password")]
        public IActionResult ResetPassword(string code)
        {
            _logger.LogInformation("Reset password page requested with code: {Code}", code);
            ViewBag.Code = code;
            return View();
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword(string code, string password)
        {
            _logger.LogInformation("Password reset attempt with code: {Code}", code);
            // TODO: call API to reset with code
            TempData["Info"] = "Your password has been reset.";
            return RedirectToAction("Login");
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            _logger.LogInformation("User logged out.");
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        private string? ParseRoleFromJwt(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            return jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? null;
        }
    }
}
