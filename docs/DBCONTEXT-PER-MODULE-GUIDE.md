# DbContext por Módulo - Guia Arquitetural

## 🎯 Decisão Arquitetural

**Sim! Cada módulo tem seu próprio DbContext.**

## 🏗️ Arquitetura Atual: Modular Monolith

### Estrutura de DbContexts

```
┌─────────────────────────────────────────────┐
│           PHCWEBAPI (Monolito Modular)      │
├─────────────────────────────────────────────┤
│                                             │
│  ┌──────────────────────────────────────┐  │
│  │  Parameters Module                   │  │
│  │  └── ParametersDbContextEFCore       │  │
│  │      └── Para1, Para2, Para3...      │  │
│  └──────────────────────────────────────┘  │
│                                             │
│  ┌──────────────────────────────────────┐  │
│  │  Orders Module                       │  │
│  │  └── OrdersDbContextEFCore           │  │
│  │      └── Order, OrderItem...         │  │
│  └──────────────────────────────────────┘  │
│                                             │
│  ┌──────────────────────────────────────┐  │
│  │  Customers Module                    │  │
│  │  └── CustomersDbContextEFCore        │  │
│  │      └── Customer, Address...        │  │
│  └──────────────────────────────────────┘  │
│                                             │
└─────────────────────────────────────────────┘
```

## ✅ Vantagens: DbContext por Módulo

### 1. **Isolamento e Independência**
```csharp
// Parameters Module - Totalmente independente
public class ParametersDbContextEFCore : DbContext
{
    public DbSet<Para1> Para1 { get; set; }
    // Apenas entidades do módulo Parameters
}

// Orders Module - Totalmente independente
public class OrdersDbContextEFCore : DbContext
{
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    // Apenas entidades do módulo Orders
}
```

**Benefícios:**
- ✅ Módulos não se acoplam pelo banco de dados
- ✅ Mudanças em um módulo não afetam outros
- ✅ Migrations independentes por módulo

### 2. **Bounded Contexts (DDD)**
```
Parameters Context:
- Para1 (entidade raiz)
- Para2
- Para3

Orders Context:
- Order (entidade raiz)
- OrderItem (agregado)
- OrderStatus

Customers Context:
- Customer (entidade raiz)
- Address (value object)
```

**Benefícios:**
- ✅ Cada contexto tem suas próprias regras de negócio
- ✅ Termos ubíquos do domínio bem definidos
- ✅ Fronteiras claras entre contextos

### 3. **Preparação para Microservices**
```
HOJE (Monolito):
┌──────────────────────────────────┐
│  PHCWEBAPI.exe                   │
│  ├── ParametersDbContextEFCore   │
│  ├── OrdersDbContextEFCore       │
│  └── CustomersDbContextEFCore    │
└──────────────────────────────────┘

AMANHÃ (Microservices):
┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐
│ Parameters.API  │  │ Orders.API      │  │ Customers.API   │
│ (porta 5001)    │  │ (porta 5002)    │  │ (porta 5003)    │
│ ├── DbContext   │  │ ├── DbContext   │  │ ├── DbContext   │
│ └── DB_Params   │  │ └── DB_Orders   │  │ └── DB_Customers│
└─────────────────┘  └─────────────────┘  └─────────────────┘
```

**Benefícios:**
- ✅ **Zero refatoração** para extrair módulo
- ✅ Cada serviço pode ter seu próprio banco
- ✅ Escalabilidade independente

### 4. **Migrations Independentes**
```bash
# Criar migration apenas para Parameters
dotnet ef migrations add AddNewFieldToPara1 --context ParametersDbContextEFCore

# Criar migration apenas para Orders
dotnet ef migrations add AddOrderStatus --context OrdersDbContextEFCore

# Cada módulo mantém seu histórico de migrations separado
```

**Benefícios:**
- ✅ Histórico de mudanças isolado
- ✅ Rollback independente
- ✅ Deploy de módulos separadamente

