using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Interfaces.Repositories;
using OrderFlow.Persistence.Context;

namespace OrderFlow.Persistence.Repositories
{
    public class OrderStatusHistoryRepository : Repository<OrderStatusHistoryEntity>, IOrderStatusHistoryRepository
    {
        public OrderStatusHistoryRepository(OrderFlowDbContext context) : base(context)
        {
        }
    }
}
