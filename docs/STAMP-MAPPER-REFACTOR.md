# 🎯 Refatoração: Stamps e Mappers

## ❌ Problemas Resolvidos:

### **1. Mapeamento Manual no Handler**

**Antes:**
```csharp
// ❌ Handler mapeando manualmente (inconsistente!)
var para1 = new Para1(
    stamp,
    request.Dto.Descricao,  // Campo por campo...
    request.Dto.Valor,
    request.Dto.Tipo,
    request.Dto.Dec,
    request.Dto.Tam,
    request.CriadoPor
);
```

**Depois:**
```csharp
// ✅ Mapper faz tudo!
var para1 = _mapper.ToEntity(request.Dto, request.CriadoPor);
```

---

### **2. GenerateStamp Acoplado**

**Antes:**
```csharp
// ❌ Duplicado em cada Handler!
private static string GenerateStamp()
{
    return DateTime.UtcNow.ToString("yyyyMMddHHmmss") + Guid.NewGuid().ToString("N")[..8].ToUpper();
}
```

**Depois:**
```csharp
// ✅ Extension Method reutilizável!
using Shared.Kernel.Extensions;

var stamp = StampExtensions.GeneratePHCStamp();
// Ou mais elegante:
var stamp = 25.GenerateStamp();  // Tamanho configurável!
```

---

## ✅ Solução Implementada:

### **1. Extension Methods para Stamps (Shared.Kernel)**

```csharp
// Shared.Kernel/Extensions/StampExtensions.cs

public static class StampExtensions
{
    // Gerar stamp com tamanho configurável
    public static string GenerateStamp(this int size, bool upperCase = true)
    {
        // Exemplo: 25.GenerateStamp()  →  "ABCDEFGHIJKLMNOPQRSTUVWXY"
    }

    // Gerar stamp padrão PHC (Timestamp + 8 chars)
    public static string GeneratePHCStamp()
    {
        // Exemplo: "20241201153045ABCDEFGH"
    }

    // Gerar RequestId com prefixo
    public static string GenerateRequestId(string prefix = "OPMT")
    {
        // Exemplo: "OPMT507f1f77-bcf8-6cd7"
    }

    // Gerar stamp customizado
    public static string GenerateCustomStamp(
        string prefix, 
        int randomSize = 8, 
        bool includeTimestamp = true)
    {
        // Exemplo: "PARA20241201153045ABCDEFGH"
    }
}
```

---

### **2. Mapper com ToEntity()**

```csharp
// CreateParameterMapper.cs

public class CreateParameterMapper : IMapper<Para1, Para1, ParameterDto>
{
    // ✅ NOVO: DTO → Entity
    public Para1 ToEntity(CreateParameterDto dto, string? createdBy)
    {
        return new Para1(
            StampExtensions.GeneratePHCStamp(),  // ← Extension method!
            dto.Descricao,
            dto.Valor,
            dto.Tipo,
            dto.Dec,
            dto.Tam,
            createdBy
        );
    }

    // Outros métodos...
    public ParameterDto ToDto(Para1 entity) => entity.Adapt<ParameterDto>();
}
```

---

### **3. Handler Limpo**

```csharp
// CreateParameterCommandHandler.cs

public class CreateParameterCommandHandler : IRequestHandler<...>
{
    private readonly CreateParameterMapper _mapper;  // ← Concreto!

    public async Task<ParameterDto> Handle(...)
    {
        // ✅ Mapper faz DTO → Entity (inclui stamp)
        var para1 = _mapper.ToEntity(request.Dto, request.CriadoPor);

        // Salvar
        var saved = await _repository.AddAsync(para1, ct);

        // ✅ Mapper faz Entity → DTO
        return _mapper.ToDto(saved);
    }
}
```

---

## 🎨 Exemplos de Uso:

### **Stamps Configuráveis:**

```csharp
// Stamp simples (8 caracteres)
var stamp1 = 8.GenerateStamp();
// "ABCDEFGH"

// Stamp grande (25 caracteres)
var stamp2 = 25.GenerateStamp();
// "ABCDEFGHIJKLMNOPQRSTUVWXY"

// Stamp lowercase
var stamp3 = 10.GenerateStamp(upperCase: false);
// "abcdefghij"

// Stamp padrão PHC
var stamp4 = StampExtensions.GeneratePHCStamp();
// "20241201153045ABCDEFGH"

// RequestId customizado
var reqId = StampExtensions.GenerateRequestId("PARA");
// "PARA507f1f77-bcf8-6cd7"

// Stamp totalmente customizado
var custom = StampExtensions.GenerateCustomStamp(
    prefix: "PARA",
    randomSize: 10,
    includeTimestamp: true
);
// "PARA20241201153045ABCDEFGHIJ"
```

