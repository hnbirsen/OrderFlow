using OrderFlow.Application.DTOs;
using OrderFlow.Application.Interfaces.Abstract;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Enums;
using OrderFlow.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace OrderFlow.Application.Interfaces.Concrete
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderAssignmentRepository _orderAssignmentRepository;
        private readonly IUserService _userService;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IOrderRepository orderRepository, IOrderAssignmentRepository orderAssignmentRepository, IUserService userService, ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _orderAssignmentRepository = orderAssignmentRepository;
            _userService = userService;
            _logger = logger;
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            _logger.LogInformation("Entering GetAllOrdersAsync method.");
            var orders = await _orderRepository.GetAllAsync();
            _logger.LogInformation("{Count} orders fetched from repository.", orders?.Count() ?? 0);
            _logger.LogInformation("Exiting GetAllOrdersAsync method.");
            return orders.Select(o => new OrderDto
            {
                Id = o.Id,
                Description = o.Description,
                TrackingCode = o.TrackingCode ?? string.Empty,
                Items = o.Items ?? new Dictionary<string, int>(),
                Status = o.Status,
                CreatedAt = o.CreatedAt
            });
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            _logger.LogInformation("Entering GetOrderByIdAsync method with id: {Id}", id);
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                _logger.LogWarning("Order not found for id: {Id}", id);
                _logger.LogInformation("Exiting GetOrderByIdAsync method.");
                return null;
            }
            _logger.LogInformation("Order found for id: {Id}", id);
            _logger.LogInformation("Exiting GetOrderByIdAsync method.");
            return new OrderDto
            {
                Id = order.Id,
                Description = order.Description,
                TrackingCode = order.TrackingCode ?? string.Empty,
                Items = order.Items ?? new Dictionary<string, int>(),
                Status = order.Status,
                CreatedAt = order.CreatedAt
            };
        }

        public async Task CreateOrderAsync(CreateOrderRequest request)
        {
            _logger.LogInformation("Entering CreateOrderAsync method for userId: {UserId}", request.UserId);
            var entity = new OrderEntity
            {
                Description = request.Description,
                Items = request.Items,
                UserId = request.UserId,
                TotalAmount = request.Items.Values.Sum(),
                Status = OrderStatusEnum.New,
                CreatedAt = DateTime.UtcNow
            };
            await _orderRepository.AddAsync(entity);
            await _orderRepository.CompleteAsync();
            _logger.LogInformation("Order created for userId: {UserId}", request.UserId);
            _logger.LogInformation("Exiting CreateOrderAsync method.");
        }

        public async Task<OrderDto?> GetOrderByTrackingCodeAsync(string trackingCode)
        {
            _logger.LogInformation("Entering GetOrderByTrackingCodeAsync method with trackingCode: {TrackingCode}", trackingCode);
            var result = await _orderRepository.FindAsync(o => o.TrackingCode == trackingCode);
            if (result == null)
            {
                _logger.LogWarning("No orders found for trackingCode: {TrackingCode}", trackingCode);
                _logger.LogInformation("Exiting GetOrderByTrackingCodeAsync method.");
                return null;
            }
            var order = result.FirstOrDefault();
            if (order == null)
            {
                _logger.LogWarning("Order not found for trackingCode: {TrackingCode}", trackingCode);
                _logger.LogInformation("Exiting GetOrderByTrackingCodeAsync method.");
                return null;
            }
            _logger.LogInformation("Order found for trackingCode: {TrackingCode}", trackingCode);
            _logger.LogInformation("Exiting GetOrderByTrackingCodeAsync method.");
            return new OrderDto
            {
                Id = order.Id,
                Description = order.Description,
                TrackingCode = order.TrackingCode ?? string.Empty,
                Items = order.Items ?? new Dictionary<string, int>(),
                Status = order.Status,
                CreatedAt = order.CreatedAt
            };
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, int status)
        {
            _logger.LogInformation("Entering UpdateOrderStatusAsync method for orderId: {OrderId} with status: {Status}", orderId, status);
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                _logger.LogWarning("Order not found for orderId: {OrderId}", orderId);
                _logger.LogInformation("Exiting UpdateOrderStatusAsync method.");
                return false;
            }
            order.Status = Enum.IsDefined(typeof(OrderStatusEnum), status)
                ? (OrderStatusEnum)status
                : order.Status;
            _orderRepository.Update(order);
            if (order.Status == OrderStatusEnum.Sent)
            {
                var courierId = await _userService.GetAvailableCourierId();
                await _orderAssignmentRepository.AddAsync(new OrderAssignmentEntity
                {
                    OrderId = order.Id,
                    CourierId = courierId,
                    AssignedAt = DateTime.UtcNow
                });
                _logger.LogInformation("Order assigned to courierId: {CourierId}", courierId);
            }
            var result = await _orderRepository.CompleteAsync();
            _logger.LogInformation("Order status updated for orderId: {OrderId}", orderId);
            _logger.LogInformation("Exiting UpdateOrderStatusAsync method.");
            return result;
        }
    }    
}