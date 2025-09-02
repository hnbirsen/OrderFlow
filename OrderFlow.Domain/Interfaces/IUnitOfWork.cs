using OrderFlow.Domain.Dependencies;

namespace OrderFlow.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable, IScoped
    {
        IOrderRepository Orders { get; }
        IUserRepository Users { get; }
        IOrderStatusHistoryRepository OrderStatusHistories { get; }
        IOrderAssignmentRepository OrderAssignments { get; }

        Task<int> SaveChangesAsync();
    }
}
