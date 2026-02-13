# Authentication & Authorization - Quick Start Guide

## 🚀 Setup (5 minutos)

### 1. Aplicar Migrations

```powershell
cd src\SGOFAPI.Host
dotnet ef database update --context AuthDbContext --project ..\Modules\Auth\Auth.Infrastructure\Auth.Infrastructure.csproj
```

### 2. Criar Usuário Administrador (via Swagger ou Postman)

Como não há usuários iniciais, você precisará criar um diretamente no banco de dados ou criar um seed:

#### Opção A: Seed Inicial (Recomendado)

Crie um arquivo `src/Modules/Auth/Auth.Infrastructure/Data/AuthDbContextSeed.cs`:

```csharp
public static class AuthDbContextSeed
{
    public static async Task SeedAsync(UserManager<IdentityUser> userManager, 
                                        RoleManager<IdentityRole> roleManager)
    {
        // Criar roles
        await CreateRoleIfNotExists(roleManager, "Administrator");
        await CreateRoleIfNotExists(roleManager, "InternalUser");
        await CreateRoleIfNotExists(roleManager, "ApiUser");
        await CreateRoleIfNotExists(roleManager, "AuditViewer");
        
        // Criar admin user
        if (await userManager.FindByNameAsync("admin") == null)
        {
            var adminUser = new IdentityUser
            {
                UserName = "admin",
                Email = "admin@phcapi.local",
                EmailConfirmed = true
            };
            
            var result = await userManager.CreateAsync(adminUser, "Admin@123");
            
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Administrator");
            }
        }
    }
    
    private static async Task CreateRoleIfNotExists(RoleManager<IdentityRole> roleManager, string roleName)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}
```

Chamar no `Program.cs` após `app.Build()`:

```csharp
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await AuthDbContextSeed.SeedAsync(userManager, roleManager);
}
```

#### Opção B: SQL Direto (Rápido teste)

Execute no SQL Server Management Studio:

```sql
-- Inserir Roles
INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
VALUES 
    (NEWID(), 'Administrator', 'ADMINISTRATOR', NEWID()),
    (NEWID(), 'InternalUser', 'INTERNALUSER', NEWID()),
    (NEWID(), 'ApiUser', 'APIUSER', NEWID()),
    (NEWID(), 'AuditViewer', 'AUDITVIEWER', NEWID());

-- Inserir User Admin (senha: Admin@123)
-- Gerar hash de senha usando ASP.NET Identity hasher ou use temporariamente outra ferramenta
```

⚠️ **Nota**: A opção SQL requer gerar o hash de senha manualmente. Use a Opção A.

### 3. Testar Autenticação

#### Fazer Login

**Request:**
```http
POST http://localhost:7298/api/authenticate/login
Content-Type: application/json

{
  "username": "admin",
  "password": "Admin@123"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiration": "2026-02-13T17:00:00Z",
  "allowed": true,
  "outputResponse": "AUTHENTICATED",
  "roles": ["Administrator"]
}
```

#### Usar Token

Copie o token e adicione nos headers das próximas requisições:

```http
GET http://localhost:7298/api/audit
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## 📋 Comandos Úteis

### Criar Nova Role

```http
POST http://localhost:7298/api/authenticate/roles
Authorization: Bearer {admin-token}
Content-Type: application/json

{
  "roleName": "CustomRole"
}
```

### Criar Novo Usuário

```http
POST http://localhost:7298/api/authenticate/register
Authorization: Bearer {admin-token}
Content-Type: application/json

{
  "username": "newuser",
  "email": "newuser@phcapi.local",
  "password": "SecureP@ssw0rd123"
}
```

### Adicionar Usuário a Role

```http
POST http://localhost:7298/api/authenticate/users/add-role
Authorization: Bearer {admin-token}
Content-Type: application/json

{
  "username": "newuser",
  "role": "InternalUser"
}
```

### Ver Roles de um Usuário

```http
GET http://localhost:7298/api/authenticate/users/newuser/roles
Authorization: Bearer {admin-token}
```

## 🔐 Políticas de Autorização

### Proteger um Endpoint

```csharp
using Microsoft.AspNetCore.Authorization;
using Shared.Kernel.Authorization;

