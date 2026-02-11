# 🗺️ Estrutura de Mapeamento - PHCAPI

## 📋 Visão Geral

Este documento descreve a arquitetura de mapeamento entre camadas usando **Mapster** e a interface genérica `IMapper`.

---

## 🏗️ Arquitetura

### Interface Genérica (Shared.Kernel)

```
src/Shared/Shared.Kernel/Shared.Kernel/Interfaces/IMapper.cs
```

```csharp
public interface IMapper<TDomain, TPersistence, TDto>
{
    TDomain ToDomain(TPersistence doc);
    TPersistence ToPersistence(TDomain entity);
    TDto ToDto(TDomain entity);
    TDto PersistenceToDto(TPersistence doc);
}
```

#### Responsabilidades:
- ✅ Converter entre **Persistência** ↔ **Domínio**
- ✅ Converter **Domínio** → **DTO**
- ✅ Converter **Persistência** → **DTO** (direto)

---

## 📦 Estrutura por Módulo

### Módulo Parameters

```
src/Modules/Parameters/
├── Parameters.Domain/
│   └── Entities/
│       └── Para1.cs                    (Entidade de Domínio)
│
├── Parameters.Application/
│   └── Features/
│       └── GetAllParameters/
│           └── GetAllParametersDtos.cs (ParameterDto)
│
└── Parameters.Infrastructure/
    └── Mappers/
        └── ParameterMapper.cs          (IMapper implementação)
```

---

## 🔧 Implementação do ParameterMapper

### Localização
```
src/Modules/Parameters/Parameters.Infrastructure/Mappers/ParameterMapper.cs
```

### Código
```csharp
public class ParameterMapper : IMapper<Para1, Para1, ParameterDto>
{
    public ParameterMapper()
    {
        ConfigureMapster();
    }

    public Para1 ToDomain(Para1 doc) => doc;
    
    public Para1 ToPersistence(Para1 entity) => entity;
    
    public ParameterDto ToDto(Para1 entity)
    {
        return entity.Adapt<ParameterDto>();
    }
    
    public ParameterDto PersistenceToDto(Para1 doc)
    {
        return doc.Adapt<ParameterDto>();
    }

    private void ConfigureMapster()
    {
        TypeAdapterConfig<Para1, ParameterDto>.NewConfig()
            .Map(dest => dest.ParaStamp, src => src.ParaStamp)
            .Map(dest => dest.Descricao, src => src.Descricao)
            // ... outros campos
            .Map(dest => dest.OUsrData, src => src.OUsrData)
            .Map(dest => dest.UsrData, src => src.UsrData);
    }
}
```

---

## 🎯 Injeção de Dependência

### Infrastructure - DependencyInjection.cs

```csharp
// Mappers
services.AddSingleton<IMapper<Para1, Para1, ParameterDto>, ParameterMapper>();
```

### Uso nos Handlers

```csharp
public class GetAllParametersQueryHandler : IRequestHandler<GetAllParametersQuery, IEnumerable<ParameterDto>>
{
    private readonly IPara1Repository _para1Repository;
    private readonly IMapper<Para1, Para1, ParameterDto> _mapper;

    public GetAllParametersQueryHandler(
        IPara1Repository para1Repository,
        IMapper<Para1, Para1, ParameterDto> mapper)
    {
        _para1Repository = para1Repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ParameterDto>> Handle(GetAllParametersQuery request, CancellationToken cancellationToken)
    {
        var para1List = await _para1Repository.GetAllAsync(request.IncludeInactive, cancellationToken);
        
        return para1List.Select(p => _mapper.ToDto(p));
    }
}
```

---

## 📊 Fluxo de Dados

### Query (Leitura)
```
Database → Repository → Domain Entity → Mapper.ToDto() → DTO → Response
```

### Command (Escrita)
```
Request → InputDto → Command → Handler → Domain Entity → Repository → Database
                                                    ↓
                                          Mapper.ToDto() → DTO → Response
```

---

## ✅ Benefícios

1. **Centralização**: Toda lógica de mapeamento em um único lugar
2. **Reutilização**: Interface genérica usada em todos os módulos
3. **Testabilidade**: Fácil de mockar e testar
4. **Performance**: Mapster é otimizado e rápido
5. **Manutenibilidade**: Código limpo e organizado
6. **Flexibilidade**: Fácil adicionar mapeamentos customizados

---

## 🔄 Como Adicionar um Novo Módulo

### 1. Criar Entities no Domain
```csharp
public class MinhaEntidade { ... }
```

### 2. Criar DTOs na Application
```csharp
public record MinhaEntidadeDto { ... }
```

