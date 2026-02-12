# Global Exception Handler - ResponseDTO Migration

## 🎯 Mudança Realizada

Migrado de **ProblemDetails** (RFC 7807) para **ResponseDTO** (padrão do projeto).

## 📊 Antes vs Depois

### ❌ ANTES (ProblemDetails)

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Validation Error",
  "status": 400,
  "detail": "One or more validation errors occurred.",
  "instance": "/api/parameters",
  "errors": {
    "Code": ["Code is required"],
    "Description": ["Description is required"]
  }
}
```

**Problemas:**
- ❌ Formato RFC 7807 (não era o padrão do projeto)
- ❌ Inconsistente com outros endpoints
- ❌ Clientes precisavam lidar com 2 formatos diferentes

### ✅ DEPOIS (ResponseDTO)

```json
{
  "response": {
    "cod": "0015",
    "codDesc": "Validation error",
    "id": null
  },
  "data": {
    "errors": {
      "Code": ["Code is required"],
      "Description": ["Description is required"]
    }
  },
  "content": {
    "path": "/api/parameters"
  }
}
```

**Vantagens:**
- ✅ Formato consistente em toda a API
- ✅ Códigos de erro padronizados (0000-9999)
- ✅ Cliente usa sempre o mesmo parsing
- ✅ Facilita logs e monitoramento

## 🔧 Implementação

### GlobalExceptionHandler.cs

**Antes:**
```csharp
private static ProblemDetails CreateProblemDetails(Exception ex, HttpContext ctx)
{
    return ex switch
    {
        ValidationException => new ValidationProblemDetails(...) { Status = 400 },
        KeyNotFoundException => new ProblemDetails { Status = 404 },
        // ...
    };
}
```

**Depois:**
```csharp
private static (ResponseDTO response, int statusCode) CreateResponse(Exception ex, HttpContext ctx)
{
    return ex switch
    {
        ValidationException validationEx => (
            ResponseDTO.Error(ResponseCodes.ValidationError, data: new { errors = ... }),
            StatusCodes.Status400BadRequest
        ),
        KeyNotFoundException notFoundEx => (
            ResponseDTO.Error(ResponseCodes.NotFound, data: new { message = ... }),
            StatusCodes.Status404NotFound
        ),
        // ...
    };
}
```

## 📋 Mapeamento de Exceções → ResponseCodes

| Exception | HTTP Status | Response Code | Description |
|-----------|-------------|---------------|-------------|
| `ValidationException` | 400 | `0015` | Validation error |
| `KeyNotFoundException` | 404 | `0014` | Resource not found |
| `ArgumentException` | 400 | `0015` | Validation error |
| `UnauthorizedAccessException` | 401 | `0013` | Unauthorized |
| `InvalidOperationException` | 400 | `0015` | Validation error |
| **Outros** | 500 | `0007` | Internal error |

## 🎯 Exemplos de Respostas

### 1. Validation Error (400)

**Request:**
```http
POST /api/parameters
Content-Type: application/json

