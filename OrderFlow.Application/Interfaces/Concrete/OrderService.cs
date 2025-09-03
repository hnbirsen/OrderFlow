using Microsoft.Extensions.Logging;
using OrderFlow.Application.DTOs;
using OrderFlow.Application.Interfaces.Abstract;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Enums;
using OrderFlow.Domain.Interfaces;

namespace OrderFlow.Application.Interfaces.Concrete
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IUnitOfWork unitOfWork, IUserService userService, ILogger<OrderService> logger)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _logger = logger;
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            _logger.LogInformation("Entering GetAllOrdersAsync method.");
            var orders = await _unitOfWork.Orders.GetAllAsync();
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

        public async Task<IEnumerable<OrderDto>> GetOrdersByIdsAsync(IEnumerable<int> orderIds)
        {
            if (orderIds == null)
            {
                return Enumerable.Empty<OrderDto>();
            }
            var ids = orderIds.Where(id => id > 0).Distinct().ToList();
            if (!ids.Any())
            {
                return Enumerable.Empty<OrderDto>();
            }

            var orders = await _unitOfWork.Orders.FindAsync(o => ids.Contains(o.Id));
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
            if (id <= 0)
            {
                _logger.LogWarning("Invalid id: {Id}", id);
                return null;
            }

            _logger.LogInformation("Entering GetOrderByIdAsync method with id: {Id}", id);
            var order = await _unitOfWork.Orders.GetByIdAsync(id);
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
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            if (request.UserId <= 0)
            {
                throw new ArgumentException("UserId must be positive.", nameof(request.UserId));
            }

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
            await _unitOfWork.Orders.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Order created for userId: {UserId}", request.UserId);
            _logger.LogInformation("Exiting CreateOrderAsync method.");
        }

        public async Task<OrderDto?> GetOrderByTrackingCodeAsync(string trackingCode)
        {
            if (string.IsNullOrWhiteSpace(trackingCode))
            {
                _logger.LogWarning("Empty tracking code provided.");
                return null;
            }

            _logger.LogInformation("Entering GetOrderByTrackingCodeAsync method with trackingCode: {TrackingCode}", trackingCode);
            var result = await _unitOfWork.Orders.FindAsync(o => o.TrackingCode == trackingCode);
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
            if (orderId <= 0)
            {
                _logger.LogWarning("Invalid orderId: {OrderId}", orderId);
                return false;
            }

            _logger.LogInformation("Entering UpdateOrderStatusAsync method for orderId: {OrderId} with status: {Status}", orderId, status);
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null)
            {
                _logger.LogWarning("Order not found for orderId: {OrderId}", orderId);
                _logger.LogInformation("Exiting UpdateOrderStatusAsync method.");
                return false;
            }
            order.Status = Enum.IsDefined(typeof(OrderStatusEnum), status)
                ? (OrderStatusEnum)status
                : order.Status;
            _unitOfWork.Orders.Update(order);
            if (order.Status == OrderStatusEnum.Sent)
            {
                var courierId = await _userService.GetAvailableCourierId();
                await _unitOfWork.OrderAssignments.AddAsync(new OrderAssignmentEntity
                {
                    OrderId = order.Id,
                    CourierId = courierId,
                    AssignedAt = DateTime.UtcNow
                });
                _logger.LogInformation("Order assigned to courierId: {CourierId}", courierId);
            }
            var result = await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Order status updated for orderId: {OrderId}", orderId);
            _logger.LogInformation("Exiting UpdateOrderStatusAsync method.");
            return result > 0;
        }
    }
}