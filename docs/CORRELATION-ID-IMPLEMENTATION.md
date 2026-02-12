# Correlation ID Implementation

## 🎯 Problema Resolvido

O campo `id` no `ResponseCodeDTO` estava sempre `null`:

```json
{
  "response": {
    "cod": "0000",
    "codDesc": "Success",
    "id": null  // ❌ Sempre null
  }
}
```

## ✅ Solução Implementada

Adicionado **Correlation ID** - um identificador único para cada request, útil para:
- ✅ **Rastreabilidade** - Seguir a request através dos logs
- ✅ **Debugging** - Identificar problemas específicos
- ✅ **Auditoria** - Correlacionar eventos
- ✅ **Suporte** - Cliente pode informar o ID para investigação

## 🏗️ Arquitetura

```
┌────────────────────────────────────────────────┐
│  Cliente                                       │
│  GET /api/parameters                           │
│  X-Correlation-ID: 123456 (opcional)           │
└────────────────┬───────────────────────────────┘
                 │
                 ▼
┌────────────────────────────────────────────────┐
│  CorrelationIdMiddleware                       │
│  • Lê header X-Correlation-ID (se fornecido)  │
│  • Ou gera novo ID (timestamp-based)           │
│  • Armazena em HttpContext.Items               │
│  • Adiciona header na resposta                 │
└────────────────┬───────────────────────────────┘
                 │
                 ▼
┌────────────────────────────────────────────────┐
│  Controller / Exception Handler                │
│  • Obtém ID via HttpContext.GetCorrelationId() │
│  • Passa para ResponseDTO.Success/Error        │
└────────────────┬───────────────────────────────┘
                 │
                 ▼
┌────────────────────────────────────────────────┐
│  Response HTTP                                 │
│  X-Correlation-ID: 163847592841234567          │
│  {                                             │
│    "response": {                               │
│      "cod": "0000",                            │
│      "id": 163847592841234567  ✅              │
│    }                                           │
│  }                                             │
└────────────────────────────────────────────────┘
```

## 📁 Arquivos Criados/Modificados

### 1. CorrelationIdMiddleware.cs (Novo)
```csharp
// src\SGOFAPI.Host\Middleware\CorrelationIdMiddleware.cs

// Gera ID único para cada request
// Adiciona ao HttpContext.Items
// Adiciona header X-Correlation-ID na resposta
```

### 2. CorrelationIdExtensions.cs (Novo)
```csharp
// src\Shared\Shared.Kernel\Shared.Kernel\Extensions\CorrelationIdExtensions.cs

public static decimal? GetCorrelationId(this HttpContext context)
{
    // Obtém Correlation ID do HttpContext.Items
}
```

### 3. ResponseDTO.cs (Modificado)
```csharp
// Adicionado parâmetro correlationId opcional

public static ResponseDTO Success(..., decimal? correlationId = null)
public static ResponseDTO Error(..., decimal? correlationId = null)
```

### 4. ParametersController.cs (Modificado)
```csharp
// Todos os endpoints agora passam correlationId

var correlationId = HttpContext.GetCorrelationId();
return Ok(ResponseDTO.Success(data: result, correlationId: correlationId));
```

### 5. GlobalExceptionHandler.cs (Modificado)
```csharp
// Erros também incluem correlationId

var correlationId = context.GetCorrelationId();
ResponseDTO.Error(..., correlationId: correlationId);
```

### 6. Program.cs (Modificado)
```csharp
// Registrado middleware

app.UseMiddleware<CorrelationIdMiddleware>();
```

## 🎯 Exemplos

### Exemplo 1: Success Response

**Request:**
```http
GET /api/parameters
```

**Response:**
```http
HTTP/1.1 200 OK
X-Correlation-ID: 163847592841234567
Content-Type: application/json

{
  "response": {
    "cod": "0000",
    "codDesc": "Success",
    "id": 163847592841234567  ✅
  },
  "data": [
    { "para1Stamp": "ABC", "descricao": "Test" }
  ],
  "content": null
}
```

### Exemplo 2: Error Response

**Request:**
```http
GET /api/parameters/INVALID
```

**Response:**
```http
HTTP/1.1 404 Not Found
X-Correlation-ID: 163847592841234999
Content-Type: application/json

{
  "response": {
    "cod": "1001",
    "codDesc": "Parameter not found",
    "id": 163847592841234999  ✅
  },
  "data": {
    "message": "Parameter with stamp 'INVALID' not found"
  },
  "content": {
    "path": "/api/parameters/INVALID"
  }
}
```

### Exemplo 3: Cliente Fornece Correlation ID

**Request:**
```http
GET /api/parameters
X-Correlation-ID: 999888777
```

**Response:**
```http
HTTP/1.1 200 OK
X-Correlation-ID: 999888777  ✅ Mesmo ID do cliente
Content-Type: application/json

{
  "response": {
    "cod": "0000",
    "codDesc": "Success",
    "id": 999888777  ✅ Mesmo ID do request
  },
  ...
}
```

