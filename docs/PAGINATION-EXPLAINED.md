# 📚 Como Funciona a Paginação - Explicação Completa

## 🎯 Conceito de Paginação

Imagine que você tem **100 logs** no banco de dados, mas quer mostrar apenas **10 por página** no frontend.

### **Problema**: 
- ❌ Carregar 100 logs de uma vez → Lento, desperdício de memória
- ✅ Carregar 10 logs por vez → Rápido, eficiente

## 🔢 A Fórmula Mágica

```csharp
.Skip((pageNumber - 1) * pageSize)
.Take(pageSize)
```

### **Vamos Decompor:**

#### **1. `pageNumber`** (Número da Página)
- Página que o usuário quer ver
- Começa em **1** (não em 0!)
- Exemplo: Página 1, Página 2, Página 3...

#### **2. `pageSize`** (Tamanho da Página)
- Quantos itens por página
- Exemplo: 10, 25, 50, 100

#### **3. `Skip(n)`** (Pular n registros)
- Pula os primeiros `n` registros
- Usado para "avançar" até a página desejada

#### **4. `Take(n)`** (Pegar n registros)
- Pega os próximos `n` registros
- Usado para limitar a quantidade de resultados

## 📊 Exemplo Prático

### **Cenário: 100 logs no banco, pageSize = 10**

```
┌─────────────────────────────────────────────────────────────────┐
│  BANCO DE DADOS (100 logs, ordenados por data descendente)     │
├─────────────────────────────────────────────────────────────────┤
│  Log #1  - 2024-02-10 15:30:00                                 │
│  Log #2  - 2024-02-10 15:29:00                                 │
│  Log #3  - 2024-02-10 15:28:00                                 │
│  Log #4  - 2024-02-10 15:27:00                                 │
│  Log #5  - 2024-02-10 15:26:00                                 │
│  ...                                                            │
│  Log #97 - 2024-02-09 10:05:00                                 │
│  Log #98 - 2024-02-09 10:04:00                                 │
│  Log #99 - 2024-02-09 10:03:00                                 │
│  Log #100 - 2024-02-09 10:02:00                                │
└─────────────────────────────────────────────────────────────────┘
```

### **Página 1** (pageNumber = 1, pageSize = 10)

```csharp
Skip((1 - 1) * 10) = Skip(0)    // ← Não pula nada
Take(10)                         // ← Pega os primeiros 10

SQL Equivalente:
SELECT * FROM ulogs 
ORDER BY data DESC 
OFFSET 0 ROWS 
FETCH NEXT 10 ROWS ONLY
```

**Resultado:**
```
┌─────────────────────────────────┐
│  Página 1 (Logs 1-10)           │
├─────────────────────────────────┤
│  Log #1  - 2024-02-10 15:30:00  │
│  Log #2  - 2024-02-10 15:29:00  │
│  Log #3  - 2024-02-10 15:28:00  │
│  Log #4  - 2024-02-10 15:27:00  │
│  Log #5  - 2024-02-10 15:26:00  │
│  Log #6  - 2024-02-10 15:25:00  │
│  Log #7  - 2024-02-10 15:24:00  │
│  Log #8  - 2024-02-10 15:23:00  │
│  Log #9  - 2024-02-10 15:22:00  │
│  Log #10 - 2024-02-10 15:21:00  │
└─────────────────────────────────┘
```

---

### **Página 2** (pageNumber = 2, pageSize = 10)

```csharp
Skip((2 - 1) * 10) = Skip(10)   // ← Pula os primeiros 10
Take(10)                         // ← Pega os próximos 10

SQL Equivalente:
SELECT * FROM ulogs 
ORDER BY data DESC 
OFFSET 10 ROWS 
FETCH NEXT 10 ROWS ONLY
```

**Resultado:**
```
┌─────────────────────────────────┐
│  Página 2 (Logs 11-20)          │
├─────────────────────────────────┤
│  Log #11 - 2024-02-10 15:20:00  │
│  Log #12 - 2024-02-10 15:19:00  │
│  Log #13 - 2024-02-10 15:18:00  │
│  ...                             │
│  Log #20 - 2024-02-10 15:11:00  │
└─────────────────────────────────┘
```

---

### **Página 3** (pageNumber = 3, pageSize = 10)

```csharp
Skip((3 - 1) * 10) = Skip(20)   // ← Pula os primeiros 20
Take(10)                         // ← Pega os próximos 10

SQL Equivalente:
SELECT * FROM ulogs 
ORDER BY data DESC 
OFFSET 20 ROWS 
FETCH NEXT 10 ROWS ONLY
```

