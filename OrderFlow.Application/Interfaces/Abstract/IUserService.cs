using OrderFlow.Application.DTOs;
using OrderFlow.Domain.Dependencies;

namespace OrderFlow.Application.Interfaces.Abstract
{
    public interface IUserService : IScoped
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task CreateUserAsync(CreateUserRequest request);
    }
}
