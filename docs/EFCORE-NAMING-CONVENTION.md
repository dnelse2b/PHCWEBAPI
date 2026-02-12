# EF Core Naming Convention - Technology Independence

## 📋 Objetivo

Tornar **explícito** que as implementações de infraestrutura são específicas do **Entity Framework Core**, facilitando a adição de outras tecnologias (Dapper, MongoDB, etc.) no futuro sem conflitos de nomenclatura.

## 🔄 Mudanças Realizadas

### Arquivos Renomeados

| Arquivo Original | Arquivo Novo | Classe Original | Classe Nova |
|-----------------|--------------|-----------------|-------------|
| `E1Repository.cs` | `Para1RepositoryEFCore.cs` | `Para1Repository` | `Para1RepositoryEFCore` |
| `ParametersDbContext.cs` | `ParametersDbContextEFCore.cs` | `ParametersDbContext` | `ParametersDbContextEFCore` |
| `E1Configuration.cs` | `Para1ConfigurationEFCore.cs` | `Para1Configuration` | `Para1ConfigurationEFCore` |
| `AuditableEntityInterceptor.cs` | `AuditableEntityInterceptorEFCore.cs` | `AuditableEntityInterceptor` | `AuditableEntityInterceptorEFCore` |

### DependencyInjection.cs Atualizado

```csharp
// ANTES
services.AddSingleton<AuditableEntityInterceptor>();
services.AddDbContext<ParametersDbContext>(...);
services.AddScoped<IPara1Repository, Para1Repository>();

// DEPOIS - Explicitamente EF Core
services.AddSingleton<AuditableEntityInterceptorEFCore>();
services.AddDbContext<ParametersDbContextEFCore>(...);
services.AddScoped<IPara1Repository, Para1RepositoryEFCore>();
```

## ✅ Benefícios

### 1. **Clareza de Tecnologia**
- Qualquer desenvolvedor sabe imediatamente que `Para1RepositoryEFCore` usa Entity Framework Core
- Evita confusão sobre qual tecnologia está sendo usada

### 2. **Facilita Múltiplas Implementações**
Agora você pode ter:
```
Infrastructure/
├── Repositories/
│   ├── Para1RepositoryEFCore.cs      ← Implementação EF Core
│   ├── Para1RepositoryDapper.cs      ← Implementação Dapper (futuro)
│   └── Para1RepositoryMongoDB.cs     ← Implementação MongoDB (futuro)
```

### 3. **Registro Flexível no DI**
```csharp
// Escolher implementação via configuração
if (configuration["Database:Provider"] == "EFCore")
{
    services.AddScoped<IPara1Repository, Para1RepositoryEFCore>();
}
else if (configuration["Database:Provider"] == "Dapper")
{
    services.AddScoped<IPara1Repository, Para1RepositoryDapper>();
}
```

### 4. **Testes Mais Fáceis**
```csharp
// Test com EF Core In-Memory
services.AddScoped<IPara1Repository, Para1RepositoryEFCore>();

// Test com Mock
services.AddScoped<IPara1Repository, Para1RepositoryMock>();

// Test com Dapper + SQLite
services.AddScoped<IPara1Repository, Para1RepositoryDapper>();
```

## 🎯 Padrão de Nomenclatura

### Para Implementações de Repositório
```
[EntityName]Repository[Technology].cs
```
**Exemplos:**
- `Para1RepositoryEFCore.cs`
- `Para1RepositoryDapper.cs`
- `Para1RepositoryMongoDB.cs`

### Para DbContext
```
[ModuleName]DbContext[Technology].cs
```
**Exemplos:**
- `ParametersDbContextEFCore.cs`
- `OrdersDbContextEFCore.cs`

### Para Configurações EF Core
```
[EntityName]Configuration[Technology].cs
```
**Exemplos:**
- `Para1ConfigurationEFCore.cs`
- `OrderConfigurationEFCore.cs`

### Para Interceptors/Middleware Específicos
```
[FunctionalityName][Technology].cs
```
**Exemplos:**
- `AuditableEntityInterceptorEFCore.cs` (✅ **Shared.Infrastructure** - reutilizável)
- `SoftDeleteInterceptorEFCore.cs`

**⚠️ Importante:** Interceptors que trabalham com abstrações do Shared (como `AuditableEntity`) devem estar em **Shared.Infrastructure**, não em módulos específicos!

## 🏗️ Estrutura Infrastructure (Atualizada)

```
Parameters.Infrastructure/
├── Persistence/
│   ├── ParametersDbContextEFCore.cs              ← DbContext EF Core
│   ├── Configurations/
│   │   └── Para1ConfigurationEFCore.cs           ← Entity Configuration
│   └── Interceptors/
│       └── AuditableEntityInterceptorEFCore.cs   ← Audit Interceptor
└── Repositories/
    └── Para1RepositoryEFCore.cs                  ← Repository EF Core
```

## 🚀 Como Adicionar Nova Tecnologia (Ex: Dapper)