### 5. **Bancos de Dados Diferentes (Opcional)**
```json
// appsettings.json
{
  "ConnectionStrings": {
    "ParametersConnection": "Server=localhost;Database=PHC_Parameters;...",
    "OrdersConnection": "Server=localhost;Database=PHC_Orders;...",
    "CustomersConnection": "Server=localhost;Database=PHC_Customers;..."
  }
}
```

Ou até tecnologias diferentes:
```json
{
  "ConnectionStrings": {
    "ParametersConnection": "Server=localhost;Database=PHC_Parameters;...", // SQL Server
    "OrdersConnection": "Host=localhost;Database=PHC_Orders;...",          // PostgreSQL
    "CustomersConnection": "mongodb://localhost:27017/PHC_Customers"       // MongoDB
  }
}
```

**Benefícios:**
- ✅ Escolher a melhor tecnologia para cada domínio
- ✅ Otimizações específicas por módulo
- ✅ Migração gradual de tecnologias

### 6. **Segurança e Permissões**
```csharp
// Usuários diferentes para cada módulo
services.AddDbContext<ParametersDbContextEFCore>(options =>
    options.UseSqlServer(
        "Server=localhost;Database=PHC;User=params_user;Password=..."));

services.AddDbContext<OrdersDbContextEFCore>(options =>
    options.UseSqlServer(
        "Server=localhost;Database=PHC;User=orders_user;Password=..."));
```

**Benefícios:**
- ✅ Princípio do menor privilégio
- ✅ Auditoria mais granular
- ✅ Isolamento de segurança

## ❌ Desvantagens: DbContext por Módulo

### 1. **Transações Distribuídas**
```csharp
// ❌ PROBLEMA: Transação entre módulos
public async Task CreateOrderWithCustomerUpdate()
{
    // Como garantir atomicidade entre 2 DbContexts?
    await _ordersContext.Orders.AddAsync(order);
    await _ordersContext.SaveChangesAsync();
    
    await _customersContext.Customers.UpdateAsync(customer);
    await _customersContext.SaveChangesAsync();
    
    // E se a segunda falhar? 🤔
}
```

**Soluções:**
1. **Saga Pattern** - Transações compensatórias
2. **Outbox Pattern** - Eventos garantidos
3. **Event Sourcing** - Log de eventos
4. **Evitar transações entre módulos** - Design melhor!

### 2. **Relacionamentos Entre Módulos**
```csharp
// ❌ NÃO é possível fazer Foreign Key entre módulos
public class Order
{
    public int OrderId { get; set; }
    
    // ❌ Não pode ter FK para Customer (está em outro DbContext)
    public int CustomerId { get; set; }  // Apenas ID, sem navegação
    
    // ❌ Isso NÃO funciona:
    // public Customer Customer { get; set; }  
}
```

**Solução:**
- ✅ Usar apenas IDs para referenciar outras entidades
- ✅ Buscar dados de outros módulos via repositórios/services
- ✅ Eventual consistency entre módulos

### 3. **Mais Connections ao Banco**
```
1 Request pode abrir múltiplas connections:
┌─────────────┐
│  Request    │
└──────┬──────┘
       ├────► Connection 1 (ParametersDbContext)
       ├────► Connection 2 (OrdersDbContext)
       └────► Connection 3 (CustomersDbContext)
```

**Mitigação:**
- ✅ Connection pooling (EF Core faz automaticamente)
- ✅ Async/await (libera threads enquanto aguarda)
- ✅ Limitar operações cross-module em uma requisição

## 🔄 Alternativa: DbContext Único (Não Recomendado)

### Estrutura
```csharp
// ❌ Anti-pattern: Tudo em um único contexto
public class ApplicationDbContext : DbContext
{
    // Parameters
    public DbSet<Para1> Para1 { get; set; }
    
    // Orders
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    
    // Customers
    public DbSet<Customer> Customers { get; set; }
    
    // ... todas as entidades juntas
}
```

