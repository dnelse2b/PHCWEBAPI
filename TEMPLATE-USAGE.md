# 📦 PHCAPI Template - Guia de Uso

Este projeto está configurado como um template .NET reutilizável.

## 🚀 Instalação Rápida

### Opção 1: PowerShell (Recomendado)
```powershell
# Execute o script de instalação
.\.template.config\install-template.ps1
```

### Opção 2: Manual
```powershell
# No diretório do projeto
dotnet new install .
```

## 📝 Criar Novo Projeto

### Exemplo Básico
```powershell
dotnet new phcapi -n MeuProjeto
cd MeuProjeto
dotnet restore
dotnet build
```

### Exemplo com Opções
```powershell
# Projeto personalizado
dotnet new phcapi -n MinhaAPI `
  --company-name "MinhaEmpresa" `
  --providers false `
  --database PostgreSQL
```

## ⚙️ Parâmetros Disponíveis

| Parâmetro | Atalho | Padrão | Descrição |
|-----------|--------|--------|-----------|
| `--company-name` | `-cn` | YourCompany | Nome da empresa/projeto |
| `--audit` | `-a` | true | Incluir módulo Audit |
| `--auth` | `-au` | true | Incluir módulo Auth |
| `--parameters` | `-p` | true | Incluir módulo Parameters |
| `--providers` | `-pr` | true | Incluir módulo Providers |
| `--admin-ui` | `-ui` | true | Incluir Admin UI |
| `--tests` | `-t` | true | Incluir projetos de teste |
| `--database` | `-db` | SqlServer | Banco de dados (SqlServer/PostgreSQL) |

## 📋 Exemplos de Uso

### API Completa com Todos os Módulos
```powershell
dotnet new phcapi -n CompletaAPI -cn "Minha Empresa"
```

### API Apenas com Auth e Parameters
```powershell
dotnet new phcapi -n SimplesAPI --audit false --providers false
```

### API Sem Interface Admin
```powershell
dotnet new phcapi -n ApiSemUI --admin-ui false
```

### API com PostgreSQL
```powershell
dotnet new phcapi -n PostgresAPI --database PostgreSQL
```

## 🔧 Após Criar o Projeto

1. **Restaurar dependências:**
   ```powershell
   dotnet restore
   ```

2. **Configurar connection string:**
   - Edite `appsettings.json` ou `appsettings.Development.json`
   - Configure a string de conexão do banco de dados

3. **Aplicar migrations:**
   ```powershell
   dotnet ef database update --project src/SGOFAPI.Host
   ```

4. **Executar:**
   ```powershell
   dotnet run --project src/SGOFAPI.Host
   ```

5. **Acessar Swagger:**
   - https://localhost:5001/swagger

## 🗑️ Desinstalar Template

### PowerShell
```powershell
.\.template.config\uninstall-template.ps1
```

### Manual
```powershell
dotnet new uninstall .
```

## 📦 Distribuir Como NuGet Package

Consulte [.template.config/PACKAGING.md](.template.config/PACKAGING.md) para instruções detalhadas.

### Resumo Rápido:
```powershell
# 1. Criar .nuspec (veja PACKAGING.md)
# 2. Empacotar
nuget pack PHCAPI.Templates.nuspec

# 3. Compartilhar ou publicar
dotnet new install .\PHCAPI.Templates.1.0.0.nupkg
```

## 🏗️ Estrutura do Template

```
PHCWEBAPI/
├── .template.config/          # Configuração do template
│   ├── template.json         # Configuração principal
│   ├── dotnetcli.host.json  # CLI parameters
│   ├── template.vstemplate  # Visual Studio integration
│   ├── install-template.ps1 # Script de instalação
│   └── README.md            # Documentação do template
├── src/
│   ├── SGOFAPI.Host/        # API Host
│   ├── Admin.UI/            # Admin Interface
│   ├── Modules/             # Módulos modulares
│   │   ├── Audit/
│   │   ├── Auth/
│   │   ├── Parameters/
│   │   └── Providers/
│   └── Shared/              # Código compartilhado
├── tests/                   # Testes
├── docs/                    # Documentação
└── scripts/                 # Scripts utilitários
```

## 💡 Dicas

- Use `dotnet new phcapi --help` para ver todas as opções
- Os módulos são condicionalmente incluídos baseado nos parâmetros
- Arquivos de build (bin/obj) são automaticamente excluídos
- O template mantém a estrutura de Clean Architecture

## 🐛 Problemas Comuns

### Template não encontrado
```powershell
# Reinstalar o template
dotnet new uninstall .
dotnet new install .
```

### Erro ao criar projeto
```powershell
# Verificar se o nome do projeto é válido (sem espaços ou caracteres especiais)
dotnet new phcapi -n "NomeValido"
```

## 📚 Recursos Adicionais

- [Documentação oficial dotnet templates](https://learn.microsoft.com/en-us/dotnet/core/tools/custom-templates)
- [Documentação do projeto](docs/)
- [Padrões de arquitetura](docs/Architecture.md)

## 📞 Suporte

Para questões ou problemas, consulte a documentação em `/docs` ou contate a equipe de desenvolvimento.
