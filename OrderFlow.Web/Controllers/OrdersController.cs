using Microsoft.AspNetCore.Mvc;

namespace OrderFlow.Web.Controllers
{
    public class OrdersController : Controller
    {
        // Customer: create order
        [HttpGet("orders/create")]
        public IActionResult Create()
        {
            return View();
        }

        // Admin: list/manage orders (uses same list style as Index)
        [HttpGet("orders")]
        public IActionResult List()
        {
            return View();
        }

        // Customer: track order
        [HttpGet("orders/track")]
        public IActionResult Track()
        {
            return View();
        }

        // Courier: my assigned deliveries
        [HttpGet("orders/my-deliveries")]
        public IActionResult MyDeliveries()
        {
            return View();
        }
    }
}


