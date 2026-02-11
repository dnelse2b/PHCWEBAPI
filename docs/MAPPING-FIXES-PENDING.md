# 🔧 Correções Pendentes - Mapeamento

## ❌ Erros Encontrados no Build:

### **1. CreateParameterCommand - Falta Campo `CriadoPor`**

**Arquivo:** `src/Modules/Parameters/Parameters.Application/Features/CreateParameter/CreateParameterCommand.cs`

**Corrigir de:**
```csharp
public record CreateParameterCommand(
    string Descricao,
    string Valor,
    string Tipo,
    int? Dec,
    int? Tam
) : IRequest<ParameterDto>;
```

**Para:**
```csharp
public record CreateParameterCommand(
    string Descricao,
    string Valor,
    string Tipo,
    int? Dec,
    int? Tam,
    string? CriadoPor
) : IRequest<ParameterDto>;
```

---

### **2. UpdateParameterCommand - Remover Campo `Ativo`**

**Arquivo:** `src/Modules/Parameters/Parameters.Application/Features/UpdateParameter/UpdateParameterCommand.cs`

**Precisa ter:**
```csharp
public record UpdateParameterCommand(
    string ParaStamp,
    string Descricao,
    string Valor,
    string Tipo,
    int? Dec,
    int? Tam,
    string? AtualizadoPor  // ← SEM ATIVO!
) : IRequest<ParameterDto>;
```

---

### **3. UpdateParameterInputDto - Remover Campo `Ativo`**

**Arquivo:** `src/Modules/Parameters/Parameters.Application/Features/UpdateParameter/UpdateParameterInputDto.cs`

**Corrigir:**
```csharp
public record UpdateParameterDto
{
    public string Descricao { get; init; } = string.Empty;
    public string Valor { get; init; } = string.Empty;
    public string Tipo { get; init; } = string.Empty;
    public int? Dec { get; init; }
    public int? Tam { get; init; }
    // ❌ REMOVER: public bool Ativo { get; init; }
}
```

---

### **4. Todos os ParameterDto - Adicionar Campos de Auditoria**

**Arquivos:**
- `CreateParameter/CreateParameterDtos.cs` (ou `CreateParameterDto.cs`)
- `UpdateParameter/UpdateParameterDtos.cs`
- `GetAllParameters/GetAllParametersDtos.cs`
- `GetParameterByStamp/GetParameterByStampDtos.cs`

**Adicionar em TODOS:**
```csharp
public record ParameterDto
{
    public string ParaStamp { get; init; } = string.Empty;
    public string Descricao { get; init; } = string.Empty;
    public string Valor { get; init; } = string.Empty;
    public string Tipo { get; init; } = string.Empty;
    public int? Dec { get; init; }
    public int? Tam { get; init; }
    
    // ✅ ADICIONAR CAMPOS DE AUDITORIA PHC:
    public DateTime OUsrData { get; init; }
    public string OUsrHora { get; init; } = string.Empty;
    public string? OUsrInis { get; init; }
    public DateTime? UsrData { get; init; }
    public string? UsrHora { get; init; }
    public string? UsrInis { get; init; }
}
```

---

## 🚀 Script PowerShell Para Corrigir Automaticamente:

Execute no terminal:

```powershell
# 1. Adicionar CriadoPor ao CreateParameterCommand
$file = "src/Modules/Parameters/Parameters.Application/Features/CreateParameter/CreateParameterCommand.cs"
$content = @"
using MediatR;

namespace Parameters.Application.Features.CreateParameter;

public record CreateParameterCommand(
    string Descricao,
    string Valor,
    string Tipo,
    int? Dec,
    int? Tam,
    string? CriadoPor
) : IRequest<ParameterDto>;
"@
Set-Content $file $content

# 2. Corrigir UpdateParameterCommand
$file = "src/Modules/Parameters/Parameters.Application/Features/UpdateParameter/UpdateParameterCommand.cs"
$content = @"
using MediatR;

namespace Parameters.Application.Features.UpdateParameter;

public record UpdateParameterCommand(
    string ParaStamp,
    string Descricao,
    string Valor,
    string Tipo,
    int? Dec,
    int? Tam,
    string? AtualizadoPor
) : IRequest<ParameterDto>;
"@
Set-Content $file $content

# 3. Adicionar campos de auditoria em CreateParameterDto
$file = "src/Modules/Parameters/Parameters.Application/Features/CreateParameter/CreateParameterDto.cs"
$content = @"
namespace Parameters.Application.Features.CreateParameter;

public record ParameterDto
{
    public string ParaStamp { get; init; } = string.Empty;
    public string Descricao { get; init; } = string.Empty;
    public string Valor { get; init; } = string.Empty;
    public string Tipo { get; init; } = string.Empty;
    public int? Dec { get; init; }
    public int? Tam { get; init; }
    public DateTime OUsrData { get; init; }
    public string OUsrHora { get; init; } = string.Empty;
    public string? OUsrInis { get; init; }
    public DateTime? UsrData { get; init; }
    public string? UsrHora { get; init; }
    public string? UsrInis { get; init; }
}
"@
Set-Content $file $content

Write-Host "✅ Correções aplicadas! Execute 'dotnet build' para verificar." -ForegroundColor Green
```

