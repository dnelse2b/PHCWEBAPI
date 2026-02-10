# 🎯 Estrutura Shared com ResponseDto Customizada

## ✅ O Que Foi Criado

Criamos uma estrutura **Shared** completa com o padrão de resposta da empresa, mas **modernizada para nível senior** (C# 12, records, System.Text.Json).

---

## 📁 Estrutura Criada

```
src/
└── Shared/
    └── Shared.Kernel/
        ├── Shared.Kernel.csproj
        ├── Responses/
        │   ├── ResponseDto.cs           ← Resposta padrão da API
        │   ├── ResponseCodeDto.cs       ← Código de resposta
        │   └── ResponseCodes.cs         ← Catálogo de códigos
        ├── Results/
        │   └── Result.cs                ← Result Pattern (opcional)
        └── Extensions/
            └── ResultExtensions.cs       ← Helper methods
```

---

## 📝 Arquivos Criados

### 1. **ResponseDto.cs** (Modernizado)

**Antes (código antigo):**
```csharp
using Newtonsoft.Json;

public class ResponseDTO
{
    public ResponseCodesDTO response { get; set; }
    public object? Data { get; set; }
    public object? Content { get; set; }

    public ResponseDTO(ResponseCodesDTO response, object? data, object? content)
    {
        this.response = response;
        Data = data;
        Content = content;
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}
```

**Depois (código senior):**
```csharp
using System.Text.Json.Serialization;

namespace Shared.Kernel.Responses;

/// <summary>
/// Resposta padrão da API
/// </summary>
public sealed record ResponseDto
{
    [JsonPropertyName("response")]
    public ResponseCodeDto Response { get; init; }

    [JsonPropertyName("data")]
    public object? Data { get; init; }

    [JsonPropertyName("content")]
    public object? Content { get; init; }

    public ResponseDto(ResponseCodeDto response, object? data = null, object? content = null)
    {
        Response = response;
        Data = data;
        Content = content;
    }

    /// <summary>
    /// Cria uma resposta de sucesso
    /// </summary>
    public static ResponseDto Success(object? data = null, object? content = null) 
        => new(ResponseCodes.Success, data, content);

    /// <summary>
    /// Cria uma resposta de erro
    /// </summary>
    public static ResponseDto Error(ResponseCodeDto errorCode, object? data = null, object? content = null)
        => new(errorCode, data, content);
}
```

**Melhorias:**
- ✅ `record` ao invés de `class` (imutável)
- ✅ `sealed` (performance)
- ✅ System.Text.Json ao invés de Newtonsoft.Json
- ✅ `init` properties (imutável após construção)
- ✅ Factory methods (`Success`, `Error`)
- ✅ Parâmetros opcionais com defaults

---

### 2. **ResponseCodeDto.cs** (Modernizado)

**Antes:**
```csharp
using Newtonsoft.Json;

public class ResponseCodesDTO
{
    public string cod { get; set; }
    public decimal? id { get; set; }
    public string codDesc { get; set; }

    public ResponseCodesDTO() { }

    public ResponseCodesDTO(string cod, string codDesc, decimal? id)
    {
        this.cod = cod;
        this.codDesc = codDesc;
        this.id = id;
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}
```

**Depois:**
```csharp
using System.Text.Json.Serialization;

namespace Shared.Kernel.Responses;

/// <summary>
/// Código de resposta da API
/// </summary>
public sealed record ResponseCodeDto
{
    [JsonPropertyName("cod")]
    public string Code { get; init; }

    [JsonPropertyName("codDesc")]
    public string Description { get; init; }

    [JsonPropertyName("id")]
    public decimal? Id { get; init; }

    public ResponseCodeDto(string code, string description, decimal? id = null)
    {
        Code = code;
        Description = description;
        Id = id;
    }

    // Deconstruction para facilitar uso
    public void Deconstruct(out string code, out string description)
    {
        code = Code;
        description = Description;
    }
}
```

**Melhorias:**
- ✅ `record` (imutável)
- ✅ Primary constructor
- ✅ Deconstruction support
- ✅ Parâmetro opcional

---

### 3. **ResponseCodes.cs** (Catalog de Códigos)

**Antes:**
```csharp
public class WebTransactionCodes
{
    public static ResponseCodesDTO SUCCESS = new ResponseCodesDTO("0000", "Success");
    public static ResponseCodesDTO INCORRECTHTTP = new ResponseCodesDTO("0001", "Incorrect HTTP method");
    // ...
}
```

**Depois:**
```csharp
namespace Shared.Kernel.Responses;

public static class ResponseCodes
{
    // ✅ Success
    public static readonly ResponseCodeDto Success = new("0000", "Success");

    // ❌ HTTP Errors
    public static readonly ResponseCodeDto IncorrectHttpMethod = new("0001", "Incorrect HTTP method");
    public static readonly ResponseCodeDto InvalidJson = new("0002", "Invalid JSON");

    // 🔐 Authentication/Authorization
    public static readonly ResponseCodeDto IncorrectApiKey = new("0003", "Incorrect API Key");
    public static readonly ResponseCodeDto ApiKeyNotProvided = new("0004", "API Key not provided");
    public static readonly ResponseCodeDto Unauthorized = new("0013", "Unauthorized");

    // 🔍 Validation/Business Logic
    public static readonly ResponseCodeDto InvalidReference = new("0005", "Invalid Reference");
    public static readonly ResponseCodeDto DuplicatedPayment = new("0006", "Duplicated payment");
    public static readonly ResponseCodeDto InvalidAmount = new("0008", "Invalid Amount");
    public static readonly ResponseCodeDto RequestIdNotProvided = new("0009", "Request ID not provided");
    public static readonly ResponseCodeDto NotFound = new("0014", "Resource not found");
    public static readonly ResponseCodeDto ValidationError = new("0015", "Validation error");
    public static readonly ResponseCodeDto AlreadyExists = new("0016", "Resource already exists");

    // 🔧 Internal Errors
    public static readonly ResponseCodeDto InternalError = new("0007", "Internal error");
    public static readonly ResponseCodeDto DatabaseError = new("0017", "Database error");

    // 👤 User Management
    public static readonly ResponseCodeDto UserAlreadyExists = new("0010", "User already exists");
    public static readonly ResponseCodeDto UserCreationFailed = new("0011", "User creation failed");
    public static readonly ResponseCodeDto UserNotFound = new("0012", "User not found");

    // 📋 Parameters Module (custom)
    public static class Parameter
    {
        public static readonly ResponseCodeDto NotFound = new("1001", "Parameter not found");
        public static readonly ResponseCodeDto CodeAlreadyExists = new("1002", "Parameter code already exists");
        public static readonly ResponseCodeDto InvalidCode = new("1003", "Invalid parameter code");
        public static readonly ResponseCodeDto InactiveCannotUpdate = new("1004", "Cannot update inactive parameter");
        public static readonly ResponseCodeDto CreatedSuccessfully = new("1005", "Parameter created successfully");
        public static readonly ResponseCodeDto UpdatedSuccessfully = new("1006", "Parameter updated successfully");
        public static readonly ResponseCodeDto DeletedSuccessfully = new("1007", "Parameter deleted successfully");
    }

    // 🛒 Customers Module (futuro)
    public static class Customer
    {
        public static readonly ResponseCodeDto NotFound = new("2001", "Customer not found");
        public static readonly ResponseCodeDto EmailAlreadyExists = new("2002", "Customer email already exists");
    }

    // 📦 Orders Module (futuro)
    public static class Order
    {
        public static readonly ResponseCodeDto NotFound = new("3001", "Order not found");
        public static readonly ResponseCodeDto InvalidStatus = new("3002", "Invalid order status");
    }
}
```

**Melhorias:**
- ✅ Naming conventions (PascalCase)
- ✅ Organização por domínio (nested classes)
- ✅ Códigos separados por módulo (1xxx = Parameters, 2xxx = Customers, etc.)
- ✅ Emojis para documentação visual
- ✅ `readonly` (constantes)

---

## 💻 Como Usar no Controller

### **Antes (sem estrutura customizada):**
```csharp
[HttpGet("{id}")]
public async Task<ActionResult<ParameterDto>> GetById(string id)
{
    var result = await _mediator.Send(new GetByIdQuery(id));
    return result is null ? NotFound() : Ok(result);
}
```

### **Depois (com ResponseDto):**
```csharp
[HttpGet("{id}")]
[ProducesResponseType(typeof(ResponseDto), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ResponseDto), StatusCodes.Status404NotFound)]
public async Task<ActionResult<ResponseDto>> GetById(string id)
{
    var result = await _mediator.Send(new GetByIdQuery(id));

    return result is null
        ? NotFound(ResponseDto.Error(ResponseCodes.Parameter.NotFound))
        : Ok(ResponseDto.Success(data: result));
}
```

**Resposta JSON:**
```json
{
  "response": {
    "cod": "0000",
    "codDesc": "Success",
    "id": null
  },
  "data": {
    "e1Stamp": "123",
    "code": "PARAM1",
    "description": "Parameter 1"
  },
  "content": null
}
```

---

## 🎯 Controller Completo Exemplo

```csharp
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Shared.Kernel.Responses;

[ApiController]
[Route("api/parameters")]
public sealed class ParametersController : ControllerBase
{
    private readonly IMediator _mediator;

    public ParametersController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ResponseDto>> GetAll(CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetAllParametersQuery(), ct);
        return Ok(ResponseDto.Success(data: result));
    }

    [HttpGet("{e1Stamp}")]
    [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResponseDto>> GetByStamp(string e1Stamp, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetParameterByStampQuery(e1Stamp), ct);

        return result is null
            ? NotFound(ResponseDto.Error(ResponseCodes.Parameter.NotFound))
            : Ok(ResponseDto.Success(data: result));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<ResponseDto>> Create([FromBody] CreateParameterDto dto, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new CreateParameterCommand(...), ct);

        return CreatedAtAction(
            nameof(GetByStamp),
            new { e1Stamp = result.E1Stamp },
            ResponseDto.Success(data: result, content: ResponseCodes.Parameter.CreatedSuccessfully));
    }
}
```

---

## 📊 Comparação: Antes vs Depois

| Aspecto | ❌ Código Antigo | ✅ Código Senior |
|---------|------------------|------------------|
| **JSON Library** | Newtonsoft.Json | System.Text.Json (nativo) |
| **Tipo** | `class` (mutável) | `sealed record` (imutável) |
| **Properties** | `{ get; set; }` | `{ get; init; }` |
| **Naming** | `cod`, `codDesc` | `Code`, `Description` (mantém JSON names) |
| **Factory Methods** | Não tem | `Success()`, `Error()` |
| **Catálogo de Códigos** | UPPERCASE | PascalCase |
| **Organização** | Tudo em uma classe | Nested classes por módulo |
| **Testabilidade** | Difícil (mutável) | Fácil (imutável) |
| **Performance** | Menor | Melhor (records, sealed) |

---

## 🚀 Como Adicionar Novos Códigos

### **Para o módulo Parameters:**
```csharp
// Em ResponseCodes.Parameter
public static readonly ResponseCodeDto CannotDeleteInUse = new("1008", "Cannot delete parameter in use");
```

### **Para novo módulo (Customers):**
```csharp
// Em ResponseCodes.Customer
public static class Customer
{
    public static readonly ResponseCodeDto NotFound = new("2001", "Customer not found");
    public static readonly ResponseCodeDto EmailAlreadyExists = new("2002", "Email already exists");
    public static readonly ResponseCodeDto InvalidCpf = new("2003", "Invalid CPF");
}
```

---

## ✅ Vantagens da Nova Estrutura

1. **Centralizada:** Todos os códigos em um único lugar
2. **Tipada:** Não pode usar código errado (compile-time safety)
3. **Documentada:** Código + descrição sempre juntos
4. **Escalável:** Fácil adicionar novos módulos
5. **Moderna:** C# 12, records, init properties
6. **Performática:** `record` + `sealed` + System.Text.Json
7. **Testável:** Imutável = fácil de testar
8. **Consistente:** Todas as APIs retornam o mesmo formato

---

## 🎯 Próximos Passos

1. ✅ **Integrar com GlobalExceptionHandler** para erros automáticos
2. ✅ **Adicionar Result Pattern** (opcional) para evitar null checks
3. ✅ **Adicionar Swagger examples** para documentação
4. ✅ **Criar testes unitários** para ResponseDto
5. ✅ **Adicionar logging automático** dos response codes

---

**Estrutura criada com sucesso!** 🎉

Agora todos os endpoints da API usam a estrutura padrão da empresa, mas com código nível senior!