**Resultado:**
```
┌─────────────────────────────────┐
│  Página 3 (Logs 21-30)          │
├─────────────────────────────────┤
│  Log #21 - 2024-02-10 15:10:00  │
│  Log #22 - 2024-02-10 15:09:00  │
│  ...                             │
│  Log #30 - 2024-02-10 15:01:00  │
└─────────────────────────────────┘
```

---

### **Página 10** (última página, pageNumber = 10, pageSize = 10)

```csharp
Skip((10 - 1) * 10) = Skip(90)  // ← Pula os primeiros 90
Take(10)                         // ← Pega os próximos 10

SQL Equivalente:
SELECT * FROM ulogs 
ORDER BY data DESC 
OFFSET 90 ROWS 
FETCH NEXT 10 ROWS ONLY
```

**Resultado:**
```
┌─────────────────────────────────┐
│  Página 10 (Logs 91-100)        │
├─────────────────────────────────┤
│  Log #91  - 2024-02-09 10:12:00 │
│  Log #92  - 2024-02-09 10:11:00 │
│  ...                             │
│  Log #100 - 2024-02-09 10:02:00 │
└─────────────────────────────────┘
```

## 🧮 Tabela de Cálculo

| Página | pageSize | Skip Cálculo         | Skip | Take | Logs Retornados |
|--------|----------|----------------------|------|------|-----------------|
| 1      | 10       | (1-1) × 10 = 0      | 0    | 10   | 1-10            |
| 2      | 10       | (2-1) × 10 = 10     | 10   | 10   | 11-20           |
| 3      | 10       | (3-1) × 10 = 20     | 20   | 10   | 21-30           |
| 4      | 10       | (4-1) × 10 = 30     | 30   | 10   | 31-40           |
| 5      | 10       | (5-1) × 10 = 40     | 40   | 10   | 41-50           |
| 10     | 10       | (10-1) × 10 = 90    | 90   | 10   | 91-100          |

### **Com pageSize = 25:**

| Página | pageSize | Skip Cálculo         | Skip | Take | Logs Retornados |
|--------|----------|----------------------|------|------|-----------------|
| 1      | 25       | (1-1) × 25 = 0      | 0    | 25   | 1-25            |
| 2      | 25       | (2-1) × 25 = 25     | 25   | 25   | 26-50           |
| 3      | 25       | (3-1) × 25 = 50     | 50   | 25   | 51-75           |
| 4      | 25       | (4-1) × 25 = 75     | 75   | 25   | 76-100          |

## 🔄 Fluxo Completo da Query

### **Request do Cliente:**
```http
GET /api/audit?pageNumber=3&pageSize=10
```

### **1. Handler recebe os parâmetros:**
```csharp
pageNumber = 3
pageSize = 10
```

### **2. Repository monta a query:**
```csharp
var query = _context.AuditLogs.AsNoTracking();

// Aplicar filtros (se houver)
// ...

// Paginação
var logs = await query
    .OrderByDescending(a => a.Data)
    .Skip((3 - 1) * 10)  // Skip(20)
    .Take(10)             // Take(10)
    .ToListAsync(cancellationToken);
```

### **3. EF Core gera SQL:**
```sql
SELECT [a].[ulogsstamp], 
       [a].[data], 
       [a].[code], 
       [a].[content],
       [a].[ip],
       [a].[operation],
       [a].[requestid],
       [a].[responsedesc],
       [a].[responsetext]
FROM [ulogs] AS [a]
ORDER BY [a].[data] DESC
OFFSET 20 ROWS        -- ← Skip(20)
FETCH NEXT 10 ROWS ONLY -- ← Take(10)
```

### **4. SQL Server executa:**
- Ordena por data descendente
- Pula os primeiros 20 registros
- Retorna os próximos 10 registros (logs 21-30)

### **5. EF Core mapeia para objetos:**
```csharp
List<AuditLog> (10 items)
```

### **6. Handler calcula metadata:**
```csharp
totalCount = 100              // ← COUNT(*) separado
totalPages = 100 / 10 = 10    // ← Ceil(totalCount / pageSize)
hasNextPage = 3 < 10 = true   // ← pageNumber < totalPages
hasPreviousPage = 3 > 1 = true // ← pageNumber > 1
```

### **7. Response ao cliente:**
```json
{
  "success": true,
  "data": {
    "items": [ /* 10 logs */ ],
    "pageNumber": 3,
    "pageSize": 10,
    "totalCount": 100,
    "totalPages": 10,
    "hasNextPage": true,
    "hasPreviousPage": true
  },
  "correlationId": "abc123"
}
```

## 💡 Por que `(pageNumber - 1)`?

### **Se usássemos apenas `pageNumber * pageSize`:**

