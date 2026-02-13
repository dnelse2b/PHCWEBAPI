# ✅ Domain Mapper Interface - Unified Architecture

## 🎯 Objetivo

Criar uma interface padronizada **`IDomainMapper<TEntity, TOutputDto>`** para garantir consistência entre todos os módulos (Parameters, Audit, futuros módulos).

## 🏗️ Arquitetura

### **1. Interface IDomainMapper (Shared.Kernel)**

```csharp
// src\Shared\Shared.Kernel\Shared.Kernel\Interfaces\IDomainMapper.cs
public interface IDomainMapper<TEntity, TOutputDto>
    where TEntity : class
    where TOutputDto : class
{
    TDto ToDto<TDto>(TEntity entity) where TDto : class;
    IEnumerable<TDto> ToDtos<TDto>(IEnumerable<TEntity> entities) where TDto : class;
    TOutputDto ToOutputDto(TEntity entity);
    IEnumerable<TOutputDto> ToOutputDtos(IEnumerable<TEntity> entities);
}
```

### **2. Implementação Concreta (Module.Application)**

Cada módulo implementa a interface:

```csharp
// Audit Module
public sealed class AuditLogMapper : IDomainMapper<AuditLog, AuditLogOutputDTO>
{
    public TDto ToDto<TDto>(AuditLog entity) where TDto : class
        => entity.Adapt<TDto>();
    
    public IEnumerable<TDto> ToDtos<TDto>(IEnumerable<AuditLog> entities) where TDto : class
        => entities.Select(e => e.Adapt<TDto>());
    
    public AuditLogOutputDTO ToOutputDto(AuditLog entity)
        => entity.Adapt<AuditLogOutputDTO>();
    
    public IEnumerable<AuditLogOutputDTO> ToOutputDtos(IEnumerable<AuditLog> entities)
        => entities.Select(e => e.Adapt<AuditLogOutputDTO>());
}
```

### **3. Extension Methods (Conveniência)**

Para facilitar o uso, cada módulo tem extension methods:

```csharp
public static class AuditLogMappingExtensions
{
    private static readonly AuditLogMapper _mapper = new();

    public static TDto ToDto<TDto>(this AuditLog entity) where TDto : class
        => _mapper.ToDto<TDto>(entity);
    
    public static AuditLogOutputDTO ToOutputDto(this AuditLog entity)
        => _mapper.ToOutputDto(entity);
    
    // ... outros métodos
}
```

### **4. Dependency Injection (Application Layer)**

Registrar mappers no DI:

```csharp
// Audit.Application/DependencyInjection.cs
public static class DependencyInjection
{
    public static IServiceCollection AddAuditApplication(this IServiceCollection services)
    {
        services.AddSingleton<IDomainMapper<AuditLog, AuditLogOutputDTO>, AuditLogMapper>();
        services.AddScoped<IAuditLogService, AuditLogService>();
        return services;
    }
}
```

## 📝 Implementação nos Módulos

### **Módulo Audit**

#### **1. AuditLogMapper**
```csharp
// src\Modules\Audit\Audit.Application\Mappings\AuditLogMappings.cs

// Classe que implementa a interface
public sealed class AuditLogMapper : IDomainMapper<AuditLog, AuditLogOutputDTO>
{
    // Implementação dos métodos usando Mapster
}

// Extension methods para conveniência
public static class AuditLogMappingExtensions
{
    private static readonly AuditLogMapper _mapper = new();
    
    public static AuditLogOutputDTO ToOutputDto(this AuditLog entity)
        => _mapper.ToOutputDto(entity);
    
    // ... outros extension methods
}
```

#### **2. DependencyInjection**
```csharp
// src\Modules\Audit\Audit.Application\DependencyInjection.cs
services.AddSingleton<IDomainMapper<AuditLog, AuditLogOutputDTO>, AuditLogMapper>();
```

#### **3. Presentation Layer**
```csharp
// src\Modules\Audit\Audit.Presentation\DependencyInjection.cs
services.AddAuditApplication(); // ✅ Registra mappers
services.AddMediatR(...);
```

### **Módulo Parameters**

#### **1. Para1Mapper**
```csharp
// src\Modules\Parameters\Parameters.Application\Mappings\Para1Mappings.cs

// Classe que implementa a interface
public sealed class Para1Mapper : IDomainMapper<Para1, ParameterOutputDTO>
{
    // Implementação dos métodos usando Mapster
}

// Extension methods (incluindo ToEntity e UpdateEntity)
public static class Para1MappingExtensions
{
    private static readonly Para1Mapper _mapper = new();
    
    public static Para1 ToEntity(this CreateParameterInputDTO dto, string? createdBy)
        => new Para1(...);
    
    public static void UpdateEntity(this Para1 entity, UpdateParameterInputDTO dto, string? updatedBy)
        => entity.Update(...);
    
    public static ParameterOutputDTO ToOutputDto(this Para1 entity)
        => _mapper.ToOutputDto(entity);
    
    // ... outros extension methods
}
```

