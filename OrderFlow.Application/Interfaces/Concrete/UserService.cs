using Microsoft.Extensions.Logging;
using OrderFlow.Application.DTOs;
using OrderFlow.Application.Interfaces.Abstract;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Helpers;
using OrderFlow.Domain.Interfaces;

namespace OrderFlow.Application.Interfaces.Concrete
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserService> _logger;

        public UserService(IUnitOfWork unitOfWork, ILogger<UserService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            _logger.LogInformation("Entering GetAllUsersAsync method.");
            var users = await _unitOfWork.Users.GetAllAsync();
            _logger.LogInformation("{Count} users fetched from repository.", users?.Count() ?? 0);
            _logger.LogInformation("Exiting GetAllUsersAsync method.");
            return users.Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                Password = u.Password,
                Role = u.Role,
                Status = u.Status
            });
        }

        public async Task<bool> CreateUserAsync(CreateUserRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            _logger.LogInformation("Entering CreateUserAsync method for email: {Email}, role: {Role}", request.Email, request.Role);
            var user = new UserEntity
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address,
                City = request.City,
                State = request.State,
                ZipCode = request.ZipCode,
                Country = request.Country,
                ProfilePictureUrl = request.ProfilePictureUrl,
                Role = request.Role,
                Password = AesEncryptionHelper.Encrypt(PasswordGenerator.Generate()),
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Users.AddAsync(user);
            var result = await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("User creation result: {Result}", result > 0);
            _logger.LogInformation("Exiting CreateUserAsync method.");
            return result > 0;
        }

        public async Task<int> GetAvailableCourierId()
        {
            _logger.LogInformation("Entering GetAvailableCourierId method.");
            var couriers = await _unitOfWork.Users.FindAsync(u => u.Role == Domain.Enums.UserRoleEnum.Courier && u.Status == Domain.Enums.StatusEnum.Active);
            if (!couriers.Any())
            {
                _logger.LogWarning("No active couriers found.");
                throw new InvalidOperationException("No active couriers found.");
            }
            var courierWithLeastOrders = couriers
                .OrderBy(c => c.Orders?.Count ?? 0)
                .First();
            _logger.LogInformation("Available courierId: {CourierId}", courierWithLeastOrders.Id);
            _logger.LogInformation("Exiting GetAvailableCourierId method.");
            return courierWithLeastOrders.Id;
        }
    }
}
