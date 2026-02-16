# 🧪 GUIA DE TESTE - LOCKOUT FUNCIONANDO

## ⚠️ **PROBLEMA REPORTADO**
"O user foi bloqueado mas continua a fazer login"

## 🔍 **CAUSAS POSSÍVEIS**

### 1️⃣ **Aplicação não foi reiniciada**
```powershell
# Parar a aplicação (Ctrl+C no terminal)
# Depois reiniciar:
cd c:\Users\dbarreto\source\repos\PHCWEBAPI\src\SGOFAPI.Host
dotnet run
```

### 2️⃣ **Usuário criado ANTES da correção** (LockoutEnabled = false)
```sql
-- Execute: scripts/FixLockoutForExistingUsers.sql
-- Isso habilita lockout para usuários legados
```

### 3️⃣ **Testando endpoint errado**
- ✅ **Admin.UI:** `http://localhost:7298/Admin/Account/Login` - LOCKOUT ATIVO
- ✅ **API JWT:** `POST /api/authenticate/login` - LOCKOUT ATIVO

---

## 🧪 **TESTE COMPLETO - PASSO A PASSO**

### **TESTE 1: Admin UI (Cookie Authentication)**

1. **Abrir navegador:**
   ```
   http://localhost:7298/Admin/Account/Login
   ```

2. **Errar senha 3 vezes:**
   - Username: `admin`
   - Password: `SENHA_ERRADA_123`
   - Clicar "Login"
   - Repetir 3 vezes

3. **Resultado esperado na 4ª tentativa:**
   ```
   ❌ Conta bloqueada. Tente novamente mais tarde.
   ```

4. **Verificar logs:**
   ```powershell
   # Ver último arquivo de log
   Get-Content c:\Users\dbarreto\source\repos\PHCWEBAPI\src\SGOFAPI.Host\logs\*.txt -Tail 50
   
   # Deve mostrar:
   # [WRN] User admin account locked out.
   ```

---

### **TESTE 2: API JWT (Postman/curl)**

1. **Requisição 1 - Senha errada:**
   ```bash
   curl -X POST https://localhost:5001/api/authenticate/login \
     -H "Content-Type: application/json" \
     -d '{"username":"admin","password":"ERRADA123"}'
   ```
   
   **Resposta:**
   ```json
   {
     "allowed": false,
     "outputResponse": "BAD_CREDENTIALS"
   }
   ```

2. **Requisição 2 - Senha errada:**
   ```bash
   curl -X POST https://localhost:5001/api/authenticate/login \
     -H "Content-Type: application/json" \
     -d '{"username":"admin","password":"ERRADA456"}'
   ```

3. **Requisição 3 - Senha errada:**
   ```bash
   curl -X POST https://localhost:5001/api/authenticate/login \
     -H "Content-Type: application/json" \
     -d '{"username":"admin","password":"ERRADA789"}'
   ```

4. **Requisição 4 - DEVE BLOQUEAR:**
   ```bash
   curl -X POST https://localhost:5001/api/authenticate/login \
     -H "Content-Type: application/json" \
     -d '{"username":"admin","password":"SENHA_CORRETA"}'
   ```
   
   **Resposta esperada:**
   ```json
   {
     "allowed": false,
     "outputResponse": "Account is locked. Please try again later."
   }
   ```

---

## 🗄️ **VERIFICAÇÃO NO BANCO DE DADOS**

```sql
-- Conectar em: SRV05\SQLDEV2022 > BILENE_DESENV

-- Ver status de lockout do usuário
SELECT 
    UserName,
    Email,
    LockoutEnabled,
    AccessFailedCount,
    LockoutEnd,
    CASE 
        WHEN LockoutEnd IS NOT NULL AND LockoutEnd > GETUTCDATE() 
        THEN 'BLOQUEADO ATÉ ' + CONVERT(VARCHAR, LockoutEnd, 120)
        ELSE 'NÃO BLOQUEADO'
    END AS Status
FROM AspNetUsers
WHERE UserName = 'admin';

-- ✅ RESULTADO ESPERADO APÓS 3 FALHAS:
-- LockoutEnabled: 1
-- AccessFailedCount: 3
-- LockoutEnd: 2026-02-16 15:30:00 (15 min no futuro)
```

---

## 🔧 **DESBLOQUEAR USUÁRIO PARA NOVO TESTE**

```sql
-- Resetar lockout manualmente
UPDATE AspNetUsers 
SET AccessFailedCount = 0, 
    LockoutEnd = NULL 
WHERE UserName = 'admin';

-- Verificar reset
SELECT UserName, AccessFailedCount, LockoutEnd 
FROM AspNetUsers 
WHERE UserName = 'admin';
```

---

## 📊 **LOGS DE AUDITORIA**

Verificar logs em tempo real:

```powershell
# Windows PowerShell
Get-Content c:\Users\dbarreto\source\repos\PHCWEBAPI\src\SGOFAPI.Host\logs\PHCAPI-*.txt -Wait | Select-String -Pattern "lockout|locked|AccessFailed"
```

**Logs esperados:**
```
[INF] User found: admin, checking password and lockout status...
[WRN] Invalid password for user: admin. Failed attempts: 1
[WRN] Invalid password for user: admin. Failed attempts: 2
[WRN] Invalid password for user: admin. Failed attempts: 3
[WRN] User admin is locked out until 2026-02-16 15:30:00
```

---

## ✅ **CHECKLIST DE VALIDAÇÃO**

- [ ] Aplicação reiniciada após correção
- [ ] Script SQL executado (`FixLockoutForExistingUsers.sql`)
- [ ] Teste Admin.UI: 3 erros = bloqueio
- [ ] Teste API JWT: 3 erros = bloqueio
- [ ] Banco mostra `LockoutEnabled = 1`
- [ ] Banco mostra `AccessFailedCount = 3`
- [ ] Banco mostra `LockoutEnd` no futuro
- [ ] Logs mostram mensagem de lockout
- [ ] Mesmo com senha correta na 4ª tentativa, login é negado

---

## 🚨 **SE AINDA NÃO FUNCIONAR**

1. **Verificar qual usuário está testando:**
   ```sql
   SELECT * FROM AspNetUsers WHERE UserName = 'SEU_USER';
   ```

2. **Verificar se LockoutEnabled = 1:**
   ```sql
   UPDATE AspNetUsers SET LockoutEnabled = 1 WHERE UserName = 'SEU_USER';
   ```

3. **Limpar cache e reiniciar:**
   ```powershell
   dotnet clean
   dotnet build
   dotnet run --project src/SGOFAPI.Host/PHCAPI.Host.csproj
   ```

4. **Verificar configuração Identity:**
   - Arquivo: `src/Modules/Auth/Auth.Infrastructure/DependencyInjection.cs`
   - Linha 40: `options.Lockout.MaxFailedAccessAttempts = 3;`
   - Linha 39: `options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);`
   - Linha 41: `options.Lockout.AllowedForNewUsers = true;`

---

## 📞 **SUPORTE**

Se após todos estes passos o lockout não funcionar:
1. Enviar print do resultado SQL (`SELECT * FROM AspNetUsers WHERE UserName = 'admin'`)
2. Enviar últimos logs (`logs/PHCAPI-*.txt`)
3. Confirmar qual endpoint está sendo testado (Admin UI ou API)
