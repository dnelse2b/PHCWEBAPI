# 🔍 Admin UI Logging: O que é capturado?

## ✅ **ATUALIZADO: 16/02/2026 - Auditoria Completa Implementada**

### 🎉 Problema RESOLVIDO!

As operações do Admin.UI (login, criação de roles, gestão de usuários) **agora são totalmente auditadas** na tabela `AuditLogs` com todos os dados necessários para fiscalização:

#### ✅ Dados Capturados em Auditoria:
- **IP Address** do cliente
- **User Agent** (navegador/dispositivo)
- **Username** do usuário autenticado
- **User ID** e **Roles**
- **Ação específica** (Login, Create User, Edit Role, etc.)
- **Request data** (form data sanitizado - passwords são ocultados)
- **Response status** e código HTTP
- **Timestamp** UTC
- **Correlation ID** para rastreabilidade
- **Referer** (página de origem)

#### 📋 Operações Auditadas:
- ✅ Login/Logout (POST)
- ✅ Criação, edição, exclusão de Usuários (POST/PUT/DELETE)
- ✅ Criação, edição, exclusão de Roles (POST/PUT/DELETE)
- ✅ Atribuição de permissões a Roles (POST)
- ✅ Visualização de listagens de Users e Roles (GET)
- ✅ Visualização de detalhes de Users e Roles (GET)

#### 🔐 Segurança:
- Passwords são automaticamente **redacted** (***REDACTED***)
- Dados sensíveis não são armazenados em texto plano

#### 📂 Verificar Auditoria (SQL):
```sql
-- Ver últimas operações do Admin.UI
SELECT TOP 50 
    Action,
    IpAddress,
    UserAgent,
    StatusCode,
    Timestamp
FROM AuditLogs
WHERE Action LIKE '%Admin UI%'
ORDER BY Timestamp DESC;
```

---

## 📚 Documentação Histórica (Antes da correção - 16/02/2026)

## ❌ Problema Identificado: Razor Pages NÃO são logadas

### 📍 Código Crítico

**Arquivo:** `src/SGOFAPI.Host/Middleware/ResponseLoggingMiddleware.cs`

```csharp
public async Task InvokeAsync(HttpContext context)
{
    // ✅ Log básico é sempre feito
    _logger.LogInformation("[MIDDLEWARE START] {Method} {Path}", method, path);
    
    try
    {
        await _next(context);
        await ProcessSuccessfulResponseAsync(context, requestBody, responseBody, originalBodyStream);
    }
    finally
    {
        // ✅ Log básico é sempre feito
        _logger.LogInformation("[MIDDLEWARE END] {Method} {Path} - Status: {StatusCode}", 
            method, path, context.Response.StatusCode);
    }
}

private async Task ProcessSuccessfulResponseAsync(...)
{
    var responseText = await ReadResponseTextAsync(responseBody);
    await responseBody.CopyToAsync(originalBodyStream);

    // ❌ AQUI ESTÁ O PROBLEMA!
    if (string.IsNullOrWhiteSpace(responseText) || !IsJsonResponse(context)) 
        return;  // ← Razor Pages retornam HTML, então SALTA AUDITORIA

    // 📝 Só chega aqui se for JSON (APIs)
    await LogAuditAsync(context, requestBody, responseDto, auditJson);
}

private static bool IsJsonResponse(HttpContext context) =>
    context.Response.ContentType?.Contains("application/json", ...) ?? false;
    // ← Razor Pages retornam "text/html", então isto é FALSE
```

---

## 📊 Comparação: O que é logado?

### ✅ **Requisições API (JSON)** - TOTALMENTE LOGADAS

```http
POST /api/auth/login
Content-Type: application/json
{
  "username": "admin@phcapi.com",
  "password": "Admin@123"
}
```

**Logs Gerados:**

```
[Information] [MIDDLEWARE START] POST /api/auth/login
[Information] Executing endpoint 'AuthController.Login'
[Information] [MIDDLEWARE END] POST /api/auth/login - Status: 200

// ✅ PLUS: Auditoria completa na tabela AuditLogs
┌───────────────────────────────────────────────────────────────┐
│ AuditLog Table                                                 │
├───────────────────────────────────────────────────────────────┤
│ Action: POST /api/auth/login                                   │
│ StatusCode: 200                                                │
│ RequestBody: {"username":"admin@phcapi.com",...}              │
│ ResponseBody: {"token":"eyJhbGc...", "expiresIn":10800}       │
│ IpAddress: 192.168.1.100                                       │
│ Timestamp: 2026-02-16 12:30:45                                 │
└───────────────────────────────────────────────────────────────┘
```

