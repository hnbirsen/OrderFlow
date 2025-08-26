public class SessionAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SessionAuthMiddleware> _logger;

    public SessionAuthMiddleware(RequestDelegate next, ILogger<SessionAuthMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLower();

        // Allow access to login and static files without authentication
        if (path != null && (path.StartsWith("/login") || path.StartsWith("/forgot-password") || path.StartsWith("/reset-password") || path.StartsWith("/css") || path.StartsWith("/js") || path.StartsWith("/images")))
        {
            _logger.LogInformation("Authentication bypassed for path: {Path}", path);
            await _next(context);
            return;
        }

        var accessToken = context.Session.GetString("access_token");
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            _logger.LogWarning("No access token found in session. Redirecting to /login for path: {Path}", path);
            context.Response.Redirect("/login");
            return;
        }

        _logger.LogInformation("Authenticated request for path: {Path}", path);
        await _next(context);
    }
}