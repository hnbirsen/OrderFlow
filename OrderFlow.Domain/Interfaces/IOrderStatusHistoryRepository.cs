using OrderFlow.Domain.Dependencies;
using OrderFlow.Domain.Entities;

namespace OrderFlow.Domain.Interfaces
{
    public interface IOrderStatusHistoryRepository : IRepository<OrderStatusHistoryEntity>, IScoped
    {
    }
}
