using Microsoft.AspNetCore.Mvc;
using OrderFlow.Web.Helpers;
using OrderFlow.Web.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace OrderFlow.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IApiRequestHelper _apiRequestHelper;

        public AccountController(IConfiguration configuration, IApiRequestHelper apiRequestHelper)
        {
            _configuration = configuration;
            _apiRequestHelper = apiRequestHelper;
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest model)
        {
            var response = await _apiRequestHelper.SendAsync(
                "/api/auth/login",
                HttpMethod.Post,
                model
            );

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Giriş başarısız.";
                return View();
            }

            var json = await response.Content.ReadAsStringAsync();
            var loginResponse = JsonSerializer.Deserialize<LoginResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            HttpContext.Session.SetString("access_token", loginResponse.AccessToken);
            HttpContext.Session.SetString("role", ParseRoleFromJwt(loginResponse.AccessToken));

            // Rol bazlı yönlendirme
            var role = HttpContext.Session.GetString("role");
            return role switch
            {
                "Admin" => RedirectToAction("Index", "Home"),
                "Customer" => RedirectToAction("Create", "Orders"),
                "Courier" => RedirectToAction("MyDeliveries", "Orders"),
                _ => RedirectToAction("Login")
            };
        }

        [HttpGet("forgot-password")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword(string email)
        {
            // TODO: call API to send OTP
            ViewBag.Info = "If the email exists, an OTP has been sent.";
            return View();
        }

        [HttpGet("reset-password")]
        public IActionResult ResetPassword(string code)
        {
            ViewBag.Code = code;
            return View();
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword(string code, string password)
        {
            // TODO: call API to reset with code
            TempData["Info"] = "Your password has been reset.";
            return RedirectToAction("Login");
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
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
