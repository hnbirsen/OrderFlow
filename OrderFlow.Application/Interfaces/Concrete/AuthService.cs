using OrderFlow.Application.DTOs;
using OrderFlow.Application.Interfaces.Abstract;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Interfaces;

namespace OrderFlow.Application.Interfaces.Concrete
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.FindAsync(u =>
                u.Email == request.Email &&
                u.Password == AesEncryptionHelper.Encrypt(request.Password));

            var found = user.FirstOrDefault();
            if (found == null)
                return null;

            // JWT üretimi yerine şimdilik sahte token
            return new LoginResponse
            {
                Token = GenerateFakeJwt(found),
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };
        }

        private string GenerateFakeJwt(UserEntity user)
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }
    }
}
