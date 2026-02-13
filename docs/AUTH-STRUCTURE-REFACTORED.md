# Auth Module - Estrutura Corrigida ✅

## Problema Identificado

A implementação inicial do módulo Auth **não seguia a estrutura dos outros módulos** (Audit, Parameters):

❌ **Antes**: Usava `Services` diretos injetados no Controller
❌ **Antes**: Não usava MediatR/CQRS
❌ **Antes**: Estrutura diferente dos outros módulos

## Solução Implementada

Refatorei completamente o módulo para seguir **exatamente o mesmo padrão** usado em Audit e Parameters:

✅ **Agora**: Controller injeta apenas `IMediator`
✅ **Agora**: Usa MediatR com CQRS (Commands & Queries)
✅ **Agora**: Estrutura idêntica aos outros módulos

## Estrutura Final

```
src/Modules/Auth/
├── Auth.Domain/
│   ├── Interfaces/              # Contratos de repository e serviços
│   │   ├── IAuthenticationService.cs
│   │   ├── ITokenService.cs
│   │   ├── IUserRepository.cs
│   │   └── IRoleRepository.cs
│   └── Constants.cs             # (Movido para Shared.Kernel/Authorization)
│
├── Auth.Application/
│   ├── Features/                # ✅ CQRS com MediatR
│   │   ├── Login/
│   │   │   ├── LoginCommand.cs
│   │   │   └── LoginCommandHandler.cs
│   │   ├── Register/
│   │   │   ├── RegisterCommand.cs
│   │   │   └── RegisterCommandHandler.cs
│   │   ├── CreateRole/
│   │   │   ├── CreateRoleCommand.cs
│   │   │   └── CreateRoleCommandHandler.cs
│   │   ├── AddUserToRole/
│   │   │   ├── AddUserToRoleCommand.cs
│   │   │   └── AddUserToRoleCommandHandler.cs
│   │   ├── GetAllRoles/
│   │   │   ├── GetAllRolesQuery.cs
│   │   │   └── GetAllRolesQueryHandler.cs
│   │   └── GetUserRoles/
│   │       ├── GetUserRolesQuery.cs
│   │       └── GetUserRolesQueryHandler.cs
│   ├── DTOs/
│   │   ├── LoginDto.cs
│   │   ├── RegisterDto.cs
│   │   └── RoleDto.cs
│   └── DependencyInjection.cs
│
├── Auth.Infrastructure/
│   ├── Persistence/
│   │   └── AuthDbContext.cs
│   ├── Repositories/
│   │   ├── IdentityUserRepository.cs
│   │   └── IdentityRoleRepository.cs
│   ├── Services/
│   │   ├── JwtTokenService.cs
│   │   └── IdentityAuthenticationService.cs
│   ├── Migrations/
│   │   └── 20260213_InitialAuthMigration.cs
│   ├── Data/
│   │   └── AuthDbContextSeed.cs
│   └── DependencyInjection.cs
│
└── Auth.Presentation/
    ├── Controllers/
    │   └── AuthenticateController.cs  # ✅ Usa apenas IMediator
    └── DependencyInjection.cs
```

## Comparação: Antes vs Depois

### ANTES (❌ Incorreto)

```csharp
// Controller
public class AuthenticateController : ControllerBase
{
    private readonly IAuthService _authService;  // ❌ Service direto
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
    {
        var result = await _authService.LoginAsync(model);  // ❌ Service
        return Ok(result);
    }
}

// Application tinha Services/
Auth.Application/
├── Services/
│   └── AuthService.cs      // ❌ Service pattern
└── Interfaces/
    └── IAuthService.cs     // ❌ Não usa MediatR
```

### DEPOIS (✅ Correto)

```csharp
// Controller
public class AuthenticateController : ControllerBase
{
    private readonly IMediator _mediator;  // ✅ Apenas MediatR
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto model, CancellationToken ct)
    {
        var command = new LoginCommand(model.Username, model.Password);  // ✅
        var result = await _mediator.Send(command, ct);  // ✅ MediatR
        return Ok(result);
    }
}

// Application tem Features/
Auth.Application/
└── Features/               // ✅ CQRS pattern
    ├── Login/
    │   ├── LoginCommand.cs
    │   └── LoginCommandHandler.cs
    └── ...
```

## Padrão CQRS Implementado

### Commands (Write Operations)

1. **LoginCommand** → Autenticar usuário
2. **RegisterCommand** → Registrar novo usuário
3. **CreateRoleCommand** → Criar role
4. **AddUserToRoleCommand** → Adicionar usuário a role

### Queries (Read Operations)

1. **GetAllRolesQuery** → Listar todas as roles
2. **GetUserRolesQuery** → Ver roles de um usuário

### Exemplo de Handler

