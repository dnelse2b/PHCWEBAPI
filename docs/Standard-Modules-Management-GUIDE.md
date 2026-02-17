# 📦 Guia de Gestão de Módulos Padronizados

## 🎯 Objetivo

Este documento define a estratégia de gestão e partilha dos módulos **Auth** e **Audit** entre múltiplos projetos da organização, utilizando **NuGet Packages** como mecanismo de distribuição.

---

## 📋 Tabela de Conteúdos

1. [Visão Geral](#-visão-geral)
2. [Arquitetura de Módulos](#-arquitetura-de-módulos)
3. [Estratégia de Versionamento](#-estratégia-de-versionamento)
4. [Configuração de Projetos](#-configuração-de-projetos)
5. [Publicação de Pacotes](#-publicação-de-pacotes)
6. [Consumo de Pacotes](#-consumo-de-pacotes)
7. [Pipeline CI/CD](#-pipeline-cicd)
8. [Workflows de Desenvolvimento](#-workflows-de-desenvolvimento)
9. [Troubleshooting](#-troubleshooting)

---

## 🏗️ Visão Geral

### Módulos Disponíveis

#### 1️⃣ **Auth Module** (Autenticação e Autorização)
- **Domínio:** Entidades de utilizadores, roles, permissões
- **Aplicação:** Comandos e queries para gestão de identidade
- **Infraestrutura:** JWT, Identity Framework, password hashing
- **Apresentação:** Controllers de login, registo, refresh tokens

#### 2️⃣ **Audit Module** (Auditoria e Logs)
- **Domínio:** Entidades de auditoria (AuditLog, LoginAudit)
- **Aplicação:** Handlers para registo de ações
- **Infraestrutura:** Persistência de logs, indexação

#### 3️⃣ **Shared.Kernel** (Funcionalidades Comuns)
- DTOs base (Result, Response)
- Extensions (StringExtensions, DateExtensions)
- Políticas de validação
- Constantes globais

### Vantagens da Abordagem NuGet

✅ **Versionamento Semântico** - Controlo preciso de versões  
✅ **Gestão de Dependências** - Resolução automática de conflitos  
✅ **Cache e Performance** - Armazenamento local de pacotes  
✅ **Segurança** - Validação de assinaturas e integridade  
✅ **CI/CD Ready** - Integração automática em pipelines  
✅ **Rollback Simples** - Downgrade para versões anteriores  

---

## 🏛️ Arquitetura de Módulos

### Estrutura de Pastas

```
PHCWEBAPI/
├── src/
│   ├── Modules/
│   │   ├── Auth/
│   │   │   ├── Auth.Domain/
│   │   │   │   ├── Entities/
│   │   │   │   ├── Interfaces/
│   │   │   │   └── Auth.Domain.csproj
│   │   │   │
│   │   │   ├── Auth.Application/
│   │   │   │   ├── Commands/
│   │   │   │   ├── Queries/
│   │   │   │   ├── Validators/
│   │   │   │   └── Auth.Application.csproj
│   │   │   │
│   │   │   ├── Auth.Infrastructure/
│   │   │   │   ├── Identity/
│   │   │   │   ├── Jwt/
│   │   │   │   ├── Persistence/
│   │   │   │   └── Auth.Infrastructure.csproj
│   │   │   │
│   │   │   └── Auth.Presentation/
│   │   │       ├── Controllers/
│   │   │       └── Auth.Presentation.csproj
│   │   │
│   │   ├── Audit/
│   │   │   ├── Audit.Domain/
│   │   │   ├── Audit.Application/
│   │   │   └── Audit.Infrastructure/
│   │   │
│   │   └── Parameters/
│   │       └── (estrutura similar)
│   │
│   └── Shared/
│       └── Shared.Kernel/
│           ├── DTOs/
│           ├── Extensions/
│           └── Shared.Kernel.csproj
│
├── tests/
│   ├── Auth.Application.Tests/
│   └── Audit.Application.Tests/
│
└── docs/
    └── Standard-Modules-Management-GUIDE.md (este ficheiro)
```

### Dependências entre Camadas

```
Presentation → Application → Infrastructure → Domain
                     ↓            ↓
                Shared.Kernel   Shared.Kernel
```

**Regras:**
- Domain **NÃO** depende de nenhuma camada
- Application depende apenas de Domain
- Infrastructure depende de Domain + Application
- Presentation depende de todas as camadas
- Shared.Kernel pode ser referenciado por todos

---

## 🔢 Estratégia de Versionamento

### Semantic Versioning (SemVer 2.0)

Formato: **`MAJOR.MINOR.PATCH[-PRERELEASE]`**

```
Exemplo: 2.3.1-beta.1
         │ │ │  └─ Pre-release identifier
         │ │ └──── PATCH: Bug fixes
         │ └────── MINOR: New features (backward compatible)
         └──────── MAJOR: Breaking changes
```

### Quando Incrementar Cada Versão

#### 🔴 **MAJOR** (Breaking Changes)
```diff
- Remoção de métodos públicos
- Alteração de assinaturas de métodos existentes
- Mudança de namespaces principais
- Remoção de propriedades de DTOs

Exemplo: 1.5.3 → 2.0.0
```

#### 🟡 **MINOR** (New Features)
```diff
+ Adição de novos endpoints
+ Novos métodos em interfaces (com default implementation)
+ Novas propriedades opcionais em DTOs
+ Novos validators ou handlers

Exemplo: 1.5.3 → 1.6.0
```

#### 🟢 **PATCH** (Bug Fixes)
```diff
~ Correção de bugs sem alterar API pública
~ Melhorias de performance
~ Ajustes de logging
~ Correções de documentação

Exemplo: 1.5.3 → 1.5.4
```

#### 🔵 **PRE-RELEASE**
```yaml
alpha:   Versão instável em desenvolvimento ativo
         Exemplo: 2.0.0-alpha.1

beta:    Versão feature-complete, em teste
         Exemplo: 2.0.0-beta.2

rc:      Release Candidate, pronto para produção se não houver bugs críticos
         Exemplo: 2.0.0-rc.1
```

### Compatibilidade de Versões

| Versão Cliente | Versão Servidor | Compatível? |
|----------------|-----------------|-------------|
| 1.5.x          | 1.5.x          | ✅ Sim      |
| 1.6.x          | 1.5.x          | ✅ Sim      |
| 1.5.x          | 1.6.x          | ⚠️ Parcial  |
| 2.0.x          | 1.x.x          | ❌ Não      |

---

## ⚙️ Configuração de Projetos

### 1. Configurar Metadados NuGet

Editar cada `*.csproj` dos módulos:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    
    <!-- ✅ Metadados NuGet -->
    <PackageId>YourCompany.Auth.Domain</PackageId>
    <Version>1.0.0</Version>
    <Authors>Your Company DevTeam</Authors>
    <Company>Your Company</Company>
    <Product>Shared Modules</Product>
    <Description>Auth Domain Layer - Entities, Interfaces, Domain Logic</Description>
    <PackageTags>auth;authentication;domain;ddd</PackageTags>
    <PackageLicenseExpression>MIT</PackageLicenseExpression>
    <RepositoryUrl>https://github.com/yourcompany/phcwebapi</RepositoryUrl>
    <RepositoryType>git</RepositoryType>
    <PackageProjectUrl>https://github.com/yourcompany/phcwebapi</PackageProjectUrl>
    
    <!-- ✅ NuGet Package Settings -->
    <GeneratePackageOnBuild>false</GeneratePackageOnBuild>
    <IncludeSymbols>true</IncludeSymbols>
    <SymbolPackageFormat>snupkg</SymbolPackageFormat>
  </PropertyGroup>

  <!-- Dependências -->
  <ItemGroup>
    <PackageReference Include="MediatR" Version="12.2.0" />
    <!-- ... outras dependências ... -->
  </ItemGroup>

</Project>
```

### 2. Criar Ficheiro `Directory.Build.props`

Na raiz de `src/Modules/`, criar:

```xml
<Project>
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    
    <!-- Versão centralizada -->
    <Version>1.0.0</Version>
    <Company>Your Company</Company>
    <Authors>Your Company DevTeam</Authors>
  </PropertyGroup>
</Project>
```

---

## 📤 Publicação de Pacotes

### Opção 1: Manual (Desenvolvimento)

```powershell
# 1️⃣ Navegar para o projeto
cd src/Modules/Auth/Auth.Domain

# 2️⃣ Criar o pacote
dotnet pack -c Release -o ../../../../nupkgs

# 3️⃣ Publicar para feed local (teste)
dotnet nuget push ../../../../nupkgs/YourCompany.Auth.Domain.1.0.0.nupkg -s C:\LocalNuGetFeed

# 4️⃣ Publicar para feed privado (Azure Artifacts, GitHub Packages, etc.)
dotnet nuget push ../../../../nupkgs/YourCompany.Auth.Domain.1.0.0.nupkg \
  -s https://pkgs.dev.azure.com/yourorg/_packaging/yourfeed/nuget/v3/index.json \
  -k YOUR_API_KEY
```

### Opção 2: Script PowerShell Automatizado

Criar `scripts/Publish-Modules.ps1`:

```powershell
<#
.SYNOPSIS
    Publica todos os módulos Auth e Audit como pacotes NuGet
.PARAMETER Version
    Versão a aplicar (Semantic Versioning)
.PARAMETER Feed
    Feed NuGet de destino (local, azureartifacts, github)
.PARAMETER Configuration
    Build configuration (Release/Debug)
.EXAMPLE
    .\Publish-Modules.ps1 -Version "1.2.0" -Feed azureartifacts -Configuration Release
#>

param(
    [Parameter(Mandatory=$true)]
    [string]$Version,
    
    [Parameter(Mandatory=$false)]
    [ValidateSet("local", "azureartifacts", "github")]
    [string]$Feed = "local",
    
    [Parameter(Mandatory=$false)]
    [ValidateSet("Release", "Debug")]
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

# 🎯 Configuração
$RootPath = Split-Path $PSScriptRoot -Parent
$ModulesPath = Join-Path $RootPath "src\Modules"
$OutputPath = Join-Path $RootPath "nupkgs"

# 📦 Módulos a publicar
$Modules = @(
    "Auth\Auth.Domain",
    "Auth\Auth.Application",
    "Auth\Auth.Infrastructure",
    "Auth\Auth.Presentation",
    "Audit\Audit.Domain",
    "Audit\Audit.Application",
    "Audit\Audit.Infrastructure",
    "Shared\Shared.Kernel"
)

# 🧹 Limpar output anterior
if (Test-Path $OutputPath) {
    Remove-Item $OutputPath -Recurse -Force
}
New-Item -ItemType Directory -Path $OutputPath | Out-Null

Write-Host "🚀 Publicando módulos versão $Version para $Feed" -ForegroundColor Cyan

foreach ($module in $Modules) {
    $projectPath = Join-Path $ModulesPath $module
    $projectFile = Get-ChildItem -Path $projectPath -Filter "*.csproj" | Select-Object -First 1
    
    if (-not $projectFile) {
        Write-Warning "⚠️ Projeto não encontrado: $module"
        continue
    }
    
    Write-Host "`n📦 Empacotando: $module" -ForegroundColor Yellow
    
    # 1️⃣ Build + Pack
    dotnet pack $projectFile.FullName `
        -c $Configuration `
        -o $OutputPath `
        -p:Version=$Version `
        --include-symbols `
        --include-source
    
    if ($LASTEXITCODE -ne 0) {
        throw "❌ Falha ao empacotar $module"
    }
}

# 🚀 Publicar pacotes
Write-Host "`n🚀 Publicando pacotes..." -ForegroundColor Cyan

$packages = Get-ChildItem -Path $OutputPath -Filter "*.nupkg" | Where-Object { $_.Name -notlike "*.symbols.nupkg" }

foreach ($package in $packages) {
    Write-Host "📤 Publicando: $($package.Name)" -ForegroundColor Green
    
    switch ($Feed) {
        "local" {
            $localFeed = "C:\LocalNuGetFeed"
            if (-not (Test-Path $localFeed)) {
                New-Item -ItemType Directory -Path $localFeed | Out-Null
            }
            Copy-Item $package.FullName -Destination $localFeed -Force
        }
        "azureartifacts" {
            dotnet nuget push $package.FullName `
                -s "https://pkgs.dev.azure.com/yourorg/_packaging/yourfeed/nuget/v3/index.json" `
                -k $env:AZURE_ARTIFACTS_TOKEN `
                --skip-duplicate
        }
        "github" {
            dotnet nuget push $package.FullName `
                -s "https://nuget.pkg.github.com/yourorg/index.json" `
                -k $env:GITHUB_TOKEN `
                --skip-duplicate
        }
    }
    
    if ($LASTEXITCODE -ne 0) {
        Write-Warning "⚠️ Falha ao publicar $($package.Name)"
    }
}

Write-Host "`n✅ Publicação concluída!" -ForegroundColor Green
Write-Host "📊 Total de pacotes: $($packages.Count)" -ForegroundColor Cyan
```

### Executar o Script

```powershell
# Feed local (desenvolvimento)
.\scripts\Publish-Modules.ps1 -Version "1.0.0" -Feed local

# Azure Artifacts (staging)
$env:AZURE_ARTIFACTS_TOKEN = "your-pat-token"
.\scripts\Publish-Modules.ps1 -Version "1.0.0" -Feed azureartifacts

# GitHub Packages (produção)
$env:GITHUB_TOKEN = "your-github-token"
.\scripts\Publish-Modules.ps1 -Version "1.2.0" -Feed github
```

---

## 📥 Consumo de Pacotes

### 1. Configurar Feed NuGet

Criar/editar `nuget.config` na raiz do projeto consumidor:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <!-- Feed público -->
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
    
    <!-- Feed local (desenvolvimento) -->
    <add key="LocalFeed" value="C:\LocalNuGetFeed" />
    
    <!-- Feed privado (Azure Artifacts) -->
    <add key="CompanyFeed" value="https://pkgs.dev.azure.com/yourorg/_packaging/yourfeed/nuget/v3/index.json" />
    
    <!-- GitHub Packages -->
    <add key="GitHubPackages" value="https://nuget.pkg.github.com/yourorg/index.json" />
  </packageSources>
  
  <packageSourceCredentials>
    <CompanyFeed>
      <add key="Username" value="youruser" />
      <add key="ClearTextPassword" value="%AZURE_ARTIFACTS_TOKEN%" />
    </CompanyFeed>
    <GitHubPackages>
      <add key="Username" value="youruser" />
      <add key="ClearTextPassword" value="%GITHUB_TOKEN%" />
    </GitHubPackages>
  </packageSourceCredentials>
</configuration>
```

### 2. Instalar Pacotes no Projeto

```powershell
# No projeto consumidor
cd YourNewProject.API

# Instalar módulo Auth completo
dotnet add package YourCompany.Auth.Domain --version 1.0.0
dotnet add package YourCompany.Auth.Application --version 1.0.0
dotnet add package YourCompany.Auth.Infrastructure --version 1.0.0
dotnet add package YourCompany.Auth.Presentation --version 1.0.0

# Instalar módulo Audit
dotnet add package YourCompany.Audit.Domain --version 1.0.0
dotnet add package YourCompany.Audit.Application --version 1.0.0
dotnet add package YourCompany.Audit.Infrastructure --version 1.0.0

# Shared Kernel
dotnet add package YourCompany.Shared.Kernel --version 1.0.0
```

### 3. Registar Serviços no `Program.cs`

```csharp
using Auth.Infrastructure;
using Audit.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ✅ Registar módulos via Extension Methods
builder.Services.AddAuthModule(builder.Configuration);
builder.Services.AddAuditModule(builder.Configuration);

// ✅ Configurar DbContext com migrations dos módulos
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("YourNewProject.API") // ⚠️ Migrations no projeto host
    )
);

var app = builder.Build();

// ✅ Usar middlewares dos módulos
app.UseAuthentication();
app.UseAuthorization();
app.UseAuditLogging(); // Custom middleware do Audit Module

app.MapControllers();
app.Run();
```

---

## 🔄 Pipeline CI/CD

### GitHub Actions Workflow

Criar `.github/workflows/publish-modules.yml`:

```yaml
name: 📦 Publish NuGet Packages

on:
  push:
    branches:
      - main
      - release/*
    paths:
      - 'src/Modules/**'
  workflow_dispatch:
    inputs:
      version:
        description: 'Version to publish (e.g., 1.2.0)'
        required: true
        type: string

jobs:
  publish:
    runs-on: windows-latest
    
    steps:
    - name: 🔍 Checkout Repository
      uses: actions/checkout@v4
      with:
        fetch-depth: 0
    
    - name: ⚙️ Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '10.0.x'
    
    - name: 📦 Restore Dependencies
      run: dotnet restore
    
    - name: 🧪 Run Tests
      run: dotnet test --no-restore --verbosity normal
    
    - name: 🏗️ Build Modules
      run: dotnet build src/Modules/Auth/Auth.Domain -c Release --no-restore
      # Repetir para cada módulo ou usar script
    
    - name: 📤 Pack Modules
      run: |
        $version = "${{ github.event.inputs.version || '1.0.0' }}"
        dotnet pack src/Modules/Auth/Auth.Domain -c Release -o ./nupkgs -p:Version=$version
        dotnet pack src/Modules/Auth/Auth.Application -c Release -o ./nupkgs -p:Version=$version
        # ... outros módulos
    
    - name: 🚀 Push to GitHub Packages
      run: |
        dotnet nuget push ./nupkgs/*.nupkg `
          --source "https://nuget.pkg.github.com/${{ github.repository_owner }}/index.json" `
          --api-key ${{ secrets.GITHUB_TOKEN }} `
          --skip-duplicate
    
    - name: 🚀 Push to Azure Artifacts (opcional)
      if: github.ref == 'refs/heads/main'
      run: |
        dotnet nuget push ./nupkgs/*.nupkg `
          --source "https://pkgs.dev.azure.com/yourorg/_packaging/yourfeed/nuget/v3/index.json" `
          --api-key ${{ secrets.AZURE_DEVOPS_TOKEN }} `
          --skip-duplicate
    
    - name: 📊 Create GitHub Release
      if: github.event_name == 'workflow_dispatch'
      uses: actions/create-release@v1
      env:
        GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
      with:
        tag_name: v${{ github.event.inputs.version }}
        release_name: Modules v${{ github.event.inputs.version }}
        draft: false
        prerelease: false
```

### Azure Pipelines (alternativa)

Criar `azure-pipelines.yml`:

```yaml
trigger:
  branches:
    include:
      - main
      - release/*
  paths:
    include:
      - src/Modules/*

pool:
  vmImage: 'windows-latest'

variables:
  buildConfiguration: 'Release'
  majorVersion: 1
  minorVersion: 0
  patchVersion: $[counter(format('{0}.{1}', variables['majorVersion'], variables['minorVersion']), 0)]
  packageVersion: '$(majorVersion).$(minorVersion).$(patchVersion)'

stages:
- stage: Build
  jobs:
  - job: BuildAndTest
    steps:
    - task: UseDotNet@2
      inputs:
        packageType: 'sdk'
        version: '10.0.x'
    
    - task: DotNetCoreCLI@2
      displayName: 'Restore'
      inputs:
        command: 'restore'
        projects: 'src/Modules/**/*.csproj'
    
    - task: DotNetCoreCLI@2
      displayName: 'Build'
      inputs:
        command: 'build'
        projects: 'src/Modules/**/*.csproj'
        arguments: '--configuration $(buildConfiguration) --no-restore'
    
    - task: DotNetCoreCLI@2
      displayName: 'Test'
      inputs:
        command: 'test'
        projects: 'tests/**/*.csproj'
        arguments: '--no-restore --verbosity normal'

- stage: Pack
  dependsOn: Build
  condition: succeeded()
  jobs:
  - job: PackModules
    steps:
    - task: DotNetCoreCLI@2
      displayName: 'Pack NuGet Packages'
      inputs:
        command: 'pack'
        packagesToPack: 'src/Modules/**/*.csproj'
        versioningScheme: 'byEnvVar'
        versionEnvVar: 'packageVersion'
        configuration: '$(buildConfiguration)'
        outputDir: '$(Build.ArtifactStagingDirectory)/nupkgs'
    
    - task: PublishBuildArtifacts@1
      inputs:
        PathtoPublish: '$(Build.ArtifactStagingDirectory)/nupkgs'
        ArtifactName: 'nupkgs'

- stage: Publish
  dependsOn: Pack
  condition: and(succeeded(), eq(variables['Build.SourceBranch'], 'refs/heads/main'))
  jobs:
  - deployment: PublishPackages
    environment: 'Production'
    strategy:
      runOnce:
        deploy:
          steps:
          - task: NuGetCommand@2
            displayName: 'Push to Azure Artifacts'
            inputs:
              command: 'push'
              packagesToPush: '$(Pipeline.Workspace)/**/*.nupkg'
              nuGetFeedType: 'internal'
              publishVstsFeed: 'yourorg/yourfeed'
```

---

## 🔄 Workflows de Desenvolvimento

### Cenário 1: Bug Fix em Módulo Existente

```bash
# 1️⃣ Criar branch de correção
git checkout -b fix/auth-token-expiration

# 2️⃣ Fazer correção no código
# Editar: src/Modules/Auth/Auth.Infrastructure/Jwt/JwtTokenGenerator.cs

# 3️⃣ Atualizar versão (PATCH)
# Editar Auth.Infrastructure.csproj: <Version>1.0.0</Version> → <Version>1.0.1</Version>

# 4️⃣ Testar localmente
dotnet test

# 5️⃣ Publicar para feed local
.\scripts\Publish-Modules.ps1 -Version "1.0.1" -Feed local

# 6️⃣ Testar em projeto consumidor
cd ..\ProjectConsumer
dotnet add package YourCompany.Auth.Infrastructure --version 1.0.1
dotnet run

# 7️⃣ Commit e Push
git add .
git commit -m "fix(auth): correct JWT token expiration calculation"
git push origin fix/auth-token-expiration

# 8️⃣ Criar Pull Request
# Após aprovação, merge para main dispara CI/CD
```

### Cenário 2: Nova Feature (Minor Version)

```bash
# 1️⃣ Branch de feature
git checkout -b feature/auth-2fa

# 2️⃣ Implementar funcionalidade
# Adicionar: Auth.Application/Commands/Enable2FACommand.cs
# Adicionar: Auth.Presentation/Controllers/TwoFactorController.cs

# 3️⃣ Incrementar versão MINOR
# <Version>1.0.1</Version> → <Version>1.1.0</Version>

# 4️⃣ Testes
dotnet test

# 5️⃣ Publicar pre-release (beta)
.\scripts\Publish-Modules.ps1 -Version "1.1.0-beta.1" -Feed local

# 6️⃣ Validação em staging
# Instalar versão beta em ambiente de teste
dotnet add package YourCompany.Auth.Application --version 1.1.0-beta.1

# 7️⃣ Após validação, publicar versão estável
.\scripts\Publish-Modules.ps1 -Version "1.1.0" -Feed azureartifacts
```

### Cenário 3: Breaking Change (Major Version)

```bash
# ⚠️ IMPORTANTE: Comunicar breaking change à equipa!

# 1️⃣ Branch de major version
git checkout -b breaking/auth-v2

# 2️⃣ Implementar mudanças incompatíveis
# Exemplo: Remover método obsoleto, alterar assinatura de interface

# 3️⃣ Atualizar documentação de migração
# Criar: docs/MIGRATION-v1-to-v2.md

# 4️⃣ Incrementar MAJOR version
# <Version>1.5.3</Version> → <Version>2.0.0</Version>

# 5️⃣ Publicar pre-release para validação
.\scripts\Publish-Modules.ps1 -Version "2.0.0-rc.1" -Feed local

# 6️⃣ Coordenar migração com projetos consumidores
# Atualizar projetos dependentes conforme guia de migração

# 7️⃣ Publicar versão final
.\scripts\Publish-Modules.ps1 -Version "2.0.0" -Feed azureartifacts
```

---

## 🛠️ Troubleshooting

### Problema 1: Pacote não encontrado

```powershell
# ❌ Erro: Package 'YourCompany.Auth.Domain 1.0.0' is not found

# ✅ Solução:
# 1. Verificar feeds configurados
dotnet nuget list source

# 2. Limpar cache NuGet
dotnet nuget locals all --clear

# 3. Restaurar novamente
dotnet restore --force
```

### Problema 2: Conflito de versões

```powershell
# ❌ Erro: Version conflict detected for YourCompany.Auth.Domain

# ✅ Solução:
# 1. Verificar versões instaladas
dotnet list package --include-transitive

# 2. Forçar versão específica no .csproj
<ItemGroup>
  <PackageReference Include="YourCompany.Auth.Domain" Version="[1.0.0]" />
</ItemGroup>

# 3. Restaurar
dotnet restore
```

### Problema 3: Migrations não aplicadas

```powershell
# ❌ Erro: No migrations found in Auth.Infrastructure

# ✅ Solução:
# Migrations devem estar no projeto HOST, não no módulo

# 1. Adicionar migrations no projeto consumidor
dotnet ef migrations add InitialAuthModule -c AuthDbContext -o Data/Migrations/Auth

# 2. Aplicar migrations
dotnet ef database update -c AuthDbContext
```

### Problema 4: Autenticação no feed privado

```powershell
# ❌ Erro: 401 Unauthorized ao publicar pacote

# ✅ Solução Azure Artifacts:
# 1. Instalar credential provider
Invoke-WebRequest -Uri https://aka.ms/install-artifacts-credprovider.ps1 | Invoke-Expression

# 2. Configurar token
$env:AZURE_ARTIFACTS_TOKEN = "your-pat-token"

# ✅ Solução GitHub Packages:
# 1. Criar Personal Access Token com scopes: write:packages, read:packages
# 2. Configurar no nuget.config
dotnet nuget add source https://nuget.pkg.github.com/yourorg/index.json `
  -n GitHubPackages `
  -u youruser `
  -p YOUR_GITHUB_TOKEN `
  --store-password-in-clear-text
```

### Problema 5: Dependências transitivas

```powershell
# ❌ Erro: Multiple versions of MediatR detected

# ✅ Solução:
# 1. Criar Directory.Packages.props na raiz (Central Package Management)
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>
  <ItemGroup>
    <PackageVersion Include="MediatR" Version="12.2.0" />
    <PackageVersion Include="FluentValidation" Version="11.9.0" />
  </ItemGroup>
</Project>

# 2. Remover versões dos .csproj
<PackageReference Include="MediatR" /> <!-- Sem Version -->
```

---

## 📚 Referências

### Documentação Oficial
- [NuGet Package Creation](https://learn.microsoft.com/en-us/nuget/create-packages/creating-a-package)
- [Semantic Versioning](https://semver.org/)
- [Central Package Management](https://learn.microsoft.com/en-us/nuget/consume-packages/central-package-management)
- [Azure Artifacts](https://learn.microsoft.com/en-us/azure/devops/artifacts/)
- [GitHub Packages](https://docs.github.com/en/packages)

### Checklist de Publicação

- [ ] Versão atualizada em todos os `.csproj`
- [ ] Testes unitários passam (100% success)
- [ ] Documentação XML gerada (`<GenerateDocumentationFile>true</GenerateDocumentationFile>`)
- [ ] Release notes criadas (CHANGELOG.md)
- [ ] Breaking changes documentadas (MIGRATION.md)
- [ ] Pacote testado localmente
- [ ] Tags Git criadas (`git tag v1.0.0`)
- [ ] CI/CD pipeline executado com sucesso
- [ ] Notificação enviada à equipa

---

## 📝 Notas Finais

### Boas Práticas

1. **Sempre testar localmente** antes de publicar para feeds partilhados
2. **Nunca publicar versões instáveis** para produção sem sufixo `-beta` ou `-rc`
3. **Manter compatibilidade retroativa** sempre que possível
4. **Documentar breaking changes** com guias de migração
5. **Usar pre-releases** para validação em staging
6. **Manter changelog atualizado** em cada release

### Próximos Passos

1. Implementar script de publicação automatizado
2. Configurar pipeline CI/CD no Azure DevOps ou GitHub Actions
3. Criar templates de migration para breaking changes
4. Estabelecer processo de code review para módulos partilhados
5. Configurar Dependabot/Renovate para atualização automática de dependências

---

**Versão do Documento:** 1.0.0  
**Última Atualização:** 2026-02-17  
**Autor:** DevOps Team  
**Contacto:** devops@yourcompany.com