---

### ⚠️ **Requisições Razor Pages (HTML)** - APENAS LOG BÁSICO

```http
POST /Admin/Account/Login
Content-Type: application/x-www-form-urlencoded

Input.Username=admin@phcapi.com&Input.Password=Admin@123
```

**Logs Gerados:**

```
[Information] [MIDDLEWARE START] POST /Admin/Account/Login
[Information] Executing endpoint 'Admin.UI.Pages.Account.LoginModel.OnPostAsync'
[Information] User admin@phcapi.com logged in.
[Information] [MIDDLEWARE END] POST /Admin/Account/Login - Status: 302

// ❌ NÃO É CRIADO REGISTO NA TABELA AuditLogs
// Porque Content-Type = "text/html", não "application/json"
```

**Motivo:** O middleware faz early return antes de chamar `LogAuditAsync()`.

---

### ⚠️ **GET Requests (Razor Pages)** - APENAS LOG BÁSICO

```http
GET /Admin/Users
Cookie: .AspNetCore.Identity.Application=CfDJ8K...
```

**Logs Gerados:**

```
[Information] [MIDDLEWARE START] GET /Admin/Users
[Information] Executing endpoint 'Admin.UI.Pages.Users.IndexModel.OnGetAsync'
[Information] [MIDDLEWARE END] GET /Admin/Users - Status: 200

// ❌ NÃO É CRIADO REGISTO NA TABELA AuditLogs
```

---

## 📋 Resumo: O que você TEM vs O que NÃO TEM

### ✅ O que ESTÁ a ser logado para Admin UI:

1. **Logs estruturados (Serilog):**
   - `[MIDDLEWARE START]` - Timestamp, Method, Path
   - `[MIDDLEWARE END]` - Timestamp, Method, Path, StatusCode
   - Logs de autenticação (Identity): "User X logged in"
   - Erros/Exceções (se houver)

2. **Onde encontrar:**
   - **Consola** (durante desenvolvimento)
   - **Ficheiro**: `logs/PHCAPI-{Date}.txt`

**Exemplo de log:**
```
2026-02-16 12:30:45.123 +00:00 [Information] [MIDDLEWARE START] POST /Admin/Account/Login
2026-02-16 12:30:45.456 +00:00 [Information] User admin@phcapi.com logged in.
2026-02-16 12:30:45.789 +00:00 [Information] [MIDDLEWARE END] POST /Admin/Account/Login - Status: 302
```

---

### ❌ O que NÃO está a ser logado para Admin UI:

1. **Auditoria completa** (tabela `AuditLogs`):
   - Request Body (formulário submetido)
   - Response Body (HTML gerado)
   - IP Address do utilizador
   - User Agent
   - Correlation ID

2. **Métricas detalhadas:**
   - Tempo de processamento
   - Tamanho da resposta

---

## 🔧 Como Ativar Logging Completo para Razor Pages

### Opção 1: Modificar o Middleware (Recomendado para Admin UI)

```csharp
// ResponseLoggingMiddleware.cs

private async Task ProcessSuccessfulResponseAsync(...)
{
    var responseText = await ReadResponseTextAsync(responseBody);
    await responseBody.CopyToAsync(originalBodyStream);

    // ✅ ANTES (atual):
    if (string.IsNullOrWhiteSpace(responseText) || !IsJsonResponse(context)) 
        return;

    // ✅ DEPOIS (opção 1 - logar tudo):
    // Remove o early return - loga JSON E HTML
    if (!string.IsNullOrWhiteSpace(responseText))
    {
        var (responseDto, auditJson) = GetResponseForAudit(responseText, context);
        await LogAuditAsync(context, requestBody, responseDto, auditJson);
    }
    
    // ✅ DEPOIS (opção 2 - selectivo):
    bool shouldLog = IsJsonResponse(context) || IsAdminUIRequest(context);
    if (!string.IsNullOrWhiteSpace(responseText) && shouldLog)
    {
        var (responseDto, auditJson) = GetResponseForAudit(responseText, context);
        await LogAuditAsync(context, requestBody, responseDto, auditJson);
    }
}

private static bool IsAdminUIRequest(HttpContext context) =>
    context.Request.Path.StartsWithSegments("/Admin", StringComparison.OrdinalIgnoreCase);
```

