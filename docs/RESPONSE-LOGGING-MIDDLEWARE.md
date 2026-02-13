# 🎯 Response Logging Middleware - Auditoria Completa

## 📋 Visão Geral

Implementação de um **middleware único** que intercepta **TODAS as respostas HTTP** (sucesso e erro) e registra automaticamente via **módulo Audit** usando **Hangfire** para processamento assíncrono.

---

## 🏗️ Arquitetura

### **Fluxo Completo da Request**

```
┌─────────────────────────────────────────────────────────────────────┐
│                        REQUEST PIPELINE                             │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  1. CorrelationIdMiddleware                                         │
│     ├─► Extrai X-Correlation-Id do header                          │
│     ├─► OU gera novo GUID se não existir                          │
│     └─► Armazena em HttpContext.Items["CorrelationId"]            │
│                                                                     │
│  2. ResponseLoggingMiddleware (NOVO!)                               │
│     ├─► Captura dados do REQUEST                                  │
│     │   ├─ Method (GET/POST/PUT/DELETE)                           │
│     │   ├─ Path (/api/parameters)                                 │
│     │   ├─ Body (se POST/PUT/PATCH)                               │
│     │   └─ Correlation ID                                         │
│     │                                                              │
│     ├─► Intercepta RESPONSE                                        │
│     │   ├─ Substitui Response.Body por MemoryStream              │
│     │   ├─ Permite ler body sem consumir stream                  │
│     │   └─ Copia de volta para stream original                   │
│     │                                                              │
│     ├─► Chama próximo middleware/controller                       │
│     │   └─► [Controller executa lógica]                          │
│     │                                                              │
│     ├─► Captura resposta                                          │
│     │   ├─ Status Code (200, 400, 500, etc)                      │
│     │   ├─ Response Body (JSON)                                  │
│     │   └─ Tempo de execução (elapsed time)                      │
│     │                                                              │
│     └─► Loga via AuditLogService                                  │
│         ├─► LogResponseAsync() - Enfileira job no Hangfire       │
│         └─► Job executa em background thread                     │
│                                                                     │
│  3. GlobalExceptionHandler                                          │
│     ├─► Captura exceções não tratadas                            │
│     ├─► Cria ResponseDTO apropriado                              │
│     ├─► Loga via AuditLogService (TAMBÉM!)                       │
│     └─► Retorna erro formatado ao cliente                        │
│                                                                     │
│  4. Controllers/Endpoints                                           │
│     └─► Lógica de negócio da aplicação                           │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

---

## 📊 Fluxo de Dados Detalhado

### **Cenário 1: Sucesso (200 OK)**

```
┌──────────────────────────────────────────────────────────────┐
│  REQUEST                                                     │
├──────────────────────────────────────────────────────────────┤
│  POST /api/parameters                                        │
│  X-Correlation-Id: abc-123-def                              │
│  Body: { "descricao": "Teste", "valor": "123" }            │
└──────────────────────────────────────────────────────────────┘
                    ▼
┌──────────────────────────────────────────────────────────────┐
│  CorrelationIdMiddleware                                     │
├──────────────────────────────────────────────────────────────┤
│  ✅ Correlation ID: abc-123-def (extraído do header)        │
└──────────────────────────────────────────────────────────────┘
                    ▼
┌──────────────────────────────────────────────────────────────┐
│  ResponseLoggingMiddleware                                   │
├──────────────────────────────────────────────────────────────┤
│  ✅ Captura request:                                        │
│     - Method: POST                                           │
│     - Path: /api/parameters                                  │
│     - Body: { "descricao": "Teste", "valor": "123" }       │
│     - CorrelationId: abc-123-def                            │
│                                                              │
│  ✅ Substitui Response.Body por MemoryStream               │
│                                                              │
│  ▼ Chama _next(context)                                     │
└──────────────────────────────────────────────────────────────┘
                    ▼
┌──────────────────────────────────────────────────────────────┐
│  ParametersController.Create()                               │
├──────────────────────────────────────────────────────────────┤
│  ✅ Valida dados                                            │
│  ✅ Cria parâmetro no banco                                 │
│  ✅ Retorna ResponseDTO.Success(...)                        │
└──────────────────────────────────────────────────────────────┘
                    ▼
