using Microsoft.AspNetCore.Mvc;
using OrderFlow.Web.Models;
using System.Text;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;

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
                "Admin" => RedirectToAction("Dashboard", "Admin"),
                "Customer" => RedirectToAction("Create", "Orders"),
                "Courier" => RedirectToAction("MyOrders", "Courier"),
                _ => RedirectToAction("Login")
            };
        }

        private string ParseRoleFromJwt(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            return jwt.Claims.FirstOrDefault(c => c.Type == "role")?.Value ?? "";
        }
    }
}
