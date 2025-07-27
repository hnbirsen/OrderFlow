namespace OrderFlow.Application.DTOs
{
    public class RefreshTokenRequest
    {
        public required string Email { get; set; }
        public required string RefreshToken { get; set; }
    }
}
