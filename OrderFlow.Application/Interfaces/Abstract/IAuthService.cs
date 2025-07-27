using OrderFlow.Application.DTOs;
using OrderFlow.Domain.Dependencies;

namespace OrderFlow.Application.Interfaces.Abstract
{
    public interface IAuthService : IScoped
    {
        Task<LoginResponse?> LoginAsync(LoginRequest request);
        Task<LoginResponse?> RefreshTokenAsync(RefreshTokenRequest request);
    }
}
