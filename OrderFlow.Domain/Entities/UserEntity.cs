using OrderFlow.Domain.Enums;

namespace OrderFlow.Domain.Entities
{
    public class UserEntity : BaseEntity
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; } = null;
        public string? Address { get; set; } = null;
        public string? City { get; set; } = null;
        public string? State { get; set; } = null;
        public string? ZipCode { get; set; } = null;
        public string? Country { get; set; } = null;
        public string? ProfilePictureUrl { get; set; } = null;
        public DateTime? LastLogin { get; set; } = null;
        public UserRoleEnum Role { get; set; } = UserRoleEnum.Customer;

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiresAt { get; set; }

        public ICollection<OrderEntity> Orders { get; set; } = new List<OrderEntity>();
    }
}
