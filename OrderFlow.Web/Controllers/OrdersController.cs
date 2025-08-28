using Microsoft.AspNetCore.Mvc;
using OrderFlow.Web.Middlewares;

namespace OrderFlow.Web.Controllers
{
    /// <summary>
    /// Handles HTTP requests related to order operations.
    /// Access to actions is restricted by user roles using the RoleAuthorize attribute.
    /// </summary>
    public class OrdersController : Controller
    {
        /// <summary>
        /// Displays the order creation view.
        /// Accessible only to users with the "Customer" role.
        /// </summary>
        [HttpGet("orders/create")]
        [RoleAuthorize("Customer")]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Displays a list of all orders.
        /// Accessible to users with "Admin" or "Customer" roles.
        /// </summary>
        [HttpGet("orders")]
        [RoleAuthorize("Admin", "Customer")]
        public IActionResult List()
        {
            return View();
        }

        /// <summary>
        /// Displays the order tracking view.
        /// Accessible to users with "Admin" or "Customer" roles.
        /// </summary>
        [HttpGet("orders/track")]
        [RoleAuthorize("Admin", "Customer")]
        public IActionResult Track()
        {
            return View();
        }

        /// <summary>
        /// Displays the deliveries assigned to the current courier.
        /// Accessible only to users with the "Courier" role.
        /// </summary>
        [HttpGet("orders/my-deliveries")]
        [RoleAuthorize("Courier")]
        public IActionResult MyDeliveries()
        {
            return View();
        }
    }
}


