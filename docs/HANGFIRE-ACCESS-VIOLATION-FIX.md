# 🚨 Hangfire Access Violation - Solução Completa

## 🔍 Problema

**Erro**: `Access violation (0xc0000005)` ao acessar `/hangfire`
- Browser fecha automaticamente
- Aplicação para de executar
- Erro fatal do Windows (acesso a memória inválida)

---

## 🛠️ Soluções Aplicadas

### **1. HangfireAuthorizationFilter Robusto**

**Problema Original**:
```csharp
// ❌ Sem tratamento de erros
public bool Authorize(DashboardContext context)
{
    var httpContext = context.GetHttpContext();
    return httpContext.Request.Host.Host == "localhost";
}
```

**Solução**:
```csharp
// ✅ Com proteção contra null e try-catch
public bool Authorize(DashboardContext context)
{
    try
    {
        if (context == null) return false;
        var httpContext = context.GetHttpContext();
        if (httpContext == null) return false;
        
        var host = httpContext.Request.Host.Host?.ToLowerInvariant() ?? string.Empty;
        
        return host == "localhost" 
            || host == "127.0.0.1"
            || host.StartsWith("192.168.")
            || string.IsNullOrEmpty(host);
    }
    catch
    {
        // Em desenvolvimento, permitir em caso de erro
        return true;
    }
}
```

---

### **2. Hangfire Storage com Schema Separado**

**Configuração**:
```csharp
builder.Services.AddHangfire(config =>
{
    config.UseSqlServerStorage(
        builder.Configuration.GetConnectionString("DBconnect"),
        new SqlServerStorageOptions
        {
            PrepareSchemaIfNecessary = true,  // ✅ Criar tabelas automaticamente
            SchemaName = "HangFire"            // ✅ Schema separado
        });
});
```

**Tabelas Criadas** (no schema `HangFire`):
```
HangFire.AggregatedCounter
HangFire.Counter
HangFire.Hash
HangFire.Job
HangFire.JobParameter
HangFire.JobQueue
HangFire.List
HangFire.Schema
HangFire.Server
HangFire.Set
HangFire.State
```

---

### **3. Dashboard com Try-Catch**

```csharp
try
{
    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = new[] { new HangfireAuthorizationFilter() },
        StatsPollingInterval = 5000,
        DisplayStorageConnectionString = false,
        DashboardTitle = "SGOFAPI - Background Jobs"
    });
    
    Log.Information("Hangfire Dashboard available at /hangfire");
}
catch (Exception ex)
{
    Log.Warning(ex, "Failed to initialize Hangfire Dashboard - continuing without it");
}
```

**Benefício**: Se Hangfire Dashboard falhar, a aplicação continua funcionando.

---

### **4. Hangfire Server com Limites**

```csharp
builder.Services.AddHangfireServer(options =>
{
    options.WorkerCount = 5;  // Limite de workers paralelos
    options.ServerTimeout = TimeSpan.FromMinutes(5);
    options.ShutdownTimeout = TimeSpan.FromMinutes(1);
});
```

---

## 🧪 Como Testar

### **1. Verificar se Hangfire Iniciou**

**Logs no Console**:
```
[15:30:00 INF] Hangfire Dashboard available at /hangfire
[15:30:01 INF] Starting Hangfire Server...
[15:30:01 INF] Using the following servers:
[15:30:01 INF]  - Server1 (workers: 5)
```

### **2. Acessar Dashboard**

```
https://localhost:7046/hangfire
```

**Se funcionar, você verá**:
- Jobs enfileirados (Enqueued)
- Jobs processados (Succeeded)
- Jobs falhados (Failed)
- Workers ativos

### **3. Verificar Tabelas no SQL Server**

```sql
-- Verificar se schema foi criado
SELECT * FROM sys.schemas WHERE name = 'HangFire';

-- Verificar tabelas
SELECT TABLE_SCHEMA, TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_SCHEMA = 'HangFire';

-- Ver jobs recentes
SELECT TOP 10 * FROM HangFire.Job ORDER BY CreatedAt DESC;
```

---

## ⚠️ Troubleshooting

### **Problema: Access Violation Persiste**

**Possíveis Causas**:

#### **1. Connection String Incorreta**

```json
// appsettings.json
{
  "ConnectionStrings": {
    "DBconnect": "Server=localhost;Database=PHC;..."
  }
}
```

**Verificar**:
```sql
-- Testar conexão
SELECT @@VERSION;
SELECT DB_NAME();
```

