public class SessionAuthMiddleware
{
    private readonly RequestDelegate _next;

    public SessionAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLower();

        // Allow access to login and static files without authentication
        if (path != null && (path.StartsWith("/login") || path.StartsWith("/forgot-password") || path.StartsWith("/reset-password") || path.StartsWith("/css") || path.StartsWith("/js") || path.StartsWith("/images")))
        {
            await _next(context);
            return;
        }

        var accessToken = context.Session.GetString("access_token");
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            context.Response.Redirect("/login");
            return;
        }

        await _next(context);
    }
}