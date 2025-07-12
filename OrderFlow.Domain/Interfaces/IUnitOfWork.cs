//using OrderFlow.Application.Dependencies;

namespace OrderFlow.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable//, IScoped
    {
        IOrderRepository Orders { get; }
        IUserRepository Users { get; }
        IOrderStatusHistoryRepository OrderStatusHistories { get; }

        Task<int> SaveChangesAsync();
    }
}
