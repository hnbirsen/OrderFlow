using Microsoft.AspNetCore.Mvc;
using OrderFlow.Web.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace OrderFlow.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public AccountController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet("account/forgot-password")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost("account/forgot-password")]
        public IActionResult ForgotPassword(string email)
        {
            // TODO: call API to send OTP
            ViewBag.Info = "If the email exists, an OTP has been sent.";
            return View();
        }

        [HttpGet("account/reset-password")]
        public IActionResult ResetPassword(string code)
        {
            ViewBag.Code = code;
            return View();
        }

        [HttpPost("account/reset-password")]
        public IActionResult ResetPassword(string code, string password)
        {
            // TODO: call API to reset with code
            TempData["Info"] = "Your password has been reset.";
            return RedirectToAction("Login");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest model)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["ApiBaseUrl"]);

            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/auth/login", content);

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

        private string ParseRoleFromJwt(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            return jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "";
        }
    }
}
