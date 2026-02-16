# 🔧 SECURITY FIXES - IMPLEMENTAÇÃO IMEDIATA

Este arquivo contém as correções **práticas e prontas para usar** de todas as vulnerabilidades críticas e altas identificadas no Security Audit.

---

## 🔴 FIX-001: CORS Restrito (CRÍTICO)

### Arquivo: `src/SGOFAPI.Host/Program.cs`

**SUBSTITUIR:**
```csharp
// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

**POR:**
```csharp
// ✅ CORS Configuration - SECURE
builder.Services.AddCors(options =>
{
    // Carregar origens permitidas da configuração
    var allowedOrigins = builder.Configuration
        .GetSection("AllowedOrigins")
        .Get<string[]>() ?? new[] { "https://phcapi.com" };

    options.AddPolicy("SecureCors", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowCredentials()  // ✅ Permite cookies/auth
              .WithMethods("GET", "POST", "PUT", "DELETE")
              .WithHeaders("Content-Type", "Authorization", "X-Requested-With")
              .SetIsOriginAllowedToAllowWildcardSubdomains(); // ✅ Permite subdomínios
    });
});
```

**E TROCAR na linha ~290:**
```csharp
// ANTES:
app.UseCors("AllowAll");

// DEPOIS:
app.UseCors("SecureCors");
```

### Adicionar em `appsettings.json`:
```json
{
  "AllowedOrigins": [
    "https://phcapi.com",
    "https://app.phcapi.com",
    "https://admin.phcapi.com"
  ]
}
```

### Adicionar em `appsettings.Development.json`:
```json
{
  "AllowedOrigins": [
    "https://localhost:5001",
    "http://localhost:5001",
    "http://localhost:3000",
    "https://localhost:3000"
  ]
}
```

---

## 🔴 FIX-002: Política de Senha Forte (CRÍTICO)

### Arquivo: `src/Modules/Auth/Auth.Infrastructure/DependencyInjection.cs`

**SUBSTITUIR (linha 32-38):**
```csharp
// Password settings
options.Password.RequireDigit = true;
options.Password.RequireLowercase = true;
options.Password.RequireUppercase = true;
options.Password.RequireNonAlphanumeric = false;
options.Password.RequiredLength = 6;
```

**POR:**
```csharp
// ✅ Password settings - NIST 800-63B compliant
options.Password.RequireDigit = true;
options.Password.RequireLowercase = true;
options.Password.RequireUppercase = true;
options.Password.RequireNonAlphanumeric = true;   // ✅ Requer caracteres especiais
options.Password.RequiredLength = 12;             // ✅ Mínimo 12 caracteres
options.Password.RequiredUniqueChars = 5;         // ✅ Mínimo 5 caracteres únicos
```

### BONUS: Validador de Senhas Comuns

**Criar arquivo:** `src/Modules/Auth/Auth.Infrastructure/Validators/CommonPasswordValidator.cs`

```csharp
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auth.Infrastructure.Validators;

/// <summary>
/// Valida senhas contra lista de senhas comuns e regras adicionais
/// </summary>
public class CommonPasswordValidator : IPasswordValidator<IdentityUser>
{
    private static readonly HashSet<string> CommonPasswords = new(StringComparer.OrdinalIgnoreCase)
    {
        "password", "password123", "123456", "123456789", "12345678",
        "admin", "admin123", "administrator", "root", "toor",
        "welcome", "welcome123", "letmein", "qwerty", "qwerty123",
        "abc123", "password1", "password!", "pass123", "pass@123",
        "admin@123", "admin!123", "test123", "test@123",
        "changeme", "changeme123", "default", "guest", "user",
        "sunshine", "princess", "dragon", "monkey", "iloveyou",
        // Adicione mais senhas comuns conforme necessário
    };

