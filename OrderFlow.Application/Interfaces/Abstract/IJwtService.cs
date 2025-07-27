using OrderFlow.Application.DTOs;
using OrderFlow.Domain.Dependencies;

namespace OrderFlow.Application.Interfaces.Abstract
{
    public interface IJwtService : IScoped
    {
        LoginResponse GenerateToken(int userId, string email, string role);
    }
}
