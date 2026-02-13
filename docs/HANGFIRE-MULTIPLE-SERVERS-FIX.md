# 🚨 Solução: Dois Servidores Hangfire Ativos

## 🔍 Problema Identificado

**Sintoma**: 2 servidores Hangfire rodando simultaneamente
```
denilsonb-lp:39744	5	default	há segundos	há segundos
denilsonb-lp:42624	5	default	há segundos	há segundos
```

## 🛠️ Causas Possíveis

### **1. Hot Reload / Debug Session Antiga** ✅ (MAIS PROVÁVEL)

Quando você usa **Hot Reload** ou reinicia o debug sem fechar completamente, o Visual Studio pode manter processos antigos.

**Solução**:
1. **Parar TODOS os processos**:
   - Shift+F5 no Visual Studio
   - Ou clicar no ❌ vermelho (Stop Debugging)

2. **Verificar Task Manager**:
   ```
   Ctrl+Shift+Esc → Details → Procurar:
   - PHCAPI.Host.exe
   - dotnet.exe
   ```

3. **Matar processos órfãos**:
   - Click direito → End Task

4. **Reabrir aplicação**:
   - F5 novamente

---

### **2. Múltiplas Instâncias da Aplicação**

Se você iniciou a aplicação **duas vezes** (por acidente):

**Verificar portas**:
```powershell
netstat -ano | findstr "7046"
```

**Resultado esperado** (1 servidor):
```
TCP    0.0.0.0:7046           0.0.0.0:0              LISTENING       12345
```

**Problema** (2 servidores):
```
TCP    0.0.0.0:7046           0.0.0.0:0              LISTENING       12345
TCP    0.0.0.0:7046           0.0.0.0:0              LISTENING       67890
```

**Solução**:
```powershell
# Matar processo específico
taskkill /PID 67890 /F
```

---

### **3. Configuração Duplicada (IMPROVÁVEL)**

Se `AddHangfireServer()` está sendo chamado duas vezes no código:

**Verificar**:
```powershell
# Procurar todas as ocorrências
Select-String -Path "src\**\*.cs" -Pattern "AddHangfireServer" -Recurse
```

**Arquivo**: `src\SGOFAPI.Host\Program.cs`
- ✅ Linha 50: `builder.Services.AddHangfireServer(...)`
- ❌ NÃO deveria haver outra ocorrência

---

### **4. WorkerCount Multiplicado**

Se você configurou `WorkerCount = 5`, cada servidor cria **5 workers**.

Com **2 servidores**, você tem **10 workers** ao invés de 5!

```csharp
// Atual (linha 50-55)
builder.Services.AddHangfireServer(options =>
{
    options.WorkerCount = 5; // ← 5 workers POR SERVIDOR
    options.ServerTimeout = TimeSpan.FromMinutes(5);
    options.ShutdownTimeout = TimeSpan.FromMinutes(1);
});
```

---

## ✅ Solução Recomendada

### **Passo 1: Limpar Processos**

```powershell
# Parar todos os processos .NET
Get-Process | Where-Object {$_.ProcessName -like "*dotnet*" -or $_.ProcessName -like "*PHCAPI*"} | Stop-Process -Force

# Verificar se ainda há processos
Get-Process | Where-Object {$_.ProcessName -like "*dotnet*"}
```

### **Passo 2: Adicionar Logs de Debug**

Atualizar `Program.cs` para logar quantos servidores foram registrados:

```csharp
// ANTES de AddHangfireServer
Log.Information("Registering Hangfire Server...");

builder.Services.AddHangfireServer(options =>
{
    options.WorkerCount = 5;
    options.ServerTimeout = TimeSpan.FromMinutes(5);
    options.ShutdownTimeout = TimeSpan.FromMinutes(1);
    
    // ✅ NOVO: Nome único do servidor
    options.ServerName = $"{Environment.MachineName}:{Guid.NewGuid().ToString("N")[..8]}";
});

Log.Information("Hangfire Server registered with WorkerCount: 5");
```

**Benefício**: Cada servidor terá nome único no dashboard.

---

### **Passo 3: Garantir Servidor Único**