#### **2. Permissões Insuficientes**

```sql
-- Verificar se o usuário pode criar schema
SELECT * FROM fn_my_permissions(NULL, 'DATABASE') 
WHERE permission_name = 'CREATE SCHEMA';

-- Se não tiver, executar como admin:
GRANT CREATE SCHEMA TO [seu_usuario];
```

#### **3. Versões Incompatíveis de Hangfire**

**Verificar versões**:
```xml
<!-- SGOFAPI.Host.csproj -->
<PackageReference Include="Hangfire.Core" Version="1.8.11" />
<PackageReference Include="Hangfire.SqlServer" Version="1.8.11" />
<PackageReference Include="Hangfire.AspNetCore" Version="1.8.11" />
```

**TODAS devem ser a mesma versão!**

#### **4. Tabelas Corrompidas**

```sql
-- Dropar schema e recriar
DROP SCHEMA IF EXISTS HangFire;
```

**Reiniciar aplicação** → Hangfire recria automaticamente.

---

### **Solução Alternativa: Desabilitar Dashboard**

Se nada funcionar, **desabilitar dashboard** mas **manter Hangfire Server**:

```csharp
// Program.cs

// ✅ Hangfire Server (para processar jobs)
builder.Services.AddHangfire(...);
builder.Services.AddHangfireServer();

// ❌ NÃO adicionar Dashboard
// app.UseHangfireDashboard("/hangfire", ...);
```

**Logs ainda funcionam**:
- Jobs são enfileirados ✅
- Jobs são processados ✅
- Logs salvos no banco ulogs ✅
- Apenas dashboard não disponível

**Monitorar via SQL**:
```sql
-- Ver jobs processados hoje
SELECT COUNT(*) FROM HangFire.Job 
WHERE CAST(CreatedAt AS DATE) = CAST(GETDATE() AS DATE)
  AND StateName = 'Succeeded';

-- Ver jobs falhados
SELECT * FROM HangFire.Job 
WHERE StateName = 'Failed'
ORDER BY CreatedAt DESC;
```

---

## 📊 Logs Importantes

### **Startup Logs (Sucesso)**

```
[INF] Building application...
[INF] Application built successfully
[INF] Hangfire Dashboard available at /hangfire
[INF] Starting Hangfire Server using job storage: 'SQL Server: localhost'
[INF] Using the following options for SQL Server job storage: ...
[INF] Now listening on: https://localhost:7046
[INF] Application started. Press Ctrl+C to shut down.
```

### **Startup Logs (Falha)**

```
[WRN] Failed to initialize Hangfire Dashboard - continuing without it
System.AccessViolationException: Attempted to read or write protected memory...
```

---

## 🔍 Debugging Adicional

### **Habilitar Logs Detalhados do Hangfire**

```csharp
// appsettings.Development.json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Hangfire": "Debug"  // ← Logs detalhados
      }
    }
  }
}
```

### **Verificar Eventos do Windows**

1. Abrir **Event Viewer** (Visualizador de Eventos)
2. **Windows Logs** → **Application**
3. Filtrar por **Source**: `.NET Runtime`
4. Procurar erros com timestamp do crash

---

## ✅ Checklist de Verificação

- [ ] Connection string correta
- [ ] Banco de dados acessível
- [ ] Usuário tem permissão `CREATE SCHEMA`
- [ ] Versões de Hangfire packages consistentes
- [ ] Schema `HangFire` criado automaticamente
- [ ] Tabelas de Hangfire existem
- [ ] Dashboard acessível em `/hangfire`
- [ ] Jobs sendo enfileirados
- [ ] Jobs sendo processados

---

## 🎯 Resumo Final

### **Mudanças Aplicadas**

1. ✅ `HangfireAuthorizationFilter` com try-catch
2. ✅ `PrepareSchemaIfNecessary = true`
3. ✅ Schema separado (`HangFire`)
4. ✅ Dashboard com tratamento de erro
5. ✅ Hangfire Server com limites

### **Resultado Esperado**

- ✅ Dashboard funciona em `/hangfire`
- ✅ Jobs processados em background
- ✅ Logs salvos no banco ulogs
- ✅ Sem crashes ou access violations

### **Se Ainda Falhar**

➡️ **Desabilitar dashboard** mas **manter background jobs**  
➡️ **Monitorar via SQL** queries nas tabelas `HangFire.*`

---

**Data**: 10 de fevereiro de 2024  
**Status**: ✅ Implementado  
**Tested**: Aguardando teste em desenvolvimento
