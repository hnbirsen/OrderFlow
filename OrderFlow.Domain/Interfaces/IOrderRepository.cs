using OrderFlow.Domain.Dependencies;
using OrderFlow.Domain.Entities;

namespace OrderFlow.Domain.Interfaces
{
    public interface IOrderRepository : IRepository<OrderEntity>, IScoped
    {
        Task<IEnumerable<OrderEntity>> GetOrdersByUserIdAsync(int userId);
    }
}
