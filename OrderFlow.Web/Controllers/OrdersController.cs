using Microsoft.AspNetCore.Mvc;
using OrderFlow.Application.DTOs;
using OrderFlow.Domain.Constants;
using OrderFlow.Web.Helpers.Abstract;
using OrderFlow.Web.Middlewares;
using System.Text.Json;

namespace OrderFlow.Web.Controllers
{
    /// <summary>
    /// Handles HTTP requests related to order operations.
    /// Access to actions is restricted by user roles using the RoleAuthorize attribute.
    /// </summary>
    public class OrdersController : Controller
    {
        private readonly ILogger<OrdersController> _logger;
        private readonly IApiRequestHelper _apiRequestHelper;

        public OrdersController(ILogger<OrdersController> logger, IApiRequestHelper apiRequestHelper)
        {
            _logger = logger;
            _apiRequestHelper = apiRequestHelper;
        }

        /// <summary>
        /// Displays the order creation view.
        /// Accessible only to users with the "Customer" role.
        /// </summary>
        [HttpGet("orders/create")]
        [RoleAuthorize(RoleNames.Customer)]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Displays a list of all orders.
        /// Accessible to users with "Admin" or "Customer" roles.
        /// </summary>
        [HttpGet("orders")]
        [RoleAuthorize(RoleNames.Admin, RoleNames.Customer)]
        public async Task<IActionResult> List()
        {
            var httpResponse = await _apiRequestHelper.SendAsync("/api/order/get-all", HttpMethod.Get);

            if (!httpResponse.IsSuccessStatusCode)
                return View();

            var json = await httpResponse.Content.ReadAsStringAsync();
            var orders = JsonSerializer.Deserialize<IEnumerable<OrderDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return View(orders);
        }

        /// <summary>
        /// Updates the status of an order.
        /// Accessible to users with "Admin", "Customer", or "Courier" roles.
        /// Sends a request to the API to update the order status and returns whether the operation was successful.
        /// </summary>
        [HttpPut("orders/update-status")]
        [RoleAuthorize(RoleNames.Admin, RoleNames.Customer, RoleNames.Courier)]
        public async Task<bool> UpdateStatus([FromBody] UpdateOrderStatusRequest updateOrderStatusRequest)
        {
            var httpResponse = await _apiRequestHelper.SendAsync($"/api/order/update-status", HttpMethod.Put, updateOrderStatusRequest);

            return httpResponse.IsSuccessStatusCode;
        }

        /// <summary>
        /// Displays the order tracking view.
        /// Accessible to users with "Admin" or "Customer" roles.
        /// </summary>
        [HttpGet("orders/track")]
        [RoleAuthorize(RoleNames.Admin, RoleNames.Customer)]
        public IActionResult Track()
        {
            return View();
        }

        /// <summary>
        /// Displays the deliveries assigned to the current courier.
        /// Accessible only to users with the "Courier" role.
        /// </summary>
        [HttpGet("orders/my-deliveries")]
        [RoleAuthorize(RoleNames.Courier)]
        public async Task<IActionResult> MyDeliveries()
        {
            // Get the authenticated user's ID from claims
            //var courierId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var courierId = HttpContext.Session.GetString("userId");

            if (string.IsNullOrEmpty(courierId))
            {
                // Optionally handle missing ID (e.g., redirect to login or show error)
                return Unauthorized();
            }

            var httpResponse = await _apiRequestHelper.SendAsync($"/api/order/assigned-orders/{courierId}", HttpMethod.Get);

            if (!httpResponse.IsSuccessStatusCode)
                return View();

            var json = await httpResponse.Content.ReadAsStringAsync();
            var orders = JsonSerializer.Deserialize<IEnumerable<OrderDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return View(orders);
        }
    }
}


