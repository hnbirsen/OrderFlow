using Microsoft.EntityFrameworkCore;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Interfaces;
using OrderFlow.Persistence.Context;

namespace OrderFlow.Persistence.Repositories
{
    public class OrderRepository : BaseRepository<OrderEntity>, IOrderRepository
    {
        public OrderRepository(OrderFlowDbContext context) : base(context)
        {
        }
    }
}