{
  "code": "",
  "description": ""
}
```

**Response:**
```json
{
  "response": {
    "cod": "0015",
    "codDesc": "Validation error",
    "id": null
  },
  "data": {
    "errors": {
      "Code": ["Code is required", "Code must be at least 3 characters"],
      "Description": ["Description is required"]
    }
  },
  "content": {
    "path": "/api/parameters"
  }
}
```

### 2. Not Found (404)

**Request:**
```http
GET /api/parameters/INVALID_STAMP
```

**Response:**
```json
{
  "response": {
    "cod": "0014",
    "codDesc": "Resource not found",
    "id": null
  },
  "data": {
    "message": "Parameter with stamp 'INVALID_STAMP' not found"
  },
  "content": {
    "path": "/api/parameters/INVALID_STAMP"
  }
}
```

### 3. Unauthorized (401)

**Request:**
```http
DELETE /api/parameters/123
Authorization: Bearer INVALID_TOKEN
```

**Response:**
```json
{
  "response": {
    "cod": "0013",
    "codDesc": "Unauthorized",
    "id": null
  },
  "data": {
    "message": "Authentication is required to access this resource."
  },
  "content": {
    "path": "/api/parameters/123"
  }
}
```

### 4. Internal Server Error (500)

**Request:**
```http
GET /api/parameters
```

**Response:**
```json
{
  "response": {
    "cod": "0007",
    "codDesc": "Internal error",
    "id": null
  },
  "data": {
    "message": "An unexpected error occurred. Please try again later."
  },
  "content": {
    "path": "/api/parameters"
  }
}
```

## 🏗️ Arquitetura

```
┌─────────────────────────────────────────┐
│     Exception thrown                    │
│     (ValidationException,               │
│      KeyNotFoundException, etc.)        │
└─────────────────┬───────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────┐
│   GlobalExceptionHandler                │
│   • Captura exceção                     │
│   • Mapeia para ResponseCode            │
│   • Cria ResponseDTO                    │
│   • Define HTTP Status Code             │
└─────────────────┬───────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────┐
│   HTTP Response                         │
│   • Status Code: 400/404/401/500        │
│   • Content-Type: application/json      │
│   • Body: ResponseDTO                   │
└─────────────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────┐
│   Cliente                               │
│   • Parse ResponseDTO                   │
│   • Verifica response.cod               │
│   • Exibe mensagem ao usuário           │
└─────────────────────────────────────────┘
```

## 📦 Dependências

**Adicionado:**
```csharp
using Shared.Kernel.Responses;  // ResponseDTO, ResponseCodes
```

**Removido:**
```csharp
using Microsoft.AspNetCore.Mvc;  // ProblemDetails (não mais necessário)
```

## 🔄 Compatibilidade

### Para Novos Endpoints
✅ Já usam ResponseDTO automaticamente

### Para Endpoints Existentes
✅ Mantém ResponseDTO (sem mudanças)

### Para Clientes
⚠️ Se clientes antigos esperavam ProblemDetails, precisam ser atualizados

**Exemplo Cliente (JavaScript):**

**Antes:**
```javascript
// Cliente espera ProblemDetails
try {
  const response = await fetch('/api/parameters');
  if (!response.ok) {
    const problem = await response.json();
    console.error(problem.title, problem.detail);  // ❌ Não funciona mais
  }
} catch (error) {
  console.error(error);
}
```

**Depois:**
```javascript
// Cliente usa ResponseDTO
try {
  const response = await fetch('/api/parameters');
  const result = await response.json();
  
  if (result.response.cod !== "0000") {
    console.error(result.response.codDesc);  // ✅ Funciona
    console.error(result.data.message);
  } else {
    console.log(result.data);  // Sucesso
  }
} catch (error) {
  console.error(error);
}
```

## 🎓 Benefícios da Mudança

### 1. **Consistência**
```
✅ Todos os endpoints retornam ResponseDTO
✅ Mesma estrutura para sucesso e erro
✅ Cliente usa mesma lógica de parsing
```

### 2. **Rastreabilidade**
```
✅ Códigos de erro padronizados (0000-9999)
✅ Fácil filtrar logs por código
✅ Documentação centralizada em ResponseCodes.cs
```

### 3. **Modularidade**
```
✅ Cada módulo pode ter seus próprios códigos
   - Parameters: 1000-1999
   - Orders: 2000-2999
   - Customers: 3000-3999
✅ Evita conflitos entre módulos
```

### 4. **Internacionalização**
```
✅ Cliente pode mapear códigos para mensagens localizadas
   - 0015 → "Erro de validação" (PT)
   - 0015 → "Validation error" (EN)
   - 0015 → "Error de validación" (ES)
```

## 🧪 Como Testar

### 1. Testar Validation Error
```bash
curl -X POST http://localhost:5000/api/parameters \
  -H "Content-Type: application/json" \
  -d '{"code":"","description":""}'
```

**Espera-se:**
```json
{
  "response": { "cod": "0015", "codDesc": "Validation error" },
  "data": { "errors": { "Code": [...], "Description": [...] } }
}
```

### 2. Testar Not Found
```bash
curl -X GET http://localhost:5000/api/parameters/INVALID
```

**Espera-se:**
```json
{
  "response": { "cod": "0014", "codDesc": "Resource not found" },
  "data": { "message": "..." }
}
```

### 3. Testar Internal Error
```bash
# Forçar erro no banco (desconectar, por exemplo)
curl -X GET http://localhost:5000/api/parameters
```

**Espera-se:**
```json
{
  "response": { "cod": "0007", "codDesc": "Internal error" },
  "data": { "message": "An unexpected error occurred..." }
}
```

## 📝 Próximos Passos

### Opcional: Adicionar Novos Códigos de Erro

```csharp
// ResponseCodes.cs
public static class ResponseCodes
{
    // Adicionar novos códigos conforme necessário
    public static readonly ResponseCodeDTO DatabaseConnectionError = 
        new("0018", "Database connection error");
    
    public static readonly ResponseCodeDTO TimeoutError = 
        new("0019", "Request timeout");
}
```

### Opcional: Incluir Stack Trace em Dev

```csharp
// GlobalExceptionHandler.cs
private (ResponseDTO, int) CreateResponse(Exception ex, HttpContext ctx)
{
    var includeStackTrace = _environment.IsDevelopment();
    
    return (
        ResponseDTO.Error(
            ResponseCodes.InternalError,
            data: new 
            { 
                message = ex.Message,
                stackTrace = includeStackTrace ? ex.StackTrace : null  // ✅ Apenas em dev
            }
        ),
        500
    );
}
```

## ✅ Resumo

| Aspecto | ProblemDetails | ResponseDTO |
|---------|----------------|-------------|
| **Formato** | RFC 7807 | Customizado |
| **Consistência** | ❌ Diferente dos endpoints | ✅ Igual aos endpoints |
| **Códigos** | HTTP only | HTTP + Custom |
| **i18n** | ❌ Difícil | ✅ Fácil |
| **Modular** | ❌ Não | ✅ Sim |

**✅ Migração concluída com sucesso!** Toda a API agora usa **ResponseDTO** consistentemente. 🎯
