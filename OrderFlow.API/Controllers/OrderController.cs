using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("GetAll orders requested.");
            var orders = await _orderService.GetAllOrdersAsync();
            _logger.LogInformation("{Count} orders returned.", orders?.Count() ?? 0);
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            _logger.LogInformation("GetById requested for orderId: {OrderId}", id);
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                _logger.LogWarning("Order not found for orderId: {OrderId}", id);
                return NotFound();
            }
            _logger.LogInformation("Order found for orderId: {OrderId}", id);
            return Ok(order);
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
        {
            _logger.LogInformation("Create order requested by userId: {UserId}", request.UserId);
            await _orderService.CreateOrderAsync(request);
            _logger.LogInformation("Order created for userId: {UserId}", request.UserId);
            return Created("", null);
        }
    }
}