    public Task<IdentityResult> ValidateAsync(
        UserManager<IdentityUser> manager,
        IdentityUser user,
        string? password)
    {
        if (string.IsNullOrEmpty(password))
        {
            return Task.FromResult(IdentityResult.Failed(
                new IdentityError { Description = "Password cannot be empty" }));
        }

        // ✅ Verificar se é senha comum
        if (CommonPasswords.Contains(password))
        {
            return Task.FromResult(IdentityResult.Failed(
                new IdentityError
                {
                    Code = "CommonPassword",
                    Description = "This password is too common. Please choose a stronger, more unique password."
                }));
        }

        // ✅ Verificar se contém username
        if (!string.IsNullOrEmpty(user.UserName) &&
            password.Contains(user.UserName, StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(IdentityResult.Failed(
                new IdentityError
                {
                    Code = "PasswordContainsUsername",
                    Description = "Password cannot contain your username"
                }));
        }

        // ✅ Verificar se contém email
        if (!string.IsNullOrEmpty(user.Email) &&
            password.Contains(user.Email.Split('@')[0], StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(IdentityResult.Failed(
                new IdentityError
                {
                    Code = "PasswordContainsEmail",
                    Description = "Password cannot contain your email address"
                }));
        }

        // ✅ Verificar padrões sequenciais simples
        if (HasSequentialChars(password))
        {
            return Task.FromResult(IdentityResult.Failed(
                new IdentityError
                {
                    Code = "SequentialPattern",
                    Description = "Password cannot contain sequential patterns (e.g., 123, abc)"
                }));
        }

        return Task.FromResult(IdentityResult.Success);
    }

    private static bool HasSequentialChars(string password)
    {
        // Verificar sequências numéricas (123, 234, etc.)
        for (int i = 0; i < password.Length - 2; i++)
        {
            if (char.IsDigit(password[i]) &&
                char.IsDigit(password[i + 1]) &&
                char.IsDigit(password[i + 2]))
            {
                int a = password[i] - '0';
                int b = password[i + 1] - '0';
                int c = password[i + 2] - '0';

                if (b == a + 1 && c == b + 1) return true;  // 123, 456
                if (b == a - 1 && c == b - 1) return true;  // 321, 654
            }
        }

        // Verificar sequências alfabéticas (abc, xyz, etc.)
        for (int i = 0; i < password.Length - 2; i++)
        {
            if (char.IsLetter(password[i]) &&
                char.IsLetter(password[i + 1]) &&
                char.IsLetter(password[i + 2]))
            {
                char a = char.ToLower(password[i]);
                char b = char.ToLower(password[i + 1]);
                char c = char.ToLower(password[i + 2]);

                if (b == a + 1 && c == b + 1) return true;  // abc, def
                if (b == a - 1 && c == b - 1) return true;  // cba, fed
            }
        }

        return false;
    }
}
```

**Registrar no DependencyInjection.cs (adicionar após AddIdentity):**
```csharp
// ✅ Custom password validator
services.AddScoped<IPasswordValidator<IdentityUser>, CommonPasswordValidator>();
```

---

## 🔴 FIX-003: Rate Limiting (CRÍTICO)

### Passo 1: Instalar Pacote
```bash
dotnet add src/SGOFAPI.Host/PHCAPI.Host.csproj package AspNetCoreRateLimit
```

### Passo 2: Arquivo `src/SGOFAPI.Host/Program.cs`

**ADICIONAR após `builder.Services.AddProblemDetails();` (linha ~29):**

```csharp
// ✅ Rate Limiting Configuration
builder.Services.AddMemoryCache();
builder.Services.AddInMemoryRateLimiting();

builder.Services.Configure<IpRateLimitOptions>(options =>
{
    options.EnableEndpointRateLimiting = true;
    options.StackBlockedRequests = false;
    options.HttpStatusCode = 429; // Too Many Requests
    options.RealIpHeader = "X-Real-IP";
    options.ClientIdHeader = "X-ClientId";

    // ✅ Rate limit global
    options.GeneralRules = new List<RateLimitRule>
    {
        new RateLimitRule
        {
            Endpoint = "*",
            Period = "1m",
            Limit = 100  // 100 requisições por minuto por IP
        },
        new RateLimitRule
        {
            Endpoint = "*",
            Period = "1h",
            Limit = 1000  // 1000 requisições por hora por IP
        },
        // ✅ Rate limit crítico: API Login
        new RateLimitRule
        {
            Endpoint = "POST:/api/auth/login",
            Period = "1m",
            Limit = 5  // Apenas 5 tentativas por minuto
        },
        new RateLimitRule
        {
            Endpoint = "POST:/api/auth/login",
            Period = "1h",
            Limit = 20  // 20 tentativas por hora
        },
        // ✅ Rate limit: Admin UI Login
        new RateLimitRule
        {
            Endpoint = "POST:/Admin/Account/Login",
            Period = "5m",
            Limit = 10  // 10 tentativas em 5 minutos
        },
        new RateLimitRule
        {
            Endpoint = "POST:/Admin/Account/Login",
            Period = "1h",
            Limit = 30
        },
        // ✅ Rate limit: Endpoints de escrita
        new RateLimitRule
        {
            Endpoint = "POST:*",
            Period = "1m",
            Limit = 30
        },
        new RateLimitRule
        {
            Endpoint = "PUT:*",
            Period = "1m",
            Limit = 30
        },
        new RateLimitRule
        {
            Endpoint = "DELETE:*",
            Period = "1m",
            Limit = 10
        }
    };

    options.QuotaExceededResponse = new QuotaExceededResponse
    {
        Content = "{{ \"error\": \"Too many requests. Please try again later.\", \"retryAfter\": \"{0}\" }}",
        ContentType = "application/json",
        StatusCode = 429
    };
});

builder.Services.Configure<IpRateLimitPolicies>(options =>
{
    options.IpRules = new List<IpRateLimitPolicy>();
});

builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
```

**ADICIONAR Middleware (após app.UseRouting(), linha ~288):**

```csharp
app.UseRouting();

// ✅ Rate Limiting Middleware (deve vir ANTES de UseCors)
app.UseIpRateLimiting();

app.UseCors("SecureCors");
```

### Passo 3: Ativar Lockout no Admin Login

**Arquivo:** `src/Admin.UI/Pages/Account/Login.cshtml.cs`

**SUBSTITUIR (linha 63):**
```csharp
var result = await _signInManager.PasswordSignInAsync(
    Input.Username, 
    Input.Password, 
    Input.RememberMe, 
    lockoutOnFailure: false);
```

**POR:**
```csharp
var result = await _signInManager.PasswordSignInAsync(
    Input.Username, 
    Input.Password, 
    Input.RememberMe, 
    lockoutOnFailure: true);  // ✅ ATIVAR Lockout!
```

**E ADICIONAR tratamento (após linha 71):**
```csharp
if (result.Succeeded)
{
    _logger.LogInformation("User {Username} logged in from IP {IP}.", 
        Input.Username, 
        HttpContext.Connection.RemoteIpAddress);
    return LocalRedirect(returnUrl);
}
if (result.RequiresTwoFactor)
{
    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, Input.RememberMe });
}
if (result.IsLockedOut)
{
    _logger.LogWarning(
        "Account {Username} locked out at {Time} from IP {IP}. Lockout expires at {Expiry}.", 
        Input.Username, 
        DateTime.UtcNow,
        HttpContext.Connection.RemoteIpAddress,
        DateTime.UtcNow.AddMinutes(5));
    
    ErrorMessage = "⚠️ Conta bloqueada devido a múltiplas tentativas falhas. Tente novamente em 5 minutos.";
    return Page();
}
else
{
    _logger.LogWarning(
        "Failed login attempt for {Username} from IP {IP}", 
        Input.Username, 
        HttpContext.Connection.RemoteIpAddress);
    
    ErrorMessage = "❌ Credenciais inválidas. Verifique o nome de utilizador e password.";
}
```

---

## 🟠 FIX-004: Security Headers (ALTA)

### Arquivo: `src/SGOFAPI.Host/Program.cs`

**ADICIONAR após `var app = builder.Build();` (linha ~217):**

```csharp
var app = builder.Build();

