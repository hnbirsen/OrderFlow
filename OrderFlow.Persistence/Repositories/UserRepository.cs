using OrderFlow.Domain.Interfaces;
using OrderFlow.Domain.Entities;
using OrderFlow.Persistence.Context;

namespace OrderFlow.Persistence.Repositories
{
    public class UserRepository : BaseRepository<UserEntity>, IUserRepository
    {
        public UserRepository(OrderFlowDbContext context) : base(context)
        {

        }

        public async Task UpdateLastLogin(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            
            if (user != null)
            {
                user.LastLogin = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }
}
