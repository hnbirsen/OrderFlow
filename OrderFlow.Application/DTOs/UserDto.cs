using OrderFlow.Domain.Enums;

namespace OrderFlow.Application.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public UserRoleEnum Role { get; set; }
        public StatusEnum Status { get; set; }
    }
}
