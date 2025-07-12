namespace OrderFlow.Domain.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IOrderRepository Orders { get; }
        IUserRepository Users { get; }
        IOrderStatusHistoryRepository OrderStatusHistories { get; }

        Task<int> SaveChangesAsync();
    }
}
