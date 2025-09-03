using OrderFlow.Application.DTOs;
using OrderFlow.Domain.Dependencies;

namespace OrderFlow.Application.Interfaces.Abstract
{
    public interface IOrderService : IScoped
    {
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<OrderDto?> GetOrderByIdAsync(int id);
        Task CreateOrderAsync(CreateOrderRequest request);
        Task<OrderDto?> GetOrderByTrackingCodeAsync(string trackingCode);
        Task<bool> UpdateOrderStatusAsync(int orderId, int status);
        Task<IEnumerable<OrderDto>> GetOrdersByIdsAsync(IEnumerable<int> orderIds);
    }
}
