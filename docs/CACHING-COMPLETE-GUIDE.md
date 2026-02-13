# 🚀 Caching - Guia Completo para Engenheiros Sênior

## 📚 Índice
1. [O que é Cache](#o-que-é-cache)
2. [Tipos de Cache](#tipos-de-cache)
3. [Estratégias de Cache](#estratégias-de-cache)
4. [Cache no .NET 8](#cache-no-net-8)
5. [Cache no Módulo Audit](#cache-no-módulo-audit)
6. [Redis vs In-Memory](#redis-vs-in-memory)
7. [Cache Invalidation](#cache-invalidation)
8. [Performance vs Complexidade](#performance-vs-complexidade)
9. [Implementações Práticas](#implementações-práticas)
10. [Antipatterns](#antipatterns)

---

## 🎯 O que é Cache?

### **Definição**
Cache é uma **camada de armazenamento temporário de alta velocidade** que guarda **dados frequentemente acessados** para reduzir:
- **Latência** (tempo de resposta)
- **Carga no banco de dados**
- **Custo computacional**

### **Princípio Fundamental**
> "Cache everything you can, invalidate what you must"

### **Trade-off Principal**
```
┌─────────────────────────────────────────────┐
│  Velocidade  ◄──────────► Atualização       │
├─────────────────────────────────────────────┤
│  Mais cache = Mais rápido, dados desatualizados │
│  Menos cache = Mais lento, dados sempre atuais  │
└─────────────────────────────────────────────┘
```

---

## 📊 Tipos de Cache

### **1. Client-Side Cache**

**Localização**: Browser, App Mobile

**Exemplos**:
- LocalStorage
- SessionStorage
- HTTP Cache Headers

**Vantagens**:
- ✅ Reduz chamadas ao servidor
- ✅ Offline-first

**Desvantagens**:
- ❌ Dados podem ficar muito desatualizados
- ❌ Sem controle do servidor

---

### **2. HTTP Cache (CDN/Proxy)**

**Localização**: Nginx, Cloudflare, Azure CDN

**HTTP Headers**:
```http
Cache-Control: public, max-age=3600
ETag: "33a64df551425fcc55e4d42a148795d9f25f89d4"
Last-Modified: Wed, 15 Nov 2023 12:45:26 GMT
```

**Vantagens**:
- ✅ Reduz latência globalmente
- ✅ Reduz custo de bandwidth
- ✅ Escalabilidade

**Desvantagens**:
- ❌ Difícil invalidar cache
- ❌ Não funciona para dados privados (usuário-específico)

---

### **3. Application Cache (In-Memory)**

**Localização**: RAM do servidor

**Tecnologias**:
- .NET: `IMemoryCache`
- Java: Caffeine, Guava
- Node.js: node-cache

**Vantagens**:
- ✅ Extremamente rápido (nanosegundos)
- ✅ Simples de implementar
- ✅ Sem dependência externa

**Desvantagens**:
- ❌ Não compartilhado entre servidores
- ❌ Dados perdidos ao reiniciar app
- ❌ Limitado pela RAM do servidor

**Exemplo .NET**:
```csharp
public class CachedService
{
    private readonly IMemoryCache _cache;
    
    public async Task<User> GetUserAsync(int id)
    {
        var cacheKey = $"user_{id}";
        
        if (_cache.TryGetValue(cacheKey, out User user))
        {
            return user; // ✅ Cache HIT (rápido!)
        }
        
        // ❌ Cache MISS (vai no banco)
        user = await _db.Users.FindAsync(id);
        
        // Guardar no cache por 5 minutos
        _cache.Set(cacheKey, user, TimeSpan.FromMinutes(5));
        
        return user;
    }
}
```

---

### **4. Distributed Cache (Redis, Memcached)**

**Localização**: Servidor separado (in-memory distribuído)

**Tecnologias**:
- **Redis** (recomendado)
- Memcached
- NCache
- Azure Cache for Redis

**Vantagens**:
- ✅ Compartilhado entre múltiplos servidores
- ✅ Persiste dados (Redis Persistence)
- ✅ Escalável horizontalmente
- ✅ Features extras (Pub/Sub, Streams, etc)

**Desvantagens**:
- ❌ Mais lento que in-memory local (~1ms vs 10ns)
- ❌ Dependência externa
- ❌ Complexidade adicional

**Exemplo .NET com Redis**:
```csharp
public class RedisService
{
    private readonly IDistributedCache _cache;
    
    public async Task<User> GetUserAsync(int id)
    {
        var cacheKey = $"user_{id}";
        
        // Buscar no Redis
        var cachedData = await _cache.GetStringAsync(cacheKey);
        
        if (cachedData != null)
        {
            return JsonSerializer.Deserialize<User>(cachedData);
        }
        
        // Cache miss
        var user = await _db.Users.FindAsync(id);
        
        // Salvar no Redis
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        };
        
        await _cache.SetStringAsync(
            cacheKey, 
            JsonSerializer.Serialize(user),
            options);
        
        return user;
    }
}
```

---

### **5. Database Cache (Query Cache)**

**Localização**: Dentro do próprio banco de dados

**Exemplos**:
- SQL Server: Query Plan Cache
- PostgreSQL: Shared Buffers
- MySQL: Query Cache (deprecated)

**Vantagens**:
- ✅ Automático (transparente)
- ✅ Gerenciado pelo SGBD

**Desvantagens**:
- ❌ Limitado ao servidor do banco
- ❌ Pouco controle

---

### **6. Output Cache (Response Cache)**

**Localização**: Servidor web

**Funcionalidade**: Cacheia a **resposta HTTP inteira**

**Exemplo ASP.NET Core**:
```csharp
[ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
[HttpGet("{id}")]
public async Task<IActionResult> GetUser(int id)
{
    var user = await _db.Users.FindAsync(id);
    return Ok(user);
}
```

**Headers enviados**:
```http
Cache-Control: public, max-age=60
```

---

## 🎨 Estratégias de Cache

### **1. Cache-Aside (Lazy Loading)**

**Padrão mais comum**

```
┌──────────────────────────────────────────┐
│ 1. Aplicação busca no cache              │
│    ├─ HIT? Retorna do cache ✅           │
│    └─ MISS? Vai para etapa 2             │
│                                           │
│ 2. Aplicação busca no banco              │
│                                           │
│ 3. Aplicação salva no cache              │
│                                           │
│ 4. Aplicação retorna dado ao cliente     │
└──────────────────────────────────────────┘
```

**Código**:
```csharp
public async Task<T> GetAsync<T>(string key, Func<Task<T>> fetchFunc, TimeSpan expiration)
{
    // 1. Tentar buscar no cache
    if (_cache.TryGetValue(key, out T cachedValue))
    {
        return cachedValue;
    }
    
    // 2. Cache miss - buscar da fonte
    var value = await fetchFunc();
    
    // 3. Salvar no cache
    _cache.Set(key, value, expiration);
    
    // 4. Retornar
    return value;
}

// Uso:
var user = await GetAsync(
    key: $"user_{id}",
    fetchFunc: () => _db.Users.FindAsync(id),
    expiration: TimeSpan.FromMinutes(5)
);
```

**Vantagens**:
- ✅ Simples de implementar
- ✅ Cache só guarda dados usados (lazy)
- ✅ Tolerante a falhas do cache

**Desvantagens**:
- ❌ Primeira requisição sempre lenta (cold start)
- ❌ Cache pode ficar desatualizado

---

### **2. Read-Through**

**Padrão**: Cache intercepta TODAS as leituras

```
┌──────────────────────────────────────────┐
│ Aplicação → Cache Layer → Database       │
│             (transparente)                │
└──────────────────────────────────────────┘
```

**Implementação**:
```csharp
public interface IRepository<T>
{
    Task<T> GetByIdAsync(int id);
}

public class CachedRepository<T> : IRepository<T>
{
    private readonly IRepository<T> _innerRepo;
    private readonly IMemoryCache _cache;
    
    public async Task<T> GetByIdAsync(int id)
    {
        var key = $"{typeof(T).Name}_{id}";
        
        if (_cache.TryGetValue(key, out T value))
            return value;
        
        // Delega para o repositório real
        value = await _innerRepo.GetByIdAsync(id);
        
        _cache.Set(key, value, TimeSpan.FromMinutes(10));
        
        return value;
    }
}
```

**Vantagens**:
- ✅ Transparente para a aplicação
- ✅ Centralizado (DRY)

**Desvantagens**:
- ❌ Mais complexo
- ❌ Pode esconder problemas de performance

---

### **3. Write-Through**

**Padrão**: Cache é atualizado SEMPRE que dados são escritos

```
┌──────────────────────────────────────────┐
│ 1. Aplicação escreve no cache            │
│ 2. Cache escreve no banco (síncrono)     │
│ 3. Retorna sucesso                       │
└──────────────────────────────────────────┘
```

**Código**:
```csharp
public async Task UpdateUserAsync(User user)
{
    // 1. Atualizar no banco
    await _db.Users.UpdateAsync(user);
    
    // 2. Atualizar no cache IMEDIATAMENTE
    var cacheKey = $"user_{user.Id}";
    _cache.Set(cacheKey, user, TimeSpan.FromMinutes(10));
}
```

**Vantagens**:
- ✅ Cache sempre atualizado
- ✅ Leituras sempre rápidas

**Desvantagens**:
- ❌ Escrita mais lenta (2 operações)
- ❌ Desperdício se dado não for lido novamente

---

### **4. Write-Behind (Write-Back)**

**Padrão**: Cache é atualizado, banco é atualizado **assincronamente** depois

```
┌──────────────────────────────────────────┐
│ 1. Aplicação escreve no cache            │
│ 2. Retorna sucesso IMEDIATAMENTE ✅      │
│ 3. Background job atualiza banco         │
└──────────────────────────────────────────┘
```

**Código**:
```csharp
public async Task UpdateUserAsync(User user)
{
    // 1. Atualizar cache IMEDIATAMENTE
    var cacheKey = $"user_{user.Id}";
    _cache.Set(cacheKey, user, TimeSpan.FromHours(1));
    
    // 2. Enfileirar job para atualizar banco (async)
    await _queue.EnqueueAsync(new UpdateUserJob(user));
    
    // 3. Retornar IMEDIATAMENTE (não espera banco)
}
```

**Vantagens**:
- ✅ Escrita MUITO rápida
- ✅ Reduz carga no banco

**Desvantagens**:
- ❌ Risco de perda de dados (se cache falhar)
- ❌ Complexidade alta
- ❌ Eventual consistency

---

### **5. Refresh-Ahead**

**Padrão**: Cache é **renovado automaticamente** antes de expirar

```
┌──────────────────────────────────────────┐
│ Cache TTL = 10 min                        │
│ Refresh threshold = 8 min                 │
│                                           │
│ T=0:  Cache miss → busca banco           │
│ T=8:  Background job renova cache        │
│ T=10: Cache ainda válido! (renovado em 8)│
└──────────────────────────────────────────┘
```

**Código**:
```csharp
public async Task<T> GetAsync<T>(string key, Func<Task<T>> fetchFunc)
{
    var entry = _cache.Get<CacheEntry<T>>(key);
    
    if (entry != null)
    {
        // ✅ Cache HIT
        
        // Se está perto de expirar, renova em background
        if (entry.ExpiresAt - DateTime.UtcNow < TimeSpan.FromMinutes(2))
        {
            _ = Task.Run(async () =>
            {
                var freshValue = await fetchFunc();
                _cache.Set(key, new CacheEntry<T>
                {
                    Value = freshValue,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(10)
                });
            });
        }
        
        return entry.Value;
    }
    
    // ❌ Cache MISS
    var value = await fetchFunc();
    _cache.Set(key, new CacheEntry<T>
    {
        Value = value,
        ExpiresAt = DateTime.UtcNow.AddMinutes(10)
    });
    
    return value;
}
```

**Vantagens**:
- ✅ Nunca tem cache miss (após primeiro load)
- ✅ Sempre dados "frescos"

**Desvantagens**:
- ❌ Pode renovar dados não usados (desperdício)
- ❌ Complexidade

---

## 💻 Cache no .NET 8

### **1. In-Memory Cache (IMemoryCache)**

**Setup**:
```csharp
// Program.cs
builder.Services.AddMemoryCache();

// Controller/Service
public class MyService
{
    private readonly IMemoryCache _cache;
    
    public MyService(IMemoryCache cache)
    {
        _cache = cache;
    }
}
```

**Uso Básico**:
```csharp
// Set
_cache.Set("myKey", myValue, TimeSpan.FromMinutes(5));

// Get
if (_cache.TryGetValue("myKey", out MyType value))
{
    // Cache hit
}

// GetOrCreate (helper)
var value = _cache.GetOrCreate("myKey", entry =>
{
    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
    return GetValueFromDatabase();
});
```

**Opções Avançadas**:
```csharp
var options = new MemoryCacheEntryOptions
{
    // Expira após 10 minutos (absoluto)
    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
    
    // Expira após 5 minutos sem uso (sliding)
    SlidingExpiration = TimeSpan.FromMinutes(5),
    
    // Prioridade (Low/Normal/High/NeverRemove)
    Priority = CacheItemPriority.High,
    
    // Tamanho (para limite de memória)
    Size = 1
};

// Callback quando expirar
options.RegisterPostEvictionCallback((key, value, reason, state) =>
{
    Console.WriteLine($"Cache evicted: {key}, Reason: {reason}");
});

_cache.Set("myKey", myValue, options);
```

**Absolute vs Sliding Expiration**:
```
┌─────────────────────────────────────────────────────┐
│ ABSOLUTE EXPIRATION                                 │
├─────────────────────────────────────────────────────┤
│ T=0:  Set(key, value, 10 min)                       │
│ T=5:  Get(key) → HIT ✅                             │
│ T=10: Get(key) → MISS ❌ (expirou independente uso)│
└─────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────┐
│ SLIDING EXPIRATION                                  │
├─────────────────────────────────────────────────────┤
│ T=0:  Set(key, value, sliding=5min)                 │
│ T=3:  Get(key) → HIT ✅ (renova para T=8)          │
│ T=6:  Get(key) → HIT ✅ (renova para T=11)         │
│ T=13: Get(key) → MISS ❌ (5min sem uso)            │
└─────────────────────────────────────────────────────┘
```

---

### **2. Distributed Cache (IDistributedCache)**

**Setup com Redis**:
```csharp
// Program.cs
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
    options.InstanceName = "MyApp_";
});
```

**Uso**:
```csharp
public class MyService
{
    private readonly IDistributedCache _cache;
    
    public async Task<User> GetUserAsync(int id)
    {
        var key = $"user_{id}";
        
        // Get (retorna byte[])
        var cachedBytes = await _cache.GetAsync(key);
        
        if (cachedBytes != null)
        {
            // Deserializar
            var json = Encoding.UTF8.GetString(cachedBytes);
            return JsonSerializer.Deserialize<User>(json);
        }
        
        // Cache miss
        var user = await _db.Users.FindAsync(id);
        
        // Set
        var json = JsonSerializer.Serialize(user);
        var bytes = Encoding.UTF8.GetBytes(json);
        
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
            SlidingExpiration = TimeSpan.FromMinutes(5)
        };
        
        await _cache.SetAsync(key, bytes, options);
        
        return user;
    }
}
```

**Helper para JSON**:
```csharp
public static class DistributedCacheExtensions
{
    public static async Task<T?> GetJsonAsync<T>(
        this IDistributedCache cache, 
        string key)
    {
        var bytes = await cache.GetAsync(key);
        
        if (bytes == null)
            return default;
        
        var json = Encoding.UTF8.GetString(bytes);
        return JsonSerializer.Deserialize<T>(json);
    }
    
    public static async Task SetJsonAsync<T>(
        this IDistributedCache cache,
        string key,
        T value,
        DistributedCacheEntryOptions options)
    {
        var json = JsonSerializer.Serialize(value);
        var bytes = Encoding.UTF8.GetBytes(json);
        await cache.SetAsync(key, bytes, options);
    }
}

// Uso simplificado:
var user = await _cache.GetJsonAsync<User>($"user_{id}");

if (user == null)
{
    user = await _db.Users.FindAsync(id);
    await _cache.SetJsonAsync($"user_{id}", user, options);
}
```

---

### **3. Output Cache (.NET 7+)**

**Setup**:
```csharp
// Program.cs
builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(builder => 
        builder.Expire(TimeSpan.FromSeconds(60)));
        
    options.AddPolicy("5min", builder => 
        builder.Expire(TimeSpan.FromMinutes(5)));
});

app.UseOutputCache();
```

**Uso**:
```csharp
[OutputCache(Duration = 60)]
[HttpGet]
public async Task<IActionResult> Get()
{
    return Ok(await _service.GetDataAsync());
}

// Com política nomeada
[OutputCache(PolicyName = "5min")]
[HttpGet("{id}")]
public async Task<IActionResult> GetById(int id)
{
    return Ok(await _service.GetByIdAsync(id));
}
```

**Vantagem**: Cacheia a **resposta HTTP inteira** (mais rápido que cache de objetos)

---

## 🎯 Cache no Módulo Audit

### **Análise: Cachear ou Não?**

#### **❌ NÃO cachear o GetPagedAsync atual**

**Motivos**:
1. **Dados mudam frequentemente** - Novos logs sendo criados constantemente
2. **Query já é otimizada** - Com índices, responde em ~200ms
3. **Filtros dinâmicos** - Cada combinação de filtros = cache key diferente
4. **Pagination** - Cache de páginas = desperdício de memória
5. **Requisitos de auditoria** - Logs devem ser sempre atuais

**Exemplo de cache ineficiente**:
```csharp
// ❌ MAU EXEMPLO
var cacheKey = $"audit_{startDate}_{endDate}_{correlationId}_{operation}_{pageNumber}_{pageSize}";

// Problemas:
// - Infinitas combinações de filtros
// - Cache nunca reutilizado
// - Memória desperdiçada
// - Dados desatualizados
```

---

#### **✅ Cachear: Contagem Total (COUNT)**

**Motivo**: COUNT é caro, mas muda pouco entre páginas

```csharp
public async Task<(IEnumerable<AuditLog> logs, int totalCount)> GetPagedAsync(
    DateTime? startDate = null,
    DateTime? endDate = null,
    string? correlationId = null,
    string? operation = null,
    int pageNumber = 1,
    int pageSize = 50,
    CancellationToken cancellationToken = default)
{
    var query = _context.AuditLogs.AsNoTracking();

    // Aplicar filtros
    // ...

    // ✅ CACHE: Count com key baseada nos filtros
    var countCacheKey = $"audit_count_{startDate?.Ticks}_{endDate?.Ticks}_{correlationId}_{operation}";
    
    int totalCount;
    if (_cache.TryGetValue(countCacheKey, out int cachedCount))
    {
        totalCount = cachedCount;
    }
    else
    {
        totalCount = await query.CountAsync(cancellationToken);
        
        // Cache por 30 segundos (curto!)
        _cache.Set(countCacheKey, totalCount, TimeSpan.FromSeconds(30));
    }

    // Buscar dados (SEM cache)
    var logs = await query
        .OrderByDescending(a => a.Data)
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync(cancellationToken);

    return (logs, totalCount);
}
```

**Benefício**:
- **Primeira página**: COUNT + SELECT = 300ms
- **Páginas seguintes (30seg)**: SELECT apenas = 50ms ✅
- **Cache expira rápido**: Dados não ficam muito desatualizados

---

#### **✅ Cachear: Lookup Tables (Operation Names, Codes)**

Se você tem queries que buscam **lista de operações** ou **códigos**:

```csharp
public async Task<IEnumerable<string>> GetAllOperationsAsync()
{
    const string cacheKey = "audit_operations_list";
    
    if (_cache.TryGetValue(cacheKey, out List<string> operations))
    {
        return operations;
    }
    
    // Buscar no banco
    operations = await _context.AuditLogs
        .Select(a => a.Operation)
        .Distinct()
        .ToListAsync();
    
    // Cache por 10 minutos (dados mudam pouco)
    _cache.Set(cacheKey, operations, TimeSpan.FromMinutes(10));
    
    return operations;
}
```

---

#### **✅ Cachear: GetByStamp (raramente muda)**

Um log específico **nunca muda** (append-only):

```csharp
public async Task<AuditLog?> GetByStampAsync(string stamp, CancellationToken cancellationToken = default)
{
    var cacheKey = $"audit_log_{stamp}";
    
    if (_cache.TryGetValue(cacheKey, out AuditLog cachedLog))
    {
        return cachedLog;
    }
    
    var log = await _context.AuditLogs
        .AsNoTracking()
        .FirstOrDefaultAsync(a => a.ULogsstamp == stamp, cancellationToken);
    
    if (log != null)
    {
        // Cache por 1 hora (log nunca muda!)
        _cache.Set(cacheKey, log, TimeSpan.FromHours(1));
    }
    
    return log;
}
```

---

## 🔴 Redis vs In-Memory

### **Quando usar In-Memory (IMemoryCache)**

✅ **Use quando**:
- App em **servidor único**
- Dados **pequenos** (< 100MB total)
- Dados **não críticos** (ok perder ao reiniciar)
- **Performance crítica** (nanosegundos)

❌ **NÃO use quando**:
- App em **múltiplos servidores** (cada um terá cache diferente!)
- Dados **grandes** (vai consumir RAM do app)
- Precisa **persistir** dados

---

### **Quando usar Redis (IDistributedCache)**

✅ **Use quando**:
- App em **múltiplos servidores** / Load Balancer
- Dados **compartilhados** entre instâncias
- Precisa **persistir** cache (Redis Persistence)
- Cache **grande** (GBs)
- Features extras: Pub/Sub, Sorted Sets, etc

❌ **NÃO use quando**:
- **Latência crítica** (Redis adiciona ~1-5ms)
- Dados **muito temporários** (< 1 segundo)
- **Overhead** não justifica

---

### **Comparação de Performance**

| Operação | In-Memory | Redis (localhost) | Redis (network) |
|----------|-----------|-------------------|-----------------|
| **Set** | 10 ns | 500 µs | 1-5 ms |
| **Get** | 10 ns | 500 µs | 1-5 ms |
| **Serialization** | N/A | + 100 µs | + 100 µs |
| **Network** | N/A | Local | RTT latency |

**Conclusão**: In-Memory é **1000x mais rápido**, mas Redis é **distribuído**!

---

## ⚠️ Cache Invalidation

> "There are only two hard things in Computer Science: cache invalidation and naming things."
> — Phil Karlton

### **Problema**

```
T=0:  Cache armazena User{name="João"}
T=5:  Banco atualiza para User{name="Maria"}
T=10: Cache ainda retorna "João" ❌ (stale data!)
```

### **Estratégias de Invalidação**

#### **1. Time-Based (TTL - Time To Live)**

**Mais simples**: Cache expira após X tempo

```csharp
_cache.Set("user_123", user, TimeSpan.FromMinutes(5));
```

**Vantagens**:
- ✅ Simples
- ✅ Previsível

**Desvantagens**:
- ❌ Dados podem ficar desatualizados até expirar
- ❌ Trade-off: TTL curto = cache inútil, TTL longo = dados velhos

---

#### **2. Explicit Invalidation (Manual)**

**Padrão**: Ao atualizar dado, **invalidar cache manualmente**

```csharp
public async Task UpdateUserAsync(User user)
{
    // 1. Atualizar banco
    await _db.Users.UpdateAsync(user);
    
    // 2. Invalidar cache EXPLICITAMENTE
    _cache.Remove($"user_{user.Id}");
}
```

**Vantagens**:
- ✅ Cache sempre atualizado
- ✅ Controle total

**Desvantagens**:
- ❌ Fácil esquecer de invalidar
- ❌ Código espalhado (não DRY)

---

#### **3. Event-Based Invalidation**

**Padrão**: Eventos de domínio disparam invalidação

```csharp
public class UserUpdatedEventHandler : INotificationHandler<UserUpdatedEvent>
{
    private readonly IMemoryCache _cache;
    
    public async Task Handle(UserUpdatedEvent notification, CancellationToken ct)
    {
        // Invalidar cache automaticamente
        _cache.Remove($"user_{notification.UserId}");
    }
}

// Ao atualizar:
await _mediator.Publish(new UserUpdatedEvent(user.Id));
```

**Vantagens**:
- ✅ Desacoplado
- ✅ Centralizado

**Desvantagens**:
- ❌ Mais complexo
- ❌ Precisa de event bus

---

#### **4. Cache Tagging (Redis)**

**Padrão**: Tags agrupam chaves relacionadas

```csharp
// Ao criar cache, adicionar tags
await _redis.SetJsonAsync("user_123", user);
await _redis.SetAddAsync("tag:users", "user_123");
await _redis.SetAddAsync("tag:org_456", "user_123");

// Ao invalidar, remover por tag
var keys = await _redis.SetMembersAsync("tag:org_456");
foreach (var key in keys)
{
    await _redis.KeyDeleteAsync(key);
}
```

**Vantagens**:
- ✅ Invalida múltiplas chaves de uma vez
- ✅ Organizado

**Desvantagens**:
- ❌ Redis-specific
- ❌ Overhead extra

---

#### **5. Write-Through/Write-Behind**

**Padrão**: Cache é atualizado junto com banco

```csharp
public async Task UpdateUserAsync(User user)
{
    // 1. Atualizar banco
    await _db.Users.UpdateAsync(user);
    
    // 2. Atualizar cache IMEDIATAMENTE (write-through)
    _cache.Set($"user_{user.Id}", user, TimeSpan.FromMinutes(10));
}
```

**Vantagens**:
- ✅ Cache sempre atualizado
- ✅ Leituras sempre rápidas

**Desvantagens**:
- ❌ Escrita mais lenta
- ❌ Pode cachear dados que não serão lidos

---

## 📈 Performance vs Complexidade

### **Trade-off Matrix**

```
┌────────────────────────────────────────────────┐
│ COMPLEXIDADE                                   │
│     ▲                                          │
│  H  │    Redis        Event-Based              │
│  I  │    Distributed  Invalidation             │
│  G  │                                           │
│  H  │    ─────────────────────────────         │
│     │                                           │
│  M  │    In-Memory    Write-Through            │
│  E  │    Cache        Manual Invalidation      │
│  D  │                                           │
│     │    ─────────────────────────────         │
│  L  │    TTL Only     No Cache                 │
│  O  │                                           │
│  W  │                                           │
│     └──────────────────────────────────────►   │
│          LOW          MED           HIGH        │
│                   PERFORMANCE                   │
└────────────────────────────────────────────────┘
```

### **Recomendação por Cenário**

| Cenário | Solução Recomendada | Complexidade | Performance |
|---------|---------------------|--------------|-------------|
| **Read-heavy, dados estáveis** | In-Memory + TTL longo | Baixa | Alta |
| **Read-heavy, dados dinâmicos** | In-Memory + TTL curto | Baixa | Média |
| **Multi-server** | Redis + TTL | Média | Alta |
| **Crítico (sempre atualizado)** | No cache / Redis + Events | Alta | Média |
| **Lookup tables** | In-Memory + Long TTL | Baixa | Alta |

---

## 🛠️ Implementações Práticas

### **1. Generic Cache Service**

```csharp
public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
    Task RemoveAsync(string key);
    Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null);
}

public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<MemoryCacheService> _logger;
    
    public MemoryCacheService(
        IMemoryCache cache,
        ILogger<MemoryCacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }
    
    public Task<T?> GetAsync<T>(string key)
    {
        _cache.TryGetValue(key, out T value);
        
        if (value != null)
            _logger.LogDebug("Cache HIT: {Key}", key);
        else
            _logger.LogDebug("Cache MISS: {Key}", key);
        
        return Task.FromResult(value);
    }
    
    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(5)
        };
        
        _cache.Set(key, value, options);
        _logger.LogDebug("Cache SET: {Key}, Expiration: {Expiration}", key, expiration);
        
        return Task.CompletedTask;
    }
    
    public Task RemoveAsync(string key)
    {
        _cache.Remove(key);
        _logger.LogDebug("Cache REMOVE: {Key}", key);
        return Task.CompletedTask;
    }
    
    public async Task<T> GetOrCreateAsync<T>(
        string key,
        Func<Task<T>> factory,
        TimeSpan? expiration = null)
    {
        if (_cache.TryGetValue(key, out T cachedValue))
        {
            _logger.LogDebug("Cache HIT: {Key}", key);
            return cachedValue;
        }
        
        _logger.LogDebug("Cache MISS: {Key}, fetching from source", key);
        
        var value = await factory();
        
        await SetAsync(key, value, expiration);
        
        return value;
    }
}
```

**Uso**:
```csharp
public class AuditService
{
    private readonly ICacheService _cache;
    private readonly IAuditLogRepository _repository;
    
    public async Task<AuditLog?> GetByStampAsync(string stamp)
    {
        return await _cache.GetOrCreateAsync(
            key: $"audit_{stamp}",
            factory: () => _repository.GetByStampAsync(stamp),
            expiration: TimeSpan.FromMinutes(10)
        );
    }
}
```

---

### **2. Decorator Pattern para Cache**

```csharp
public class CachedAuditLogRepository : IAuditLogRepository
{
    private readonly IAuditLogRepository _innerRepository;
    private readonly ICacheService _cache;
    
    public CachedAuditLogRepository(
        IAuditLogRepository innerRepository,
        ICacheService cache)
    {
        _innerRepository = innerRepository;
        _cache = cache;
    }
    
    public async Task<AuditLog?> GetByStampAsync(string stamp, CancellationToken ct = default)
    {
        var cacheKey = $"audit_log_{stamp}";
        
        return await _cache.GetOrCreateAsync(
            cacheKey,
            () => _innerRepository.GetByStampAsync(stamp, ct),
            TimeSpan.FromMinutes(10)
        );
    }
    
    // Outros métodos delegam para _innerRepository
    public Task<(IEnumerable<AuditLog> logs, int totalCount)> GetPagedAsync(...)
        => _innerRepository.GetPagedAsync(...);
}

// DI Setup
services.AddScoped<IAuditLogRepository, AuditLogRepositoryEFCore>();
services.Decorate<IAuditLogRepository, CachedAuditLogRepository>();
```

**Vantagem**: Cache totalmente desacoplado!

---

### **3. Cache para Paginação (COUNT otimizado)**

```csharp
public class AuditLogRepositoryEFCore : IAuditLogRepository
{
    private readonly AuditDbContextEFCore _context;
    private readonly IMemoryCache _cache;
    
    public async Task<(IEnumerable<AuditLog> logs, int totalCount)> GetPagedAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? correlationId = null,
        string? operation = null,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var query = _context.AuditLogs.AsNoTracking();

        // Aplicar filtros
        if (startDate.HasValue)
            query = query.Where(a => a.Data >= startDate.Value);
        
        if (endDate.HasValue)
            query = query.Where(a => a.Data <= endDate.Value);
        
        if (!string.IsNullOrWhiteSpace(correlationId))
            query = query.Where(a => a.RequestId == correlationId);
        
        if (!string.IsNullOrWhiteSpace(operation))
            query = query.Where(a => a.Operation != null && a.Operation.Contains(operation));

        // ✅ CACHE: COUNT com TTL curto
        var countCacheKey = BuildCountCacheKey(startDate, endDate, correlationId, operation);
        
        int totalCount;
        if (!_cache.TryGetValue(countCacheKey, out totalCount))
        {
            totalCount = await query.CountAsync(cancellationToken);
            _cache.Set(countCacheKey, totalCount, TimeSpan.FromSeconds(30));
        }

        // Buscar logs (SEM cache - dados mudam muito)
        var logs = await query
            .OrderByDescending(a => a.Data)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (logs, totalCount);
    }
    
    private static string BuildCountCacheKey(
        DateTime? startDate,
        DateTime? endDate,
        string? correlationId,
        string? operation)
    {
        return $"audit_count_{startDate?.Ticks ?? 0}_{endDate?.Ticks ?? 0}_{correlationId ?? ""}_{operation ?? ""}";
    }
}
```

---

## 🚫 Antipatterns

### **1. Cachear TUDO**

```csharp
// ❌ MAU: Cachear dados que mudam frequentemente
_cache.Set("current_stock_price", price, TimeSpan.FromHours(1));

// ✅ BOM: Apenas dados estáveis
_cache.Set("product_description", description, TimeSpan.FromHours(1));
```

---

### **2. Cache sem TTL**

```csharp
// ❌ MAU: Cache infinito
_cache.Set("user_123", user); // Sem expiration!

// ✅ BOM: Sempre definir TTL
_cache.Set("user_123", user, TimeSpan.FromMinutes(10));
```

---

### **3. Cache de Objetos Grandes**

```csharp
// ❌ MAU: Cachear listas gigantes
_cache.Set("all_users", await _db.Users.ToListAsync()); // 10.000 usuários!

// ✅ BOM: Cachear por ID
foreach (var user in users)
{
    _cache.Set($"user_{user.Id}", user, TimeSpan.FromMinutes(10));
}
```

---

### **4. Não Medir Performance**

```csharp
// ❌ MAU: Adicionar cache sem medir
// "Achismo" que cache vai melhorar

// ✅ BOM: MEDIR antes e depois
var sw = Stopwatch.StartNew();
var data = await GetDataAsync();
sw.Stop();
_logger.LogInformation("Query took {ElapsedMs}ms", sw.ElapsedMilliseconds);
```

---

### **5. Cache Keys Duplicados**

```csharp
// ❌ MAU: Key ambígua
_cache.Set("user", user1); // Qual user?

// ✅ BOM: Key única e descritiva
_cache.Set($"user_{user1.Id}", user1);
```

---

## 📊 Métricas e Monitoramento

### **Cache Hit Rate**

```csharp
public class CacheMetrics
{
    private long _hits;
    private long _misses;
    
    public void RecordHit() => Interlocked.Increment(ref _hits);
    public void RecordMiss() => Interlocked.Increment(ref _misses);
    
    public double HitRate
    {
        get
        {
            var total = _hits + _misses;
            return total == 0 ? 0 : (double)_hits / total * 100;
        }
    }
}

// Uso:
if (_cache.TryGetValue(key, out var value))
{
    _metrics.RecordHit();
    return value;
}
else
{
    _metrics.RecordMiss();
    // Buscar da fonte
}

// Expor métricas:
[HttpGet("cache-stats")]
public IActionResult GetCacheStats()
{
    return Ok(new
    {
        HitRate = _metrics.HitRate,
        Hits = _metrics.Hits,
        Misses = _metrics.Misses
    });
}
```

**Meta Ideal**: Hit Rate > 80%

---

## ✅ Resumo Final

### **Quando Cachear**

✅ **SIM**:
- Dados **lidos frequentemente**
- Dados **caros** de calcular/buscar
- Dados **estáveis** (mudam pouco)
- **Lookup tables** (lista de países, categorias)
- **Sessões de usuário**

❌ **NÃO**:
- Dados **mudam constantemente**
- Dados **críticos** (precisão absoluta)
- Dados **grandes** (> 1MB)
- **Write-heavy** workloads
- Dados **privados/sensíveis** sem criptografia

---

### **Escolha de Tecnologia**

| Cenário | Escolha |
|---------|---------|
| App único, dados pequenos | `IMemoryCache` |
| Multi-server, dados compartilhados | Redis (`IDistributedCache`) |
| Response HTTP inteira | Output Cache |
| Session state | Redis Session Store |
| Pub/Sub, features avançadas | Redis direto (StackExchange.Redis) |

---

### **Estratégia Recomendada**

1. **Medir primeiro** - Identifique gargalos reais
2. **Começar simples** - In-Memory + TTL
3. **Monitorar Hit Rate** - Ajustar TTLs
4. **Evoluir gradualmente** - Redis se necessário
5. **Documentar** - Cache keys e TTLs

---

**"Premature optimization is the root of all evil" - Donald Knuth**

**Mas**: Caching bem feito é 🚀 **transformador** para performance!

---

**Data**: 10 de fevereiro de 2024  
**Autor**: Engenheiro Sênior - Análise Técnica Completa