---

### Opção 2: Criar Middleware Separado para Admin UI

```csharp
// src/Admin.UI/Middleware/AdminAuditMiddleware.cs

public class AdminAuditMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AdminAuditMiddleware> _logger;

    public AdminAuditMiddleware(RequestDelegate next, ILogger<AdminAuditMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Só processa requisições do Admin UI
        if (!context.Request.Path.StartsWithSegments("/Admin"))
        {
            await _next(context);
            return;
        }

        var startTime = DateTime.UtcNow;
        var method = context.Request.Method;
        var path = context.Request.Path;
        var username = context.User.Identity?.Name ?? "Anonymous";
        var ipAddress = context.Connection.RemoteIpAddress?.ToString();

        // Capturar form data (POST)
        string? formData = null;
        if (context.Request.HasFormContentType)
        {
            formData = string.Join(", ", 
                context.Request.Form.Keys
                    .Where(k => !k.Contains("Password", StringComparison.OrdinalIgnoreCase))
                    .Select(k => $"{k}={context.Request.Form[k]}"));
        }

        try
        {
            await _next(context);

            var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;
            var statusCode = context.Response.StatusCode;

            // Log estruturado
            _logger.LogInformation(
                "[ADMIN UI] {Username} | {Method} {Path} | Status: {StatusCode} | Duration: {Duration}ms | IP: {IP} | Form: {Form}",
                username, method, path, statusCode, duration, ipAddress, formData ?? "N/A"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "[ADMIN UI ERROR] {Username} | {Method} {Path} | IP: {IP}",
                username, method, path, ipAddress
            );
            throw;
        }
    }
}

// Program.cs - Registar middleware
app.UseMiddleware<AdminAuditMiddleware>();
```

**Resultado:**
```
[Information] [ADMIN UI] admin@phcapi.com | POST /Admin/Account/Login | Status: 302 | Duration: 234ms | IP: 192.168.1.100 | Form: Input.Username=admin@phcapi.com, Input.RememberMe=true
[Information] [ADMIN UI] admin@phcapi.com | GET /Admin/Users | Status: 200 | Duration: 145ms | IP: 192.168.1.100 | Form: N/A
[Information] [ADMIN UI] admin@phcapi.com | POST /Admin/Users/Create | Status: 302 | Duration: 567ms | IP: 192.168.1.100 | Form: Input.Email=newuser@test.com, Input.Roles=Administrator
```

---

### Opção 3: Usar ASP.NET Core Request Logging Built-in

```csharp
// Program.cs

// Adicionar ANTES de outros middlewares
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "{RemoteIpAddress} {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms";
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("RemoteIpAddress", httpContext.Connection.RemoteIpAddress);
        diagnosticContext.Set("UserName", httpContext.User.Identity?.Name ?? "Anonymous");
        diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent);
        
        // Para Admin UI, capturar form data
        if (httpContext.Request.Path.StartsWithSegments("/Admin") && 
            httpContext.Request.HasFormContentType)
        {
            var safeFormData = string.Join(", ", 
                httpContext.Request.Form.Keys
                    .Where(k => !k.Contains("Password", StringComparison.OrdinalIgnoreCase))
                    .Select(k => $"{k}={httpContext.Request.Form[k]}"));
            diagnosticContext.Set("FormData", safeFormData);
        }
    };
});
```

**Resultado:**
```json
{
  "Timestamp": "2026-02-16T12:30:45.1234567Z",
  "Level": "Information",
  "MessageTemplate": "{RemoteIpAddress} {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed}ms",
  "Properties": {
    "RemoteIpAddress": "192.168.1.100",
    "RequestMethod": "POST",
    "RequestPath": "/Admin/Account/Login",
    "StatusCode": 302,
    "Elapsed": 234.5678,
    "UserName": "admin@phcapi.com",
    "UserAgent": "Mozilla/5.0...",
    "FormData": "Input.Username=admin@phcapi.com, Input.RememberMe=true"
  }
}
```

---

## 🎯 Recomendação

### Para Admin UI (onde segurança é crítica):

**Implementar Opção 2 (Middleware dedicado)** porque:

