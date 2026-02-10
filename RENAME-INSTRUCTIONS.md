# 🔄 Scripts de Renomeação de Projeto

Scripts PowerShell para renomear todo o projeto de **SGOFAPI** para **PHCAPI** (ou qualquer outro nome) de forma segura e automatizada.

---

## 📋 O que os scripts fazem?

### ✅ Renomeia automaticamente:
- Nomes de arquivos (`.cs`, `.csproj`, `.sln`, etc.)
- Nomes de pastas
- Conteúdo dos arquivos (namespaces, classes, strings)
- Configurações (appsettings, URLs, endpoints)
- Documentação e comentários

### 🛡️ Segurança:
- ✅ Cria backup automático antes de qualquer alteração
- ✅ Modo Dry-Run para simular sem fazer alterações
- ✅ Script de reversão incluído
- ✅ Ignora pastas desnecessárias (bin, obj, .git, etc.)

---

## 🚀 Como Usar

### 1️⃣ Renomeação Simples (SGOFAPI → PHCAPI)

```powershell
.\Rename-Project.ps1
```

### 2️⃣ Modo Simulação (Dry-Run)

Veja o que será alterado **sem fazer mudanças**:

```powershell
.\Rename-Project.ps1 -DryRun
```

### 3️⃣ Renomeação Personalizada

```powershell
.\Rename-Project.ps1 -OldName "SGOFAPI" -NewName "PHCAPI"
```

### 4️⃣ Sem Backup (NÃO RECOMENDADO)

```powershell
.\Rename-Project.ps1 -CreateBackup:$false
```

### 5️⃣ Reverter Alterações

Se precisar voltar atrás:

```powershell
.\Revert-Project.ps1
```

---

## 📦 Exemplo Completo

```powershell
# 1. Primeiro, teste em modo simulação
.\Rename-Project.ps1 -DryRun

# 2. Se estiver tudo ok, execute a renomeação
.\Rename-Project.ps1

# 3. Se precisar reverter
.\Revert-Project.ps1
```

---

## ⚙️ Parâmetros

| Parâmetro | Descrição | Padrão |
|-----------|-----------|--------|
| `-OldName` | Nome atual do projeto | `"SGOFAPI"` |
| `-NewName` | Novo nome do projeto | `"PHCAPI"` |
| `-CreateBackup` | Criar backup antes de renomear | `$true` |
| `-DryRun` | Simular sem fazer alterações | `$false` |

---

## 🗂️ Arquivos Processados

O script processa os seguintes tipos de arquivo:
- **Código**: `.cs`, `.csproj`, `.sln`, `.slnx`
- **Configuração**: `.json`, `.xml`, `.config`, `.yaml`, `.yml`
- **Web**: `.razor`, `.cshtml`, `.http`
- **Documentação**: `.md`, `.txt`

---

## 🚫 Pastas Ignoradas

O script **NÃO** processa:
- `bin/`, `obj/` (compilação)
- `.vs/` (Visual Studio)
- `.git/` (controle de versão)
- `node_modules/` (npm)
- `logs/` (logs)
- `packages/` (NuGet)

---

## ✅ Após a Renomeação

### Passos Recomendados:

1. **Feche o Visual Studio** completamente
2. **Delete pastas temporárias**:
   ```powershell
   Get-ChildItem -Path . -Recurse -Directory -Include bin,obj | Remove-Item -Recurse -Force
   ```
3. **Reabra o Visual Studio**
4. **Limpe a solução**: `Build > Clean Solution`
5. **Reconstrua**: `Build > Rebuild Solution`
6. **Teste a aplicação**

### Atualize o Git (se necessário):

```bash
# Atualize a URL do repositório (se mudou)
git remote set-url origin https://github.com/usuario/PHCAPI

# Commit das alterações
git add .
git commit -m "Renomear projeto de SGOFAPI para PHCAPI"
git push
```

---

## 🔧 Troubleshooting

### Erro de Permissão
```powershell
# Execute como administrador ou:
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

### Backup não foi criado
- Verifique se há espaço em disco
- Certifique-se de ter permissões de escrita na pasta pai

### Alguns arquivos não foram renomeados
- Feche todos os programas que possam estar usando os arquivos
- Execute o Visual Studio como Administrador se necessário

---

## ⚠️ Avisos Importantes

1. ⚠️ **Sempre faça backup manual importante antes**
2. ⚠️ **Teste primeiro com `-DryRun`**
3. ⚠️ **Feche o Visual Studio antes de executar**
4. ⚠️ **Commit suas alterações no Git antes**
5. ⚠️ **Não interrompa o script durante execução**

---

## 📊 Exemplo de Saída

```
=========================================
  Renomeação de Projeto
  SGOFAPI → PHCAPI
=========================================

📦 Criando backup...
✅ Backup criado em: ..\BACKUP_SGOFAPI_20240124_153045

🔍 Coletando arquivos e pastas...
   Encontrados 147 arquivos para processar

📝 Substituindo conteúdo nos arquivos...
   ✓ Program.cs
   ✓ appsettings.json
   ✓ SGOFAPI.Host.csproj
   147 arquivos modificados

📄 Renomeando arquivos...
   SGOFAPI.Host.csproj → PHCAPI.Host.csproj
   SGOFAPI.Host.http → PHCAPI.Host.http
   12 arquivos renomeados

📁 Renomeando pastas...
   SGOFAPI.Host → PHCAPI.Host
   3 pastas renomeadas

=========================================
  RESUMO DA RENOMEAÇÃO
=========================================
✅ Conteúdo modificado: 147 arquivos
✅ Arquivos renomeados: 12
✅ Pastas renomeadas: 3
📦 Backup salvo em: ..\BACKUP_SGOFAPI_20240124_153045

🎉 Renomeação concluída com sucesso!

⚠️  PRÓXIMOS PASSOS:
   1. Feche e reabra o Visual Studio
   2. Limpe a solução (Clean Solution)
   3. Reconstrua a solução (Rebuild Solution)
   4. Atualize o repositório Git se necessário
=========================================
```

---

## 📝 Notas

- Os backups são criados na pasta pai do projeto
- Formato do backup: `BACKUP_<NomeAntigo>_<timestamp>`
- O script preserva a estrutura de pastas completa
- Arquivos binários não são processados

---

## 🆘 Suporte

Se encontrar problemas:
1. Verifique o backup criado
2. Use `.\Revert-Project.ps1` para reverter
3. Ou restaure manualmente da pasta de backup

---

**Desenvolvido para facilitar a renomeação de projetos .NET de forma segura e eficiente! 🚀**
