using OrderFlow.Domain.Dependencies;
using OrderFlow.Domain.Entities;

namespace OrderFlow.Domain.Interfaces
{
    public interface IUserRepository : IBaseRepository<UserEntity>, IScoped
    {
        Task UpdateLastLogin(int userId);
    }
}
