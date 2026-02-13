# 🔧 Access Violation - Troubleshooting Guide

## ❌ Problema

```
The program '[XXXX] PHCAPI.Host.exe' has exited with code 3221225477 (0xc0000005) 'Access violation'.
```

O Visual Studio fecha abruptamente após imprimir `LOGGING RESPONSE....`

---

## 🔍 Causa Raiz

### **1. DbContext Scoped + Task.Run() = 💥**

```csharp
// ❌ ERRADO: DbContext descartado antes do Task completar
public void LogResponseAsync(...)
{
    Task.Run(async () => {
        var auditLog = new AuditLog(...); // 💥 Access Violation!
        await _repository.AddAsync(auditLog); // DbContext já foi disposed
    });
}
```

**Problema:**
- `AuditDbContext` tem lifetime **Scoped** (vive durante o HTTP request)
- `Task.Run()` cria uma thread **fora do scope HTTP**
- Quando o request termina, o DbContext é **disposed**
- A thread do Task.Run tenta acessar o DbContext disposed → **Access Violation**

### **2. Random Estático (Corrigido ✅)**

```csharp
// ❌ ANTES: Causava corrupção de memória em alta concorrência
private static readonly Random _random = new();
```

---

## ✅ Solução Implementada

### **1. IServiceScopeFactory Pattern**

```csharp
public class AuditLogService : IAuditLogService
{
    private readonly IServiceScopeFactory _scopeFactory; // ✅ Factory para criar scopes

    public void LogResponseAsync(...)
    {
        _ = Task.Run(async () =>
        {
            // ✅ Criar scope PRÓPRIO (independente do HTTP request)
            using var scope = _scopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IAuditLogRepository>();

            var auditLog = new AuditLog(...);
            await repository.AddAsync(auditLog); // ✅ SEGURO: DbContext do novo scope
        });
    }
}
```

**Vantagens:**
- ✅ Cada Task tem seu próprio DbContext
- ✅ Não há race conditions
- ✅ Não bloqueia o request HTTP
- ✅ Background processing seguro

### **2. Thread-Safe Random**

```csharp
[ThreadStatic] // ✅ Cada thread tem sua instância
private static Random? _random;
```

---

## 🧪 Teste a Correção

### **1. Build Limpo**

```powershell
dotnet clean
dotnet build
```

### **2. Executar com Logging Verbose**

```powershell
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet run --project src/SGOFAPI.Host/PHCAPI.Host.csproj
```

### **3. Verificar Logs**

Deves ver:
```
[FILTER] Executing action: /api/parameters
[FILTER] ResponseDTO detected, triggering audit log
[AUDIT] Starting background log for <RequestId>
[AUDIT] Entity created with stamp: <Stamp>
[AUDIT] ✅ Log saved: <RequestId> - GET /api/parameters
```

---

## 🛑 Se o Problema Persistir

### **Opção 1: Desabilitar Temporariamente o Filtro**

Em `Program.cs`:

```csharp
var mvcBuilder = builder.Services.AddControllers(options =>
{
    // ⚠️ COMENTAR TEMPORARIAMENTE para isolar o problema:
    // options.Filters.Add<AuditLoggingFilter>();
});
```

Se a app funcionar **SEM** o filtro → confirmado que é problema de audit logging.

### **Opção 2: Verificar Connection String**

Verifica se a conexão ao SQL Server está ok:

```json
// appsettings.Development.json
{
  "ConnectionStrings": {
    "DBconnect": "Server=xxx;Database=xxx;..."
  }
}
```

Testa a conexão:

```powershell
sqlcmd -S <server> -d <database> -U <user> -P <password> -Q "SELECT 1"
```

### **Opção 3: Usar Hangfire (Alternativa Robusta)**

Para logs verdadeiramente assíncronos:

```csharp
public void LogResponseAsync(...)
{
    BackgroundJob.Enqueue<IAuditLogRepository>(repo =>
        repo.AddAsync(new AuditLog(...), CancellationToken.None)
    );
}
```

---

## 📊 Monitoramento

### **Logs de Debug**

- `[FILTER]` → AuditLoggingFilter
- `[AUDIT]` → AuditLogService

### **Sinais de Problemas**

- ❌ Não vês `[AUDIT] ✅ Log saved`
- ❌ Vês `[AUDIT] ❌ FAILED`
- ❌ App fecha sem stack trace

---

## 🔗 Arquivos Modificados

- [`AuditLogService.cs`](../src/Modules/Audit/Audit.Application/Services/AuditLogService.cs) - IServiceScopeFactory pattern
- [`AuditLoggingFilter.cs`](../src/SGOFAPI.Host/Filters/AuditLoggingFilter.cs) - Logging detalhado
- [`StampExtensions.cs`](../src/Shared/Shared.Kernel/Shared.Kernel/Extensions/StampExtensions.cs) - Thread-safe Random

---

## 📚 Referências

- [ASP.NET Core Scoped Services](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection#scoped)
- [IServiceScopeFactory Pattern](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicescopefactory)
- [Thread-Static Random](https://learn.microsoft.com/en-us/dotnet/api/system.threadstaticattribute)