| Página | Cálculo ERRADO       | Skip | Logs Retornados | ❌ Problema         |
|--------|----------------------|------|-----------------|---------------------|
| 1      | 1 × 10 = 10         | 10   | 11-20           | Pula os primeiros!  |
| 2      | 2 × 10 = 20         | 20   | 21-30           | Pula página 1 e 2!  |

### **Com `(pageNumber - 1) * pageSize` (✅ CORRETO):**

| Página | Cálculo CORRETO      | Skip | Logs Retornados | ✅ OK               |
|--------|----------------------|------|-----------------|---------------------|
| 1      | (1-1) × 10 = 0      | 0    | 1-10            | Primeira página OK  |
| 2      | (2-1) × 10 = 10     | 10   | 11-20           | Segunda página OK   |

## 🎨 Visualização Gráfica

### **100 logs divididos em páginas de 10:**

```
┌──────────┬──────────┬──────────┬──────────┬──────────┐
│ Página 1 │ Página 2 │ Página 3 │ Página 4 │ Página 5 │
├──────────┼──────────┼──────────┼──────────┼──────────┤
│ 1-10     │ 11-20    │ 21-30    │ 31-40    │ 41-50    │
└──────────┴──────────┴──────────┴──────────┴──────────┘

┌──────────┬──────────┬──────────┬──────────┬──────────┐
│ Página 6 │ Página 7 │ Página 8 │ Página 9 │ Página 10│
├──────────┼──────────┼──────────┼──────────┼──────────┤
│ 51-60    │ 61-70    │ 71-80    │ 81-90    │ 91-100   │
└──────────┴──────────┴──────────┴──────────┴──────────┘
```

### **Request: Página 3**
```
           Skip(20) →
┌──────────┬──────────┬──────────┬──────────┬──────────┐
│          │          │ ■■■■■■■■ │          │          │
│ 1-10     │ 11-20    │ 21-30    │ 31-40    │ 41-50    │
│ (pula)   │ (pula)   │ (RETORNA)│          │          │
└──────────┴──────────┴──────────┴──────────┴──────────┘
                       ↑
                   Take(10)
```

## 🚀 Performance: Banco vs Memória

### **❌ ANTES (Paginação em Memória):**

```csharp
// 1. Buscar TODOS os registros do banco
var logs = await _context.AuditLogs.ToListAsync(); // ← 10.000 logs!

// 2. Paginar em memória
var page = logs
    .Skip((pageNumber - 1) * pageSize)
    .Take(pageSize);
```

**SQL Gerado:**
```sql
SELECT * FROM ulogs  -- ← Busca TUDO!
```

**Problemas:**
- 🔥 Carrega 10.000 registros na RAM
- 🔥 Transfer de 10MB+ do banco para app
- 🔥 OutOfMemoryException com milhões de logs
- 🔥 Lento (segundos)

---

### **✅ DEPOIS (Paginação no Banco):**

```csharp
var logs = await _context.AuditLogs
    .OrderByDescending(a => a.Data)
    .Skip((pageNumber - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync();
```

**SQL Gerado:**
```sql
SELECT * FROM ulogs
ORDER BY data DESC
OFFSET 20 ROWS
FETCH NEXT 10 ROWS ONLY  -- ← SQL Server só retorna 10!
```

**Benefícios:**
- ✅ Carrega apenas 10 registros na RAM
- ✅ Transfer de ~10KB do banco para app
- ✅ Funciona com milhões de logs
- ✅ Rápido (milissegundos)

## 📊 Comparação de Performance

| Aspecto                | Paginação em Memória | Paginação no Banco |
|------------------------|----------------------|--------------------|
| **Logs no Banco**      | 10.000               | 10.000             |
| **Logs Carregados**    | 10.000 ❌            | 10 ✅              |
| **Memória Usada**      | ~10 MB ❌            | ~10 KB ✅          |
| **Tempo de Resposta**  | 2-5 segundos ❌      | 50-100 ms ✅       |
| **Network Transfer**   | 10 MB ❌             | 10 KB ✅           |
| **Escalável?**         | NÃO ❌               | SIM ✅             |

## 🎯 Metadata de Paginação

### **Cálculo do Total de Páginas:**

```csharp
totalCount = 100   // ← Total de logs no banco (COUNT(*))
pageSize = 10      // ← Logs por página

totalPages = Math.Ceiling(100 / 10.0) = 10
```

### **Exemplo com números quebrados:**

```csharp
totalCount = 95    // ← Total de logs
pageSize = 10      // ← Logs por página

totalPages = Math.Ceiling(95 / 10.0) = Math.Ceiling(9.5) = 10
```

**Páginas:**
- Páginas 1-9: 10 logs cada
- Página 10: 5 logs (última página)

### **Flags de Navegação:**

