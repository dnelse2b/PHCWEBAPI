# 🚨 SOLUÇÃO: Timeouts no Módulo Audit

## 🔍 Problema

Após implementar a paginação otimizada, começaram a ocorrer **timeouts** nas queries de auditoria.

### **Causa Raiz**

O método `GetPagedAsync()` executa **2 queries**:
1. **`CountAsync()`** - Conta total de registros (COM filtros)
2. **`Skip().Take()`** - Busca página específica

**Sem índices**, o `COUNT(*)` em uma tabela grande (milhares/milhões de logs) pode levar **segundos ou minutos**, causando timeout.

```csharp
// Esta linha está causando timeout:
var totalCount = await query.CountAsync(cancellationToken);
```

## ✅ Soluções Implementadas

### **1️⃣ Aumentar Timeout (TEMPORÁRIO - JÁ APLICADO)**

```csharp
// src\Modules\Audit\Audit.Infrastructure\DependencyInjection.cs
services.AddDbContext<AuditDbContextEFCore>(options =>
{
    options.UseSqlServer(
        configuration.GetConnectionString("DBconnect"),
        sqlOptions => 
        {
            sqlOptions.CommandTimeout(120); // ✅ 120 segundos
            sqlOptions.EnableRetryOnFailure(maxRetryCount: 3);
        });
});
```

**Status**: ✅ **JÁ IMPLEMENTADO**  
**Efeito**: Permite que queries lentas completem sem timeout

---

### **2️⃣ Criar Índices no Banco (PERMANENTE - RECOMENDADO)**

#### **Script SQL Criado**

📄 **Arquivo**: `scripts\CreateAuditIndexes.sql`

#### **Índices a Criar**

| Índice | Colunas | Propósito |
|--------|---------|-----------|
| `IX_ulogs_Data` | `data DESC` | Ordenação por data (query mais comum) |
| `IX_ulogs_RequestId` | `requestid` | Busca por correlation ID |
| `IX_ulogs_Operation` | `operation` | Filtro por operação |
| `IX_ulogs_Data_RequestId` | `data DESC, requestid` | Query composta (otimização extra) |

#### **Como Executar**

1. **Abrir SQL Server Management Studio (SSMS)**
2. **Conectar no banco de dados**
3. **Abrir o arquivo**: `scripts\CreateAuditIndexes.sql`
4. **ALTERAR o nome do banco** na linha 7:
   ```sql
   USE PHC; -- ⚠️ ALTERE PARA O NOME DO SEU BANCO!
   ```
5. **Executar o script** (F5)

#### **Resultado Esperado**

```
✅ Índice IX_ulogs_Data criado com sucesso!
✅ Índice IX_ulogs_RequestId criado com sucesso!
✅ Índice IX_ulogs_Operation criado com sucesso!
✅ Índice IX_ulogs_Data_RequestId criado com sucesso!
```

#### **Performance Esperada**

**ANTES (sem índices)**:
```sql
SELECT COUNT(*) FROM ulogs WHERE data >= '2024-01-01'
-- Tempo: 5-30 segundos (table scan completo)
```

**DEPOIS (com índices)**:
```sql
SELECT COUNT(*) FROM ulogs WHERE data >= '2024-01-01'
-- Tempo: 50-200 ms (index seek)
```

---

### **3️⃣ Otimizar Repository (ALTERNATIVA)**

Se mesmo com índices o COUNT for lento, use esta versão otimizada:

📄 **Arquivo**: `src\Modules\Audit\Audit.Infrastructure\Repositories\AuditLogRepositoryEFCore.OPTIMIZED-TIMEOUT.cs`

**Mudanças**:
1. **Count com try-catch**: Se falhar (timeout), usa estimativa
2. **Paginação ANTES do Count**: Query principal sempre rápida
3. **Validação de página vazia**: Retorna 0 se não houver logs

**Como usar**:
- Substitua o conteúdo do `AuditLogRepositoryEFCore.cs` pelo arquivo `.OPTIMIZED-TIMEOUT`

---

## 📊 Comparação de Performance

### **Cenário: 1 milhão de logs na tabela ulogs**

| Ação | Sem Índices | Com Índices | Otimização Extra |
|------|-------------|-------------|------------------|
| **COUNT(*)** | 30 seg ❌ | 100 ms ✅ | 100 ms ✅ |
| **COUNT com filtro data** | 45 seg ❌ | 150 ms ✅ | 150 ms ✅ |
| **SELECT com paginação** | 10 seg ❌ | 50 ms ✅ | 50 ms ✅ |
| **Query completa (COUNT + SELECT)** | 40 seg ❌ | 200 ms ✅ | 200 ms (ou estimativa) ✅ |

---

## 🧪 Como Testar

### **1. Verificar Timeout Atual**

Fazer uma chamada à API:

```http
GET /api/audit?pageNumber=1&pageSize=50
```

**Antes**: Timeout após 30 segundos  
**Depois (timeout maior)**: Responde em até 120 segundos  
**Depois (com índices)**: Responde em ~200ms

### **2. Verificar Índices Criados**

```sql
-- Executar no SSMS
SELECT 
    i.name AS IndexName,
    i.type_desc AS IndexType,
    STATS_DATE(i.object_id, i.index_id) AS LastUpdated
FROM sys.indexes i
WHERE OBJECT_NAME(i.object_id) = 'ulogs'
ORDER BY i.name;
```

**Resultado esperado**:
```
IX_ulogs_Data
IX_ulogs_Data_RequestId
IX_ulogs_Operation
IX_ulogs_RequestId
```

