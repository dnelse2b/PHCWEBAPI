# Parameters Module

Módulo de **Parâmetros** implementado com **Clean Architecture** para aplicações enterprise.

## 🏗️ Arquitetura

O módulo segue Clean Architecture dividido em 4 camadas:

```
Parameters/
├── Parameters.Domain/          ← Entidades, Value Objects, Interfaces
├── Parameters.Application/     ← Use Cases, DTOs, Commands, Queries
├── Parameters.Infrastructure/  ← Implementações EF Core (Repositories, DbContext, Configurations)
│                                  └── usa Shared.Infrastructure (AuditableEntityInterceptorEFCore)
└── Parameters.Presentation/    ← REST Controllers, GraphQL, Endpoints
```

**🔧 Desacoplamento de Tecnologia:**
- Classes de infraestrutura com sufixo `EFCore` (ex: `Para1RepositoryEFCore`, `ParametersDbContextEFCore`)
- **Auditoria centralizada** em `Shared.Infrastructure` - reutilizada por todos os módulos
- Facilita adicionar outras implementações (Dapper, MongoDB, etc.) sem conflitos
- Domain e Application **nunca** dependem de tecnologias específicas

## 📊 Modelo de Dados

### Entidades

#### **E1** (Tabela Principal - `e1`)
- `e1stamp` (PK) - Identificador único (string)
- `code` - Código do parâmetro (único)
- `description` - Descrição
- `active` - Status ativo/inativo
- `created_at` - Data de criação
- `updated_at` - Data de atualização
- `created_by` - Usuário que criou
- `updated_by` - Usuário que atualizou

#### **E4** (Tabela Complementar - `e4`)
- `e4stamp` (PK, FK) - Mesmo valor que `e1stamp`
- `value` - Valor do parâmetro
- `additional_info` - Informações adicionais
- `sequence` - Sequência/ordem
- `created_at` - Data de criação
- `updated_at` - Data de atualização

**Relação:** 1:1 entre E1 e E4 através do stamp (e1stamp = e4stamp)

## 🚀 Funcionalidades

### CQRS Pattern com MediatR

#### **Commands**
- `CreateParameterCommand` - Criar novo parâmetro
- `UpdateParameterCommand` - Atualizar parâmetro
- `DeleteParameterCommand` - Deletar parâmetro

#### **Queries**
- `GetParameterByStampQuery` - Buscar por stamp
- `GetAllParametersQuery` - Listar todos (com filtro de ativos)

### Validações com FluentValidation
- Validação de campos obrigatórios
- Validação de tamanhos máximos
- Validação de regras de negócio

## 📡 API Endpoints

```http
GET    /api/parameters              # Listar todos
GET    /api/parameters/{e1stamp}    # Buscar por stamp
POST   /api/parameters              # Criar
PUT    /api/parameters/{e1stamp}    # Atualizar
DELETE /api/parameters/{e1stamp}    # Deletar
```

### Exemplo de Request (POST)

```json
{
  "code": "PARAM001",
  "description": "Parâmetro de teste",
  "e4Details": {
    "value": "Valor do parâmetro",
    "additionalInfo": "Informações adicionais",
    "sequence": 1
  }
}
```

### Exemplo de Response

```json
{
  "e1Stamp": "202401151430001A2B3C4D",
  "code": "PARAM001",
  "description": "Parâmetro de teste",
  "active": true,
  "createdAt": "2024-01-15T14:30:00Z",
  "updatedAt": null,
  "createdBy": "admin",
  "updatedBy": null,
  "e4Details": {
    "e4Stamp": "202401151430001A2B3C4D",
    "value": "Valor do parâmetro",
    "additionalInfo": "Informações adicionais",
    "sequence": 1,
    "createdAt": "2024-01-15T14:30:00Z",
    "updatedAt": null
  }
}
```

## 🔧 Configuração

### 1. Database Connection (Database First)

**⚠️ Importante:** Este projeto usa **Database First** - as tabelas **já existem** no banco PHC.

Adicionar no `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DBconnect": "Server=SRV05\\SQLDEV2022;Database=BILENE_DESENV;User Id=websa;password=W3B123;..."
  }
}
```

### 2. Registrar o Módulo

No `Program.cs`:

```csharp
// Add Parameters Module (Database First)
builder.Services.AddParametersInfrastructure(builder.Configuration);
builder.Services.AddParametersApplication();
builder.Services.AddParametersPresentation();
```

### 3. ❌ NÃO precisa de Migrations

**Database First** - as tabelas **já existem** no banco PHC:
- ✅ Tabela `para1` já existe
- ✅ EF Core apenas mapeia as colunas
- ❌ **NÃO** executar `dotnet ef migrations`
- ❌ **NÃO** executar `database update`

Ver documentação completa em: [docs/DATABASE-FIRST-APPROACH.md](../../../docs/DATABASE-FIRST-APPROACH.md)

## 🧪 Testes

### Unit Tests (Futuramente)
```
tests/Unit/Parameters.Domain.Tests/
tests/Unit/Parameters.Application.Tests/
```

### Integration Tests (Futuramente)
```
tests/Integration/Parameters.API.Tests/
```

## 🔐 Segurança Enterprise

- Autenticação via JWT (integrar com Identity.Server)
- Autorização baseada em roles
- Auditoria de mudanças (CreatedBy, UpdatedBy)
- Validação de entrada com FluentValidation
- Rate limiting (implementar)

## 📈 Performance

- Índices otimizados no banco
- Paginação nas consultas
- Async/Await em todas operações
- Connection pooling do EF Core

## 🏢 Padrões Enterprise

✅ **Clean Architecture** - Separação clara de responsabilidades  
✅ **CQRS** - Commands e Queries separados  
✅ **Repository Pattern** - Abstração de acesso a dados  
✅ **Dependency Injection** - Inversão de dependência  
✅ **Domain-Driven Design** - Entidades ricas  
✅ **SOLID Principles** - Código limpo e manutenível  
✅ **Technology Independence** - Fácil trocar EF Core por Dapper, MongoDB, etc.

## 🔄 Estrutura Infrastructure (EF Core)

```
Infrastructure/
├── Persistence/
│   ├── ParametersDbContextEFCore.cs         ← DbContext EF Core
│   ├── Configurations/
│   │   └── Para1ConfigurationEFCore.cs      ← Entity Configuration
│   └── Interceptors/
│       └── AuditableEntityInterceptorEFCore.cs  ← Auditoria automática
└── Repositories/
    └── Para1RepositoryEFCore.cs             ← Implementação EF Core
```

**Para adicionar outra tecnologia (ex: Dapper):**
1. Criar `Para1RepositoryDapper.cs : IPara1Repository`
2. Registrar no DI: `services.AddScoped<IPara1Repository, Para1RepositoryDapper>()`
3. **Zero mudanças nas camadas Domain e Application** ✨

## 📦 Dependências

- .NET 8.0
- Entity Framework Core 8.0
- MediatR 12.2
- FluentValidation 11.9
- Serilog (logging)
- Swashbuckle (Swagger/OpenAPI)

## 🔄 Próximos Passos

1. Implementar testes unitários e de integração
2. Adicionar cache (Redis)
3. Implementar eventos de domínio
4. Adicionar health checks
5. Implementar pagination avançada
6. Adicionar suporte a multi-tenancy
7. Implementar audit trail completo
8. Configurar CI/CD pipeline

## 📚 Referências

- [Clean Architecture - Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [CQRS Pattern](https://martinfowler.com/bliki/CQRS.html)
- [Entity Framework Core Best Practices](https://docs.microsoft.com/ef/core/)
