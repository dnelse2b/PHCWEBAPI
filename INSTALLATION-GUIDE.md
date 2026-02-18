# 🚀 Guia de Instalação - PHCAPI Template

## 📦 Instalação Via GitHub (RECOMENDADO)

### Para qualquer pessoa na equipe:

```powershell
# Uma única linha - instala direto do GitHub
dotnet new install https://github.com/dnelse2b/PHCWEBAPI.git
```

✅ **Vantagens:**
- Não precisa clonar o repositório
- Sempre pega a versão mais recente
- Funciona de qualquer lugar
- Ideal para distribuir para a equipe

---

## 🔧 Instalação Local (Desenvolvedores do template)

Se você está desenvolvendo/modificando o template:

```powershell
# 1. Clonar o repositório
git clone https://github.com/dnelse2b/PHCWEBAPI.git
cd PHCWEBAPI

# 2. Instalar localmente
dotnet new install .
```

---

## 📝 Criar Novo Projeto

Após instalar o template (qualquer método acima):

```powershell
# Criar projeto completo
dotnet new phcapi -n MeuProjeto

# Ver todas as opções
dotnet new phcapi --help
```

### Exemplos práticos:

```powershell
# API para sistema de gestão
dotnet new phcapi -n SGOFWS

# API sem módulo de Providers
dotnet new phcapi -n MinhaAPI --providers false

# API com PostgreSQL
dotnet new phcapi -n PostgresAPI --database PostgreSQL

# API apenas com Auth e Audit
dotnet new phcapi -n SimplesAPI --parameters false --providers false
```

---

## 🔄 Atualizar Template

### Se instalou do GitHub:
```powershell
dotnet new uninstall PHCAPI.Template
dotnet new install https://github.com/dnelse2b/PHCWEBAPI.git
```

### Se instalou localmente:
```powershell
cd PHCWEBAPI
git pull
dotnet new uninstall .
dotnet new install .
```

---

## 🗑️ Desinstalar

```powershell
dotnet new uninstall PHCAPI.Template
```

---

## 📋 Verificar Instalação

```powershell
# Listar todos templates instalados
dotnet new list

# Verificar se phcapi está disponível
dotnet new list phcapi
```

Você deve ver:
```
Template Name                             Short Name  Language  Tags
----------------------------------------  ----------  --------  -----------------------------------------------
PHC Web API - Modular Clean Architecture  phcapi      [C#]      Web/API/Clean Architecture/Modular/ASP.NET Core
```

---

## 🌐 Compartilhar com a Equipe

Envie para sua equipe:

```text
Para criar APIs com nossa arquitetura padrão:

1. Instalar o template:
   dotnet new install https://github.com/dnelse2b/PHCWEBAPI.git

2. Criar seu projeto:
   dotnet new phcapi -n NomeDaSuaAPI

3. Configurar e executar:
   cd NomeDaSuaAPI
   dotnet restore
   dotnet build
```

---

## 🔒 Repositório Privado (Se necessário)

Se o repositório se tornar privado:

```powershell
# Configurar credenciais do Git
git config --global credential.helper store

# Instalar (vai pedir credenciais na primeira vez)
dotnet new install https://github.com/dnelse2b/PHCWEBAPI.git

# Ou usar token diretamente
dotnet new install https://ghp_SEU_TOKEN@github.com/dnelse2b/PHCWEBAPI.git
```

---

## ❓ Problemas Comuns

### "No templates found matching: 'phcapi'"
**Solução:** Template não está instalado
```powershell
dotnet new install https://github.com/dnelse2b/PHCWEBAPI.git
```

### "Template already installed"
**Solução:** Desinstale e reinstale
```powershell
dotnet new uninstall PHCAPI.Template
dotnet new install https://github.com/dnelse2b/PHCWEBAPI.git
```

### Falha ao conectar no GitHub
**Solução:** Verifique sua conexão e credenciais Git
```powershell
git config --global credential.helper store
```

---

## 📞 Suporte

- **Repositório:** https://github.com/dnelse2b/PHCWEBAPI
- **Issues:** https://github.com/dnelse2b/PHCWEBAPI/issues
- **Documentação:** Veja pasta `/docs` no projeto criado