┌──────────────────────────────────────────────────────────────┐
│  ResponseLoggingMiddleware (volta do stack)                  │
├──────────────────────────────────────────────────────────────┤
│  ✅ Captura response:                                       │
│     - Status: 201 Created                                    │
│     - Body: {                                                │
│         "success": true,                                     │
│         "code": "201",                                       │
│         "content": "Created successfully",                   │
│         "correlationId": "abc-123-def"                      │
│       }                                                      │
│     - Elapsed: 145ms                                         │
│                                                              │
│  ✅ Parseia body como ResponseDTO                          │
│                                                              │
│  ✅ Loga via AuditLogService:                              │
│     auditLogService.LogResponseAsync(                        │
│       response: ResponseDTO {...},                           │
│       requestId: "abc-123-def",                             │
│       operation: "POST /api/parameters",                     │
│       httpContext: {...}                                     │
│     );                                                       │
│                                                              │
│  ✅ Hangfire enfileira job em background                   │
│                                                              │
│  ✅ Copia MemoryStream para Response.Body original         │
└──────────────────────────────────────────────────────────────┘
                    ▼
┌──────────────────────────────────────────────────────────────┐
│  Cliente recebe resposta                                     │
├──────────────────────────────────────────────────────────────┤
│  HTTP 201 Created                                            │
│  {                                                           │
│    "success": true,                                          │
│    "code": "201",                                            │
│    "content": "Created successfully",                        │
│    "correlationId": "abc-123-def"                           │
│  }                                                           │
└──────────────────────────────────────────────────────────────┘
                    ▼
┌──────────────────────────────────────────────────────────────┐
│  Hangfire Job (background - assíncrono)                      │
├──────────────────────────────────────────────────────────────┤
│  AuditLogBackgroundJob.LogAsync(...)                         │
│     ▼                                                        │
│  AuditLogRepository.AddAsync(...)                            │
│     ▼                                                        │
│  Salva no banco ulogs:                                       │
│     - ulogsstamp: "generated-stamp-123"                     │
│     - requestid: "abc-123-def"                              │
│     - data: 2024-02-10 15:30:00                             │
│     - code: "201"                                            │
│     - operation: "POST /api/parameters"                      │
│     - ip: "192.168.1.100"                                   │
│     - responsetext: "Created successfully"                   │
└──────────────────────────────────────────────────────────────┘
```

---

### **Cenário 2: Erro de Validação (400 Bad Request)**

```
┌──────────────────────────────────────────────────────────────┐
│  REQUEST                                                     │
├──────────────────────────────────────────────────────────────┤
│  POST /api/parameters                                        │
│  X-Correlation-Id: xyz-456-ghi                              │
│  Body: { "descricao": "", "valor": "" }  ❌ INVÁLIDO       │
└──────────────────────────────────────────────────────────────┘
                    ▼
┌──────────────────────────────────────────────────────────────┐
│  CorrelationIdMiddleware                                     │
├──────────────────────────────────────────────────────────────┤
│  ✅ Correlation ID: xyz-456-ghi                             │
└──────────────────────────────────────────────────────────────┘
                    ▼
┌──────────────────────────────────────────────────────────────┐
│  ResponseLoggingMiddleware                                   │
├──────────────────────────────────────────────────────────────┤
│  ✅ Captura request                                         │
│  ✅ Substitui Response.Body                                 │
│  ▼ Chama _next(context)                                     │
└──────────────────────────────────────────────────────────────┘
                    ▼
┌──────────────────────────────────────────────────────────────┐
│  MediatR ValidationBehavior                                  │
├──────────────────────────────────────────────────────────────┤
│  ❌ FluentValidation FALHA                                  │
│  ❌ Lança ValidationException                               │
└──────────────────────────────────────────────────────────────┘
                    ▼
