# Authentication & Authorization Implementation Guide

## Overview

This document provides a comprehensive guide for the Authentication & Authorization module implementation in PHCAPI. The module follows **the same architecture pattern as Audit and Parameters modules** using **MediatR with CQRS**.

## Architecture Philosophy

### Module Structure (Same as Audit/Parameters)

```
Auth/
├── Auth.Domain/              
│   ├── Interfaces/          # Repository and service contracts
│   └── Constants.cs         # Authorization constants (moved to Shared.Kernel)
│
├── Auth.Application/
│   ├── Features/            # ✅ CQRS Commands & Queries
│   │   ├── Login/          # LoginCommand + LoginCommandHandler
│   │   ├── Register/       # RegisterCommand + RegisterCommandHandler
│   │   ├── CreateRole/     # CreateRoleCommand + Handler
│   │   ├── AddUserToRole/  # AddUserToRoleCommand + Handler
│   │   ├── GetAllRoles/    # GetAllRolesQuery + Handler
│   │   └── GetUserRoles/   # GetUserRolesQuery + Handler
│   ├── DTOs/
│   └── DependencyInjection.cs
│
├── Auth.Infrastructure/
│   ├── Persistence/         # AuthDbContext
│   ├── Repositories/        # Identity repositories
│   ├── Services/            # JWT token service, Identity auth service
│   ├── Migrations/          
│   ├── Data/               # Seed data
│   └── DependencyInjection.cs
│
└── Auth.Presentation/
    ├── Controllers/         # AuthenticateController (uses IMediator only)
    └── DependencyInjection.cs
```

### Key Pattern: CQRS with MediatR

**Exactly like Audit module:**

1. **Controller** → Injects `IMediator`, sends Commands/Queries
2. **Command/Query** → Records implementing `IRequest<TResponse>`
3. **Handler** → `IRequestHandler<TRequest, TResponse>` with business logic
4. **Repository** → Injected into handlers for data access

### Example: Login Flow

```csharp
// 1. Controller
[HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginRequestDto model, CancellationToken ct)
{
    var command = new LoginCommand(model.Username, model.Password);
    var result = await _mediator.Send(command, ct);
    return Ok(result);
}

// 2. Command
public sealed record LoginCommand(string Username, string Password) 
    : IRequest<LoginResponseDto>;

// 3. Handler
public sealed class LoginCommandHandler 
    : IRequestHandler<LoginCommand, LoginResponseDto>
{
    private readonly IAuthenticationService _authService;
    
    public async Task<LoginResponseDto> Handle(
        LoginCommand request, 
        CancellationToken ct)
    {
        var result = await _authService.LoginAsync(
            request.Username, 
            request.Password, 
            ct);
        
        return new LoginResponseDto { /* map result */ };
    }
}
```

**Purpose**: Provide concrete implementations of domain interfaces.

**Key Files**:
- `Persistence/AuthDbContext.cs` - Database context for Identity tables
- `Services/JwtTokenService.cs` - JWT token generation implementation
- `Services/IdentityAuthenticationService.cs` - ASP.NET Identity authentication
- `Repositories/IdentityUserRepository.cs` - User management using Identity
- `Repositories/IdentityRoleRepository.cs` - Role management using Identity
- `Migrations/` - Database migrations
- `DependencyInjection.cs` - Service registration

**Implementation Details**:

#### JWT Token Service
Uses `System.IdentityModel.Tokens.Jwt` to generate and validate tokens:
- Symmetric key encryption (HMAC-SHA256)
- Configurable expiration time
- Claims-based authentication

#### Identity Authentication Service
Leverages ASP.NET Identity:
- Secure password hashing (PBKDF2)
- User lockout after failed attempts
- Email confirmation support
- Role-based authorization

### 4. Presentation Layer (`Auth.Presentation`)

**Purpose**: Expose HTTP endpoints for authentication operations.

**Key Files**:
- `Controllers/AuthenticateController.cs` - REST API endpoints
- `DependencyInjection.cs` - Controller registration

**API Design**:
- Follows RESTful conventions
- Returns consistent response format (matches legacy system)
- Uses proper HTTP status codes
- Includes authorization attributes

