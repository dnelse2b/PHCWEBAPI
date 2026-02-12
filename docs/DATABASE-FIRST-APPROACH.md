# Database First Approach - PHCWEBAPI

## 🎯 Abordagem Utilizada

Este projeto utiliza **Database First** - as tabelas **já existem** no banco de dados e o EF Core apenas **mapeia** as entidades para as tabelas existentes.

## 🏗️ Estrutura Database First com Clean Architecture

```
┌─────────────────────────────────────────────┐
│    Banco de Dados Existente (PHC)          │
│                                             │
│  ┌────────────┐  ┌────────────┐            │
│  │   para1    │  │   e1       │            │
│  │   (tabela) │  │   (tabela) │            │
│  └────────────┘  └────────────┘            │
└─────────────────────────────────────────────┘
                    ▲
                    │ Mapeia (não cria)
                    │
┌─────────────────────────────────────────────┐
│   EF Core Configuration                     │
│                                             │
│  Para1ConfigurationEFCore                   │
│  └── builder.ToTable("para1")               │
│  └── builder.Property(p => p.ParaStamp)     │
│      .HasColumnName("parastamp")            │
└─────────────────────────────────────────────┘
                    ▲
                    │ Usa
                    │
┌─────────────────────────────────────────────┐
│   Domain Entity                             │
│                                             │
│  public class Para1 : AuditableEntity       │
│  {                                          │
│      public string ParaStamp { get; set; }  │
│      public string Descricao { get; set; }  │
│  }                                          │
└─────────────────────────────────────────────┘
```

## ✅ O Que Database First Significa

### 1. **Tabelas Já Existem**
```sql
-- Tabela criada FORA do EF Core (já existe no banco PHC)
CREATE TABLE para1 (
    parastamp VARCHAR(50) PRIMARY KEY,
    descricao VARCHAR(200) NOT NULL,
    valor VARCHAR(500) NOT NULL,
    tipo VARCHAR(50) NOT NULL,
    dec INT NULL,
    tam INT NULL,
    ousrdata DATE NOT NULL,
    ousrhora VARCHAR(8) NOT NULL,
    ousrinis VARCHAR(100) NULL,
    usrdata DATE NULL,
    usrhora VARCHAR(8) NULL,
    usrinis VARCHAR(100) NULL
)
```

### 2. **EF Core Apenas Mapeia**
```csharp
// Para1ConfigurationEFCore.cs
// NÃO cria a tabela - apenas mapeia propriedades → colunas
public class Para1ConfigurationEFCore : IEntityTypeConfiguration<Para1>
{
    public void Configure(EntityTypeBuilder<Para1> builder)
    {
        // Mapeia para tabela existente
        builder.ToTable("para1");
        
        // Mapeia propriedade → coluna existente
        builder.Property(p => p.ParaStamp)
            .HasColumnName("parastamp")  // ← Coluna já existe!
            .HasMaxLength(50)
            .IsRequired();
        
        // Não cria nada - apenas diz ao EF Core como mapear
    }
}
```

### 3. **Sem Migrations**
```csharp
// ❌ NÃO precisa disso
dotnet ef migrations add InitialCreate
dotnet ef database update

// ❌ NÃO precisa no Program.cs
dbContext.Database.Migrate();

// ✅ Tabelas já existem - EF Core só usa!
```

## 🔄 Database First vs Code First

### ❌ Code First (NÃO usado aqui)
```csharp
// 1. Cria entidade
public class Para1 { ... }

// 2. Cria migration
dotnet ef migrations add CreatePara1

// 3. Cria tabela no banco
dotnet ef database update

// ❌ Problema: Banco PHC já existe e tem suas próprias tabelas!
```

### ✅ Database First (USADO aqui)
```csharp
// 1. Tabela já existe no banco PHC
// SELECT * FROM para1

// 2. Cria entidade que reflete a tabela
public class Para1 { ... }

// 3. Cria configuração que mapeia entidade → tabela
public class Para1ConfigurationEFCore { ... }

// ✅ Pronto! EF Core usa a tabela existente
```

## 📋 Workflow Database First

### 1. Identificar Tabela Existente
```sql
-- Verificar estrutura da tabela no banco PHC
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'para1'
```

