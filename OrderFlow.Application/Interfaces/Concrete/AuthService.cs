using OrderFlow.Application.DTOs;
using OrderFlow.Application.Interfaces.Abstract;
using OrderFlow.Domain.Helpers;
using OrderFlow.Domain.Interfaces;

namespace OrderFlow.Application.Interfaces.Concrete
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;  

        public AuthService(IUserRepository userRepository, IJwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            var a = await _userRepository.FindAsync(u =>
                u.Email == request.Email &&
                u.Password == AesEncryptionHelper.Encrypt(request.Password));
            
            var user = a.FirstOrDefault();

            if (user == null)
                return null;

            LoginResponse loginResponse = _jwtService.GenerateToken(user.Id, user.Email, user.Role.ToString());
            user.RefreshToken = loginResponse.RefreshToken;
            user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);
            user.LastLogin = DateTime.UtcNow;

            _userRepository.Update(user);
            bool successs = await _userRepository.CompleteAsync();

            return loginResponse;
        }

        public async Task<LoginResponse?> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var user = (await _userRepository.FindAsync(u =>
                u.Email == request.Email &&
                u.RefreshToken == request.RefreshToken &&
                u.RefreshTokenExpiresAt > DateTime.UtcNow)).FirstOrDefault();

            if (user == null)
                return null;

            LoginResponse loginResponse = _jwtService.GenerateToken(user.Id, user.Email, user.Role.ToString());
            user.RefreshToken = loginResponse.RefreshToken;
            user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);
            
            _userRepository.Update(user);
            bool successs = await _userRepository.CompleteAsync();

            return loginResponse;
        }
    }
}
