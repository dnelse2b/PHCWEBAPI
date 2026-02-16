# 🔒 API Authentication Security - User Enumeration Fix

## 🚨 **PROBLEMA IDENTIFICADO**

### **Vulnerabilidade: User Enumeration**

Mensagens de erro diferentes permitem que atacantes descubram informações sobre o sistema:

```http
❌ VULNERÁVEL (antes):
POST /api/authenticate/login
{ "username": "admin", "password": "wrong" }

Cenário 1: Username não existe
→ Response: "BAD_CREDENTIALS"

Cenário 2: Username existe mas senha errada
→ Response: "BAD_CREDENTIALS"

Cenário 3: Username existe e conta bloqueada
→ Response: "Account is locked. Please try again later."
↑ VAZA INFORMAÇÃO!
```

**Problema:** Atacante descobre:
1. ✅ Username "admin" **EXISTE** no sistema
2. ✅ Houve tentativas de login com falha
3. ✅ Sistema tem mecanismo de lockout (pode parar de tentar por 15 min)

---

## ✅ **CORREÇÃO IMPLEMENTADA**

### **API JWT - Mensagem Genérica (Segurança Máxima)**

```http
✅ SEGURO (agora):
POST /api/authenticate/login

TODAS as falhas retornam:
{
  "allowed": false,
  "outputResponse": "BAD_CREDENTIALS",  ← SEMPRE A MESMA MENSAGEM
  "token": null
}
```

**Cenários cobertos:**
- ❌ Username não existe → `BAD_CREDENTIALS`
- ❌ Senha incorreta → `BAD_CREDENTIALS`
- ❌ Conta bloqueada → `BAD_CREDENTIALS`
- ❌ Usuário desabilitado → `BAD_CREDENTIALS`

**Resultado:** Atacante NÃO consegue descobrir se username existe!

---

### **Admin.UI - Mensagem Específica (UX para Humanos)**

```
✅ Mantido para Admin.UI (Cookie Auth):
/Admin/Account/Login

❌ Conta bloqueada. Tente novamente mais tarde.
```

**Por quê mensagens diferentes?**

| Contexto | Mensagem | Motivo |
|----------|----------|--------|
| **API JWT** | Genérica | Previne user enumeration em integrações automatizadas |
| **Admin.UI** | Específica | Usuário humano precisa saber o que fazer (esperar 15 min) |

---

## 🔍 **AUDITORIA INTERNA - Logs Detalhados**

Mesmo com mensagem genérica na API, **logs internos** continuam detalhados:

```csharp
// Logs mantidos para auditoria (não expostos ao cliente):
_logger.LogWarning("User {Username} is locked out until {LockoutEnd}", username, user.LockoutEnd);
_logger.LogWarning("Invalid password for user: {Username}. Failed attempts: {FailedCount}", username, failedCount);
```

**Logs em:** `logs/PHCAPI-*.txt`

```
[WRN] User admin is locked out until 2026-02-16 15:30:00
[WRN] Invalid password for user: testuser. Failed attempts: 2
```

**Auditoria completa para time interno, zero informação vazada para atacantes!**

---

## 🔑 **RECOMENDAÇÃO: API Keys para Integrações**

### **Problema de usar User/Password para APIs:**

```
❌ Sistema A → User/Password → Sistema B
           (3 tentativas de erro)
                    ↓
         🔒 Integração PARA!
```

**Riscos:**
- 🔴 Lockout acidental → API fica indisponível
- 🔴 Senha expira → Integração quebra
- 🔴 Credenciais em código → Risco de vazamento

---

### **✅ SOLUÇÃO: API Keys (Próxima Implementação)**

```http
POST /api/parameters
Authorization: ApiKey sk_live_abc123xyz...
Content-Type: application/json

{ "code": "PARAM1", "description": "..." }
```

**Benefícios:**
- ✅ **Sem lockout** - API Keys não têm tentativas limitadas
- ✅ **Sem expiração** - Válidas até revogação manual
- ✅ **Revogação individual** - Revogar sem afetar outras integrações
- ✅ **Permissões específicas** - Scope: `parameters:read`, `parameters:write`
- ✅ **Auditoria** - Rastrear qual integração fez cada request