### 3. Criar Mapper na Infrastructure
```csharp
// Infrastructure/Mappers/MinhaEntidadeMapper.cs
public class MinhaEntidadeMapper : IMapper<MinhaEntidade, MinhaEntidade, MinhaEntidadeDto>
{
    public MinhaEntidadeMapper()
    {
        ConfigureMapster();
    }

    public MinhaEntidade ToDomain(MinhaEntidade doc) => doc;
    
    public MinhaEntidade ToPersistence(MinhaEntidade entity) => entity;
    
    public MinhaEntidadeDto ToDto(MinhaEntidade entity)
    {
        return entity.Adapt<MinhaEntidadeDto>();
    }
    
    public MinhaEntidadeDto PersistenceToDto(MinhaEntidade doc)
    {
        return doc.Adapt<MinhaEntidadeDto>();
    }

    private void ConfigureMapster()
    {
        TypeAdapterConfig<MinhaEntidade, MinhaEntidadeDto>.NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Nome, src => src.Nome);
    }
}
```

### 4. Registrar no DI
```csharp
services.AddSingleton<IMapper<MinhaEntidade, MinhaEntidade, MinhaEntidadeDto>, MinhaEntidadeMapper>();
```

### 5. Usar nos Handlers
```csharp
public MyHandler(IMapper<MinhaEntidade, MinhaEntidade, MinhaEntidadeDto> mapper)
{
    _mapper = mapper;
}

// Uso
var dto = _mapper.ToDto(entidade);
```

---

## 📝 Padrão de Nomenclatura

| Tipo | Padrão | Exemplo |
|------|--------|---------|
| **Interface** | `IMapper.cs` | Shared.Kernel/Interfaces/ |
| **Mapper** | `{Entity}Mapper.cs` | ParameterMapper.cs |
| **Pasta** | `Mappers` | Infrastructure/Mappers/ |
| **Registro DI** | `AddSingleton` | DependencyInjection.cs |

---

## 🚫 O Que NÃO Fazer

❌ **Não criar métodos de extensão** para mapeamento (ex: `.ToDto()`)
```csharp
// ❌ ERRADO
public static class ParameterExtensions
{
    public static ParameterDto ToDto(this Para1 entity) { ... }
}
```

✅ **Use o Mapper injetado**
```csharp
// ✅ CORRETO
public class Handler
{
    private readonly IMapper<Para1, Para1, ParameterDto> _mapper;
    
    public Handler(IMapper<Para1, Para1, ParameterDto> mapper)
    {
        _mapper = mapper;
    }
    
    public ParameterDto Convert(Para1 entity)
    {
        return _mapper.ToDto(entity);
    }
}
```

---

## 📦 Dependências

### Shared.Kernel
- Nenhuma dependência externa
- Contém apenas a interface `IMapper`

### Module.Infrastructure
```xml
<PackageReference Include="Mapster" Version="7.4.0" />
```

### Module.Application
```xml
<ProjectReference Include="..\..\..\Shared\Shared.Kernel\Shared.Kernel\Shared.Kernel.csproj" />
```

---

## 🎓 Exemplo Completo - Parameters Module

### 1. Interface (Shared.Kernel)
```csharp
IMapper<TDomain, TPersistence, TDto>
```

### 2. Implementação (Infrastructure)
```csharp
ParameterMapper : IMapper<Para1, Para1, ParameterDto>
```

### 3. Registro (Infrastructure DI)
```csharp
services.AddSingleton<IMapper<Para1, Para1, ParameterDto>, ParameterMapper>();
```

### 4. Uso (Application Handler)
```csharp
private readonly IMapper<Para1, Para1, ParameterDto> _mapper;

public async Task<ParameterDto> Handle(...)
{
    var entity = await _repository.GetAsync(...);
    return _mapper.ToDto(entity);
}
```

---

## 🔍 Troubleshooting

### Erro: "Cannot resolve IMapper"
**Solução**: Verifique se o Mapper foi registrado no DI

### Erro: "Mapster não encontrado"
**Solução**: Adicione o pacote NuGet:
```bash
dotnet add package Mapster
```

### Erro: "Shared.Kernel não encontrado"
**Solução**: Adicione a referência ao projeto:
```xml
<ProjectReference Include="..\..\..\Shared\Shared.Kernel\Shared.Kernel\Shared.Kernel.csproj" />
```

---

## 📚 Referências

- [Mapster Documentation](https://github.com/MapsterMapper/Mapster)
- [Clean Architecture Patterns](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [DDD Mapping Patterns](https://martinfowler.com/eaaCatalog/dataMapper.html)

---

**Desenvolvido seguindo Clean Architecture e DDD principles! 🏗️**
