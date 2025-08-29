using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderFlow.Application.DTOs;
using OrderFlow.Application.Interfaces.Abstract;

namespace OrderFlow.API.Controllers
{
    public class OrderController : BaseApiControler
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderService orderService, ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all orders.
        /// </summary>
        [HttpGet("get-all")]
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
        [HttpGet("update-status")]
        public async Task<IActionResult> UpdateStatus(Guid orderId, string status)
        {
            _logger.LogInformation("Entering UpdateStatus method.");
            _logger.LogInformation("UpdateStatus requested for orderId: {OrderId} with status: {Status}", orderId, status);
            var result = await _orderService.UpdateOrderStatusAsync(orderId, status);
            if (!result)
            {
                _logger.LogWarning("Failed to update status for orderId: {OrderId}", orderId);
                _logger.LogInformation("Exiting UpdateStatus method.");
                return BadRequest("Failed to update order status.");
            }
            _logger.LogInformation("Order status updated for orderId: {OrderId}", orderId);
            _logger.LogInformation("Exiting UpdateStatus method.");
            return Ok();
        }

        /// <summary>
        /// Retrieves an order by its unique identifier.
        /// </summary>
        /// <param name="id">Order identifier.</param>
        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetById(Guid id)
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
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
        {
            _logger.LogInformation("Entering Create method.");
            _logger.LogInformation("Create order requested by userId: {UserId}", request.UserId);
            await _orderService.CreateOrderAsync(request);
            _logger.LogInformation("Order created for userId: {UserId}", request.UserId);
            _logger.LogInformation("Exiting Create method.");
            return Created("", null);
        }
    }
}