#### **2. DependencyInjection**
```csharp
// src\Modules\Parameters\Parameters.Application\DependencyInjection.cs (NOVO)
public static class DependencyInjection
{
    public static IServiceCollection AddParametersApplication(this IServiceCollection services)
    {
        services.AddSingleton<IDomainMapper<Para1, ParameterOutputDTO>, Para1Mapper>();
        return services;
    }
}
```

#### **3. Presentation Layer**
```csharp
// src\Modules\Parameters\Parameters.Presentation\DependencyInjection.cs
services.AddParametersApplication(); // ✅ Registra mappers
services.AddMediatR(...);
```

## 🔄 Comparação: Antes vs Depois

### **Antes (❌ Inconsistente)**

```csharp
// Audit.Application
public static class AuditLogMappings // ❌ Apenas extension methods estáticos
{
    public static AuditLogOutputDTO ToOutputDto(this AuditLog entity)
        => entity.Adapt<AuditLogOutputDTO>();
}

// Parameters.Application
public static class Para1Mappings // ❌ Apenas extension methods estáticos
{
    public static TDto ToDto<TDto>(this Para1 entity) where TDto : class
        => entity.Adapt<TDto>();
}
```

**Problemas**:
- ❌ Sem interface padronizada
- ❌ Não injetável via DI
- ❌ Difícil de testar (mockar)
- ❌ Inconsistência entre módulos

### **Depois (✅ Padronizado)**

```csharp
// Shared.Kernel - Interface comum
public interface IDomainMapper<TEntity, TOutputDto>
{
    TDto ToDto<TDto>(TEntity entity) where TDto : class;
    TOutputDto ToOutputDto(TEntity entity);
    // ...
}

// Audit.Application - Implementação concreta
public sealed class AuditLogMapper : IDomainMapper<AuditLog, AuditLogOutputDTO>
{
    // Implementação
}

// Extension methods mantidos para conveniência
public static class AuditLogMappingExtensions
{
    private static readonly AuditLogMapper _mapper = new();
    
    public static AuditLogOutputDTO ToOutputDto(this AuditLog entity)
        => _mapper.ToOutputDto(entity);
}
```

**Benefícios**:
- ✅ Interface padronizada em todos os módulos
- ✅ Injetável via DI quando necessário
- ✅ Testável (pode mockar a interface)
- ✅ Extension methods mantidos para conveniência
- ✅ Consistência garantida

## 🧪 Como Usar

### **1. Usando Extension Methods (Recomendado)**

```csharp
// No Handler
public async Task<AuditLogOutputDTO?> Handle(
    GetAuditLogByStampQuery request,
    CancellationToken cancellationToken)
{
    var log = await _repository.GetByStampAsync(request.ULogsstamp, cancellationToken);
    
    if (log is null) return null;
    
    return log.ToOutputDto(); // ✅ Extension method
}
```

### **2. Usando via Dependency Injection**

```csharp
public class MyService
{
    private readonly IDomainMapper<AuditLog, AuditLogOutputDTO> _mapper;
    
    public MyService(IDomainMapper<AuditLog, AuditLogOutputDTO> mapper)
    {
        _mapper = mapper;
    }
    
    public AuditLogOutputDTO Convert(AuditLog log)
    {
        return _mapper.ToOutputDto(log); // ✅ Via DI
    }
}
```

### **3. Mapeamento Genérico**

```csharp
// Mapear para qualquer DTO
var customDto = log.ToDto<CustomAuditDto>();

// Mapear coleção
var dtos = logs.ToDtos<AuditLogSummaryDto>();

// Mapear para DTO principal
var outputDto = log.ToOutputDto();
var outputDtos = logs.ToOutputDtos();
```

## ✅ Benefícios da Nova Arquitetura

### **1. Padronização**
Todos os módulos seguem a mesma estrutura:
- Interface `IDomainMapper<TEntity, TOutputDto>`
- Classe concreta `EntityMapper`
- Extension methods `EntityMappingExtensions`

