# OrderFlow

OrderFlow is a modular .NET solution for order management with a Web MVC front end and a RESTful API, featuring JWT authentication, session-based access, and role-based authorization.

## Projects

- OrderFlow.API: ASP.NET Core Web API for authentication and order operations
- OrderFlow.Application: Application layer (services, DTOs, configuration)
- OrderFlow.Persistence: EF Core context, configurations, repositories, migrations
- OrderFlow.Domain: Entities, enums, interfaces, helpers
- OrderFlow.Web: ASP.NET Core MVC app (UI), session auth, role-aware navigation
- OrderFlow.Tests: xUnit-based unit tests for API, Application, and Persistence layers

## Architecture

- Clean layering between Domain, Application, Persistence, API, and Web
- EF Core with repository/unit-of-work in Persistence
- JWT creation/validation in API; Web uses access token via session
- Role-based authorization via custom `RoleAuthorize` attribute
- Sidebar menu visibility is derived from controller/action `RoleAuthorize` attributes and cached at startup

## Prerequisites

- .NET SDK 8.0+
- SQL Server (or local SQL Server Express)

## Configuration

1) API base URL for Web UI
- File: `OrderFlow.Web/appsettings.json`
- Key: `ApiBaseUrl` (e.g., `https://localhost:5001`)

2) Connection string for Persistence/API
- File: `OrderFlow.API/appsettings.json`
- Key: `ConnectionStrings:Default`

3) JWT settings (API)
- File: `OrderFlow.API/appsettings.json` (and/or Development)
- Section: `Jwt` (issuer, audience, key, expiry)

## Database

- Apply migrations (from `OrderFlow.Persistence`)
- Typical commands (from solution root):

```bash
# Update database using API as startup
dotnet ef database update --project OrderFlow.Persistence --startup-project OrderFlow.API
```

Alternatively, configure your IDE to run migrations with `OrderFlow.API` as startup.

## Running the Solution

- Start both API and Web projects
- In development, typical URLs:
  - API: `https://localhost:5001`
  - Web: `https://localhost:5003`
- Adjust ports via `launchSettings.json` if needed

## Authentication & Authorization

- Login occurs in the Web app via API `/api/auth/login`
- On success, Web stores `access_token` and `role` in session
- `SessionAuthMiddleware` in Web redirects unauthenticated users to `/login`
- `RoleAuthorize` (Web) enforces role-based access at controller/action level
- Sidebar menu (`_Sidebar.cshtml`) filters items for the current role based on discovered attributes

### Role Discovery for Sidebar

- `SidebarMenuConfig` builds a list of menu items and resolves required roles by reflecting on controller/action `RoleAuthorize` attributes
- The resolved items are cached at startup via `SidebarMenuConfig.Initialize()` and reused for all requests

## Key Files

- Web
  - `Program.cs`: Middleware pipeline and `SidebarMenuConfig` initialization
  - `Middlewares/SessionAuthMiddleware.cs`: Session token guard
  - `Middlewares/RoleAuthorizeAttribute.cs`: Role-based authorization attribute
  - `Helpers/Concrete/SidebarMenuConfig.cs`: Menu config with role discovery + startup cache
  - `Views/Shared/_Sidebar.cshtml`: Renders menu filtered by role
- API
  - `Controllers/AuthController.cs`: Login/refresh endpoints
  - `Controllers/OrderController.cs`: Order endpoints
- Application
  - `Interfaces/Concrete/*Service.cs`: Core service logic
  - `DTOs/*`: Request/response contracts
- Persistence
  - `Context/OrderFlowDbContext.cs`, `Migrations/*`, `Repositories/*`
- Domain
  - `Entities/*`, `Enums/*` (`UserRoleEnum`, `OrderStatusEnum`), `Helpers/*`
- Tests
  - `OrderFlow.Tests`: xUnit-based unit tests for API, Application, and Persistence

## Development Tips

- Logs: Each project writes logs to its configured sinks (Serilog in Web)
- Adjust CORS for API if hosting separately from Web
- Ensure JWT role claim type matches `ClaimTypes.Role` in Web

## Unit Testing

Unit tests are located in the `OrderFlow.Tests` project. The test suite uses xUnit and EF Core InMemory for fast, isolated tests. To run all tests:

```bash
dotnet test OrderFlow.Tests
```

Test coverage is collected using `coverlet.collector`.

### Example Test

```csharp
[Fact]
public async Task GetOrder_ReturnsOrder()
{
    // Arrange
    var service = new OrderService(/* dependencies */);
    // Act
    var result = await service.GetOrderAsync(1);
    // Assert
    Assert.NotNull(result);
}
```

## Troubleshooting

- 401/redirect loop in Web: Verify `ApiBaseUrl` and that login returns a valid JWT
- 403 on pages: Check `RoleAuthorize` on the action and the user role claim
- Sidebar items missing: Ensure controller/action names in `SidebarMenuConfig` match actual types/methods and that `SidebarMenuConfig.Initialize()` is called at startup

## Scripts

Common CLI commands:

```bash
# Restore & build
dotnet restore
dotnet build

# Run API
dotnet run --project OrderFlow.API

# Run Web
dotnet run --project OrderFlow.Web

# Add EF migration (example)
dotnet ef migrations add AddNewColumns --project OrderFlow.Persistence --startup-project OrderFlow.API

# Update database
dotnet ef database update --project OrderFlow.Persistence --startup-project OrderFlow.API
```

## Contributing

Pull requests are welcome. For major changes, please open an issue first.

## License

MIT

