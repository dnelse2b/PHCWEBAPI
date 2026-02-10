# 🚀 GUIA RÁPIDO - Renomeação de Projeto

## ⚡ Uso Mais Simples (Windows)

**Duplo-clique em:**
```
QUICK-RENAME.bat
```

---

## 🎯 Passos Recomendados

### ANTES de executar:
1. ✅ Commit suas alterações no Git
2. ✅ Feche o Visual Studio
3. ✅ Faça um backup manual (opcional, script já faz)

### EXECUTAR:
```powershell
# Opção 1: Arquivo .bat (mais simples)
QUICK-RENAME.bat

# Opção 2: PowerShell direto
.\Rename-Project.ps1

# Opção 3: Teste primeiro (simulação)
.\Rename-Project.ps1 -DryRun
```

### DEPOIS de executar:
1. ✅ Delete pastas bin/obj:
   ```powershell
   Get-ChildItem -Path . -Recurse -Directory -Include bin,obj | Remove-Item -Recurse -Force
   ```
2. ✅ Reabra o Visual Studio
3. ✅ Clean Solution
4. ✅ Rebuild Solution
5. ✅ Teste a aplicação

---

## 📋 Checklist Completo

```
□ Commit Git realizado
□ Visual Studio fechado
□ Script executado com sucesso
□ Backup verificado
□ bin/obj deletados
□ Visual Studio reaberto
□ Clean Solution executado
□ Rebuild Solution executado
□ Aplicação testada
□ Git atualizado (se necessário)
```

---

## 🆘 Se algo der errado

### Opção 1: Script de Reversão
```powershell
.\Revert-Project.ps1
```

### Opção 2: Backup Manual
```
1. Vá para a pasta pai do projeto
2. Procure a pasta: BACKUP_SGOFAPI_YYYYMMDD_HHMMSS
3. Copie de volta para o local original
```

### Opção 3: Git Reset
```bash
git reset --hard HEAD
git clean -fd
```

---

## 🔥 Comandos Úteis

### Limpar projeto completamente:
```powershell
# Delete bin, obj, .vs
Get-ChildItem -Path . -Recurse -Directory -Include bin,obj,.vs | Remove-Item -Recurse -Force

# Delete packages (se necessário)
Get-ChildItem -Path . -Recurse -Directory -Include packages | Remove-Item -Recurse -Force
```

### Ver o que será alterado:
```powershell
# Lista arquivos com SGOFAPI no nome
Get-ChildItem -Path . -Recurse -Filter "*SGOFAPI*" | Select-Object FullName

# Lista arquivos com SGOFAPI no conteúdo
Get-ChildItem -Path . -Recurse -Include *.cs,*.json,*.csproj | Select-String -Pattern "SGOFAPI" | Select-Object Path -Unique
```

---

## 💡 Dicas

1. **Sempre teste com `-DryRun` primeiro**
2. **Não interrompa o script durante execução**
3. **Mantenha backups por alguns dias antes de deletar**
4. **Verifique URLs e conexões após renomear**
5. **Atualize documentação do projeto**

---

## ⏱️ Tempo Estimado

- Backup: ~30 segundos
- Renomeação: ~1-2 minutos
- Rebuild: ~2-5 minutos

**Total: ~5 minutos** ⚡

---

**Pronto para começar? Execute `QUICK-RENAME.bat` agora! 🚀**