// ✅ Security Headers Middleware (DEVE vir no início)
app.Use(async (context, next) =>
{
    // ✅ HSTS: Força HTTPS por 1 ano (apenas se não estiver em desenvolvimento)
    if (!app.Environment.IsDevelopment())
    {
        context.Response.Headers.Append(
            "Strict-Transport-Security",
            "max-age=31536000; includeSubDomains; preload");
    }

    // ✅ Clickjacking Protection
    context.Response.Headers.Append("X-Frame-Options", "DENY");

    // ✅ MIME Sniffing Protection
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

    // ✅ XSS Protection (legacy browsers support)
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");

    // ✅ Content Security Policy
    context.Response.Headers.Append(
        "Content-Security-Policy",
        "default-src 'self'; " +
        "script-src 'self' 'unsafe-inline' 'unsafe-eval' https://cdn.jsdelivr.net https://unpkg.com https://cdnjs.cloudflare.com; " +
        "style-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net https://fonts.googleapis.com https://cdnjs.cloudflare.com; " +
        "font-src 'self' data: https://fonts.gstatic.com https://cdn.jsdelivr.net https://cdnjs.cloudflare.com; " +
        "img-src 'self' data: https:; " +
        "connect-src 'self'; " +
        "frame-ancestors 'none'; " +
        "base-uri 'self'; " +
        "form-action 'self'");

    // ✅ Referrer Policy
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");

    // ✅ Permissions Policy (disable dangerous browser features)
    context.Response.Headers.Append(
        "Permissions-Policy",
        "geolocation=(), microphone=(), camera=(), payment=(), usb=(), magnetometer=(), gyroscope=()");

    await next();
});

