# SQL Server SSL/TLS Configuration Guide

## 🔍 Erro Comum: Certificado Não Confiável

```
A cadeia de certificado foi emitida por uma autoridade não fidedigna
(The certificate chain was issued by an untrusted authority)
```

### Causa
SQL Server está usando SSL/TLS, mas:
- ❌ Certificado auto-assinado
- ❌ Certificado não confiável
- ❌ Connection string sem configuração SSL

## ✅ Soluções

### Solução 1: TrustServerCertificate=True (Dev)

```json
{
  "ConnectionStrings": {
    "DBconnect": "Server=SERVER;Database=DB;User Id=user;password=pass;TrustServerCertificate=True;"
  }
}
```

**Comportamento:**
- ✅ Aceita qualquer certificado
- ✅ Mantém encriptação SSL ativa
- ⚠️ Não valida certificado (menos seguro)

**Use em:**
- ✅ Desenvolvimento
- ✅ Testes
- ✅ Rede interna confiável
- ❌ **NÃO** em produção (se possível)

### Solução 2: Encrypt=False (Alternativa)

```json
{
  "ConnectionStrings": {
    "DBconnect": "Server=SERVER;Database=DB;User Id=user;password=pass;Encrypt=False;"
  }
}
```

**Comportamento:**
- ❌ Desabilita SSL completamente
- ❌ Dados em texto plano
- ⚠️ Muito inseguro

**Use apenas em:**
- ✅ Rede totalmente isolada
- ❌ **NÃO** recomendado

### Solução 3: Certificado Válido (Produção)

```json
{
  "ConnectionStrings": {
    "DBconnect": "Server=SERVER;Database=DB;User Id=user;password=pass;Encrypt=True;"
  }
}
```