### 1. Criar Implementação
```csharp
// Para1RepositoryDapper.cs
public class Para1RepositoryDapper : IPara1Repository
{
    private readonly IDbConnection _connection;
    
    public Para1RepositoryDapper(IDbConnection connection)
    {
        _connection = connection;
    }
    
    public async Task<Para1> AddAsync(Para1 para1, CancellationToken ct)
    {
        const string sql = @"
            INSERT INTO para1 (parastamp, descricao, valor, tipo, dec, tam, ousrdata, ousrhora, ousrinis)
            VALUES (@ParaStamp, @Descricao, @Valor, @Tipo, @Dec, @Tam, @OUsrData, @OUsrHora, @OUsrInis)";
            
        await _connection.ExecuteAsync(sql, para1);
        return para1;
    }
    
    // ... outros métodos
}
```

### 2. Registrar no DI
```csharp
// DependencyInjection.cs
public static IServiceCollection AddParametersInfrastructureDapper(
    this IServiceCollection services,
    IConfiguration configuration)
{
    // Dapper Connection
    services.AddScoped<IDbConnection>(sp => 
        new SqlConnection(configuration.GetConnectionString("ParametersConnection")));
    
    // Repository - Dapper implementation
    services.AddScoped<IPara1Repository, Para1RepositoryDapper>();
    
    return services;
}
```

### 3. Usar no Program.cs
```csharp
// Escolher implementação
if (builder.Configuration["Database:Provider"] == "EFCore")
{
    builder.Services.AddParametersInfrastructure(builder.Configuration);
}
else if (builder.Configuration["Database:Provider"] == "Dapper")
{
    builder.Services.AddParametersInfrastructureDapper(builder.Configuration);
}
```

## ⚠️ O Que NÃO Mudou

✅ **Camada Domain** - Zero mudanças  
✅ **Camada Application** - Zero mudanças  
✅ **Interfaces de Repositório** - Zero mudanças  
✅ **Entidades** - Zero mudanças  
✅ **Use Cases (Handlers)** - Zero mudanças  

**Por quê?** Porque seguimos **Dependency Inversion Principle** - camadas superiores dependem apenas de **abstrações (interfaces)**, não de implementações concretas!

## 🚀 Shared Infrastructure - Componentes Reutilizáveis

### AuditableEntityInterceptorEFCore (Movido para Shared)

**Localização:** `Shared.Infrastructure\Persistence\Interceptors\AuditableEntityInterceptorEFCore.cs`

**Por quê no Shared?**
1. ✅ `AuditableEntity` está no **Shared.Abstractions**
2. ✅ **Todos os módulos** usam auditoria
3. ✅ **DRY Principle** - Evita duplicação em cada módulo
4. ✅ **Manutenção Centralizada** - Uma mudança beneficia todos os módulos

**Estrutura:**
```
Shared.Infrastructure/
└── Persistence/
    └── Interceptors/
        └── AuditableEntityInterceptorEFCore.cs  ← Compartilhado entre todos os módulos
```

**Uso em qualquer módulo:**
```csharp
using Shared.Infrastructure.Persistence.Interceptors;

public static class DependencyInjection
{
    public static IServiceCollection AddModuleInfrastructure(...)
    {
        // Reutilizar interceptor do Shared
        services.AddSingleton<AuditableEntityInterceptorEFCore>();

        services.AddDbContext<ModuleDbContextEFCore>((sp, options) =>
        {
            var auditInterceptor = sp.GetRequiredService<AuditableEntityInterceptorEFCore>();
            options.UseSqlServer(...).AddInterceptors(auditInterceptor);
        });

        return services;
    }
}
```

## 📊 Comparação: Antes vs Depois

### Antes (Acoplado)
```
❌ Para1Repository.cs (qual tecnologia?)
❌ ParametersDbContext.cs (sabemos que é EF, mas não é explícito)
❌ Para1Configuration.cs (configuração de quê?)
```

### Depois (Explícito e Desacoplado)
```
✅ Para1RepositoryEFCore.cs (claramente EF Core)
✅ ParametersDbContextEFCore.cs (explicitamente EF Core)
✅ Para1ConfigurationEFCore.cs (configuração EF Core)
✅ Fácil adicionar: Para1RepositoryDapper.cs
```

## 🎓 Princípios Seguidos

1. **Dependency Inversion Principle (SOLID)** - Dependemos de abstrações
2. **Clean Architecture** - Infraestrutura isolada e substituível
3. **Explicit Dependencies** - Nomes claros e descritivos
4. **Technology Independence** - Não ficamos presos a uma tecnologia

## 📝 Convenção para Outros Módulos

**Aplicar o mesmo padrão em todos os módulos:**
```
Orders.Infrastructure/
├── Repositories/
│   └── OrderRepositoryEFCore.cs
└── Persistence/
    └── OrdersDbContextEFCore.cs

Customers.Infrastructure/
├── Repositories/
│   └── CustomerRepositoryEFCore.cs
└── Persistence/
    └── CustomersDbContextEFCore.cs
```

## ✨ Conclusão

Esta convenção de nomenclatura:
- ✅ Torna o código **mais claro** e **autoexplicativo**
- ✅ Facilita **migração** para outras tecnologias
- ✅ Permite **múltiplas implementações** coexistirem
- ✅ Mantém **Clean Architecture** intacta
- ✅ Segue **princípios SOLID**
- ✅ Prepara o projeto para **escalabilidade**

**Você está correto:** Não devemos ficar presos a uma tecnologia. Esta abordagem garante isso! 🎯
