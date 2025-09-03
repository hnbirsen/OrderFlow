using Microsoft.Extensions.Logging;
using OrderFlow.Application.DTOs;
using OrderFlow.Application.Interfaces.Abstract;
using OrderFlow.Domain.Interfaces;

namespace OrderFlow.Application.Interfaces.Concrete
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUnitOfWork unitOfWork, IJwtService jwtService, ILogger<AuthService> logger)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _logger = logger;
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            _logger.LogInformation("Entering LoginAsync method for email: {Email}", request.Email);
            var candidates = await _unitOfWork.Users.FindAsync(u =>
                u.Email == request.Email &&
                u.Password == AesEncryptionHelper.Encrypt(request.Password));
            var user = candidates.FirstOrDefault();
            if (user == null)
            {
                _logger.LogWarning("Login failed for email: {Email}", request.Email);
                _logger.LogInformation("Exiting LoginAsync method.");
                return null;
            }
            LoginResponse loginResponse = _jwtService.GenerateToken(user.Id, user.Email, user.Role.ToString());
            user.RefreshToken = loginResponse.RefreshToken;
            user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);
            user.LastLogin = DateTime.UtcNow;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("User {Email} logged in successfully.", request.Email);
            _logger.LogInformation("Exiting LoginAsync method.");
            return loginResponse;
        }

        public async Task<LoginResponse?> RefreshTokenAsync(RefreshTokenRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            _logger.LogInformation("Entering RefreshTokenAsync method for email: {Email}", request.Email);
            var user = (await _unitOfWork.Users.FindAsync(u =>
                u.Email == request.Email &&
                u.RefreshToken == request.RefreshToken &&
                u.RefreshTokenExpiresAt > DateTime.UtcNow)).FirstOrDefault();
            if (user == null)
            {
                _logger.LogWarning("Refresh token failed for email: {Email}", request.Email);
                _logger.LogInformation("Exiting RefreshTokenAsync method.");
                return null;
            }
            LoginResponse loginResponse = _jwtService.GenerateToken(user.Id, user.Email, user.Role.ToString());
            user.RefreshToken = loginResponse.RefreshToken;
            user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Refresh token succeeded for email: {Email}", request.Email);
            _logger.LogInformation("Exiting RefreshTokenAsync method.");
            return loginResponse;
        }
    }
}
