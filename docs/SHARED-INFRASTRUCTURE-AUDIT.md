# Shared Infrastructure - Auditoria Centralizada

## 🎯 Mudança Realizada

Movido **`AuditableEntityInterceptorEFCore`** do módulo Parameters para **Shared.Infrastructure**.

## 📁 Movimentação de Arquivos

### Antes
```
❌ src\Modules\Parameters\Parameters.Infrastructure\
   └── Persistence\Interceptors\
       └── AuditableEntityInterceptorEFCore.cs
```

### Depois
```
✅ src\Shared\Shared.Infrastructure\
   └── Persistence\Interceptors\
       └── AuditableEntityInterceptorEFCore.cs
```

## 🤔 Por Quê Mover Para Shared?

### Razão Principal: **Reutilização**

| Conceito | Localização | Motivo |
|----------|-------------|--------|
| `AuditableEntity` | **Shared.Abstractions** | Classe abstrata usada por todos os módulos |
| `AuditableEntityInterceptorEFCore` | **Shared.Infrastructure** | Implementação EF Core que trabalha com `AuditableEntity` |
| `Para1`, `Order`, `Customer`, etc. | **Módulos específicos** | Entidades concretas que herdam de `AuditableEntity` |

### Princípios Aplicados

1. ✅ **DRY (Don't Repeat Yourself)**
   - Evita duplicar o interceptor em cada módulo
   
2. ✅ **Shared Kernel Pattern**
   - Componentes comuns = Shared
   
3. ✅ **Single Responsibility**
   - O interceptor tem uma responsabilidade: preencher campos de auditoria
   - Essa responsabilidade é a mesma para todos os módulos

4. ✅ **Open/Closed Principle**
   - O interceptor está fechado para modificação
   - Mas aberto para extensão (pode ser usado por qualquer módulo)

## 🔧 Mudanças nos Arquivos

### 1. Shared.Infrastructure.csproj (Novo)
```xml
<ItemGroup>
  <PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
</ItemGroup>

<ItemGroup>
  <ProjectReference Include="..\..\Shared.Abstractions\Shared.Domain.csproj" />
</ItemGroup>
```

### 2. Parameters.Infrastructure.csproj (Atualizado)
```xml
<ItemGroup>
  <!-- NOVO: Referência ao Shared.Infrastructure -->
  <ProjectReference Include="..\..\..\Shared\Shared.Infrastructure\Shared.Infrastructure\Shared.Infrastructure.csproj" />
</ItemGroup>
```

### 3. Parameters.Infrastructure\DependencyInjection.cs (Atualizado)
```csharp
// ANTES
using Parameters.Infrastructure.Persistence.Interceptors;

// DEPOIS
using Shared.Infrastructure.Persistence.Interceptors;

public static IServiceCollection AddParametersInfrastructure(...)
{
    // ANTES E DEPOIS: Mesmo código!
    services.AddSingleton<AuditableEntityInterceptorEFCore>();
    
    services.AddDbContext<ParametersDbContextEFCore>((sp, options) =>
    {
        var auditInterceptor = sp.GetRequiredService<AuditableEntityInterceptorEFCore>();
        options.UseSqlServer(...).AddInterceptors(auditInterceptor);
    });
    
    return services;
}
```

## 📊 Benefícios da Mudança

### Para Novos Módulos
```csharp
// Orders Module - SEM duplicação de código!
using Shared.Infrastructure.Persistence.Interceptors;

public static class DependencyInjection
{
    public static IServiceCollection AddOrdersInfrastructure(...)
    {
        // ✨ Reutiliza o mesmo interceptor do Shared
        services.AddSingleton<AuditableEntityInterceptorEFCore>();
        
        services.AddDbContext<OrdersDbContextEFCore>((sp, options) =>
        {
            var auditInterceptor = sp.GetRequiredService<AuditableEntityInterceptorEFCore>();
            options.UseSqlServer(...).AddInterceptors(auditInterceptor);
        });
        
        return services;
    }
}
```

### Comparação: Com vs Sem Shared

#### ❌ SEM Shared (Antes)
```
Parameters.Infrastructure/
  └── Interceptors/AuditableEntityInterceptorEFCore.cs (duplicado)
  
Orders.Infrastructure/
  └── Interceptors/AuditableEntityInterceptorEFCore.cs (duplicado)
  
Customers.Infrastructure/
  └── Interceptors/AuditableEntityInterceptorEFCore.cs (duplicado)
```
**Problemas:**
- Código duplicado em 3+ lugares
- Bugfix precisa ser aplicado em todos os módulos
- Inconsistências podem surgir

#### ✅ COM Shared (Depois)
```
Shared.Infrastructure/
  └── Interceptors/AuditableEntityInterceptorEFCore.cs (✨ único)
  
Parameters.Infrastructure/ (usa do Shared)
Orders.Infrastructure/ (usa do Shared)
Customers.Infrastructure/ (usa do Shared)
```
**Vantagens:**
- Código centralizado em 1 lugar
- Bugfix aplicado uma vez, todos se beneficiam
- Consistência garantida

## 🧪 Testabilidade

O interceptor agora pode ser testado de forma isolada:

```csharp
// Shared.Infrastructure.Tests
public class AuditableEntityInterceptorEFCoreTests
{
    [Fact]
    public async Task Should_Set_OUsrData_When_Entity_Added()
    {
        // Arrange
        var interceptor = new AuditableEntityInterceptorEFCore();
        var context = new TestDbContext(...);
        var entity = new TestAuditableEntity();
        
        // Act
        context.Add(entity);
        await context.SaveChangesAsync();
        
        // Assert
        Assert.NotEqual(default(DateTime), entity.OUsrData);
        Assert.NotNull(entity.OUsrHora);
    }
}
```

## 🏗️ Arquitetura Final

```
┌─────────────────────────────────────────────────────┐
│               Application Layer                      │
│  (Parameters.App, Orders.App, Customers.App)        │
└─────────────────────────────────────────────────────┘
                        ↓
┌─────────────────────────────────────────────────────┐
│               Domain Layer                           │
│  (Parameters.Domain, Orders.Domain, etc.)           │
│  ← depende de Shared.Abstractions (AuditableEntity) │
└─────────────────────────────────────────────────────┘
                        ↓
┌─────────────────────────────────────────────────────┐
│            Infrastructure Layer                      │
│  Parameters.Infra ←┐                                │
│  Orders.Infra      ├→ Shared.Infrastructure         │
│  Customers.Infra  ←┘  (AuditableEntityInterceptor)  │
└─────────────────────────────────────────────────────┘
```

## 📝 Convenção para Futuros Componentes Shared

### O que vai para Shared.Infrastructure?

✅ **SIM - Vai para Shared:**
- Interceptors que trabalham com abstrações do Shared
- Repositórios base/genéricos (ex: `GenericRepositoryEFCore<T>`)
- Configurações EF Core compartilhadas
- Migrations compartilhadas (se houver DB compartilhado)
- Utilities de persistência reutilizáveis

❌ **NÃO - Fica no módulo:**
- DbContext específico do módulo (ex: `ParametersDbContextEFCore`)
- Configurações EF Core de entidades específicas (ex: `Para1ConfigurationEFCore`)
- Repositórios específicos de entidades (ex: `Para1RepositoryEFCore`)
- Migrations específicas do módulo

### Exemplo: GenericRepository (Futuro)

Se você criar um `GenericRepositoryEFCore<T>`, ele também deve ir para **Shared.Infrastructure**:

```csharp
// Shared.Infrastructure/Repositories/GenericRepositoryEFCore.cs
public class GenericRepositoryEFCore<T> : IGenericRepository<T> where T : class
{
    protected readonly DbContext _context;
    
    public GenericRepositoryEFCore(DbContext context)
    {
        _context = context;
    }
    
    public async Task<T> AddAsync(T entity, CancellationToken ct)
    {
        await _context.Set<T>().AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
        return entity;
    }
    
    // ... outros métodos CRUD genéricos
}
```

**Por quê no Shared?**
- É genérico e reutilizável
- Não tem lógica de negócio específica
- Todos os módulos podem se beneficiar

## ✅ Checklist de Build Verificado

- [x] Shared.Infrastructure compila
- [x] Parameters.Infrastructure compila
- [x] Referência ao Shared.Infrastructure adicionada
- [x] Namespace atualizado no DependencyInjection
- [x] Program.cs atualizado (ParametersDbContextEFCore)
- [x] Build geral bem-sucedido
- [x] Documentação atualizada

## 🎓 Lições Aprendidas

1. **Pense "Shared First"** quando trabalhar com abstrações do Shared
2. **Interceptors são infraestrutura transversal** - se trabalham com Shared, vão para Shared
3. **Mantenha módulos independentes** - mas compartilhe código comum
4. **Clean Architecture permite isso** - camadas de infraestrutura podem depender de Shared.Infrastructure

## 🚀 Próximos Passos

Aplicar o mesmo padrão para outros componentes comuns:

1. ✅ **AuditableEntityInterceptorEFCore** - Feito!
2. 🔜 **SoftDeleteInterceptorEFCore** - Se implementar soft delete
3. 🔜 **GenericRepositoryEFCore<T>** - Se criar padrões genéricos
4. 🔜 **UnitOfWorkEFCore** - Se implementar Unit of Work pattern

**Resultado:** Arquitetura mais limpa, menos duplicação, mais manutenível! 🎯