### Problemas
```
❌ Acoplamento entre módulos
❌ Migrations gigantes e complexas
❌ Impossível separar em microservices
❌ Viola Bounded Contexts
❌ Dificulta manutenção
❌ Testes mais lentos
❌ Deploy all-or-nothing
```

## 📋 Exemplo Completo: Comunicação Entre Módulos

### Cenário: Order precisa validar Customer

**❌ ERRADO (acoplamento direto):**
```csharp
public class OrdersDbContext : DbContext
{
    public DbSet<Order> Orders { get; set; }
    public DbSet<Customer> Customers { get; set; }  // ❌ Entidade de outro módulo!
}
```

**✅ CORRETO (comunicação via interface):**
```csharp
// Orders.Application/Interfaces/ICustomerService.cs
public interface ICustomerService
{
    Task<bool> CustomerExistsAsync(int customerId);
    Task<CustomerDto> GetCustomerAsync(int customerId);
}

// Orders.Application/Features/CreateOrder/CreateOrderCommandHandler.cs
public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<Order>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerService _customerService;  // ✅ Interface, não dependência direta
    
    public async Task<Result<Order>> Handle(CreateOrderCommand request, CancellationToken ct)
    {
        // 1. Validar se customer existe (chamada cross-module)
        if (!await _customerService.CustomerExistsAsync(request.CustomerId))
            return Result<Order>.Failure("Customer not found");
        
        // 2. Criar order (dentro do próprio módulo)
        var order = new Order(request.CustomerId, request.Items);
        await _orderRepository.AddAsync(order, ct);
        
        return Result<Order>.Success(order);
    }
}

// Customers.Application/Services/CustomerService.cs (implementação)
public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    
    public async Task<bool> CustomerExistsAsync(int customerId)
    {
        return await _customerRepository.ExistsAsync(customerId);
    }
}
```

## 🎯 Recomendação Final

### ✅ Use DbContext por Módulo se:
- ✅ Planeja crescer para microservices no futuro
- ✅ Quer módulos verdadeiramente independentes
- ✅ Valoriza bounded contexts e DDD
- ✅ Quer migrations independentes
- ✅ Pode trabalhar com eventual consistency

### ❌ Use DbContext Único apenas se:
- ❌ É uma aplicação pequena que nunca vai crescer
- ❌ Precisa de transações ACID cross-module constantemente
- ❌ Não planeja separar em microservices
- ❌ Equipe pequena sem conhecimento de arquitetura distribuída

## 📊 Comparação Visual

```
┌───────────────────────────────────────────────────────────────┐
│                      DBCONTEXT STRATEGY                       │
├───────────────────────┬───────────────────────────────────────┤
│  Por Módulo (✅)      │  Único (❌)                           │
├───────────────────────┼───────────────────────────────────────┤
│ Isolamento            │ Acoplamento                           │
│ Bounded Contexts      │ Big Ball of Mud                       │
│ Microservices Ready   │ Monolito para sempre                  │
│ Migrations separadas  │ Migrations gigantes                   │
│ Escalável             │ Limitado                              │
│ Complexidade inicial+ │ Simplicidade inicial                  │
└───────────────────────┴───────────────────────────────────────┘
```

## 🚀 Sua Arquitetura Atual (Correta!)

```csharp
// ✅ Parameters Module - Isolado
Parameters.Infrastructure/
└── ParametersDbContextEFCore
    └── DbSet<Para1>

// ✅ Future Orders Module - Isolado
Orders.Infrastructure/
└── OrdersDbContextEFCore
    └── DbSet<Order>
    └── DbSet<OrderItem>

// ✅ Future Customers Module - Isolado
Customers.Infrastructure/
└── CustomersDbContextEFCore
    └── DbSet<Customer>
    └── DbSet<Address>
```

**Conclusão:** Você está no caminho certo! Continue com **DbContext por módulo**. 🎯
