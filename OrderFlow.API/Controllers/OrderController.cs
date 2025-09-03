using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderFlow.Application.DTOs;
using OrderFlow.Application.Interfaces.Abstract;
using OrderFlow.Domain.Constants;

namespace OrderFlow.API.Controllers
{
    public class OrderController : BaseApiControler
    {
        private readonly IOrderService _orderService;
        private readonly IOrderAssignmentService _orderAssignmentService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderService orderService, ILogger<OrderController> logger, IOrderAssignmentService orderAssignmentService)
        {
            _orderService = orderService;
            _orderAssignmentService = orderAssignmentService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all orders.
        /// </summary>
        [HttpGet("get-all")]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Entering GetAll method.");
            _logger.LogInformation("GetAll orders requested.");
            var orders = await _orderService.GetAllOrdersAsync();
            _logger.LogInformation("{Count} orders returned.", orders?.Count() ?? 0);
            _logger.LogInformation("Exiting GetAll method.");
            return Ok(orders);
        }

        /// <summary>
        /// Retrieves an order by its tracking code.
        /// </summary>
        /// <param name="trackingCode">Tracking code of the order.</param>
        [HttpGet("track")]
        [Authorize(Roles = $"{RoleNames.Admin},{RoleNames.Customer}")]
        public async Task<IActionResult> GetByTrackingCode(string trackingCode)
        {
            _logger.LogInformation("Entering GetByTrackingCode method.");
            _logger.LogInformation("GetByTrackingCode requested for trackingCode: {TrackingCode}", trackingCode);

            var order = await _orderService.GetOrderByTrackingCodeAsync(trackingCode);
            if (order == null)
            {
                _logger.LogWarning("Order not found for trackingCode: {TrackingCode}", trackingCode);
                _logger.LogInformation("Exiting GetByTrackingCode method.");
                return NotFound();
            }
            _logger.LogInformation("Order found for trackingCode: {TrackingCode}", trackingCode);
            _logger.LogInformation("Exiting GetByTrackingCode method.");
            return Ok(order);
        }

        /// <summary>
        /// Updates the status of an order.
        /// </summary>
        /// <param name="orderId">Order identifier.</param>
        /// <param name="status">New status value.</param>
        [HttpPut("update-status")]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateOrderStatusRequest updateOrderStatusRequest)
        {
            _logger.LogInformation("Entering UpdateStatus method.");
            _logger.LogInformation("UpdateStatus requested for orderId: {OrderId} with status: {Status}", updateOrderStatusRequest.OrderId, updateOrderStatusRequest.NewStatus);
            var result = await _orderService.UpdateOrderStatusAsync(updateOrderStatusRequest.OrderId, updateOrderStatusRequest.NewStatus);
            if (!result)
            {
                _logger.LogWarning("Failed to update status for orderId: {OrderId}", updateOrderStatusRequest.OrderId);
                _logger.LogInformation("Exiting UpdateStatus method.");
                return BadRequest("Failed to update order status.");
            }
            _logger.LogInformation("Order status updated for orderId: {OrderId}", updateOrderStatusRequest.OrderId);
            _logger.LogInformation("Exiting UpdateStatus method.");
            return Ok();
        }

        /// <summary>
        /// Retrieves an order by its unique identifier.
        /// </summary>
        /// <param name="id">Order identifier.</param>
        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("Entering GetById method.");
            _logger.LogInformation("GetById requested for orderId: {OrderId}", id);
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                _logger.LogWarning("Order not found for orderId: {OrderId}", id);
                _logger.LogInformation("Exiting GetById method.");
                return NotFound();
            }
            _logger.LogInformation("Order found for orderId: {OrderId}", id);
            _logger.LogInformation("Exiting GetById method.");
            return Ok(order);
        }

        /// <summary>
        /// Creates a new order.
        /// </summary>
        /// <param name="request">Order creation request data.</param>
        [HttpPost("create")]
        [Authorize(Roles = RoleNames.Customer)]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequest createOrderRequest)
        {
            _logger.LogInformation("Entering Create method.");
            _logger.LogInformation("Create order requested by userId: {UserId}", createOrderRequest.UserId);
            await _orderService.CreateOrderAsync(createOrderRequest);
            _logger.LogInformation("Order created for userId: {UserId}", createOrderRequest.UserId);
            _logger.LogInformation("Exiting Create method.");
            return Created("", null);
        }

        /// <summary>
        /// Assigns an order to a courier.
        /// </summary>
        /// <param name="request">Order assignment request data.</param>
        [HttpPost("assign-order")]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<IActionResult> AssignOrder([FromBody] AssignOrderRequest assignOrderRequest)
        {
            _logger.LogInformation("Entering AssignOrder method.");
            _logger.LogInformation("AssignOrder requested for orderId: {OrderId} to courierId: {CourierId}", assignOrderRequest.OrderId, assignOrderRequest.CourierId);
            var result = await _orderAssignmentService.AssignOrderAsync(assignOrderRequest);
            if (!result)
            {
                _logger.LogWarning("Failed to assign orderId: {OrderId} to courierId: {CourierId}", assignOrderRequest.OrderId, assignOrderRequest.CourierId);
                _logger.LogInformation("Exiting AssignOrder method.");
                return BadRequest("Failed to assign order to courier.");
            }
            _logger.LogInformation("Order assigned for orderId: {OrderId} to courierId: {CourierId}", assignOrderRequest.OrderId, assignOrderRequest.CourierId);
            _logger.LogInformation("Exiting AssignOrder method.");
            return Ok();
        }

        /// <summary>
        /// Retrieves orders assigned to a specific courier.
        /// </summary>
        /// <param name="courierId">Courier identifier.</param>
        [HttpGet("assigned-orders/{courierId}")]
        [Authorize(Roles = RoleNames.Courier)]
        public async Task<IActionResult> GetAssignedOrdersByCourierId(int courierId)
        {
            _logger.LogInformation("Entering GetAssignedOrdersByCourierId method.");
            _logger.LogInformation("GetAssignedOrdersByCourierId requested for courierId: {CourierId}", courierId);

            // Get assignments for the courier
            var orderAssignments = await _orderAssignmentService.GetByCourierIdAsync(courierId);

            // Get order IDs from assignments
            var orderIds = orderAssignments.Select(x => x.OrderId).ToList();

            // If no assignments, return empty list
            if (!orderIds.Any())
            {
                _logger.LogInformation("No assigned orders found for courierId: {CourierId}", courierId);
                _logger.LogInformation("Exiting GetAssignedOrdersByCourierId method.");
                return Ok(new List<OrderDto>());
            }

            // Get all orders and filter by assigned IDs
            var allOrders = await _orderService.GetAllOrdersAsync();
            var assignedOrders = allOrders.Where(o => orderIds.Contains(o.Id)).ToList();

            _logger.LogInformation("{Count} assigned orders returned for courierId: {CourierId}", assignedOrders.Count, courierId);
            _logger.LogInformation("Exiting GetAssignedOrdersByCourierId method.");

            return Ok(assignedOrders);
        }
    }
}
