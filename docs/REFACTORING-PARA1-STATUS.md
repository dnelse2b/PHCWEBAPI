# 🚧 Refatoração Para1 - Status e Próximos Passos

## ✅ O Que Foi Feito:

### **Domain Layer (Completo)**
- ✅ `E1.cs` → `Para1.cs` renomeado e atualizado com campos reais (descricao, valor, tipo, dec, tam)
- ✅ `E4.cs` removido (não existe na estrutura real)
- ✅ `IE1Repository.cs` → `IPara1Repository.cs` renomeado
- ✅ `IE4Repository.cs` removido

### **Application Layer - Feature CreateParameter (Completo)**
- ✅ `CreateParameterDto` atualizado com campos reais
- ✅ `CreateParameterCommand` atualizado
- ✅ `CreateParameterCommandHandler` atualizado (usa IPara1Repository)
- ✅ `CreateParameterCommandValidator` atualizado
- ✅ `ParameterDto` (response) atualizado

### **API Layer - Controller (Parcial)**
- ✅ Endpoint `Create` atualizado

---

## 🔴 O Que Falta Fazer:

### **1. Infrastructure Layer**
#### **Repository E1Repository.cs:**
```csharp
// Atual: Implementa IE1Repository
// Precisa: Implementar IPara1Repository e usar Para1 ao invés de E1
```

#### **ParametersDbContext.cs:**
```csharp
// Atual:
public DbSet<E1> E1 { get; set; }
public DbSet<E4> E4 { get; set; }

// Precisa:
public DbSet<Para1> Para1 { get; set; }  // Mapear para tabela "para1"
```

#### **E1Configuration.cs:**
```csharp
// Renomear para: Para1Configuration.cs
// Mapear campos: parastamp, descricao, valor, tipo, dec, tam
// Exemplo:
builder.ToTable("para1");
builder.HasKey(p => p.ParaStamp);
builder.Property(p => p.ParaStamp).HasColumnName("parastamp");
builder.Property(p => p.Descricao).HasColumnName("descricao");
builder.Property(p => p.Valor).HasColumnName("valor");
builder.Property(p => p.Tipo).HasColumnName("tipo");
builder.Property(p => p.Dec).HasColumnName("dec");
builder.Property(p => p.Tam).HasColumnName("tam");
```

#### **DependencyInjection.cs (Infrastructure):**
```csharp
// Atual:
services.AddScoped<IE1Repository, E1Repository>();
services.AddScoped<IE4Repository, E4Repository>();

// Precisa:
services.AddScoped<IPara1Repository, Para1Repository>();
```

---

### **2. Application Layer - Outras Features**

#### **UpdateParameter:**
- ❌ `UpdateParameterDto` - atualizar campos
- ❌ `UpdateParameterCommand` - atualizar campos
- ❌ `UpdateParameterCommandHandler` - remover E4, usar Para1
- ❌ `UpdateParameterDtos` - atualizar response

#### **DeleteParameter:**
- ❌ `DeleteParameterCommandHandler` - remover E4Repository, usar apenas Para1Repository

#### **GetAllParameters:**
- ❌ `GetAllParametersQueryHandler` - remover E4, usar Para1
- ❌ `GetAllParametersDtos` - atualizar response

#### **GetParameterByStamp:**
- ❌ `GetParameterByStampQuery` - mudar de e1Stamp para paraStamp
- ❌ `GetParameterByStampQueryHandler` - remover E4, usar Para1
- ❌ `GetParameterByStampDtos` - atualizar response

---

### **3. API Layer - Controller**

#### **Endpoints Restantes:**
```csharp
// ❌ GetAll - atualizar route parameter se necessário
// ❌ GetByStamp - mudar de e1Stamp para paraStamp
// ❌ Update - mudar route e DTO
// ❌ Delete - mudar route parameter
```

---

## 🎯 Ordem Recomendada de Refatoração:

### **Passo 1: Infrastructure (Base)**
1. Renomear `E1Repository.cs` → `Para1Repository.cs`
2. Atualizar implementação para `IPara1Repository`
3. Renomear `E1Configuration.cs` → `Para1Configuration.cs`
4. Mapear campos corretos da tabela `para1`
5. Atualizar `ParametersDbContext.cs`
6. Atualizar `DependencyInjection.cs` (Infrastructure)

### **Passo 2: Application - UpdateParameter**
1. Atualizar `UpdateParameterDto` (InputDto)
2. Atualizar `UpdateParameterCommand`
3. Atualizar `UpdateParameterCommandHandler`
4. Atualizar `UpdateParameterDtos`
5. Atualizar `UpdateParameterInputDto`

### **Passo 3: Application - DeleteParameter**
1. Atualizar `DeleteParameterCommandHandler` (remover E4)

### **Passo 4: Application - GetAllParameters**
1. Atualizar `GetAllParametersQueryHandler`
2. Atualizar `GetAllParametersDtos`

### **Passo 5: Application - GetParameterByStamp**
1. Atualizar `GetParameterByStampQuery`
2. Atualizar `GetParameterByStampQueryHandler`
3. Atualizar `GetParameterByStampDtos`

### **Passo 6: Controller**
1. Atualizar todos os endpoints restantes

### **Passo 7: Build & Test**
1. Build completo
2. Testar todos os endpoints

---

## 📋 Checklist Completo:

### Domain
- [x] Para1.cs criado
- [x] E4.cs removido
- [x] IPara1Repository.cs criado
- [x] IE4Repository.cs removido

### Infrastructure
- [ ] Para1Repository.cs
- [ ] E4Repository.cs removido
- [ ] Para1Configuration.cs
- [ ] E4Configuration.cs removido
- [ ] ParametersDbContext.cs atualizado
- [ ] DependencyInjection.cs atualizado

### Application
- [x] CreateParameter - completo
- [ ] UpdateParameter
- [ ] DeleteParameter
- [ ] GetAllParameters
- [ ] GetParameterByStamp

### API
- [x] Create endpoint
- [ ] GetAll endpoint
- [ ] GetByStamp endpoint
- [ ] Update endpoint
- [ ] Delete endpoint

---

## ⚠️ Notas Importantes:

1. **Não criar migrations** - Tabela `para1` já existe
2. **Mapeamento EF Core** - Usar `.ToTable("para1")` e `.HasColumnName()` para cada campo
3. **Nomenclatura** - Confirmar se os campos no DB são lowercase (`parastamp`, `descricao`, etc.)
4. **Primary Key** - Confirmar se é `parastamp` ou outro campo

---

## 🚀 Continuar Refatoração?

Quer que eu continue refatorando todos os arquivos restantes agora?
Ou prefere fazer manualmente seguindo este guia?

**Total de arquivos faltando:** ~15 arquivos
**Tempo estimado:** 10-15 minutos