```csharp
using MediatR;
using Auth.Application.DTOs;
using Auth.Domain.Interfaces;

namespace Auth.Application.Features.Login;

public sealed class LoginCommandHandler 
    : IRequestHandler<LoginCommand, LoginResponseDto>
{
    private readonly IAuthenticationService _authenticationService;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        IAuthenticationService authenticationService,
        ILogger<LoginCommandHandler> logger)
    {
        _authenticationService = authenticationService;
        _logger = logger;
    }

    public async Task<LoginResponseDto> Handle(
        LoginCommand request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authenticationService.LoginAsync(
                request.Username, 
                request.Password, 
                cancellationToken);

            return new LoginResponseDto
            {
                Token = result.Token,
                Expiration = result.Expiration,
                Allowed = result.Success,
                OutputResponse = result.Success 
                    ? AuthMessages.Authenticated 
                    : AuthMessages.BadCredentials,
                Roles = result.Roles
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for user {Username}", request.Username);
            return new LoginResponseDto
            {
                Token = string.Empty,
                Expiration = null,
                Allowed = false,
                OutputResponse = AuthMessages.InternalError
            };
        }
    }
}
```

## Registro de Dependências

### Auth.Presentation/DependencyInjection.cs

```csharp
public static IServiceCollection AddAuthPresentation(
    this IServiceCollection services,
    IConfiguration configuration)
{
    // Application Layer
    services.AddAuthApplication();

    // ✅ MediatR - registra todos os handlers
    services.AddMediatR(cfg =>
    {
        cfg.RegisterServicesFromAssembly(
            typeof(Auth.Application.Features.Login.LoginCommand).Assembly);
    });

    // Infrastructure Layer
    services.AddAuthInfrastructure(configuration);

    return services;
}
```

### Program.cs

```csharp
// ✅ Registro simples no Host
builder.Services.AddAuthPresentation(builder.Configuration);
```

## Benefícios da Nova Estrutura

1. **Consistência**: Mesma estrutura que Audit e Parameters
2. **Manutenibilidade**: Código organizado por feature
3. **Testabilidade**: Handlers fáceis de testar isoladamente
4. **Escalabilidade**: Fácil adicionar novas features
5. **Separation of Concerns**: Cada handler tem uma responsabilidade

## Features Implementadas

| Feature | Type | Descrição | Policy |
|---------|------|-----------|--------|
| Login | Command | Autenticar usuário e retornar JWT | AnonymousAllowed |
| Register | Command | Criar novo usuário | AdminOnly |
| CreateRole | Command | Criar nova role | AdminOnly |
| AddUserToRole | Command | Adicionar usuário a role | AdminOnly |
| GetAllRoles | Query | Listar todas as roles | AdminOnly |
| GetUserRoles | Query | Ver roles de um usuário | AdminOnly |

## Authorization Policies

As policies foram movidas para `Shared.Kernel/Authorization/`:

```csharp
// Shared.Kernel/Authorization/AuthorizationConstants.cs
public static class AppRoles
{
    public const string Administrator = "Administrator";
    public const string InternalUser = "InternalUser";
    public const string ApiUser = "ApiUser";
    public const string ExternalStakeholder = "ExternalStakeholder";
    public const string AuditViewer = "AuditViewer";
}

public static class AppPolicies
{
    public const string InternalOnly = "InternalOnly";
    public const string AdminOnly = "AdminOnly";
    public const string ApiAccess = "ApiAccess";
    public const string Authenticated = "Authenticated";
}
```

## Módulo Audit Protegido

O módulo Audit agora está protegido:

```csharp
[ApiController]
[Route("api/audit")]
[Authorize(Policy = AppPolicies.InternalOnly)]  // ✅ Protegido
public sealed class AuditController : ControllerBase
{
    // Apenas Administrator, InternalUser, AuditViewer podem acessar
}
```

## Como Usar

### 1. Aplicar Migrations

```powershell
cd src\SGOFAPI.Host
dotnet ef database update --context AuthDbContext --project ..\Modules\Auth\Auth.Infrastructure\Auth.Infrastructure.csproj
```

### 2. Rodar Aplicação

```powershell
dotnet run --project src\SGOFAPI.Host
```

O seed cria automaticamente:
- ✅ Roles (Administrator, InternalUser, etc.)
- ✅ Admin user (username: `admin`, senha: `Admin@123`)

### 3. Fazer Login

```bash
POST /api/authenticate/login
{
  "username": "admin",
  "password": "Admin@123"
}
```

### 4. Usar Token

```bash
GET /api/audit
Authorization: Bearer {token}
```

## Arquivos Removidos

Removidos para seguir o padrão correto:

❌ `Auth.Application/Services/AuthService.cs`
❌ `Auth.Application/Interfaces/IAuthService.cs`

Substituídos por:

✅ `Auth.Application/Features/**/*Command.cs`
✅ `Auth.Application/Features/**/*CommandHandler.cs`
✅ `Auth.Application/Features/**/*Query.cs`
✅ `Auth.Application/Features/**/*QueryHandler.cs`

## Documentação

- [README do Módulo](../src/Modules/Auth/README.md)
- [Guia de Implementação](./AUTH-MODULE-IMPLEMENTATION.md)
- [Quick Start](./AUTH-QUICK-START.md)

---

**Status**: ✅ Estrutura corrigida e alinhada com outros módulos
**Padrão**: CQRS com MediatR (igual Audit e Parameters)
**Data**: 13 de Fevereiro de 2026
