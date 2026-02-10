# 📚 Guia Completo: Modular Monolith com Clean Architecture

> **Autor:** PHCAPI Team  
> **Versão:** 1.0  
> **Data:** 2024  
> **.NET Version:** 8.0

---

## 📋 Índice

1. [Conceitos Fundamentais](#conceitos-fundamentais)
2. [Estrutura do Projeto](#estrutura-do-projeto)
3. [Criação Passo a Passo](#criação-passo-a-passo)
4. [Configuração de Dependências](#configuração-de-dependências)
5. [Criação de Código](#criação-de-código)
6. [Como Funciona a Conexão](#como-funciona-a-conexão)
7. [Deploy e Publish](#deploy-e-publish)
8. [Boas Práticas](#boas-práticas)

---

## 🎯 Conceitos Fundamentais

### O Que É Clean Architecture?

Clean Architecture é um padrão arquitetural que separa as responsabilidades em camadas concêntricas:

```
┌─────────────────────────────────┐
│  API (Presentation)              │ ← Controllers, DTOs
├─────────────────────────────────┤
│  Application (Use Cases)         │ ← Commands, Queries, Handlers
├─────────────────────────────────┤
│  Domain (Business Logic)         │ ← Entities, Interfaces, Regras
├─────────────────────────────────┤
│  Infrastructure (External)       │ ← Database, Repositories, APIs
└─────────────────────────────────┘
```

**Princípios:**
- ✅ **Dependências apontam para dentro** (Infrastructure → Domain, nunca o contrário)
- ✅ **Domain não tem dependências externas** (puro C#)
- ✅ **Use Cases isolados** (cada comando/query faz uma coisa)

### O Que É Modular Monolith?

Modular Monolith é uma arquitetura onde:
- ✅ **Um único processo executável** (não são microserviços)
- ✅ **Módulos isolados** (cada módulo = domínio de negócio)
- ✅ **Comunicação direta** (chamadas de método, não HTTP/RPC)
- ✅ **Deploy unificado** (um único publish)

```
❌ Microserviços:
   [Parameters Service] → HTTP → [Customers Service] → HTTP → [Orders Service]
   (3 processos, 3 deploys, complexidade distribuída)

✅ Modular Monolith:
   [Host] → [Parameters Module] + [Customers Module] + [Orders Module]
   (1 processo, 1 deploy, simplicidade monolítica)
```

**Vantagens:**
- 🚀 Simplicidade de deploy
- 🔧 Fácil refatoração entre módulos
- 📊 Transações ACID nativas
- 🐛 Debug direto (não precisa orquestrar containers)

**Quando usar Modular Monolith:**
- Projetos pequenos/médios
- Equipes pequenas
- Domínios bem definidos mas acoplados
- Precisa de performance e simplicidade

---

## 🏗️ Estrutura do Projeto

### Estrutura Completa

```
📦 Solution Root
├── 📁 src/
│   ├── 📁 PHCAPI.Host/                    ← ✅ ÚNICO EXECUTÁVEL
│   │   ├── 📄 Program.cs
│   │   ├── 📄 appsettings.json
│   │   ├── 📄 PHCAPI.Host.csproj
│   │   └── 📁 Properties/
│   │       └── 📄 launchSettings.json
│   │
│   └── 📁 Modules/
│       ├── 📁 Parameters/                  ← Módulo 1
│       │   ├── 📁 Parameters.Domain/
│       │   │   ├── 📁 Entities/
│       │   │   │   ├── 📄 E1.cs
│       │   │   │   └── 📄 E4.cs
│       │   │   ├── 📁 Repositories/
│       │   │   │   ├── 📄 IE1Repository.cs
│       │   │   │   └── 📄 IE4Repository.cs
│       │   │   └── 📄 Parameters.Domain.csproj
│       │   │
│       │   ├── 📁 Parameters.Application/
│       │   │   ├── 📁 Commands/
│       │   │   │   ├── 📄 CreateParameterCommand.cs
│       │   │   │   ├── 📄 UpdateParameterCommand.cs
│       │   │   │   └── 📄 DeleteParameterCommand.cs
│       │   │   ├── 📁 Queries/
│       │   │   │   ├── 📄 GetAllParametersQuery.cs
│       │   │   │   └── 📄 GetParameterByStampQuery.cs
│       │   │   ├── 📁 Handlers/
│       │   │   │   ├── 📄 CreateParameterCommandHandler.cs
│       │   │   │   └── 📄 GetAllParametersQueryHandler.cs
│       │   │   ├── 📁 DTOs/
│       │   │   │   ├── 📄 ParameterDto.cs
│       │   │   │   └── 📄 CreateParameterDto.cs
│       │   │   ├── 📁 Validators/
│       │   │   │   └── 📄 CreateParameterCommandValidator.cs
│       │   │   └── 📄 Parameters.Application.csproj
│       │   │
│       │   ├── 📁 Parameters.Infrastructure/
│       │   │   ├── 📁 Persistence/
│       │   │   │   ├── 📄 ParametersDbContext.cs
│       │   │   │   └── 📁 Migrations/
│       │   │   ├── 📁 Repositories/
│       │   │   │   ├── 📄 E1Repository.cs
│       │   │   │   └── 📄 E4Repository.cs
│       │   │   ├── 📄 DependencyInjection.cs
│       │   │   └── 📄 Parameters.Infrastructure.csproj
│       │   │
│       │   └── 📁 Parameters.API/
│       │       ├── 📁 Controllers/
│       │       │   └── 📄 ParametersController.cs
│       │       ├── 📄 DependencyInjection.cs
│       │       └── 📄 Parameters.API.csproj
│       │
│       ├── 📁 Customers/                   ← Módulo 2 (futuro)
│       │   ├── 📁 Customers.Domain/
│       │   ├── 📁 Customers.Application/
│       │   ├── 📁 Customers.Infrastructure/
│       │   └── 📁 Customers.API/
│       │
│       └── 📁 Orders/                      ← Módulo 3 (futuro)
│           └── ...
│
└── 📁 docs/
    └── 📄 Arquitecture-Build-Guide.md     ← Este arquivo
```

### Responsabilidades das Camadas

| Camada | Responsabilidade | Dependências | Tipo |
|--------|------------------|--------------|------|
| **Domain** | Entidades, Interfaces, Regras de Negócio | Nenhuma | Class Library |
| **Application** | Casos de Uso, Commands, Queries, DTOs | Domain | Class Library |
| **Infrastructure** | Implementação de Repositórios, DbContext | Domain, Application | Class Library |
| **API** | Controllers, DTOs de Entrada/Saída | Application, Infrastructure | Class Library |
| **Host** | Configuração, Startup, DI Container | Todos os módulos API | Web Application |

---

## 🛠️ Criação Passo a Passo

### PASSO 1: Criar Estrutura de Diretórios

Abra o **PowerShell** na raiz da solution:

```powershell
# Criar estrutura base
New-Item -ItemType Directory -Path "src" -Force
Set-Location src

# Criar o Host
New-Item -ItemType Directory -Path "PHCAPI.Host" -Force

# Criar estrutura de módulos
New-Item -ItemType Directory -Path "Modules\Parameters" -Force

# Criar camadas do módulo Parameters
New-Item -ItemType Directory -Path "Modules\Parameters\Parameters.Domain" -Force
New-Item -ItemType Directory -Path "Modules\Parameters\Parameters.Application" -Force
New-Item -ItemType Directory -Path "Modules\Parameters\Parameters.Infrastructure" -Force
New-Item -ItemType Directory -Path "Modules\Parameters\Parameters.API" -Force

Set-Location ..
```

---

### PASSO 2: Criar os Projetos (.csproj)

#### 2.1: Criar o Host (Web Application)

```powershell
Set-Location src\PHCAPI.Host
dotnet new webapi -n PHCAPI.Host --framework net8.0
# Remover arquivos de exemplo
Remove-Item WeatherForecast.cs -ErrorAction SilentlyContinue
Set-Location ..\..
```

**Arquivos criados automaticamente:**
- ✅ `PHCAPI.Host.csproj`
- ✅ `Program.cs`
- ✅ `appsettings.json`
- ✅ `Properties/launchSettings.json`

#### 2.2: Criar Camadas do Módulo (Class Libraries)

```powershell
# Domain
Set-Location src\Modules\Parameters\Parameters.Domain
dotnet new classlib -n Parameters.Domain --framework net8.0
Remove-Item Class1.cs

# Application
Set-Location ..\Parameters.Application
dotnet new classlib -n Parameters.Application --framework net8.0
Remove-Item Class1.cs

# Infrastructure
Set-Location ..\Parameters.Infrastructure
dotnet new classlib -n Parameters.Infrastructure --framework net8.0
Remove-Item Class1.cs

# API (Class Library, não Web!)
Set-Location ..\Parameters.API
dotnet new classlib -n Parameters.API --framework net8.0
Remove-Item Class1.cs

Set-Location ..\..\..\..\..
```

---

### PASSO 3: Configurar Referências Entre Projetos

As dependências devem seguir a Clean Architecture:

```
Host → Parameters.API
         ↓
    Application → Domain
         ↓
    Infrastructure → Domain
```

#### 3.1: Application → Domain

```powershell
Set-Location src\Modules\Parameters\Parameters.Application
dotnet add reference ..\Parameters.Domain\Parameters.Domain.csproj
Set-Location ..\..\..\..\..
```

#### 3.2: Infrastructure → Domain + Application

```powershell
Set-Location src\Modules\Parameters\Parameters.Infrastructure
dotnet add reference ..\Parameters.Domain\Parameters.Domain.csproj
dotnet add reference ..\Parameters.Application\Parameters.Application.csproj
Set-Location ..\..\..\..\..
```

#### 3.3: API → Application + Infrastructure

```powershell
Set-Location src\Modules\Parameters\Parameters.API
dotnet add reference ..\Parameters.Application\Parameters.Application.csproj
dotnet add reference ..\Parameters.Infrastructure\Parameters.Infrastructure.csproj
Set-Location ..\..\..\..\..
```

#### 3.4: Host → Parameters.API

```powershell
Set-Location src\PHCAPI.Host
dotnet add reference ..\Modules\Parameters\Parameters.API\Parameters.API.csproj
Set-Location ..\..
```

**Por que o Host só referencia o API?**
- O módulo API já tem referências transitivas para todas as outras camadas
- O Host não precisa conhecer os detalhes internos do módulo

---

### PASSO 4: Adicionar Pacotes NuGet

Cada camada tem suas dependências específicas:

#### 4.1: Domain (SEM pacotes - regras puras)

```powershell
# Domain não precisa de pacotes externos!
# É a camada mais pura, apenas C# e lógica de negócio
```

#### 4.2: Application (MediatR + FluentValidation)

```powershell
Set-Location src\Modules\Parameters\Parameters.Application
dotnet add package MediatR --version 12.2.0
dotnet add package FluentValidation --version 11.9.0
Set-Location ..\..\..\..\..
```

#### 4.3: Infrastructure (Entity Framework Core)

```powershell
Set-Location src\Modules\Parameters\Parameters.Infrastructure
dotnet add package Microsoft.EntityFrameworkCore --version 8.0.0
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.0
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.0
Set-Location ..\..\..\..\..
```

#### 4.4: API (ASP.NET Core + MediatR + FluentValidation)

```powershell
Set-Location src\Modules\Parameters\Parameters.API
dotnet add package MediatR --version 12.2.0
dotnet add package FluentValidation.AspNetCore --version 11.3.0
dotnet add package Microsoft.Extensions.Logging.Abstractions --version 8.0.0
Set-Location ..\..\..\..\..
```

**Editar `Parameters.API.csproj` manualmente** para adicionar:

```xml
<ItemGroup>
  <FrameworkReference Include="Microsoft.AspNetCore.App" />
</ItemGroup>
```

#### 4.5: Host (Swagger + Serilog + EF Core)

```powershell
Set-Location src\PHCAPI.Host
dotnet add package Swashbuckle.AspNetCore --version 6.5.0
dotnet add package Serilog.AspNetCore --version 8.0.0
dotnet add package Microsoft.EntityFrameworkCore --version 8.0.0
Set-Location ..\..
```

---

## 💻 Criação de Código

### PASSO 5: Camada Domain

#### 5.1: Criar Entidades

**Arquivo:** `src/Modules/Parameters/Parameters.Domain/Entities/E1.cs`

```csharp
namespace Parameters.Domain.Entities;

/// <summary>
/// Entidade principal de Parâmetros
/// </summary>
public class E1
{
    public string E1Stamp { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool Active { get; set; } = true;
    public DateTime CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public string? ModifiedBy { get; set; }

    // Relacionamento
    public virtual ICollection<E4> E4Details { get; set; } = new List<E4>();
}
```

#### 5.2: Criar Interfaces de Repositório

**Arquivo:** `src/Modules/Parameters/Parameters.Domain/Repositories/IE1Repository.cs`

```csharp
using Parameters.Domain.Entities;

namespace Parameters.Domain.Repositories;

public interface IE1Repository
{
    Task<IEnumerable<E1>> GetAllAsync(bool includeInactive = false);
    Task<E1?> GetByStampAsync(string e1Stamp);
    Task<E1?> GetByCodeAsync(string code);
    Task<E1> AddAsync(E1 entity);
    Task<E1> UpdateAsync(E1 entity);
    Task<bool> DeleteAsync(string e1Stamp);
    Task<bool> ExistsAsync(string code);
}
```

---

### PASSO 6: Camada Application

#### 6.1: Criar Commands

**Arquivo:** `src/Modules/Parameters/Parameters.Application/Commands/CreateParameterCommand.cs`

```csharp
using MediatR;
using Parameters.Application.DTOs;

namespace Parameters.Application.Commands;

public record CreateParameterCommand(
    string Code,
    string Description,
    List<E4DetailDto>? E4Details,
    string? CreatedBy
) : IRequest<ParameterDto>;
```

#### 6.2: Criar Handlers

**Arquivo:** `src/Modules/Parameters/Parameters.Application/Handlers/CreateParameterCommandHandler.cs`

```csharp
using MediatR;
using Parameters.Application.Commands;
using Parameters.Application.DTOs;
using Parameters.Domain.Entities;
using Parameters.Domain.Repositories;

namespace Parameters.Application.Handlers;

public class CreateParameterCommandHandler 
    : IRequestHandler<CreateParameterCommand, ParameterDto>
{
    private readonly IE1Repository _repository;

    public CreateParameterCommandHandler(IE1Repository repository)
    {
        _repository = repository;
    }

    public async Task<ParameterDto> Handle(
        CreateParameterCommand request, 
        CancellationToken cancellationToken)
    {
        // Validar se código já existe
        if (await _repository.ExistsAsync(request.Code))
            throw new ArgumentException($"Code {request.Code} already exists");

        // Criar entidade
        var entity = new E1
        {
            E1Stamp = Guid.NewGuid().ToString(),
            Code = request.Code,
            Description = request.Description,
            Active = true,
            CreatedDate = DateTime.UtcNow,
            CreatedBy = request.CreatedBy
        };

        // Salvar
        var result = await _repository.AddAsync(entity);

        // Mapear para DTO
        return new ParameterDto
        {
            E1Stamp = result.E1Stamp,
            Code = result.Code,
            Description = result.Description,
            Active = result.Active
        };
    }
}
```

---

### PASSO 7: Camada Infrastructure

#### 7.1: Criar DbContext

**Arquivo:** `src/Modules/Parameters/Parameters.Infrastructure/Persistence/ParametersDbContext.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using Parameters.Domain.Entities;

namespace Parameters.Infrastructure.Persistence;

public class ParametersDbContext : DbContext
{
    public ParametersDbContext(DbContextOptions<ParametersDbContext> options)
        : base(options)
    {
    }

    public DbSet<E1> E1 { get; set; }
    public DbSet<E4> E4 { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuração E1
        modelBuilder.Entity<E1>(entity =>
        {
            entity.HasKey(e => e.E1Stamp);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(200);
            entity.HasIndex(e => e.Code).IsUnique();
        });
    }
}
```

#### 7.2: Criar DependencyInjection

**Arquivo:** `src/Modules/Parameters/Parameters.Infrastructure/DependencyInjection.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Parameters.Domain.Repositories;
using Parameters.Infrastructure.Persistence;
using Parameters.Infrastructure.Repositories;

namespace Parameters.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddParametersInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<ParametersDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("ParametersConnection"),
                b => b.MigrationsAssembly(typeof(ParametersDbContext).Assembly.FullName)));

        // Repositories
        services.AddScoped<IE1Repository, E1Repository>();

        return services;
    }
}
```

---

### PASSO 8: Camada API

#### 8.1: Criar Controller

**Arquivo:** `src/Modules/Parameters/Parameters.API/Controllers/ParametersController.cs`

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MediatR;
using Parameters.Application.Commands;
using Parameters.Application.Queries;
using Parameters.Application.DTOs;

namespace Parameters.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ParametersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ParametersController> _logger;

    public ParametersController(IMediator mediator, ILogger<ParametersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var query = new GetAllParametersQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateParameterDto dto)
    {
        var command = new CreateParameterCommand(
            dto.Code,
            dto.Description,
            dto.E4Details,
            User.Identity?.Name
        );

        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetByStamp), new { e1Stamp = result.E1Stamp }, result);
    }
}
```

#### 8.2: Criar DependencyInjection do Módulo

**Arquivo:** `src/Modules/Parameters/Parameters.API/DependencyInjection.cs`

```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using Parameters.Infrastructure;

namespace Parameters.API;

public static class DependencyInjection
{
    public static IServiceCollection AddParametersModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // MediatR
        services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssembly(
                typeof(Application.Commands.CreateParameterCommand).Assembly));

        // FluentValidation
        services.AddValidatorsFromAssembly(
            typeof(Application.Validators.CreateParameterCommandValidator).Assembly);

        // Infrastructure
        services.AddParametersInfrastructure(configuration);

        return services;
    }
}
```

---

### PASSO 9: Configurar o Host

#### 9.1: Atualizar Program.cs

**Arquivo:** `src/PHCAPI.Host/Program.cs`

```csharp
using Parameters.API;
using Parameters.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/PHCAPI-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// ✅ Registrar TODOS os módulos aqui
builder.Services.AddParametersModule(builder.Configuration);

// Add Controllers and API Explorer
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure pipeline
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

---

## 🔗 Como Funciona a Conexão

### 1. Referências de Projeto (.csproj)

Quando você adiciona uma referência:

```powershell
dotnet add reference ..\Other\Other.csproj
```

O MSBuild adiciona ao `.csproj`:

```xml
<ItemGroup>
  <ProjectReference Include="..\Other\Other.csproj" />
</ItemGroup>
```

**O que acontece:**
1. Durante o **build**, o MSBuild compila `Other.csproj` primeiro
2. A DLL é copiada para `bin`
3. O compilador pode "ver" as classes públicas

### 2. Dependency Injection

```csharp
builder.Services.AddParametersModule(builder.Configuration);
```

Registra **todos os serviços** do módulo no DI Container.

### 3. Controller Discovery

```csharp
builder.Services.AddControllers();
```

ASP.NET Core varre assemblies e descobre controllers automaticamente.

---

## 🚀 Deploy e Publish

### Como Fazer Publish

Publish **APENAS do Host**:

```powershell
cd src\PHCAPI.Host
dotnet publish -c Release -o .\publish
```

### O Que é Incluído

```
publish/
├── PHCAPI.Host.dll               ← Principal
├── Parameters.API.dll             ← Módulos (automático!)
├── Parameters.Application.dll
├── Parameters.Infrastructure.dll
├── Parameters.Domain.dll
└── ... (dependências)
```

**✅ Todos os módulos incluídos automaticamente!**

---

## ✅ Boas Práticas

1. **Domain:** Sem dependências externas
2. **Application:** Casos de uso isolados (Commands/Queries)
3. **Infrastructure:** Implementações de repositórios
4. **API:** Controllers delegam para MediatR
5. **Host:** Apenas configuração e startup

---

## 📚 Comandos Úteis

```powershell
# Build
dotnet build

# Run
dotnet run --project src\PHCAPI.Host

# Watch (hot reload)
dotnet watch --project src\PHCAPI.Host

# Migrations
dotnet ef migrations add InitialCreate \
  --project src\Modules\Parameters\Parameters.Infrastructure \
  --startup-project src\PHCAPI.Host

# Publish
dotnet publish -c Release -o .\publish
```

---

**Fim do Guia**

Para dúvidas: team@PHCAPI.com