┌──────────────────────────────────────────────────────────────┐
│  GlobalExceptionHandler.TryHandleAsync()                     │
├──────────────────────────────────────────────────────────────┤
│  ✅ Captura ValidationException                             │
│                                                              │
│  ✅ Cria ResponseDTO:                                       │
│     ResponseDTO.Error(                                       │
│       code: "400",                                           │
│       data: {                                                │
│         errors: {                                            │
│           "Descricao": ["Campo obrigatório"],              │
│           "Valor": ["Campo obrigatório"]                   │
│         }                                                    │
│       },                                                     │
│       correlationId: "xyz-456-ghi"                          │
│     )                                                        │
│                                                              │
│  ✅ Loga via AuditLogService (AQUI TAMBÉM!)                │
│     auditLogService.LogResponseAsync(...)                    │
│                                                              │
│  ✅ Retorna 400 ao cliente                                  │
└──────────────────────────────────────────────────────────────┘
                    ▼
┌──────────────────────────────────────────────────────────────┐
│  ResponseLoggingMiddleware (volta do stack)                  │
├──────────────────────────────────────────────────────────────┤
│  ✅ Captura response 400                                    │
│                                                              │
│  ⚠️ NÃO loga novamente (GlobalExceptionHandler já logou)   │
│     - Exception handling aconteceu ANTES                     │
│     - Response já foi escrita pelo ExceptionHandler         │
│                                                              │
│  ✅ Copia stream de volta                                   │
└──────────────────────────────────────────────────────────────┘
                    ▼
┌──────────────────────────────────────────────────────────────┐
│  Cliente recebe resposta                                     │
├──────────────────────────────────────────────────────────────┤
│  HTTP 400 Bad Request                                        │
│  {                                                           │
│    "success": false,                                         │
│    "code": "400",                                            │
│    "data": {                                                 │
│      "errors": {                                             │
│        "Descricao": ["Campo obrigatório"],                 │
│        "Valor": ["Campo obrigatório"]                      │
│      }                                                       │
│    },                                                        │
│    "correlationId": "xyz-456-ghi"                           │
│  }                                                           │
└──────────────────────────────────────────────────────────────┘
                    ▼
┌──────────────────────────────────────────────────────────────┐
│  Hangfire Job (background)                                   │
├──────────────────────────────────────────────────────────────┤
│  Salva no banco ulogs:                                       │
│     - code: "400"                                            │
│     - operation: "POST /api/parameters"                      │
│     - responsedesc: "Validation error"                       │
│     - responsetext: JSON com erros                           │
└──────────────────────────────────────────────────────────────┘
```

---

### **Cenário 3: Exceção Não Tratada (500 Internal Server Error)**

```
┌──────────────────────────────────────────────────────────────┐
│  REQUEST                                                     │
├──────────────────────────────────────────────────────────────┤
│  GET /api/parameters/ABC123                                  │
│  X-Correlation-Id: err-789-jkl                              │
└──────────────────────────────────────────────────────────────┘
                    ▼
┌──────────────────────────────────────────────────────────────┐
│  ResponseLoggingMiddleware                                   │
├──────────────────────────────────────────────────────────────┤
│  ✅ Captura request                                         │
│  ✅ Substitui Response.Body                                 │
│  ▼ Chama _next(context)                                     │
└──────────────────────────────────────────────────────────────┘
                    ▼
┌──────────────────────────────────────────────────────────────┤
│  ParametersController.GetByStamp()                           │
├──────────────────────────────────────────────────────────────┤
│  ▼ Handler.Handle()                                          │
│  ▼ Repository.GetByStampAsync()                              │
│  ❌ DbConnection falha (banco offline)                      │
│  ❌ Lança SqlException                                       │
└──────────────────────────────────────────────────────────────┘
                    ▼
┌──────────────────────────────────────────────────────────────┐
│  ResponseLoggingMiddleware (catch block)                     │
├──────────────────────────────────────────────────────────────┤
│  ❌ Exception capturada no try/catch                        │
│                                                              │
│  ✅ Loga erro:                                              │
│     _logger.LogError(ex, "Exception during...")             │
│                                                              │
│  ❌ Re-throw exception                                       │
│     throw; // Deixa GlobalExceptionHandler tratar          │
└──────────────────────────────────────────────────────────────┘
                    ▼