// ✅ Remove headers que expõem informação sensível
app.Use(async (context, next) =>
{
    context.Response.Headers.Remove("Server");  // Remove "Kestrel"
    context.Response.Headers.Remove("X-Powered-By");  // Remove "ASP.NET"
    context.Response.Headers.Remove("X-AspNet-Version");
    context.Response.Headers.Remove("X-AspNetMvc-Version");
    await next();
});
```

---

## 🟠 FIX-005: JWT Secret Seguro (ALTA)

### Opção 1: User Secrets (Desenvolvimento)

```bash
# Execute na raiz do projeto Host
cd src/SGOFAPI.Host
dotnet user-secrets init
dotnet user-secrets set "JWT:Secret" "$(openssl rand -base64 64)"
dotnet user-secrets set "JWT:ValidIssuer" "https://phcapi.com"
dotnet user-secrets set "JWT:ValidAudience" "https://phcapi.com"
```

### Opção 2: Azure Key Vault (Produção)

**Passo 1: Instalar pacotes**
```bash
dotnet add src/SGOFAPI.Host/PHCAPI.Host.csproj package Azure.Identity
dotnet add src/SGOFAPI.Host/PHCAPI.Host.csproj package Azure.Extensions.AspNetCore.Configuration.Secrets
```

**Passo 2: Modificar Program.cs**

```csharp
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

// ✅ Carregar secrets do Azure Key Vault em produção
if (builder.Environment.IsProduction())
{
    var keyVaultUri = builder.Configuration["KeyVaultUri"];
    if (!string.IsNullOrEmpty(keyVaultUri))
    {
        builder.Configuration.AddAzureKeyVault(
            new Uri(keyVaultUri),
            new DefaultAzureCredential());
    }
}
else if (builder.Environment.IsDevelopment())
{
    // ✅ Usar User Secrets em desenvolvimento
    builder.Configuration.AddUserSecrets<Program>();
}
```

**Passo 3: Validar JWT Secret**

**Arquivo:** `src/Modules/Auth/Auth.Infrastructure/DependencyInjection.cs`

**SUBSTITUIR (linha ~86):**
```csharp
IssuerSigningKey = new SymmetricSecurityKey(
    Encoding.UTF8.GetBytes(configuration["JWT:Secret"] ?? 
        throw new InvalidOperationException("JWT:Secret not configured")))