---

## 🧪 **TESTE - User Enumeration Prevenido**

### **Antes da Correção:** ❌

```bash
# Teste 1: Username inexistente
curl -X POST https://localhost:5001/api/authenticate/login \
  -H "Content-Type: application/json" \
  -d '{"username":"naoexiste","password":"123"}'

Response: {"allowed": false, "outputResponse": "BAD_CREDENTIALS"}

# Teste 2: Username existe, conta bloqueada
curl -X POST https://localhost:5001/api/authenticate/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"123"}'

Response: {"allowed": false, "outputResponse": "Account is locked"}
                                                 ↑ VAZA INFORMAÇÃO!
```

### **Depois da Correção:** ✅

```bash
# Teste 1: Username inexistente
curl -X POST https://localhost:5001/api/authenticate/login \
  -H "Content-Type: application/json" \
  -d '{"username":"naoexiste","password":"123"}'

Response: {"allowed": false, "outputResponse": "BAD_CREDENTIALS"}

# Teste 2: Username existe, conta bloqueada
curl -X POST https://localhost:5001/api/authenticate/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"123"}'

Response: {"allowed": false, "outputResponse": "BAD_CREDENTIALS"}
                                                 ↑ MESMA MENSAGEM!
```

**Atacante não consegue mais descobrir se username existe!**

---

## 📊 **COMPARAÇÃO: Admin.UI vs API**

| Característica | Admin.UI (Cookie) | API JWT |
|----------------|-------------------|---------|
| **Usuário** | Humano | Máquina/Integração |
| **Mensagem lockout** | "Conta bloqueada..." | "BAD_CREDENTIALS" |
| **User enumeration** | Aceitável (UX) | **BLOQUEADO** ✅ |
| **Logs detalhados** | ✅ Sim | ✅ Sim |
| **Botão unlock** | ✅ Sim | N/A |
| **Auditoria** | ✅ ResponseLoggingMiddleware | ✅ ResponseLoggingMiddleware |

---

## 🛡️ **PRÓXIMAS MELHORIAS**

### 1. **API Keys (Recomendado para Integrações)**
```csharp
[ApiKey] // Middleware de validação
public class ParametersController : ControllerBase
{
    // Sem risco de lockout!
}
```

### 2. **OAuth 2.0 Client Credentials**
```http
POST /oauth/token
grant_type=client_credentials
client_id=integration_system_a
client_secret=secret123
```

### 3. **Rate Limiting por IP (VULN-003)**
```
Limite: 5 login attempts/min por IP
Previne brute force mesmo sem descobrir username
```

---

## 📚 **Referências de Segurança**

- **OWASP:** [User Enumeration](https://owasp.org/www-project-web-security-testing-guide/latest/4-Web_Application_Security_Testing/03-Identity_Management_Testing/04-Testing_for_Account_Enumeration_and_Guessable_User_Account)
- **CWE-204:** Observable Response Discrepancy
- **NIST:** Generic Error Messages for Authentication

---

## ✅ **CHECKLIST DE VALIDAÇÃO**

- [x] API retorna sempre "BAD_CREDENTIALS" para qualquer falha
- [x] Admin.UI mantém mensagens específicas (UX)
- [x] Logs internos continuam detalhados (auditoria)
- [x] Lockout funciona em ambos (3 tentativas = bloqueio)
- [x] User enumeration prevenido na API
- [ ] **Próximo:** Implementar API Keys para integrações B2B
- [ ] **Próximo:** Rate Limiting por IP (VULN-003)

---

## 🚀 **CONCLUSÃO**

**User/Password NA API:**
- ✅ Mensagem genérica previne user enumeration
- ✅ Lockout protege contra brute force
- ⚠️ **MAS:** Não ideal para integrações (risco de lockout)

**API Keys (Próxima Fase):**
- ✅ Sem risco de lockout
- ✅ Revogação granular
- ✅ Auditoria por integração
- ✅ **RECOMENDADO** para integrações B2B

---

**Implementado em:** 2026-02-16  
**Vulnerabilidade:** CWE-204 (Observable Response Discrepancy)  
**Status:** ✅ CORRIGIDO