### 2. Criar Entidade Domain
```csharp
// Parameters.Domain/Entities/Para1.cs
public class Para1 : AuditableEntity
{
    // Propriedades que refletem as colunas da tabela
    public string ParaStamp { get; private set; }
    public string Descricao { get; private set; }
    public string Valor { get; private set; }
    // ...
}
```

### 3. Criar Configuração EF Core
```csharp
// Parameters.Infrastructure/Persistence/Configurations/Para1ConfigurationEFCore.cs
public class Para1ConfigurationEFCore : IEntityTypeConfiguration<Para1>
{
    public void Configure(EntityTypeBuilder<Para1> builder)
    {
        // Mapear para tabela existente
        builder.ToTable("para1");
        
        // Mapear cada propriedade → coluna
        builder.Property(p => p.ParaStamp)
            .HasColumnName("parastamp");
        
        builder.Property(p => p.Descricao)
            .HasColumnName("descricao");
        
        // ...
    }
}
```

### 4. Registrar no DbContext
```csharp
// Parameters.Infrastructure/Persistence/ParametersDbContextEFCore.cs
public class ParametersDbContextEFCore : DbContext
{
    public DbSet<Para1> Para1 { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Aplicar configuração
        modelBuilder.ApplyConfiguration(new Para1ConfigurationEFCore());
    }
}
```

### 5. Testar Mapeamento
```csharp
// Verificar se o mapeamento está correto
var para1 = await _context.Para1.FirstOrDefaultAsync();
// ✅ Se funcionar, o mapeamento está correto!
```

## 🎯 Vantagens Database First (PHC)

### 1. **Integração com Sistema Legado**
```
✅ Banco PHC já existe com anos de dados
✅ Outras aplicações já usam as mesmas tabelas
✅ DBA controla a estrutura do banco
✅ Não pode modificar tabelas arbitrariamente
```

### 2. **Separação de Responsabilidades**
```
DBA:          Cuida do schema do banco
Desenvolvedor: Mapeia entidades para tabelas existentes
```

### 3. **Módulos Independentes (mas mesmo banco)**
```sql
-- Mesmo banco, tabelas diferentes por módulo

-- Parameters Module
SELECT * FROM para1

-- Orders Module (futuro)
SELECT * FROM orders

-- Customers Module (futuro)
SELECT * FROM customers
```

### 4. **Clean Architecture Mantida**
```
✅ Domain não conhece banco (entidades puras)
✅ Infrastructure faz o mapeamento
✅ Database First não quebra Clean Architecture!
```

## ⚠️ Cuidados Database First

### 1. **Sincronização Manual**
```csharp
// Se DBA adicionar coluna nova:
ALTER TABLE para1 ADD new_column VARCHAR(100)

// Você precisa:
// 1. Adicionar propriedade na entidade
public string NewColumn { get; set; }

// 2. Mapear na configuração
builder.Property(p => p.NewColumn)
    .HasColumnName("new_column");
```

### 2. **Convenções de Nomes**
```csharp
// Banco PHC usa lowercase
parastamp, descricao, valor

// Entidade C# usa PascalCase
ParaStamp, Descricao, Valor

// Configuração faz o mapeamento
builder.Property(p => p.ParaStamp)
    .HasColumnName("parastamp");  // ← Importante!
```

### 3. **Campos de Auditoria**
```csharp
// Banco PHC tem padrão específico:
ousrdata, ousrhora, ousrinis  // Criação
usrdata, usrhora, usrinis      // Atualização

// Entidade herda de AuditableEntity que já mapeia isso
public class Para1 : AuditableEntity  // ← Já tem campos mapeados
```

### 4. **Não Usar Migrations**
```bash
# ❌ NÃO fazer:
dotnet ef migrations add AddNewField
dotnet ef database update

# ✅ Processo correto:
# 1. DBA altera tabela no banco
# 2. Você atualiza entidade + configuração
# 3. Testa o mapeamento
```

## 🔧 Configuração Connection Strings

### appsettings.json
```json
{
  "ConnectionStrings": {
    // Banco PHC existente
    "DBconnect": "Server=SRV05\\SQLDEV2022;Database=BILENE_DESENV;...",
    
    // Outro banco PHC (CFM)
    "CFMGERALConnStr": "Server=nacala;Database=E14E105BD_CFM;..."
  }
}
```

