using OrderFlow.Application.DTOs;
using OrderFlow.Application.Interfaces.Abstract;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Helpers;
using OrderFlow.Domain.Interfaces;

namespace OrderFlow.Application.Interfaces.Concrete
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();

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

            await _userRepository.AddAsync(user);
            return await _userRepository.CompleteAsync();
        }
    }
}
