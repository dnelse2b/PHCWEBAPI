# 🔒 CORS Configurado - Teste de Validação

## ✅ Implementado com Sucesso

As configurações de CORS foram atualizadas para **restringir origens** conforme especificado.

---

## 📋 Origens Configuradas

### **Produção** (`appsettings.json`)
- ✅ `https://uat1.portmaputo.com:9000`
- ✅ `http://localhost:7298`

### **Desenvolvimento** (`appsettings.Development.json`)
- ✅ `http://localhost:7298`
- ✅ `https://localhost:7298`
- ✅ `http://localhost:5001`
- ✅ `https://localhost:5001`

---

## 🔧 Como Adicionar Novas Origens

Edite **apenas** o arquivo `appsettings.json` ou `appsettings.Development.json`:

```json
{
  "AllowedOrigins": [
    "https://uat1.portmaputo.com:9000",
    "http://localhost:7298",
    "https://nova-origem.com"  // ← Adicione aqui
  ]
}
```

**Não é necessário modificar código!** A aplicação carrega automaticamente as origens na inicialização.

---

## 🧪 Testes de Validação

### Teste 1: Origem Permitida (deve funcionar ✅)
```bash
curl -X OPTIONS https://localhost:5001/api/auth/login \
  -H "Origin: http://localhost:7298" \
  -H "Access-Control-Request-Method: POST" \
  -H "Access-Control-Request-Headers: Content-Type" \
  -v
```

**Resultado esperado:**
```
< HTTP/1.1 204 No Content
< Access-Control-Allow-Origin: http://localhost:7298
< Access-Control-Allow-Credentials: true
< Access-Control-Allow-Methods: GET, POST, PUT, DELETE, OPTIONS
```

---

### Teste 2: Origem NÃO Permitida (deve bloquear ❌)
```bash
curl -X OPTIONS https://localhost:5001/api/auth/login \
  -H "Origin: https://evil-site.com" \
  -H "Access-Control-Request-Method: POST" \
  -v
```

**Resultado esperado:**
```
< HTTP/1.1 204 No Content
# ❌ SEM headers Access-Control-Allow-Origin
# = Origem bloqueada!
```

---

### Teste 3: JavaScript (Browser Console)

Abra o console do navegador em `https://uat1.portmaputo.com:9000` e execute:

```javascript
// ✅ Deve funcionar (origem permitida)
fetch('https://seu-servidor/api/auth/login', {
    method: 'POST',
    headers: {
        'Content-Type': 'application/json'
    },
    body: JSON.stringify({
        username: 'test',
        password: 'test'
    })
})
.then(r => r.json())
.then(data => console.log('✅ Sucesso:', data))
.catch(err => console.error('❌ Erro:', err));
```

Se a origem não estiver configurada, você verá:
```
Access to fetch at 'https://...' from origin 'https://evil.com' 
has been blocked by CORS policy: No 'Access-Control-Allow-Origin' header
```

---

## 📊 Logs da Aplicação

Ao iniciar a aplicação, você verá no log:

```
[Information] ✅ CORS configured with 2 allowed origins: https://uat1.portmaputo.com:9000, http://localhost:7298
```

Se **não** houver origens configuradas:
```
[Warning] ⚠️ No AllowedOrigins configured in appsettings.json. CORS will block all origins.
```

---

## 🔍 Verificar Configuração Atual

```powershell
# Ver origens configuradas em produção
Get-Content src/SGOFAPI.Host/appsettings.json | Select-String -Pattern "AllowedOrigins" -Context 0,5

# Ver origens configuradas em desenvolvimento
Get-Content src/SGOFAPI.Host/appsettings.Development.json | Select-String -Pattern "AllowedOrigins" -Context 0,5
```

---

## ⚠️ Importante

1. **Nunca use `AllowAnyOrigin()`** em produção
2. **Sempre especifique protocolo completo**: `https://` ou `http://`
3. **Inclua porta se necessário**: `:9000`, `:7298`
4. **Não use trailing slash**: ❌ `https://site.com/` → ✅ `https://site.com`
5. **Reinicie a aplicação** após modificar `appsettings.json`

---

## 🛡️ Segurança Implementada

✅ **AllowCredentials**: Permite cookies e autenticação  
✅ **Métodos específicos**: Apenas GET, POST, PUT, DELETE, OPTIONS  
✅ **Headers específicos**: Content-Type, Authorization, X-Requested-With  
✅ **Logging**: Registra origens configuradas na inicialização  
✅ **Validação**: Alerta se nenhuma origem estiver configurada  

---

## 🔄 Próximas Correções de Segurança

- [ ] Rate Limiting (VULN-003)
- [ ] Política de Senha Forte (VULN-002)
- [ ] Security Headers (VULN-004)
- [ ] JWT Secret Seguro (VULN-005)

Consulte `SECURITY-FIXES-IMPLEMENTATION.md` para as próximas correções.