✅ Não interfere com logging de APIs  
✅ Captura form data (exceto passwords)  
✅ Log estruturado com username, IP, duration  
✅ Fácil de filtrar logs (grep `[ADMIN UI]`)  
✅ Performance não é crítica (baixo volume comparado com APIs)

### Para APIs (produção com alto tráfego):

**Manter sistema atual** porque:

✅ Evita logs massivos de HTML  
✅ Auditoria completa via `AuditLogs` table  
✅ Performance otimizada

---

## 📝 Exemplo Completo de Logs (Com Middleware Dedicado)

```
// Login bem-sucedido
2026-02-16 12:30:45.123 [Information] [MIDDLEWARE START] POST /Admin/Account/Login
2026-02-16 12:30:45.234 [Information] [ADMIN UI] admin@phcapi.com | POST /Admin/Account/Login | Status: 302 | Duration: 111ms | IP: 192.168.1.100 | Form: Input.Username=admin@phcapi.com, Input.RememberMe=true
2026-02-16 12:30:45.345 [Information] User admin@phcapi.com logged in.
2026-02-16 12:30:45.456 [Information] [MIDDLEWARE END] POST /Admin/Account/Login - Status: 302

// Visualizar users
2026-02-16 12:31:00.123 [Information] [MIDDLEWARE START] GET /Admin/Users
2026-02-16 12:31:00.268 [Information] [ADMIN UI] admin@phcapi.com | GET /Admin/Users | Status: 200 | Duration: 145ms | IP: 192.168.1.100 | Form: N/A
2026-02-16 12:31:00.345 [Information] [MIDDLEWARE END] GET /Admin/Users - Status: 200

// Criar utilizador
2026-02-16 12:32:15.123 [Information] [MIDDLEWARE START] POST /Admin/Users/Create
2026-02-16 12:32:15.690 [Information] [ADMIN UI] admin@phcapi.com | POST /Admin/Users/Create | Status: 302 | Duration: 567ms | IP: 192.168.1.100 | Form: Input.Email=newuser@test.com, Input.Username=newuser@test.com, Input.Roles=Administrator
2026-02-16 12:32:15.789 [Information] [MIDDLEWARE END] POST /Admin/Users/Create - Status: 302

// Login falhado
2026-02-16 12:35:00.123 [Information] [MIDDLEWARE START] POST /Admin/Account/Login
2026-02-16 12:35:00.234 [Warning] [ADMIN UI] Anonymous | POST /Admin/Account/Login | Status: 200 | Duration: 111ms | IP: 192.168.1.200 | Form: Input.Username=hacker@evil.com, Input.RememberMe=false
2026-02-16 12:35:00.345 [Warning] Failed login attempt for user hacker@evil.com
2026-02-16 12:35:00.456 [Information] [MIDDLEWARE END] POST /Admin/Account/Login - Status: 200
```

---

## 🔒 Segurança: O que NÃO logar

```csharp
// ❌ NUNCA logar:
- Passwords (Input.Password)
- Tokens (Authorization headers)
- Session cookies (completos)
- Dados sensíveis (SSN, cartões de crédito, etc.)

// ✅ PODE logar:
- Usernames
- Email addresses
- IP addresses
- Timestamps
- Status codes
- Form field names (não valores sensíveis)
- User agents
- Request paths
```

---

## 📊 Onde Encontrar os Logs Atuais

### 1. Consola (Desenvolvimento)
```powershell
dotnet run --project src/SGOFAPI.Host
```

### 2. Ficheiros (Produção)
```
logs/
├── PHCAPI-2026-02-16.txt
├── PHCAPI-2026-02-15.txt
└── PHCAPI-2026-02-14.txt
```

### 3. Filtrar Logs do Admin UI
```powershell
# Ver apenas requisições do Admin
Select-String -Path "logs/PHCAPI-*.txt" -Pattern "/Admin"

# Ver apenas logins
Select-String -Path "logs/PHCAPI-*.txt" -Pattern "/Admin/Account/Login"

# Ver erros
Select-String -Path "logs/PHCAPI-*.txt" -Pattern "\[Error\]"
```

---

## 🎯 Conclusão

**Estado Atual:**
- ✅ Requisições API → Logging completo + AuditLogs table
- ⚠️ Requisições Admin UI → Apenas logs básicos (START/END)

**Ação Recomendada:**
Implementar middleware dedicado para Admin UI (Opção 2) para ter auditoria completa de todas as ações administrativas.