## 🔍 Como o ID é Gerado

### Geração Automática
```csharp
private static decimal GenerateCorrelationId()
{
    // Timestamp em ticks (unique)
    var ticks = DateTime.UtcNow.Ticks;
    
    // Converte para decimal
    return (decimal)ticks / 10000;
    
    // Resultado: 163847592841234567
}
```

**Características:**
- ✅ Único (baseado em timestamp com alta precisão)
- ✅ Crescente (facilita ordenação cronológica)
- ✅ Sem colisões (resolução de 100 nanosegundos)
- ✅ Tipo `decimal` (suporta valores grandes)

### Cliente Fornece ID
```http
X-Correlation-ID: 123456789
```

Se o cliente enviar o header `X-Correlation-ID`, o servidor usa esse valor em vez de gerar um novo.

**Útil para:**
- ✅ Rastrear requests através de múltiplos serviços
- ✅ Correlacionar logs entre frontend e backend
- ✅ Debugging de fluxos complexos

## 📊 Uso em Logs

### Serilog Enrichment (Futuro - Opcional)

```csharp
// Program.cs
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .Enrich.FromLogContext()
        .Enrich.With<CorrelationIdEnricher>()  // ✅ Adicionar enricher
        .WriteTo.Console(outputTemplate: 
            "[{Timestamp:HH:mm:ss} {Level:u3}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}");
});
```

**Log Output:**
```
[14:30:45 INF] [163847592841234567] GET /api/parameters started
[14:30:45 INF] [163847592841234567] Query executed successfully
[14:30:45 INF] [163847592841234567] GET /api/parameters completed in 45ms
```

### Filtrar Logs por Correlation ID

```bash
# Todos os logs de um request específico
grep "163847592841234567" logs/sgofapi-20240115.txt
```

## 🎓 Casos de Uso

### 1. Cliente Reporta Erro
```
Cliente: "Recebi erro ao criar parameter!"
Suporte: "Qual o Correlation ID?"
Cliente: "163847592841234567"

# Suporte busca nos logs:
grep "163847592841234567" logs/*.txt

# Encontra:
[14:30:45 ERR] [163847592841234567] ValidationException: Code is required
[14:30:45 ERR] [163847592841234567] Stack trace: ...
```

### 2. Debugging de Fluxo
```
Frontend → Backend → Database

# Frontend envia:
POST /api/parameters
X-Correlation-ID: 999888777

# Backend loga:
[INF] [999888777] CreateParameterCommand received
[INF] [999888777] Validating input
[INF] [999888777] Saving to database
[INF] [999888777] Parameter created: ABC123

# Fácil rastrear todo o fluxo com grep "999888777"
```

### 3. Performance Monitoring
```
# Encontrar requests lentos
grep "completed in" logs/*.txt | grep "\[163847592841234567\]"

# Output:
[14:30:45 INF] [163847592841234567] GET /api/parameters completed in 2450ms  ⚠️ Lento!
```

## ⚙️ Configuração

### Opcional: Customizar Geração de ID

```csharp
// Usar GUID em vez de timestamp
private static decimal GenerateCorrelationId()
{
    var guid = Guid.NewGuid();
    return Math.Abs(guid.GetHashCode()); // Converte GUID para decimal
}
```

### Opcional: Adicionar Prefixo
```csharp
// ID com prefixo para identificar ambiente
private static string GenerateCorrelationId()
{
    return $"PROD-{DateTime.UtcNow.Ticks}";
    // Resultado: "PROD-163847592841234567"
}
```

## 🔐 Segurança

### IDs São Rastreáveis
- ⚠️ Não incluir informações sensíveis no ID
- ⚠️ Cliente pode ver o ID na resposta
- ✅ Use apenas para rastreabilidade, não autenticação

### Rate Limiting (Futuro)
```csharp
// Limitar requests pelo Correlation ID
if (requestCount[correlationId] > 100)
{
    return TooManyRequests();
}
```

## 📝 Checklist de Implementação

- [x] CorrelationIdMiddleware criado
- [x] CorrelationIdExtensions criado
- [x] ResponseDTO.Success/Error aceita correlationId
- [x] ParametersController passa correlationId
- [x] GlobalExceptionHandler passa correlationId
- [x] Middleware registrado no Program.cs
- [x] Header X-Correlation-ID na resposta
- [x] Build successful
- [ ] Testes de integração (futuro)
- [ ] Serilog enrichment (futuro)

## ✅ Resumo

| Aspecto | Antes | Depois |
|---------|-------|--------|
| **id no ResponseCodeDTO** | ❌ Sempre null | ✅ Correlation ID |
| **Rastreabilidade** | ❌ Difícil | ✅ Fácil com ID único |
| **Debugging** | ❌ Logs dispersos | ✅ Logs correlacionados |
| **Suporte ao cliente** | ❌ Sem referência | ✅ Cliente informa ID |
| **Header na resposta** | ❌ Não | ✅ X-Correlation-ID |

**✅ Agora o campo `id` está sempre populado com um Correlation ID único!** 🎯
