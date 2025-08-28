using Serilog;
using OrderFlow.Web.Helpers.Abstract;
using OrderFlow.Web.Helpers.Concrete;
using OrderFlow.Web.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add Serilog configuration
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IApiRequestHelper, ApiRequestHelper>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseMiddleware<SessionAuthMiddleware>();
app.UseMiddleware<TokenRefreshMiddleware>();

app.UseAuthentication();

// Initialize static sidebar menu cache once on startup
SidebarMenuConfig.Initialize();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
