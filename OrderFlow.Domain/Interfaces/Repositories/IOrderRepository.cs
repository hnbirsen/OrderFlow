using OrderFlow.Domain.Entities;

namespace OrderFlow.Domain.Interfaces.Repositories
{
    public interface IOrderRepository : IRepository<OrderEntity>
    {
        Task<IEnumerable<OrderEntity>> GetOrdersByUserIdAsync(int userId);
    }
}
