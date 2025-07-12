using OrderFlow.Domain.Interfaces.Repositories;
using OrderFlow.Persistence.Context;

namespace OrderFlow.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly OrderFlowDbContext _context;

        public IOrderRepository Orders { get; }
        public IUserRepository Users { get; }
        public IOrderStatusHistoryRepository OrderStatusHistories { get; }

        public UnitOfWork(
            OrderFlowDbContext context,
            IOrderRepository orders,
            IUserRepository users,
            IOrderStatusHistoryRepository orderStatusHistories)
        {
            _context = context;
            Orders = orders;
            Users = users;
            OrderStatusHistories = orderStatusHistories;
        }

        public async Task<int> SaveChangesAsync()
            => await _context.SaveChangesAsync();

        public void Dispose()
            => _context.Dispose();
    }
}
