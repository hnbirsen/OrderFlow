using OrderFlow.Domain.Enums;

namespace OrderFlow.Application.DTOs
{
    public class CreateUserRequest
    {
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? PhoneNumber { get; set; } = null;
        public string? Address { get; set; } = null;
        public string? City { get; set; } = null;
        public string? State { get; set; } = null;
        public string? ZipCode { get; set; } = null;
        public string? Country { get; set; } = null;
        public string? ProfilePictureUrl { get; set; } = null;
        public UserRoleEnum Role { get; set; } = UserRoleEnum.Customer;
    }
}