---

## 📊 Comparação Antes x Depois:

### **Handler:**

| Aspecto | Antes ❌ | Depois ✅ |
|---------|---------|----------|
| **Linhas de código** | 15 | 3 |
| **Mapeamento** | Manual | Mapper |
| **Stamp** | Acoplado | Extension Method |
| **Reutilizável** | NÃO | SIM |
| **Testável** | Difícil | Fácil |

### **Stamp Generation:**

| Aspecto | Antes ❌ | Depois ✅ |
|---------|---------|----------|
| **Localização** | Cada Handler | Shared.Kernel |
| **Duplicação** | SIM | NÃO |
| **Configurável** | NÃO | SIM |
| **Reutilizável** | NÃO | Todos os módulos |

---

## 🚀 Benefícios Alcançados:

### **1. Handler Super Limpo:**
```csharp
public async Task<ParameterDto> Handle(...)
{
    var entity = _mapper.ToEntity(request.Dto, request.CriadoPor);  // 1 linha
    var saved = await _repository.AddAsync(entity, ct);             // 1 linha
    return _mapper.ToDto(saved);                                    // 1 linha
}
// Total: 3 linhas! (antes: 15 linhas)
```

### **2. Stamps Reutilizáveis em Todos os Módulos:**
```csharp
// Módulo Parameters
var paraStamp = StampExtensions.GeneratePHCStamp();

// Módulo Clientes
var clienteStamp = 30.GenerateStamp();

// Módulo Pedidos
var pedidoId = StampExtensions.GenerateCustomStamp("PED", 8, true);
```

### **3. Testabilidade:**
```csharp
[Fact]
public void GenerateStamp_Should_Return_Correct_Size()
{
    var stamp = 25.GenerateStamp();
    Assert.Equal(25, stamp.Length);
}

[Fact]
public void Mapper_ToEntity_Should_Create_Para1_With_Stamp()
{
    var mapper = new CreateParameterMapper();
    var dto = new CreateParameterDto { Descricao = "Test", ... };
    
    var entity = mapper.ToEntity(dto, "user");
    
    Assert.NotNull(entity.ParaStamp);
    Assert.Equal(22, entity.ParaStamp.Length);  // Timestamp(14) + Random(8)
}
```

---

## 📁 Arquivos Modificados:

1. ✅ **Shared.Kernel/Extensions/StampExtensions.cs** - NOVO
2. ✅ **CreateParameterMapper.cs** - Adicionado `ToEntity()`
3. ✅ **CreateParameterCommandHandler.cs** - Usa mapper
4. ✅ **DependencyInjection.cs** - Registra mapper concreto

---

## 🎯 Próximos Passos:

### **Para Outros Handlers:**

Aplicar o mesmo padrão:

```csharp
// UpdateParameterMapper.cs
public Para1 UpdateEntity(Para1 existing, UpdateParameterDto dto, string? updatedBy)
{
    existing.Update(dto.Descricao, dto.Valor, dto.Tipo, dto.Dec, dto.Tam, updatedBy);
    return existing;
}

// UpdateParameterCommandHandler.cs
public async Task<ParameterDto> Handle(...)
{
    var para1 = await _repository.GetByStampAsync(request.ParaStamp, ct);
    var updated = _mapper.UpdateEntity(para1, request.Dto, request.AtualizadoPor);
    await _repository.UpdateAsync(updated, ct);
    return _mapper.ToDto(updated);
}
```

---

## ✅ Conclusão:

**Código Limpo + Reutilizável + Testável + Configurável!**

🎯 **Handler:** 3 linhas (antes: 15)  
🎯 **Stamp:** Configurável em qualquer módulo  
🎯 **Mapper:** Faz DTO ↔ Entity  
🎯 **Zero Duplicação:** Extension methods compartilhados  

**Arquitetura Senior-Level Perfeita!** 🚀