### **2. Testabilidade**
```csharp
[Fact]
public void ToOutputDto_Should_Map_All_Properties()
{
    // Arrange
    var mapper = new AuditLogMapper();
    var entity = new AuditLog(...);
    
    // Act
    var dto = mapper.ToOutputDto(entity);
    
    // Assert
    Assert.Equal(entity.ULogsstamp, dto.ULogsstamp);
}

// Ou mockar para testes de integração
var mockMapper = new Mock<IDomainMapper<AuditLog, AuditLogOutputDTO>>();
```

### **3. Manutenibilidade**
- Único lugar para mudanças de mapeamento
- Interface garante consistência
- Facilita refatoração

### **4. Dependency Injection**
- Mappers registrados no DI container
- Lifecycle: Singleton (sem estado)
- Injetável em qualquer classe que precise

### **5. Conveniência**
- Extension methods mantidos para uso direto
- Syntax fluente: `entity.ToOutputDto()`
- Não quebra código existente

## 📋 Checklist de Implementação

### ✅ Shared.Kernel
- [x] Interface `IDomainMapper<TEntity, TOutputDto>`
- [x] Documentação da interface

### ✅ Módulo Audit
- [x] Classe `AuditLogMapper : IDomainMapper<AuditLog, AuditLogOutputDTO>`
- [x] Extension methods `AuditLogMappingExtensions`
- [x] Registro no DI: `AddAuditApplication()`
- [x] Atualização do `Audit.Presentation/DependencyInjection.cs`

### ✅ Módulo Parameters
- [x] Classe `Para1Mapper : IDomainMapper<Para1, ParameterOutputDTO>`
- [x] Extension methods `Para1MappingExtensions`
- [x] Criação do `Parameters.Application/DependencyInjection.cs`
- [x] Atualização do `Parameters.Presentation/DependencyInjection.cs`

### ✅ Build & Tests
- [x] Build successful
- [x] Todos os handlers continuam funcionando
- [ ] Testes unitários dos mappers (próximo passo)

## 🚀 Próximos Módulos

Para cada novo módulo, seguir o padrão:

```csharp
// 1. Criar Mapper
public sealed class EntityMapper : IDomainMapper<Entity, EntityOutputDTO>
{
    public TDto ToDto<TDto>(Entity entity) where TDto : class
        => entity.Adapt<TDto>();
    
    public EntityOutputDTO ToOutputDto(Entity entity)
        => entity.Adapt<EntityOutputDTO>();
    
    // Implementar outros métodos
}

// 2. Extension Methods
public static class EntityMappingExtensions
{
    private static readonly EntityMapper _mapper = new();
    
    public static EntityOutputDTO ToOutputDto(this Entity entity)
        => _mapper.ToOutputDto(entity);
}

// 3. DependencyInjection
public static class DependencyInjection
{
    public static IServiceCollection AddModuleApplication(this IServiceCollection services)
    {
        services.AddSingleton<IDomainMapper<Entity, EntityOutputDTO>, EntityMapper>();
        return services;
    }
}

// 4. Chamar no Presentation Layer
services.AddModuleApplication();
```

## 🔍 Diferenças vs IMapper<TDomain, TPersistence, TDto>

### **IMapper (Original)**
```csharp
public interface IMapper<TDomain, TPersistence, TDto>
{
    TDomain ToDomain(TPersistence doc);
    TPersistence ToPersistence(TDomain entity);
    TDto ToDto(TDomain entity);
    TDto PersistenceToDto(TPersistence doc);
}
```
- **Escopo**: Conversões entre 3 camadas (Domain, Persistence, DTO)
- **Uso**: Quando há camada de persistência separada (MongoDB docs, etc)
- **Complexidade**: Maior, mais conversões

### **IDomainMapper (Novo)**
```csharp
public interface IDomainMapper<TEntity, TOutputDto>
{
    TDto ToDto<TDto>(TEntity entity) where TDto : class;
    TOutputDto ToOutputDto(TEntity entity);
    // Sem conversões de persistência
}
```
- **Escopo**: Conversões Domain -> DTO apenas
- **Uso**: Quando EF Core gerencia persistência automaticamente
- **Complexidade**: Menor, focado em output

**Quando usar cada um?**
- **IMapper**: MongoDB, múltiplas camadas de persistência
- **IDomainMapper**: EF Core, conversão simples para API

## ✅ Status

**Status**: ✅ **CONCLUÍDO**
**Data**: 10 de fevereiro de 2024
**Build**: ✅ **SUCCESSFUL**

---

**Problema**: ❌ Falta de interface padronizada para mappers  
**Solução**: ✅ `IDomainMapper<TEntity, TOutputDto>` + implementações concretas  
**Benefícios**: ✅ Padronização, testabilidade, DI, manutenibilidade
