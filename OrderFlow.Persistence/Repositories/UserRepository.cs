using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Interfaces.Repositories;
using OrderFlow.Persistence.Context;

namespace OrderFlow.Persistence.Repositories
{
    public class UserRepository : Repository<UserEntity>, IUserRepository
    {
        public UserRepository(OrderFlowDbContext context) : base(context)
        {
        }
    }
}
