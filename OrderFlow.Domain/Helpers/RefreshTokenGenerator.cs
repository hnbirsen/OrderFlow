using System.Security.Cryptography;

namespace OrderFlow.Domain.Helpers
{
    public static class RefreshTokenGenerator
    {
        public static string Generate()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
    }
}
