# 📊 Paginação - Resumo Visual Rápido

## 🎯 Fórmula

```csharp
.Skip((pageNumber - 1) * pageSize)
.Take(pageSize)
```

## 📐 Exemplo: 100 logs, pageSize = 10

```
╔═══════════════════════════════════════════════════════════════╗
║  PÁGINA 1 (pageNumber = 1)                                    ║
╠═══════════════════════════════════════════════════════════════╣
║  Skip((1-1) × 10) = Skip(0)  ← Não pula nada                 ║
║  Take(10)                    ← Pega os primeiros 10          ║
║                                                               ║
║  [████████████████████████] Logs 1-10                         ║
║   └─ Retorna estes                                            ║
╚═══════════════════════════════════════════════════════════════╝

╔═══════════════════════════════════════════════════════════════╗
║  PÁGINA 2 (pageNumber = 2)                                    ║
╠═══════════════════════════════════════════════════════════════╣
║  Skip((2-1) × 10) = Skip(10) ← Pula os primeiros 10          ║
║  Take(10)                    ← Pega os próximos 10           ║
║                                                               ║
║  [----------][████████████████████████] Logs 11-20            ║
║   └─ Pula    └─ Retorna estes                                ║
╚═══════════════════════════════════════════════════════════════╝

╔═══════════════════════════════════════════════════════════════╗
║  PÁGINA 3 (pageNumber = 3)                                    ║
╠═══════════════════════════════════════════════════════════════╣
║  Skip((3-1) × 10) = Skip(20) ← Pula os primeiros 20          ║
║  Take(10)                    ← Pega os próximos 10           ║
║                                                               ║
║  [----------][----------][████████████████████████]           ║
║   └─ Pula    └─ Pula    └─ Retorna Logs 21-30                ║
╚═══════════════════════════════════════════════════════════════╝
```

## 🔢 Tabela Rápida

| Página | Skip Fórmula     | Skip | Take | Logs      |
|--------|------------------|------|------|-----------|
| 1      | (1-1) × 10 = 0   | 0    | 10   | 1-10      |
| 2      | (2-1) × 10 = 10  | 10   | 10   | 11-20     |
| 3      | (3-1) × 10 = 20  | 20   | 10   | 21-30     |
| 4      | (4-1) × 10 = 30  | 30   | 10   | 31-40     |
| 5      | (5-1) × 10 = 40  | 40   | 10   | 41-50     |

## 🎯 SQL Gerado

```sql
-- PÁGINA 1
SELECT * FROM ulogs 
ORDER BY data DESC 
OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY

-- PÁGINA 2
SELECT * FROM ulogs 
ORDER BY data DESC 
OFFSET 10 ROWS FETCH NEXT 10 ROWS ONLY

-- PÁGINA 3
SELECT * FROM ulogs 
ORDER BY data DESC 
OFFSET 20 ROWS FETCH NEXT 10 ROWS ONLY
```

## ⚡ Performance

```
❌ ANTES (Memória):
┌───────────────────┐
│ Banco: 10.000 logs│
└───────────────────┘
         │
         │ SELECT * FROM ulogs (TUDO!)
         ▼
┌───────────────────┐
│ RAM: 10.000 logs  │ ← 10 MB
│ Skip/Take aqui    │
└───────────────────┘
         │
         ▼
┌───────────────────┐
│ Cliente: 10 logs  │
└───────────────────┘

✅ DEPOIS (Banco):
┌───────────────────┐
│ Banco: 10.000 logs│
│ Skip/Take aqui ✅ │
└───────────────────┘
         │
         │ SELECT * ... OFFSET 20 ROWS FETCH NEXT 10 ROWS ONLY
         ▼
┌───────────────────┐
│ RAM: 10 logs      │ ← 10 KB
└───────────────────┘
         │
         ▼
┌───────────────────┐
│ Cliente: 10 logs  │
└───────────────────┘
```

## 💡 Por que `(pageNumber - 1)`?

```
SE usássemos pageNumber × pageSize:

Página 1: 1 × 10 = 10 → Skip(10) → Logs 11-20 ❌ (Pulou a primeira página!)
Página 2: 2 × 10 = 20 → Skip(20) → Logs 21-30 ❌ (Pulou duas páginas!)

COM (pageNumber - 1) × pageSize:

Página 1: (1-1) × 10 = 0  → Skip(0)  → Logs 1-10  ✅
Página 2: (2-1) × 10 = 10 → Skip(10) → Logs 11-20 ✅
```

## 📱 Response Completo

```json
{
  "data": {
    "items": [ /* 10 logs */ ],
    "pageNumber": 2,        ← Página atual
    "pageSize": 10,         ← Tamanho da página
    "totalCount": 100,      ← Total de logs (com filtros)
    "totalPages": 10,       ← Total de páginas (100 / 10)
    "hasNextPage": true,    ← Pode avançar? (2 < 10)
    "hasPreviousPage": true ← Pode voltar? (2 > 1)
  }
}
```

## ✅ Implementação no Código

```csharp
// Repository
public async Task<(IEnumerable<AuditLog> logs, int totalCount)> GetPagedAsync(
    int pageNumber = 1,
    int pageSize = 50,
    CancellationToken cancellationToken = default)
{
    var query = _context.AuditLogs.AsNoTracking();
    
    // 1. Contar total
    var totalCount = await query.CountAsync(cancellationToken);
    
    // 2. Aplicar paginação
    var logs = await query
        .OrderByDescending(a => a.Data)
        .Skip((pageNumber - 1) * pageSize)  // ← AQUI!
        .Take(pageSize)                      // ← E AQUI!
        .ToListAsync(cancellationToken);
    
    return (logs, totalCount);
}
```

---

**Veja `docs\PAGINATION-EXPLAINED.md` para explicação completa!** 📚