## Authorization Strategy

### Shared Authorization Constants

Authorization constants are defined in `Shared.Kernel.Authorization` to be accessible across all modules:

```csharp
public static class AppRoles
{
    public const string Administrator = "Administrator";
    public const string InternalUser = "InternalUser";
    public const string ApiUser = "ApiUser";
    // ...
}

public static class AppPolicies
{
    public const string InternalOnly = "InternalOnly";
    public const string AdminOnly = "AdminOnly";
    // ...
}
```

### Policy Configuration (Program.cs)

```csharp
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AppPolicies.InternalOnly, policy =>
        policy.RequireRole(
            AppRoles.Administrator, 
            AppRoles.InternalUser, 
            AppRoles.AuditViewer));
    
    // Other policies...
});
```

### Protecting Modules

Example: Audit module is restricted to internal users only:

```csharp
[ApiController]
[Route("api/audit")]
[Authorize(Policy = AppPolicies.InternalOnly)]
public sealed class AuditController : ControllerBase
{
    // Only Administrator, InternalUser, and AuditViewer can access
}
```

## Database Design

### Identity Tables

The module uses standard ASP.NET Identity schema:

- **AspNetUsers**: User accounts
  - Id, UserName, Email, PasswordHash, etc.
  
- **AspNetRoles**: Role definitions
  - Id, Name, NormalizedName
  
- **AspNetUserRoles**: Many-to-many relationship
  - UserId, RoleId
  
- **AspNetUserClaims**: Additional user properties
- **AspNetUserLogins**: External authentication providers
- **AspNetUserTokens**: Secure tokens for operations
- **AspNetRoleClaims**: Claims associated with roles

### Isolated Database Context

The `AuthDbContext` is completely isolated from other modules:
- Uses its own connection string (or shared if preferred)
- Own migration history
- No dependencies on other module contexts

## Security Implementation

### Password Security

ASP.NET Identity handles password security:
```csharp
options.Password.RequireDigit = true;
options.Password.RequireLowercase = true;
options.Password.RequireUppercase = true;
options.Password.RequiredLength = 6;
```

### Token Security

JWT tokens are signed and validated:
```csharp
var token = new JwtSecurityToken(
    issuer: _issuer,
    audience: _audience,
    expires: DateTime.Now.AddHours(3),
    claims: authClaims,
    signingCredentials: new SigningCredentials(
        authSigningKey, 
        SecurityAlgorithms.HmacSha256)
);
```

### User Lockout

Protect against brute-force attacks:
```csharp
options.Lockout.MaxFailedAccessAttempts = 5;
options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
```

## Integration Guide

### Step 1: Add Module Reference

In `PHCAPI.Host.csproj`:
```xml
<ProjectReference Include="..\Modules\Auth\Auth.Presentation\Auth.Presentation.csproj" />
<ProjectReference Include="..\Modules\Auth\Auth.Infrastructure\Auth.Infrastructure.csproj" />
```

### Step 2: Configure Services

In `Program.cs`:
```csharp
// Infrastructure (Identity + JWT)
builder.Services.AddAuthInfrastructure(builder.Configuration);

// Presentation (Controllers + Services)
builder.Services.AddAuthPresentation();

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(/* configuration */);

// Authorization Policies
builder.Services.AddAuthorization(/* policies */);
```

### Step 3: Configure Middleware Pipeline

Order matters!
```csharp
app.UseAuthentication();  // MUST come before UseAuthorization
app.UseAuthorization();
```

### Step 4: Update Configuration

In `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "..."
  },
  "JWT": {
    "ValidIssuer": "...",
    "ValidAudience": "...",
    "Secret": "...",
    "ExpirationHours": 3
  }
}
```

### Step 5: Run Migrations

```powershell
cd src/Modules/Auth/Auth.Infrastructure
dotnet ef database update --context AuthDbContext
```

## Testing Strategy

### Unit Testing

Test application services with mocked repositories:

