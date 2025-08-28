using OrderFlow.Web.Helpers.Abstract;
using OrderFlow.Web.Models;
using System.Text.Json;

namespace OrderFlow.Web.Middlewares
{
    /// <summary>
    /// Middleware to handle JWT token refresh before token expiration.
    /// </summary>
    public class TokenRefreshMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TokenRefreshMiddleware> _logger;
        private readonly TimeSpan _tokenRefreshThreshold = TimeSpan.FromMinutes(5);

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenRefreshMiddleware"/> class.
        /// </summary>
        public TokenRefreshMiddleware(
            RequestDelegate next,
            ILogger<TokenRefreshMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Processes the request and refreshes the token if needed.
        /// </summary>
        public async Task InvokeAsync(HttpContext context)
        {
            var accessToken = context.Session.GetString("access_token");
            var refreshToken = context.Session.GetString("refresh_token");
            var email = context.Session.GetString("email");

            if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken) && !string.IsNullOrEmpty(email))
            {
                var tokenExpiration = GetTokenExpiration(accessToken);
                if (ShouldRefreshToken(tokenExpiration))
                {
                    // Resolve IApiRequestHelper from the request's IServiceProvider
                    var apiRequestHelper = context.RequestServices.GetRequiredService<IApiRequestHelper>();
                    await RefreshTokenAsync(context, email, refreshToken, apiRequestHelper);
                }
            }

            await _next(context);
        }

        private DateTime? GetTokenExpiration(string accessToken)
        {
            try
            {
                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(accessToken);
                return token.ValidTo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing JWT token expiration");
                return null;
            }
        }

        private bool ShouldRefreshToken(DateTime? tokenExpiration)
        {
            if (!tokenExpiration.HasValue) return false;
            return tokenExpiration.Value.Subtract(_tokenRefreshThreshold) <= DateTime.UtcNow;
        }

        private async Task RefreshTokenAsync(HttpContext context, string email, string refreshToken, IApiRequestHelper apiRequestHelper)
        {
            try
            {
                var response = await apiRequestHelper.SendAsync(
                    "/api/auth/refresh-token",
                    HttpMethod.Post,
                    new { Email = email, RefreshToken = refreshToken }
                );

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var loginResponse = JsonSerializer.Deserialize<LoginResponse>(result,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (loginResponse != null)
                    {
                        context.Session.SetString("access_token", loginResponse.AccessToken);
                        context.Session.SetString("refresh_token", loginResponse.RefreshToken);
                        _logger.LogInformation("Token refreshed successfully for user: {Email}", email);
                    }
                }
                else
                {
                    _logger.LogWarning("Failed to refresh token for user: {Email}", email);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing token for user: {Email}", email);
            }
        }
    }
}