```csharp
// Tem próxima página?
hasNextPage = pageNumber < totalPages
// Exemplo: Página 3 de 10 → 3 < 10 = true ✅

// Tem página anterior?
hasPreviousPage = pageNumber > 1
// Exemplo: Página 3 → 3 > 1 = true ✅
```

## 🔍 Exemplo Real de Request/Response

### **Request:**
```http
GET /api/audit?pageNumber=2&pageSize=25&startDate=2024-02-01&operation=GetAll
```

### **Handler Processa:**
```csharp
// 1. Monta query com filtros
var query = _context.AuditLogs.AsNoTracking();

query = query.Where(a => a.Data >= DateTime.Parse("2024-02-01"));
query = query.Where(a => a.Operation != null && a.Operation.Contains("GetAll"));

// 2. Conta total (com filtros)
var totalCount = await query.CountAsync(); // ← Retorna 73

// 3. Aplica paginação
var logs = await query
    .OrderByDescending(a => a.Data)
    .Skip((2 - 1) * 25)  // Skip(25)
    .Take(25)             // Take(25)
    .ToListAsync();      // ← Retorna 25 logs (26-50)

// 4. Calcula metadata
totalPages = Math.Ceiling(73 / 25.0) = 3
hasNextPage = 2 < 3 = true
hasPreviousPage = 2 > 1 = true
```

### **SQL Gerado:**
```sql
-- Query 1: Count total (com filtros)
SELECT COUNT(*)
FROM ulogs
WHERE data >= '2024-02-01'
  AND operation LIKE '%GetAll%'
-- Resultado: 73

-- Query 2: Buscar página 2
SELECT *
FROM ulogs
WHERE data >= '2024-02-01'
  AND operation LIKE '%GetAll%'
ORDER BY data DESC
OFFSET 25 ROWS
FETCH NEXT 25 ROWS ONLY
-- Resultado: 25 logs (registros 26-50)
```

### **Response:**
```json
{
  "success": true,
  "data": {
    "items": [
      { "uLogsstamp": "...", "data": "2024-02-09T15:20:00", ... },
      { "uLogsstamp": "...", "data": "2024-02-09T15:19:00", ... },
      // ... 25 logs total
    ],
    "pageNumber": 2,
    "pageSize": 25,
    "totalCount": 73,
    "totalPages": 3,
    "hasNextPage": true,
    "hasPreviousPage": true
  },
  "content": "Retrieved 25 logs out of 73 (Page 2/3)",
  "correlationId": "abc123"
}
```

## 📱 Como o Frontend Usa

### **Botões de Navegação:**

```typescript
// Página Anterior
if (result.hasPreviousPage) {
  showButton("Previous", () => fetchPage(result.pageNumber - 1));
}

// Página Seguinte
if (result.hasNextPage) {
  showButton("Next", () => fetchPage(result.pageNumber + 1));
}

// Info
showText(`Page ${result.pageNumber} of ${result.totalPages}`);
showText(`Showing ${result.items.length} of ${result.totalCount} logs`);
```

### **UI Example:**
```
┌──────────────────────────────────────────────────────────┐
│  Audit Logs                                              │
├──────────────────────────────────────────────────────────┤
│  Showing 25 of 73 logs                                   │
│                                                          │
│  [ Log 26 - 2024-02-09 15:20:00 ]                       │
│  [ Log 27 - 2024-02-09 15:19:00 ]                       │
│  ...                                                     │
│  [ Log 50 - 2024-02-09 14:56:00 ]                       │
│                                                          │
│  [ ← Previous ]  Page 2 of 3  [ Next → ]                │
└──────────────────────────────────────────────────────────┘
```

## ✅ Resumo Final

### **Fórmula:**
```csharp
Skip((pageNumber - 1) * pageSize).Take(pageSize)
```

### **O que faz:**
1. **`(pageNumber - 1) * pageSize`** → Calcula quantos registros pular
2. **`Skip(n)`** → Pula os n primeiros registros
3. **`Take(pageSize)`** → Pega os próximos pageSize registros

### **Exemplo:**
- **Página 3, pageSize 10**: Skip(20).Take(10) → Logs 21-30
- **Página 5, pageSize 25**: Skip(100).Take(25) → Logs 101-125

### **Performance:**
- ✅ Query otimizada no banco
- ✅ Apenas os dados necessários são transferidos
- ✅ Escalável para milhões de registros

### **Metadata:**
- `totalCount` → Total de registros (com filtros)
- `totalPages` → Total de páginas disponíveis
- `hasNextPage` → Pode avançar?
- `hasPreviousPage` → Pode voltar?

---

**Agora a paginação funciona corretamente no BANCO, não na memória!** 🚀