Se ainda tiver problema, **desabilitar temporariamente** para testar:

```csharp
// ❌ COMENTAR AddHangfireServer
// builder.Services.AddHangfireServer(options => { ... });

// ✅ Adicionar servidor MANUALMENTE no pipeline
app.Services.GetRequiredService<IBackgroundJobClient>();
```

---

### **Passo 4: Verificar Dashboard**

Após reiniciar:

1. **Acessar**: `https://localhost:7046/hangfire`
2. **Ir para**: **Servers** (menu superior)
3. **Verificar**: Deve aparecer **APENAS 1 servidor**

**Exemplo esperado**:
```
Server Name               | Workers | Started            | Last Heartbeat
──────────────────────────────────────────────────────────────────────
denilsonb-lp:a1b2c3d4    | 5       | há 10 segundos     | há segundos
```

**Problema (2 servidores)**:
```
Server Name               | Workers | Started            | Last Heartbeat
──────────────────────────────────────────────────────────────────────
denilsonb-lp:39744       | 5       | há 2 minutos       | há segundos
denilsonb-lp:42624       | 5       | há 10 segundos     | há segundos
```

---

## 🔧 Correção Aplicada

Vou adicionar nome único ao servidor:

```csharp
// src\SGOFAPI.Host\Program.cs (linha 50)
builder.Services.AddHangfireServer(options =>
{
    options.WorkerCount = 5;
    options.ServerTimeout = TimeSpan.FromMinutes(5);
    options.ShutdownTimeout = TimeSpan.FromMinutes(1);
    
    // ✅ NOVO: Nome único (facilita identificação no dashboard)
    options.ServerName = $"{Environment.MachineName}:{DateTime.UtcNow.Ticks}";
});
```

---

## 📊 Monitoramento

### **Ver Servidores Ativos (SQL)**

```sql
-- Ver servidores registrados
SELECT * FROM HangFire.Server ORDER BY StartedAt DESC;

-- Ver heartbeats (servidores vivos)
SELECT 
    Id AS ServerName,
    StartedAt,
    Heartbeat AS LastHeartbeat,
    WorkerCount,
    DATEDIFF(SECOND, Heartbeat, GETUTCDATE()) AS SecondsSinceLastHeartbeat
FROM HangFire.Server
WHERE DATEDIFF(SECOND, Heartbeat, GETUTCDATE()) < 30 -- Vivos (heartbeat < 30s)
ORDER BY Heartbeat DESC;
```

**Resultado esperado (1 servidor)**:
```
ServerName          | LastHeartbeat       | WorkerCount | SecondsSinceLastHeartbeat
──────────────────────────────────────────────────────────────────────────────────
denilsonb-lp:12345 | 2024-02-10 16:00:05 | 5           | 2
```

---

## 🚨 Se o Problema Persistir

### **Forçar Limpeza de Servidores Antigos**

```sql
-- Ver TODOS os servidores (incluindo mortos)
SELECT * FROM HangFire.Server;

-- Deletar servidores antigos (cuidado!)
DELETE FROM HangFire.Server
WHERE DATEDIFF(MINUTE, Heartbeat, GETUTCDATE()) > 5; -- Sem heartbeat há mais de 5 min

-- Verificar
SELECT COUNT(*) FROM HangFire.Server;
```

---

## ✅ Checklist de Verificação

- [ ] Parei TODOS os processos no Visual Studio (Shift+F5)
- [ ] Verifiquei Task Manager (nenhum `dotnet.exe` ou `PHCAPI.Host.exe`)
- [ ] Reiniciei a aplicação (F5)
- [ ] Acessei `/hangfire` → Servers
- [ ] Vejo **APENAS 1 servidor** ativo
- [ ] WorkerCount = 5 (não 10)

---

## 🎯 Resultado Esperado

**Dashboard Hangfire → Servers**:
```
✅ 1 servidor ativo
✅ 5 workers
✅ Nome único identificável
✅ Heartbeat recente (< 10 segundos)
```

---

**Data**: 10 de fevereiro de 2024  
**Status**: ⚠️ Aguardando teste  
**Próximo Passo**: Limpar processos e reiniciar aplicação
