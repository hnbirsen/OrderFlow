using Microsoft.EntityFrameworkCore;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Interfaces;
using OrderFlow.Persistence.Context;

namespace OrderFlow.Persistence.Repositories
{
    public class OrderRepository : Repository<OrderEntity>, IOrderRepository
    {
        public OrderRepository(OrderFlowDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<OrderEntity>> GetOrdersByUserIdAsync(int userId)
        {
            return await _dbSet
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }
    }
}