### **3. Testar Performance da Query**

```sql
-- Ativar estatísticas
SET STATISTICS TIME ON;
SET STATISTICS IO ON;

-- Testar Count
SELECT COUNT(*) FROM ulogs WHERE data >= DATEADD(DAY, -7, GETDATE());

-- Testar paginação
SELECT TOP 50 * 
FROM ulogs 
WHERE data >= DATEADD(DAY, -7, GETDATE())
ORDER BY data DESC;

SET STATISTICS TIME OFF;
SET STATISTICS IO OFF;
```

**Análise**:
- `Logical reads` → Deve ser BAIXO (< 100)
- `CPU time` → Deve ser < 100ms
- `Elapsed time` → Deve ser < 200ms

---

## 🔧 Troubleshooting

### **❌ Ainda tem timeout mesmo com índices**

**Possíveis causas**:
1. Índices não foram criados corretamente
2. Tabela está muito fragmentada
3. Estatísticas desatualizadas

**Solução**:
```sql
-- 1. Verificar índices
SELECT name FROM sys.indexes WHERE object_id = OBJECT_ID('ulogs');

-- 2. Atualizar estatísticas
UPDATE STATISTICS ulogs;

-- 3. Reorganizar índices
ALTER INDEX ALL ON ulogs REORGANIZE;

-- 4. Reconstruir índices (se necessário)
ALTER INDEX ALL ON ulogs REBUILD;
```

### **❌ Query lenta mesmo com índices**

**Causa**: Index seek não está sendo usado (table scan)

**Solução**: Verificar plano de execução

```sql
-- Ver plano de execução
SET SHOWPLAN_ALL ON;
SELECT COUNT(*) FROM ulogs WHERE data >= '2024-01-01';
SET SHOWPLAN_ALL OFF;
```

**Procurar por**:
- ✅ **Index Seek** (bom)
- ❌ **Table Scan** ou **Index Scan** (ruim)

### **❌ Tabela muito grande (milhões de logs)**

**Solução 1**: Arquivar logs antigos

```sql
-- Criar tabela de arquivo
CREATE TABLE ulogs_archive (LIKE ulogs);

-- Mover logs antigos (> 1 ano)
INSERT INTO ulogs_archive
SELECT * FROM ulogs WHERE data < DATEADD(YEAR, -1, GETDATE());

-- Deletar da tabela principal
DELETE FROM ulogs WHERE data < DATEADD(YEAR, -1, GETDATE());
```

**Solução 2**: Particionamento de tabela

(Mais complexo, mas muito eficiente para tabelas gigantes)

---

## 📋 Checklist de Implementação

### **Fase 1: Solução Imediata (JÁ FEITA)**
- [x] Aumentar timeout para 120 segundos
- [x] Adicionar retry logic

### **Fase 2: Solução Permanente (FAZER AGORA)**
- [ ] Executar script `CreateAuditIndexes.sql`
- [ ] Verificar se índices foram criados
- [ ] Testar performance da API
- [ ] Verificar logs de queries lentas

### **Fase 3: Otimizações Extras (SE NECESSÁRIO)**
- [ ] Implementar versão otimizada do Repository
- [ ] Configurar cache de contagem
- [ ] Arquivar logs antigos
- [ ] Considerar particionamento

---

## 📊 Monitoramento

### **Queries Lentas (SQL Server)**

```sql
-- Ver queries lentas no SQL Server
SELECT TOP 10
    total_elapsed_time / execution_count AS avg_elapsed_time,
    total_worker_time / execution_count AS avg_worker_time,
    execution_count,
    SUBSTRING(st.text, (qs.statement_start_offset/2)+1,
        ((CASE qs.statement_end_offset
            WHEN -1 THEN DATALENGTH(st.text)
            ELSE qs.statement_end_offset
        END - qs.statement_start_offset)/2) + 1) AS query_text
FROM sys.dm_exec_query_stats AS qs
CROSS APPLY sys.dm_exec_sql_text(qs.sql_handle) AS st
WHERE st.text LIKE '%ulogs%'
ORDER BY avg_elapsed_time DESC;
```

### **Logs da Aplicação**

```csharp
// Handler já tem logs
_logger.LogInformation(
    "Fetching audit logs - Page: {PageNumber}, Size: {PageSize}",
    request.PageNumber, request.PageSize);

_logger.LogInformation(
    "Retrieved {Count} audit logs out of {TotalCount} (Page {PageNumber}/{TotalPages})",
    dtos.Count, totalCount, request.PageNumber, totalPages);
```

---

## ✅ Resultado Esperado

**Após implementar as soluções**:

| Métrica | Antes | Depois |
|---------|-------|--------|
| **Tempo de Resposta** | 30+ seg (timeout) | 200-500 ms |
| **Count Query** | 30 seg | 100-200 ms |
| **Select Query** | 10 seg | 50-100 ms |
| **Erro Rate** | 50%+ (timeouts) | 0% |

---

## 🎯 Próximos Passos

1. **EXECUTAR** o script de índices (`scripts\CreateAuditIndexes.sql`)
2. **TESTAR** a API após criar índices
3. **MONITORAR** performance por 24h
4. **Se ainda houver problemas**, implementar versão otimizada do Repository

---

**Status**: ⚠️ **SOLUÇÃO PARCIAL APLICADA**  
**Próximo Passo**: 🔴 **CRIAR ÍNDICES NO BANCO** (CRÍTICO!)

**Data**: 10 de fevereiro de 2024