┌──────────────────────────────────────────────────────────────┐
│  GlobalExceptionHandler.TryHandleAsync()                     │
├──────────────────────────────────────────────────────────────┤
│  ✅ Captura SqlException                                    │
│                                                              │
│  ✅ Cria ResponseDTO:                                       │
│     ResponseDTO.Error(                                       │
│       code: "500",                                           │
│       content: "Database connection failed",                 │
│       correlationId: "err-789-jkl"                          │
│     )                                                        │
│                                                              │
│  ✅ Loga via AuditLogService                                │
│                                                              │
│  ✅ Retorna 500 ao cliente                                  │
└──────────────────────────────────────────────────────────────┘
                    ▼
┌──────────────────────────────────────────────────────────────┐
│  Hangfire Job                                                │
├──────────────────────────────────────────────────────────────┤
│  Salva log de erro no banco (quando reconectar)             │
└──────────────────────────────────────────────────────────────┘
```

---

## 🔧 Componentes

### **1. ResponseLoggingMiddleware**

**Arquivo**: `src/SGOFAPI.Host/Middleware/ResponseLoggingMiddleware.cs`

**Responsabilidades**:
- ✅ Interceptar **request** (method, path, body, headers)
- ✅ Interceptar **response** (status, body)
- ✅ Substituir `Response.Body` por `MemoryStream` para permitir leitura
- ✅ Medir tempo de execução (Stopwatch)
- ✅ Logar via `AuditLogService.LogResponseAsync()`
- ✅ Excluir endpoints de infraestrutura (health, swagger, hangfire)
- ✅ Log estruturado via `ILogger` (Application Insights, Seq)

**Endpoints Excluídos**:
```csharp
/health
/healthz
/ready
/swagger
/swagger/index.html
/swagger/v1/swagger.json
/hangfire
```

---

### **2. GlobalExceptionHandler (Atualizado)**

**Arquivo**: `src/SGOFAPI.Host/Middleware/GlobalExceptionHandler.cs`

**Responsabilidades**:
- ✅ Capturar **exceções não tratadas**
- ✅ Criar `ResponseDTO` apropriado baseado no tipo de exceção
- ✅ Logar via `AuditLogService.LogResponseAsync()`
- ✅ Retornar resposta formatada ao cliente
- ✅ **Não deixar logging falhar** (try-catch ao logar)

**Tipos de Exceção Tratados**:
- `ValidationException` → 400 Bad Request
- `KeyNotFoundException` → 404 Not Found
- `ArgumentException` → 400 Bad Request
- `UnauthorizedAccessException` → 401 Unauthorized
- `SqlException` / `DbException` → 500 Internal Server Error
- `Exception` (genérico) → 500 Internal Server Error

---

### **3. AuditLogService**

**Arquivo**: `src/Modules/Audit/Audit.Application/Services/AuditLogService.cs`

**Responsabilidades**:
- ✅ Interface para logging de auditoria
- ✅ **LogResponseAsync()** - Enfileira job no Hangfire (assíncrono)
- ✅ **LogResponseSyncAsync()** - Salva diretamente no banco (síncrono - não recomendado)

---

### **4. AuditLogBackgroundJob**

**Arquivo**: `src/Modules/Audit/Audit.Infrastructure/BackgroundJobs/AuditLogBackgroundJob.cs`

**Responsabilidades**:
- ✅ Job executado pelo Hangfire
- ✅ Recebe dados do log
- ✅ Salva no banco via `AuditLogRepository`
- ✅ **Não bloqueia o request** (executa em background)

---

## 📊 Tabela de Auditoria (ulogs)

```sql
CREATE TABLE ulogs (
    ulogsstamp NVARCHAR(25) PRIMARY KEY,      -- Stamp único
    requestid NVARCHAR(50),                    -- Correlation ID
    data DATETIME,                             -- Timestamp
    code NVARCHAR(10),                         -- HTTP Status Code (200, 400, 500)
    content NVARCHAR(MAX),                     -- Content/mensagem curta
    ip NVARCHAR(45),                           -- IP do cliente
    responsedesc NVARCHAR(500),                -- Descrição da resposta
    responsetext NVARCHAR(MAX),                -- JSON completo da resposta
    operation NVARCHAR(100)                    -- "GET /api/parameters"
);

