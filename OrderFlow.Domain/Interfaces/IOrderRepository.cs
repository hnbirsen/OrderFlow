using OrderFlow.Domain.Entities;

namespace OrderFlow.Domain.Interfaces
{
    public interface IOrderRepository : IRepository<OrderEntity>
    {
        Task<IEnumerable<OrderEntity>> GetOrdersByUserIdAsync(int userId);
    }
}