[ApiController]
[Route("api/mycontroller")]
public class MyController : ControllerBase
{
    // Apenas usuários internos
    [HttpGet("internal")]
    [Authorize(Policy = AppPolicies.InternalOnly)]
    public IActionResult InternalData() { }
    
    // Apenas administradores
    [HttpPost("admin")]
    [Authorize(Policy = AppPolicies.AdminOnly)]
    public IActionResult AdminAction() { }
    
    // Qualquer usuário autenticado
    [HttpGet("public")]
    [Authorize(Policy = AppPolicies.Authenticated)]
    public IActionResult PublicData() { }
}
```

### Políticas Disponíveis

| Policy | Descrição | Roles Permitidas |
|--------|-----------|------------------|
| `AppPolicies.InternalOnly` | Acesso interno apenas | Administrator, InternalUser, AuditViewer |
| `AppPolicies.AdminOnly` | Apenas administradores | Administrator |
| `AppPolicies.ApiAccess` | Acesso de API | Administrator, ApiUser, InternalUser |
| `AppPolicies.Authenticated` | Qualquer autenticado | Qualquer role |

## 🎯 Módulos Protegidos

### Audit Module

O módulo Audit está protegido com `AppPolicies.InternalOnly`:

```csharp
[Authorize(Policy = AppPolicies.InternalOnly)]
public sealed class AuditController : ControllerBase
```

**Acesso permitido para:**
- Administrator
- InternalUser  
- AuditViewer

**Acesso negado para:**
- ApiUser (usuários externos da API)
- ExternalStakeholder
- Usuários não autenticados

## 🔧 Configuração

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=...;..."
  },
  "JWT": {
    "ValidAudience": "http://localhost:7298",
    "ValidIssuer": "http://localhost:7298",
    "Secret": "JWTAuthenticationHIGHsecuredPasswordVVIWzyLROEoXVp1OH7Xzyr",
    "ExpirationHours": 3
  }
}
```

⚠️ **Produção**: Altere o `Secret` para uma chave mais segura e armazene em variáveis de ambiente ou Azure Key Vault.

## 🧪 Testar no Swagger

1. Executar aplicação: `dotnet run --project src/SGOFAPI.Host`
2. Abrir Swagger: `http://localhost:7298`
3. Fazer login em `/api/authenticate/login`
4. Copiar o token da resposta
5. Clicar no botão **"Authorize"** no topo do Swagger
6. Inserir: `Bearer {seu-token}`
7. Clicar "Authorize"
8. Agora você pode testar endpoints protegidos

## ❓ Troubleshooting

### Erro: "No authentication handler is configured"

**Solução**: Verificar ordem dos middlewares em `Program.cs`:
```csharp
app.UseAuthentication();  // ANTES
app.UseAuthorization();   // DEPOIS
```

### Erro: "401 Unauthorized" com token válido

**Possíveis causas**:
1. Token expirado (3 horas por padrão)
2. Configuração JWT não bate (Secret, Issuer, Audience)
3. Usuário não tem a role necessária para a policy

**Debug**:
```csharp
// Ver claims do token atual
var claims = User.Claims.Select(c => new { c.Type, c.Value });
```

### Erro: "A network-related error occurred"

**Solução**: Verificar connection string em `appsettings.json`

### Erro: "Cannot insert NULL into AspNetUsers.UserName"

**Solução**: Username é obrigatório no registro.

## 📚 Documentação Completa

- [README do Módulo](../src/Modules/Auth/README.md)
- [Guia de Implementação](./AUTH-MODULE-IMPLEMENTATION.md)

## 🎓 Próximos Passos

1. ✅ Implementar seed de usuários iniciais
2. ✅ Testar autenticação e autorização
3. ⏭️ Adicionar refresh tokens (opcional)
4. ⏭️ Implementar rate limiting no login (proteção brute-force)
5. ⏭️ Adicionar logging de tentativas de login
6. ⏭️ Configurar Auth0 ou Firebase (se necessário)

---

**Dúvidas?** Consulte a documentação completa ou a equipe de desenvolvimento.
