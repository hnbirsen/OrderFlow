using OrderFlow.Application.DTOs;
using OrderFlow.Application.Interfaces.Abstract;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Enums;
using OrderFlow.Domain.Interfaces;

namespace OrderFlow.Application.Interfaces.Concrete
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllAsync();

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

        public async Task<OrderDto?> GetOrderByIdAsync(Guid id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) return null;

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
        }

        public async Task<OrderDto?> GetOrderByTrackingCodeAsync(string trackingCode)
        {
            var result = await _orderRepository.FindAsync(o => o.TrackingCode == trackingCode);
            if (result == null) return null;
            
            var order = result.FirstOrDefault();
            if (order == null) return null;

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

        public async Task<bool> UpdateOrderStatusAsync(Guid orderId, string status)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) return false;

            order.Status = Enum.TryParse<OrderStatusEnum>(status, out var newStatus) ? newStatus : order.Status;
            _orderRepository.Update(order);
            return await _orderRepository.CompleteAsync();
        }
    }    
}