-- Índices para performance
CREATE INDEX IX_ulogs_RequestId ON ulogs(requestid);
CREATE INDEX IX_ulogs_Data ON ulogs(data DESC);
CREATE INDEX IX_ulogs_Code ON ulogs(code);
CREATE INDEX IX_ulogs_Operation ON ulogs(operation);
```

---

## ✅ Vantagens da Implementação

### **1. Auditoria Completa e Automática**
- ✅ **TODOS os requests** são logados (sucesso e erro)
- ✅ **Sem código manual** nos controllers
- ✅ **Correlation ID** em todos os logs
- ✅ **Rastreabilidade end-to-end**

### **2. Performance**
- ✅ **Assíncrono** - Hangfire processa em background
- ✅ **Não bloqueia requests** - Cliente recebe resposta imediatamente
- ✅ **Retry automático** - Hangfire retenta em caso de falha

### **3. Consistência**
- ✅ **Formato único** - Todos os logs seguem mesmo padrão
- ✅ **ResponseDTO** - Estrutura padronizada
- ✅ **Correlation ID** - Rastreamento de ponta a ponta

### **4. Observabilidade**
- ✅ **Logs estruturados** - JSON parseável
- ✅ **Métricas** - Tempo de execução, status codes
- ✅ **Debugging** - Fácil rastrear erros via Correlation ID

### **5. Compliance**
- ✅ **Auditoria legal** - Todos os acessos registrados
- ✅ **LGPD/GDPR** - Rastreamento de dados pessoais
- ✅ **SOC 2** - Logs de segurança

---

## 🚨 Cenários de Erro e Tratamento

### **1. Falha ao Logar (Hangfire Offline)**

```csharp
try
{
    _auditLogService.LogResponseAsync(...);
}
catch (Exception logEx)
{
    // ✅ NÃO deixar logging quebrar a response
    _logger.LogError(logEx, "Failed to log to audit service");
}
```

**Resultado**: Cliente recebe resposta normalmente, log é perdido.

---

### **2. Banco de Dados Offline**

**Fluxo**:
1. Request processa normalmente
2. Hangfire enfileira job
3. Job tenta salvar no banco → **FALHA**
4. Hangfire **retenta automaticamente** (3x por padrão)
5. Se falhar tudo, job vai para **Failed Jobs** no Dashboard

**Ação**: Admin monitora Hangfire Dashboard e processa failed jobs manualmente.

---

### **3. Response Body Muito Grande**

```csharp
// ⚠️ CUIDADO: Response > 100MB pode consumir memória
var responseBody = await ReadResponseBodyAsync(context.Response);
```

**Solução Futura**: Implementar limite de tamanho + stream diretamente para blob storage.

---

## 📈 Monitoramento

### **Logs Estruturados (Serilog)**

```csharp
_logger.LogInformation(
    "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMs}ms | CorrelationId: {CorrelationId}",
    "POST",
    "/api/parameters",
    201,
    145,
    "abc-123-def"
);
```

**Saída**:
```json
{
  "Timestamp": "2024-02-10T15:30:00.000Z",
  "Level": "Information",
  "MessageTemplate": "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMs}ms...",
  "Properties": {
    "Method": "POST",
    "Path": "/api/parameters",
    "StatusCode": 201,
    "ElapsedMs": 145,
    "CorrelationId": "abc-123-def"
  }
}
```

---

### **Hangfire Dashboard**

**URL**: `https://localhost:7046/hangfire`

**Métricas**:
- ✅ Jobs enfileirados (Enqueued)
- ✅ Jobs processados (Succeeded)
- ✅ Jobs falhados (Failed)
- ✅ Taxa de sucesso (Success Rate)

---

### **Queries Úteis para Monitoramento**

