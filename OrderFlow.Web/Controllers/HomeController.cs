using Microsoft.AspNetCore.Mvc;
using OrderFlow.Web.Middlewares;
using OrderFlow.Web.Models;
using System.Diagnostics;

namespace OrderFlow.Web.Controllers
{
    /// <summary>
    /// Handles requests for the home page and error page.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Displays the main dashboard view.
        /// Accessible only to users with the "Admin" role.
        /// </summary>
        [Route("")]
        [RoleAuthorize("Admin")]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Displays the error view with request details.
        /// </summary>
        [Route("error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
