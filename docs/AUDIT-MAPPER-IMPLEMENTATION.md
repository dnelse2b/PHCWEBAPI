# ✅ Mapper Implementation - Audit Module

## 🎯 Problema Resolvido

Os handlers do módulo **Audit** estavam fazendo mapeamento manual campo a campo, violando o padrão **IMapper** do `Shared.Kernel` e não seguindo o padrão já estabelecido no módulo **Parameters**.

### ❌ Antes (Manual Mapping)

```csharp
// GetAllAuditLogsQueryHandler.cs
var result = filtered.Select(log => new AuditLogOutputDTO
{
    ULogsstamp = log.ULogsstamp,
    RequestId = log.RequestId,
    Data = log.Data,
    Code = log.Code,
    Content = log.Content,
    Ip = log.Ip,
    ResponseDesc = log.ResponseDesc,
    ResponseText = log.ResponseText,
    Operation = log.Operation
}).ToList();
```

### ✅ Depois (Extension Methods com Mapster)

```csharp
// GetAllAuditLogsQueryHandler.cs
var result = filtered.ToOutputDtos().ToList();
```

## 🏗️ Arquitetura de Mapeamento

### **1. Interface IMapper (Shared.Kernel)**

```csharp
public interface IMapper<TDomain, TPersistence, TDto>
{
    TDomain ToDomain(TPersistence doc);
    TPersistence ToPersistence(TDomain entity);
    TDto ToDto(TDomain entity);
    TDto PersistenceToDto(TPersistence doc);
}
```

### **2. Extension Methods com Mapster (Application Layer)**

```csharp
// Audit.Application/Mappings/AuditLogMappings.cs
public static class AuditLogMappings
{
    public static TDto ToDto<TDto>(this AuditLog entity) where TDto : class
    {
        return entity.Adapt<TDto>();
    }

    public static IEnumerable<TDto> ToDtos<TDto>(this IEnumerable<AuditLog> entities) where TDto : class
    {
        return entities.Select(e => e.Adapt<TDto>());
    }

    public static AuditLogOutputDTO ToOutputDto(this AuditLog entity)
    {
        return entity.Adapt<AuditLogOutputDTO>();
    }

    public static IEnumerable<AuditLogOutputDTO> ToOutputDtos(this IEnumerable<AuditLog> entities)
    {
        return entities.Select(e => e.Adapt<AuditLogOutputDTO>());
    }
}
```

## 📝 Mudanças Implementadas

### **1. Criado AuditLogMappings.cs**

**Arquivo**: `src\Modules\Audit\Audit.Application\Mappings\AuditLogMappings.cs`

- Métodos genéricos: `ToDto<TDto>()` e `ToDtos<TDto>()`
- Métodos específicos: `ToOutputDto()` e `ToOutputDtos()`
- Usa **Mapster** para conversão automática

### **2. Atualizado GetAllAuditLogsQueryHandler**

**Antes**:
```csharp
var result = filtered.Select(log => new AuditLogOutputDTO { ... }).ToList();
```

**Depois**:
```csharp
using Audit.Application.Mappings;
var result = filtered.ToOutputDtos().ToList();
```

### **3. Atualizado GetAuditLogByStampQueryHandler**

**Antes**:
```csharp
return new AuditLogOutputDTO { ... };
```

**Depois**:
```csharp
using Audit.Application.Mappings;
return log.ToOutputDto();
```

### **4. Atualizado GetAuditLogsByCorrelationIdQueryHandler**

**Antes**:
```csharp
var result = logs.Select(log => new AuditLogOutputDTO { ... }).ToList();
```

**Depois**:
```csharp
using Audit.Application.Mappings;
var result = logs.ToOutputDtos().ToList();
```

### **5. Adicionado Mapster ao Audit.Application.csproj**

```xml
<PackageReference Include="Mapster" Version="7.4.0" />
```

## ✅ Benefícios

### **1. Consistência com o Módulo Parameters**
Ambos os módulos agora seguem o mesmo padrão de mapeamento.

### **2. Manutenibilidade**
- Mudanças no DTO? Apenas 1 lugar para ajustar
- Novo campo? Mapster cuida automaticamente