```csharp
[Fact]
public async Task Login_WithValidCredentials_ReturnsToken()
{
    // Arrange
    var mockAuthService = new Mock<IAuthenticationService>();
    mockAuthService
        .Setup(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>(), default))
        .ReturnsAsync(new AuthenticationResult { Success = true, Token = "test-token" });
    
    var service = new AuthService(mockAuthService.Object, /* ... */);
    
    // Act
    var result = await service.LoginAsync(new LoginRequestDto 
    { 
        Username = "test", 
        Password = "password" 
    });
    
    // Assert
    Assert.True(result.Allowed);
    Assert.NotEmpty(result.Token);
}
```

### Integration Testing

Test full authentication flow with a test database:

```csharp
[Fact]
public async Task Login_EndToEnd_ReturnsValidToken()
{
    // Arrange: Create test user in database
    // Act: Call login endpoint
    // Assert: Token is valid and contains correct claims
}
```

## Migration from Legacy System

### Differences from Old System

| Aspect | Legacy | New System |
|--------|--------|------------|
| Architecture | Monolithic, single controller | Modular, layered architecture |
| Extensibility | Tightly coupled to Identity | Provider-agnostic with interfaces |
| Authorization | Basic roles | Roles + Policies |
| Testing | Difficult (tight coupling) | Easy (dependency injection) |
| Module isolation | Shared context | Isolated context per module |

### API Compatibility

The new system maintains the same API response format:

**Legacy Response:**
```json
{
  "token": "...",
  "expiration": "...",
  "allowed": true,
  "output_response": "AUTHENTICATED"
}
```

**New Response:**
```json
{
  "token": "...",
  "expiration": "...",
  "allowed": true,
  "outputResponse": "AUTHENTICATED",
  "roles": ["Administrator"]  // NEW: Added roles array
}
```

## Future Enhancements

### 1. Token Refresh

Implement refresh tokens for better UX:

```csharp
public interface ITokenService
{
    string GenerateRefreshToken();
    Task<bool> ValidateRefreshToken(string refreshToken);
}
```

### 2. Auth0 Integration

Create new implementation:

```csharp
public class Auth0AuthenticationService : IAuthenticationService
{
    // Implement using Auth0 SDK
}
```

Register in DI:
```csharp
services.AddScoped<IAuthenticationService, Auth0AuthenticationService>();
```

### 3. Multi-Factor Authentication (MFA)

Add to domain interface:

```csharp
public interface IAuthenticationService
{
    Task<MfaChallengeResult> InitiateMfaAsync(string username);
    Task<AuthenticationResult> ValidateMfaAsync(string username, string code);
}
```

### 4. OAuth2/OpenID Connect

Implement external providers:

```csharp
builder.Services.AddAuthentication()
    .AddGoogle(options => { /* ... */ })
    .AddMicrosoft(options => { /* ... */ });
```

## Common Issues & Solutions

### Issue: Unauthorized 401 even with valid token

**Solution**: Ensure middleware order is correct:
```csharp
app.UseAuthentication();  // Must come first
app.UseAuthorization();
```

### Issue: CORS errors with Authorization header

**Solution**: Configure CORS to allow Authorization header:
```csharp
policy.AllowAnyHeader();  // Includes Authorization
```

### Issue: Token validation fails

**Solution**: Check JWT configuration matches between issuer and consumer:
- Same Secret key
- Same Issuer
- Same Audience
- Clock skew tolerance

## Performance Considerations

### Database Indexes

Identity tables include optimized indexes:
- Username (unique, filtered)
- Email (non-unique)
- Role normalization

### Token Caching

Consider caching token validation results:
```csharp
// Cache validated tokens to reduce I/O
var cacheKey = $"token:{tokenHash}";
```

### Connection Pooling

Entity Framework Core automatically manages connection pooling. Ensure `MultipleActiveResultSets=true` in connection string.

## Conclusion

This authentication module provides a solid foundation that balances:
- ✅ Security best practices
- ✅ Clean architecture principles
- ✅ Extensibility for future needs
- ✅ Backward compatibility
- ✅ Developer experience

The modular, interface-based design ensures that as requirements evolve (Auth0, Firebase, etc.), the system can adapt without major refactoring.

---

**Questions?** Review the code comments and unit tests for implementation details.