```

**POR:**
```csharp
IssuerSigningKey = new SymmetricSecurityKey(
    Encoding.UTF8.GetBytes(GetValidatedJwtSecret(configuration)))
```

**E ADICIONAR método helper no final da classe:**
```csharp
private static string GetValidatedJwtSecret(IConfiguration configuration)
{
    var secret = configuration["JWT:Secret"];
    
    if (string.IsNullOrWhiteSpace(secret))
    {
        throw new InvalidOperationException(
            "JWT:Secret is not configured. " +
            "Please configure it in User Secrets (dev) or Azure Key Vault (prod).");
    }
    
    // ✅ NIST recommendation: minimum 256 bits (32 bytes = 32 characters)
    if (secret.Length < 32)
    {
        throw new InvalidOperationException(
            $"JWT:Secret must be at least 32 characters long for security. Current length: {secret.Length}");
    }
    
    return secret;
}
```

### Opção 3: Variáveis de Ambiente (Simples)

**Linux/Mac:**
```bash
export JWT__Secret="$(openssl rand -base64 64)"
export JWT__ValidIssuer="https://phcapi.com"
export JWT__ValidAudience="https://phcapi.com"
```

**Windows (PowerShell):**
```powershell
$secret = [Convert]::ToBase64String((1..64 | ForEach-Object { Get-Random -Minimum 0 -Maximum 256 }))
[Environment]::SetEnvironmentVariable("JWT__Secret", $secret, "Machine")
[Environment]::SetEnvironmentVariable("JWT__ValidIssuer", "https://phcapi.com", "Machine")
[Environment]::SetEnvironmentVariable("JWT__ValidAudience", "https://phcapi.com", "Machine")
```

---

## 🟠 FIX-006: CSRF Protection (ALTA)

### Validar que está ativo

**Arquivo:** `src/Admin.UI/DependencyInjection.cs`

**ADICIONAR após linha 22:**

```csharp
services.AddRazorPages(options =>
{
    // ✅ CSRF Protection explícito (já ativo por padrão, mas garantir)
    options.Conventions.ConfigureFilter(new AutoValidateAntiforgeryTokenAttribute());
    
    // Require authentication for all admin pages (except login/logout/access denied)
    options.Conventions.AuthorizeFolder("/Users", "AdminOnly");
    // ... resto do código
});

// ✅ Adicionar Antiforgery service com configuração segura
services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
    options.Cookie.Name = "__Host-X-CSRF-TOKEN";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;  // ✅ Apenas HTTPS
    options.Cookie.SameSite = SameSiteMode.Strict;  // ✅ Proteção adicional
});
```

### Verificar em todas as páginas `.cshtml`

**Todas as páginas devem ter:**
```html
<form method="post">
    @* ✅ ASP.NET Core adiciona token automaticamente quando method="post" *@
    @* Mas pode verificar visualmente com: *@
    @Html.AntiForgeryToken()
    
    <!-- restante do form -->
</form>
```

---

## 🟠 FIX-007: Hangfire Dashboard Seguro (ALTA)

### Arquivo: `src/SGOFAPI.Host/Middleware/HangfireAuthorizationFilter.cs`

**CRIAR ou SUBSTITUIR:**

```csharp
using Hangfire.Dashboard;
using Microsoft.Extensions.Logging;

namespace PHCAPI.Host.Middleware;