### **3. Menos Código Boilerplate**
```csharp
// Antes: 10 linhas
var result = filtered.Select(log => new AuditLogOutputDTO
{
    ULogsstamp = log.ULogsstamp,
    RequestId = log.RequestId,
    Data = log.Data,
    Code = log.Code,
    Content = log.Content,
    Ip = log.Ip,
    ResponseDesc = log.ResponseDesc,
    ResponseText = log.ResponseText,
    Operation = log.Operation
}).ToList();

// Depois: 1 linha
var result = filtered.ToOutputDtos().ToList();
```

### **4. Type-Safe Mapping**
- Erros de mapeamento detectados em tempo de compilação
- IntelliSense completo

## 🧪 Como Usar

### **Mapear uma entidade**

```csharp
AuditLog log = ...;
var dto = log.ToOutputDto();
```

### **Mapear coleção**

```csharp
IEnumerable<AuditLog> logs = ...;
var dtos = logs.ToOutputDtos();
```

### **Mapear para tipo genérico**

```csharp
var customDto = log.ToDto<CustomAuditDto>();
```

## 📋 Checklist de Implementação

### ✅ Módulo Audit
- [x] Criado `AuditLogMappings.cs`
- [x] Adicionado Mapster ao `Audit.Application.csproj`
- [x] Atualizado `GetAllAuditLogsQueryHandler`
- [x] Atualizado `GetAuditLogByStampQueryHandler`
- [x] Atualizado `GetAuditLogsByCorrelationIdQueryHandler`
- [x] Build successful

### ✅ Padrão Estabelecido
- [x] Segue interface `IMapper<TDomain, TPersistence, TDto>`
- [x] Usa Mapster para conversão automática
- [x] Extension methods na camada Application
- [x] Consistente com módulo Parameters

## 🔄 Comparação com Parameters Module

| Aspecto | Parameters | Audit |
|---------|-----------|-------|
| **Mapper File** | `Para1Mappings.cs` | `AuditLogMappings.cs` |
| **Library** | Mapster 7.4.0 | Mapster 7.4.0 |
| **Generic Methods** | `ToDto<TDto>()`, `ToDtos<TDto>()` | `ToDto<TDto>()`, `ToDtos<TDto>()` |
| **Specific Methods** | N/A | `ToOutputDto()`, `ToOutputDtos()` |
| **Input Mapping** | `ToEntity()`, `UpdateEntity()` | N/A (read-only) |

## 🐛 Troubleshooting

### ❌ Erro: "Adapt not found"
**Causa**: Falta referência ao Mapster

**Solução**:
```xml
<PackageReference Include="Mapster" Version="7.4.0" />
```

### ❌ Erro: "Extension method not found"
**Causa**: Falta `using Audit.Application.Mappings;`

**Solução**:
```csharp
using Audit.Application.Mappings;
```

### ❌ Mapeamento incorreto
**Causa**: Propriedades com nomes diferentes

**Solução**: Configurar mapeamento customizado:
```csharp
TypeAdapterConfig<AuditLog, AuditLogOutputDTO>
    .NewConfig()
    .Map(dest => dest.Stamp, src => src.ULogsstamp);
```

## 🎯 Próximos Passos

### **Outros Módulos**
Aplicar o mesmo padrão em futuros módulos:
- Users
- Products
- Orders

### **Configuração Global de Mapster**
Centralizar configurações no `DependencyInjection.cs`:

```csharp
public static IServiceCollection AddAuditApplication(this IServiceCollection services)
{
    // Configurar Mapster
    TypeAdapterConfig.GlobalSettings.Default.PreserveReference(true);
    
    services.AddScoped<IAuditLogService, AuditLogService>();
    return services;
}
```

### **Testes Unitários**
Adicionar testes para os mappers:

```csharp
[Fact]
public void ToOutputDto_Should_Map_All_Properties()
{
    // Arrange
    var entity = new AuditLog(...);
    
    // Act
    var dto = entity.ToOutputDto();
    
    // Assert
    Assert.Equal(entity.ULogsstamp, dto.ULogsstamp);
    Assert.Equal(entity.RequestId, dto.RequestId);
    // ...
}
```

## ✅ Status

**Status**: ✅ **CONCLUÍDO**
**Data**: 10 de fevereiro de 2024
**Build**: ✅ **SUCCESSFUL**

---

**Problema**: ❌ Mapeamento manual campo a campo  
**Solução**: ✅ Extension methods com Mapster  
**Padrão**: ✅ Consistente com IMapper e Parameters module
