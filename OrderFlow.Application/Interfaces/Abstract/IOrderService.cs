using OrderFlow.Application.DTOs;
using OrderFlow.Domain.Dependencies;

namespace OrderFlow.Application.Interfaces.Abstract
{
    public interface IOrderService : IScoped
    {
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<OrderDto?> GetOrderByIdAsync(Guid id);
        Task CreateOrderAsync(CreateOrderRequest request);
    }
}