**Requer:**
1. DBA instalar certificado SSL válido no SQL Server
2. Certificado assinado por CA confiável (Let's Encrypt, DigiCert, etc.)
3. Sem `TrustServerCertificate=True`

**Use em:**
- ✅ Produção
- ✅ Ambientes públicos
- ✅ Máxima segurança

## 🏗️ Configuração por Ambiente

### appsettings.json (Base)
```json
{
  "ConnectionStrings": {
    "ParametersConnection": "Server=(localdb)\\mssqllocaldb;Database=ParametersDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

### appsettings.Development.json
```json
{
  "ConnectionStrings": {
    "DBconnect": "Server=SRV05\\SQLDEV2022;Database=BILENE_DESENV;User Id=websa;password=W3B123;TrustServerCertificate=True;"
  }
}
```
- ✅ `TrustServerCertificate=True` para aceitar certificado auto-assinado
- ✅ Senhas de desenvolvimento

### appsettings.Production.json
```json
{
  "ConnectionStrings": {
    "DBconnect": "Server=SRV05\\SQLPROD;Database=BILENE_PROD;User Id=websa_prod;password=SECURE_PASSWORD;Encrypt=True;"
  }
}
```
- ✅ `Encrypt=True` sem `TrustServerCertificate`
- ✅ Senhas fortes
- ✅ Usuários diferentes
- ✅ Bancos de dados diferentes

## 📊 Comparação de Opções

| Parâmetro | Segurança | Encriptação | Valida Cert | Uso Recomendado |
|-----------|-----------|-------------|-------------|-----------------|
| `Encrypt=True` | ⭐⭐⭐⭐⭐ | ✅ Sim | ✅ Sim | Produção |
| `TrustServerCertificate=True` | ⭐⭐⭐ | ✅ Sim | ❌ Não | Dev/Test |
| `Encrypt=False` | ⭐ | ❌ Não | ❌ Não | Rede isolada |

## 🔧 Como o .NET Carrega Configurações

```
1. appsettings.json (base)
   ↓
2. appsettings.{Environment}.json (sobrescreve)
   ↓
3. Environment Variables (sobrescreve)
   ↓
4. User Secrets (dev only)
```

**Exemplo:**
```bash
# Desenvolvimento
ASPNETCORE_ENVIRONMENT=Development
→ Usa appsettings.Development.json
→ Connection string com TrustServerCertificate=True

# Produção
ASPNETCORE_ENVIRONMENT=Production
→ Usa appsettings.Production.json
→ Connection string com Encrypt=True
```

## ⚠️ Segurança: O Que NÃO Fazer

### ❌ Senha no appsettings.json em produção
```json
// ❌ RUIM - Senha exposta
{
  "ConnectionStrings": {
    "DBconnect": "...;password=senha123;..."
  }
}
```

### ✅ Use Azure Key Vault ou Variáveis de Ambiente
```csharp
// Program.cs
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddAzureKeyVault(...);
```

```bash
# Variável de ambiente
export ConnectionStrings__DBconnect="Server=...;password=SECURE;"
```

### ❌ TrustServerCertificate=True em produção
```json
// ❌ RUIM - Aceita qualquer certificado
{
  "ConnectionStrings": {
    "DBconnect": "...;TrustServerCertificate=True;..."
  }
}
```

### ✅ Use certificado válido
```json
// ✅ BOM - Valida certificado
{
  "ConnectionStrings": {
    "DBconnect": "...;Encrypt=True;..."  // Sem TrustServerCertificate
  }
}
```

## 🎯 Checklist de Produção

SQL Server:
- [ ] Certificado SSL instalado
- [ ] Certificado assinado por CA confiável
- [ ] Certificado não está expirado
- [ ] SQL Server configurado para usar SSL

Connection String:
- [ ] `Encrypt=True`
- [ ] Sem `TrustServerCertificate=True`
- [ ] Senha forte
- [ ] Usuário com privilégios mínimos
- [ ] Timeout adequado

Aplicação:
- [ ] Senhas em Key Vault/Env Variables
- [ ] `appsettings.Production.json` separado
- [ ] Logging de erros SSL
- [ ] Retry policy configurado

## 🔐 Instalar Certificado SSL no SQL Server

### Passo 1: Obter Certificado
```bash
# Opção 1: Let's Encrypt (grátis)
certbot certonly --standalone -d sqlserver.yourdomain.com

# Opção 2: Certificado empresarial
# Solicitar ao seu provedor de certificados
```

### Passo 2: Importar no SQL Server
```sql
-- SQL Server Management Studio
-- Tools → SQL Server Configuration Manager
-- SQL Server Network Configuration → Protocols for MSSQLSERVER
-- Properties → Certificate → [Selecionar certificado]
-- Restart SQL Server Service
```

### Passo 3: Testar Connection String
```csharp
// Sem TrustServerCertificate
var connStr = "Server=SERVER;Database=DB;...;Encrypt=True;";
using var conn = new SqlConnection(connStr);
await conn.OpenAsync();  // ✅ Deve funcionar sem erros
```

## 📝 Exemplo Completo: PHCWEBAPI

### Estrutura de Arquivos
```
src/SGOFAPI.Host/
├── appsettings.json                    # Base (sem senhas)
├── appsettings.Development.json        # Dev (TrustServerCertificate=True)
└── appsettings.Production.json         # Prod (Encrypt=True)
```

### appsettings.json (Versionado no Git)
```json
{
  "Logging": { "LogLevel": { "Default": "Information" } },
  "ConnectionStrings": {
    "ParametersConnection": "Server=(localdb)\\mssqllocaldb;Database=ParametersDb;Trusted_Connection=true"
  }
}
```

### appsettings.Development.json (Versionado no Git)
```json
{
  "Logging": { "LogLevel": { "Default": "Debug" } },
  "ConnectionStrings": {
    "DBconnect": "Server=SRV05\\SQLDEV2022;Database=BILENE_DESENV;User Id=websa;password=W3B123;TrustServerCertificate=True;",
    "CFMGERALConnStr": "Server=nacala;Database=E14E105BD_CFM;User Id=saweb;password=Sp3eD!!1989$$00203198969;TrustServerCertificate=True;"
  }
}
```

### appsettings.Production.json (NÃO versionado - .gitignore)
```json
{
  "Logging": { "LogLevel": { "Default": "Information" } },
  "ConnectionStrings": {
    "DBconnect": "Server=SRV05\\SQLPROD;Database=BILENE_PROD;User Id=websa_prod;password=${DB_PASSWORD};Encrypt=True;",
    "CFMGERALConnStr": "Server=nacala;Database=E14E105BD_CFM_PROD;User Id=saweb_prod;password=${CFM_PASSWORD};Encrypt=True;"
  }
}
```

### .gitignore
```
# Não versionar arquivos com senhas de produção
appsettings.Production.json
appsettings.*.local.json
```

## 🚀 Deploy em Produção

### 1. Criar appsettings.Production.json no servidor
```bash
# No servidor de produção
cd /app
nano appsettings.Production.json
# Colar conteúdo com senhas reais
```

### 2. Ou usar variáveis de ambiente
```bash
# Azure App Service / Docker
export ConnectionStrings__DBconnect="Server=...;password=REAL_PASSWORD;Encrypt=True;"
export ConnectionStrings__CFMGERALConnStr="Server=...;password=REAL_PASSWORD;Encrypt=True;"
```

### 3. Verificar ambiente
```csharp
// Program.cs
var environment = builder.Environment.EnvironmentName;
Log.Information("Running in {Environment} environment", environment);

// Output:
// Development: Running in Development environment
// Production:  Running in Production environment
```

## 🔍 Troubleshooting

### Erro: "Certificate chain issued by untrusted authority"
**Solução:**
- Dev: Adicionar `TrustServerCertificate=True`
- Prod: Instalar certificado válido no SQL Server

### Erro: "Login failed for user"
**Solução:**
- Verificar usuário e senha
- Verificar permissões no banco
- Verificar firewall do SQL Server

### Erro: "A network-related error occurred"
**Solução:**
- Verificar se SQL Server está rodando
- Verificar firewall (porta 1433)
- Verificar nome do servidor/instância

### Erro: "Connection timeout"
**Solução:**
- Adicionar `Connection Timeout=30;` na connection string
- Verificar latência de rede
- Verificar se SQL Server está sobrecarregado

## 📚 Referências

- [Microsoft Docs: Connection Strings](https://docs.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqlconnection.connectionstring)
- [SQL Server SSL Configuration](https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/configure-sql-server-encryption)
- [ASP.NET Core Configuration](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/)

## ✅ Resumo

| Ambiente | TrustServerCertificate | Encrypt | Senhas | Versionado |
|----------|------------------------|---------|--------|------------|
| **Development** | ✅ True | ✅ True | Dev | ✅ Sim |
| **Production** | ❌ False | ✅ True | Fortes | ❌ Não |

**Regra de Ouro:** Em produção, use sempre certificado válido + `Encrypt=True` + senhas em Key Vault! 🔐