/// <summary>
/// Filtro de autorização para Hangfire Dashboard
/// Requer autenticação + role Administrator
/// </summary>
public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        
        // ✅ 1. Verificar autenticação
        if (!httpContext.User.Identity?.IsAuthenticated ?? true)
        {
            LogUnauthorizedAccess(httpContext, "Not authenticated");
            return false;
        }
        
        // ✅ 2. Verificar role Administrator
        if (!httpContext.User.IsInRole("Administrator"))
        {
            LogUnauthorizedAccess(httpContext, "Not in Administrator role");
            return false;
        }
        
        // ✅ 3. Log de acesso autorizado
        var logger = httpContext.RequestServices
            .GetRequiredService<ILogger<HangfireAuthorizationFilter>>();
        
        logger.LogWarning(
            "🔐 Hangfire Dashboard accessed by {User} (ID: {UserId}) from IP {IP}",
            httpContext.User.Identity.Name ?? "Unknown",
            httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "Unknown",
            httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown");
        
        return true;
    }
    
    private static void LogUnauthorizedAccess(Microsoft.AspNetCore.Http.HttpContext httpContext, string reason)
    {
        var logger = httpContext.RequestServices
            .GetRequiredService<ILogger<HangfireAuthorizationFilter>>();
        
        logger.LogWarning(
            "❌ Unauthorized Hangfire Dashboard access attempt: {Reason}. IP: {IP}, User: {User}",
            reason,
            httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
            httpContext.User.Identity?.Name ?? "Anonymous");
    }
}
```

### E RESTRINGIR apenas para Development:

**Arquivo:** `src/SGOFAPI.Host/Program.cs` (linha ~263)

```csharp
// ✅ Hangfire Dashboard APENAS em Development (ou com IP whitelist em produção)
if (app.Environment.IsDevelopment())
{
    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = new[] { new HangfireAuthorizationFilter() },
        StatsPollingInterval = 5000,
        DisplayStorageConnectionString = false,
        DashboardTitle = "PHCAPI - Background Jobs [DEV ONLY]",
        IsReadOnlyFunc = (DashboardContext context) => false  // ✅ Apenas admins podem modificar
    });
}
else if (app.Environment.IsProduction())
{
    // ⚠️ Em produção: considerar desabilitar ou usar IP whitelist
    // app.UseHangfireDashboard("/hangfire", new DashboardOptions
    // {
    //     Authorization = new[] { new HangfireAuthorizationFilter(), new IpWhitelistFilter() },
    //     // ...
    // });
}
```

---

## ✅ CHECKLIST DE IMPLEMENTAÇÃO

Marque conforme implementar:

### Fase 1 - CRÍTICO (HOJE)
- [ ] FIX-001: CORS Restrito
- [ ] FIX-002: Política de Senha Forte
- [ ] FIX-003: Rate Limiting + Lockout

### Fase 2 - ALTA (ESTA SEMANA)
- [ ] FIX-004: Security Headers
- [ ] FIX-005: JWT Secret Seguro
- [ ] FIX-006: CSRF Protection validado
- [ ] FIX-007: Hangfire Dashboard seguro

### Validação
- [ ] Compilar aplicação sem erros
- [ ] Testar login com senha fraca (deve rejeitar)
- [ ] Testar brute force (deve bloquear após 5 tentativas)
- [ ] Testar CORS de origem não permitida (deve rejeitar)
- [ ] Verificar headers HTTP com curl/Postman
- [ ] Testar Hangfire Dashboard sem autenticação (deve redirecionar)

---

## 🔬 TESTE RÁPIDO APÓS IMPLEMENTAÇÃO

```bash
# 1. Testar Rate Limiting
for i in {1..10}; do
    curl -X POST https://localhost:5001/api/auth/login \
         -H "Content-Type: application/json" \
         -d '{"username":"test","password":"test"}'
done
# ✅ Deve retornar 429 após 5 tentativas

# 2. Testar CORS
curl -H "Origin: https://evil.com" \
     -H "Access-Control-Request-Method: POST" \
     -H "Access-Control-Request-Headers: Content-Type" \
     -X OPTIONS https://localhost:5001/api/auth/login
# ✅ Não deve retornar Access-Control-Allow-Origin para evil.com

# 3. Testar Security Headers
curl -I https://localhost:5001
# ✅ Deve retornar todos os security headers
```

---

## 📝 NOTAS FINAIS

**Após implementar todas as correções:**
1. Fazer commit em branch separado
2. Testar localmente
3. Realizar code review
4. Deploy em ambiente de staging primeiro
5. Validar com novo pentest
6. Deploy em produção

**Dúvidas?** Consulte o `SECURITY-AUDIT-REPORT.md` para detalhes técnicos de cada vulnerabilidade.