### DependencyInjection.cs
```csharp
public static IServiceCollection AddParametersInfrastructure(...)
{
    services.AddDbContext<ParametersDbContextEFCore>((sp, options) =>
    {
        // Conecta ao banco PHC existente
        options.UseSqlServer(
            configuration.GetConnectionString("DBconnect"));
    });
}
```

## 📊 Exemplo Completo: Adicionar Nova Entidade

### Cenário: Mapear tabela `e1` existente

**1. Verificar tabela no banco:**
```sql
SELECT * FROM e1
-- Descobrir colunas: e1stamp, code, description, active, created_at...
```

**2. Criar entidade:**
```csharp
// Parameters.Domain/Entities/E1.cs
public class E1 : AuditableEntity
{
    public string E1Stamp { get; private set; }
    public string Code { get; private set; }
    public string Description { get; private set; }
    public bool Active { get; private set; }
    
    private E1() { }
    
    public E1(string code, string description)
    {
        E1Stamp = StampExtensions.GenerateStamp();
        Code = code;
        Description = description;
        Active = true;
    }
}
```

**3. Criar configuração:**
```csharp
// Parameters.Infrastructure/Persistence/Configurations/E1ConfigurationEFCore.cs
public class E1ConfigurationEFCore : IEntityTypeConfiguration<E1>
{
    public void Configure(EntityTypeBuilder<E1> builder)
    {
        builder.ToTable("e1");  // ← Tabela existente
        
        builder.HasKey(e => e.E1Stamp);
        
        builder.Property(e => e.E1Stamp)
            .HasColumnName("e1stamp")
            .HasMaxLength(50);
        
        builder.Property(e => e.Code)
            .HasColumnName("code")
            .HasMaxLength(50);
        
        builder.Property(e => e.Description)
            .HasColumnName("description")
            .HasMaxLength(200);
        
        builder.Property(e => e.Active)
            .HasColumnName("active");
    }
}
```

**4. Registrar no DbContext:**
```csharp
public class ParametersDbContextEFCore : DbContext
{
    public DbSet<Para1> Para1 { get; set; }
    public DbSet<E1> E1 { get; set; }  // ← Novo
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new Para1ConfigurationEFCore());
        modelBuilder.ApplyConfiguration(new E1ConfigurationEFCore());  // ← Novo
    }
}
```

**5. Criar repositório:**
```csharp
// Parameters.Domain/Repositories/IE1Repository.cs
public interface IE1Repository
{
    Task<E1?> GetByStampAsync(string e1Stamp, CancellationToken ct);
    Task<E1> AddAsync(E1 e1, CancellationToken ct);
    // ...
}

// Parameters.Infrastructure/Repositories/E1RepositoryEFCore.cs
public class E1RepositoryEFCore : IE1Repository
{
    private readonly ParametersDbContextEFCore _context;
    
    public async Task<E1?> GetByStampAsync(string e1Stamp, CancellationToken ct)
    {
        return await _context.E1
            .FirstOrDefaultAsync(e => e.E1Stamp == e1Stamp, ct);
    }
    
    // ...
}
```

**✅ Pronto! Tabela `e1` existente agora está mapeada.**

## 🎓 Resumo

| Aspecto | Database First (PHCWEBAPI) |
|---------|---------------------------|
| **Tabelas** | ✅ Já existem no banco PHC |
| **Migrations** | ❌ Não são usadas |
| **Configurações EF** | ✅ Apenas mapeiam |
| **Controle Schema** | DBA controla |
| **Clean Architecture** | ✅ Mantida |
| **Modularização** | ✅ DbContext por módulo |
| **Vantagem** | Integração com sistema legado |

## 📝 Checklist: Adicionar Nova Tabela

- [ ] Verificar estrutura da tabela no banco
- [ ] Criar entidade no Domain
- [ ] Criar configuração EF Core na Infrastructure
- [ ] Registrar no DbContext
- [ ] Criar interface de repositório
- [ ] Implementar repositório EF Core
- [ ] Registrar repositório no DI
- [ ] Testar mapeamento
- [ ] ❌ **NÃO** criar migration

**Database First + Clean Architecture + Modular = Arquitetura Robusta!** 🎯