---

## 📝 Correções Manuais Necessárias:

### **Copie e cole estes conteúdos:**

#### **GetAllParametersDtos.cs:**
```csharp
namespace Parameters.Application.Features.GetAllParameters;

public record ParameterDto
{
    public string ParaStamp { get; init; } = string.Empty;
    public string Descricao { get; init; } = string.Empty;
    public string Valor { get; init; } = string.Empty;
    public string Tipo { get; init; } = string.Empty;
    public int? Dec { get; init; }
    public int? Tam { get; init; }
    public DateTime OUsrData { get; init; }
    public string OUsrHora { get; init; } = string.Empty;
    public string? OUsrInis { get; init; }
    public DateTime? UsrData { get; init; }
    public string? UsrHora { get; init; }
    public string? UsrInis { get; init; }
}
```

#### **GetParameterByStampDtos.cs:**
```csharp
namespace Parameters.Application.Features.GetParameterByStamp;

public record ParameterDto
{
    public string ParaStamp { get; init; } = string.Empty;
    public string Descricao { get; init; } = string.Empty;
    public string Valor { get; init; } = string.Empty;
    public string Tipo { get; init; } = string.Empty;
    public int? Dec { get; init; }
    public int? Tam { get; init; }
    public DateTime OUsrData { get; init; }
    public string OUsrHora { get; init; } = string.Empty;
    public string? OUsrInis { get; init; }
    public DateTime? UsrData { get; init; }
    public string? UsrHora { get; init; }
    public string? UsrInis { get; init; }
}
```

#### **UpdateParameterDtos.cs:**
```csharp
namespace Parameters.Application.Features.UpdateParameter;

public record ParameterDto
{
    public string ParaStamp { get; init; } = string.Empty;
    public string Descricao { get; init; } = string.Empty;
    public string Valor { get; init; } = string.Empty;
    public string Tipo { get; init; } = string.Empty;
    public int? Dec { get; init; }
    public int? Tam { get; init; }
    public DateTime OUsrData { get; init; }
    public string OUsrHora { get; init; } = string.Empty;
    public string? OUsrInis { get; init; }
    public DateTime? UsrData { get; init; }
    public string? UsrHora { get; init; }
    public string? UsrInis { get; init; }
}
```

#### **UpdateParameterInputDto.cs (Remover Ativo):**
```csharp
namespace Parameters.Application.Features.UpdateParameter;

public record UpdateParameterDto
{
    public string Descricao { get; init; } = string.Empty;
    public string Valor { get; init; } = string.Empty;
    public string Tipo { get; init; } = string.Empty;
    public int? Dec { get; init; }
    public int? Tam { get; init; }
}
```

---

## ✅ Após Correções, Execute:

```sh
dotnet build
```

Se tudo estiver correto, verá:

```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

---

## 🎯 Resultado Final:

**Controllers:**
- ✅ Create: `var command = dto.ToCommand(User.Identity?.Name);` ← 1 linha!
- ✅ Update: `var command = dto.ToCommand(paraStamp, User.Identity?.Name);` ← 1 linha!

**Handlers:**
- ✅ Create: `return saved.ToDto();` ← 1 linha!
- ✅ Update: `return para1.ToDto();` ← 1 linha!
- ✅ GetAll: `return para1List.Select(p => p.ToDto());` ← 1 linha!
- ✅ GetByStamp: `return para1?.ToDto();` ← 1 linha!

**Código limpo e senior-level!** 🚀
