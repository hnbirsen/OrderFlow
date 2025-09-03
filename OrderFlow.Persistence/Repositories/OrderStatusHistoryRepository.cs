using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Interfaces;
using OrderFlow.Persistence.Context;

namespace OrderFlow.Persistence.Repositories
{
    public class OrderStatusHistoryRepository(OrderFlowDbContext context) : BaseRepository<OrderStatusHistoryEntity>(context), IOrderStatusHistoryRepository
    {
    }
}