```sql
-- ✅ Total de logs por dia
SELECT CAST(data AS DATE) AS Dia, COUNT(*) AS Total
FROM ulogs
GROUP BY CAST(data AS DATE)
ORDER BY Dia DESC;

-- ✅ Erros por código
SELECT code, COUNT(*) AS Total
FROM ulogs
WHERE code >= '400'
GROUP BY code
ORDER BY Total DESC;

-- ✅ Operações mais lentas (se salvar elapsed time)
SELECT operation, AVG(CAST(responsedesc AS INT)) AS AvgMs
FROM ulogs
WHERE responsedesc LIKE '%ms%'
GROUP BY operation
ORDER BY AvgMs DESC;

-- ✅ Logs por Correlation ID (rastreamento)
SELECT *
FROM ulogs
WHERE requestid = 'abc-123-def'
ORDER BY data DESC;
```

---

## 🎯 Ordem dos Middlewares (CRÍTICO!)

```csharp
app.UseExceptionHandler();           // 1️⃣ PRIMEIRO - Captura exceções
app.UseMiddleware<CorrelationIdMiddleware>();  // 2️⃣ Correlation ID
app.UseMiddleware<ResponseLoggingMiddleware>(); // 3️⃣ Logging
app.UseSerilogRequestLogging();      // 4️⃣ Serilog (adicional)
app.UseHttpsRedirection();           // 5️⃣ HTTPS
app.UseCors("AllowAll");             // 6️⃣ CORS
app.UseAuthorization();              // 7️⃣ Auth
app.MapControllers();                // 8️⃣ Routing
```

**Por quê essa ordem?**
- `ExceptionHandler` **PRIMEIRO** - captura tudo
- `CorrelationIdMiddleware` **ANTES** do logging - garante ID em todos os logs
- `ResponseLoggingMiddleware` **DEPOIS** - captura responses já formatadas

---

## 📊 Performance Impact

### **Overhead Estimado**

| Ação | Tempo | Impacto |
|------|-------|---------|
| **MemoryStream substitution** | ~1ms | Mínimo |
| **Response body read** | ~2ms | Mínimo |
| **JSON parsing** | ~1ms | Mínimo |
| **Hangfire enqueue** | ~5ms | Baixo |
| **Total overhead** | **~10ms** | ✅ Aceitável |

**Nota**: Job do Hangfire executa **DEPOIS** da resposta ao cliente (0ms de impacto!)

---

## 🔮 Melhorias Futuras

### **1. Filtros Dinâmicos**

```csharp
public class AuditOptions
{
    public List<string> ExcludedPaths { get; set; } = new();
    public bool LogRequestBody { get; set; } = true;
    public bool LogResponseBody { get; set; } = true;
    public int MaxBodySize { get; set; } = 1024 * 100; // 100KB
}
```

### **2. Sampling (Amostragem)**

```csharp
// Logar apenas 10% dos GET requests (reduzir volume)
if (request.Method == "GET" && Random.Shared.Next(100) > 10)
{
    await _next(context);
    return;
}
```

### **3. Compressão de Logs**

```csharp
// Comprimir responsetext com Gzip antes de salvar
var compressed = Compress(JsonSerializer.Serialize(responseDto));
```

### **4. Armazenamento Híbrido**

```csharp
// Logs < 30 dias → SQL Server
// Logs > 30 dias → Azure Blob Storage (mais barato)
```

---

## ✅ Checklist de Implementação

- [x] **ResponseLoggingMiddleware** criado
- [x] **GlobalExceptionHandler** atualizado com logging
- [x] **Program.cs** registrado middleware
- [x] **AuditLogService** integrado
- [x] **Ordem dos middlewares** correta
- [x] **Documentação** completa
- [ ] **Testes** (pendente)
- [ ] **Monitoramento** em produção (Hangfire Dashboard)

---

## 🎓 Resumo Final

**O que mudou:**
- ✅ **Antes**: Cada controller precisava logar manualmente
- ✅ **Depois**: Logging **100% automático** via middleware

**Benefícios:**
- ✅ Auditoria completa (sucesso + erro)
- ✅ Correlation ID em todos os logs
- ✅ Performance não impactada (assíncrono)
- ✅ Código limpo (DRY)

**Fluxo Simplificado:**
```
Request → CorrelationID → ResponseLogging → Controller → Response → Hangfire → DB
```

---

**Data**: 10 de fevereiro de 2024  
**Status**: ✅ Implementado  
**Next Steps**: Testar em desenvolvimento, monitorar Hangfire Dashboard

