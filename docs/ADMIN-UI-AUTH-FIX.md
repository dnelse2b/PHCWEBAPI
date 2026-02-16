# Admin UI Authentication Fix

## Problemas Identificados e Resolvidos

### ❌ Problema 1: Redirect para Login Não Funcionava
**Sintoma:** Ao tentar aceder `/Admin/` sem estar autenticado, não redirecionava para a página de login.

**Causa:** Os paths de autenticação Cookie estavam incorretos:
- Configurado: `/Account/Login`
- Correto: `/Admin/Account/Login`

**Solução:** Corrigidos os paths no arquivo `Auth.Infrastructure/DependencyInjection.cs`:
```csharp
options.LoginPath = "/Admin/Account/Login";
options.LogoutPath = "/Admin/Account/Logout";
options.AccessDeniedPath = "/Admin/Account/AccessDenied";
```

### ❌ Problema 2: Após Login, Não Redirecionava para o Painel
**Sintoma:** Depois de fazer login com sucesso, não redirecionava para `/Admin`.

**Causa:** Conflito entre esquemas de autenticação (JWT Bearer vs Cookie). O Identity precisa usar Cookie authentication, mas o esquema padrão estava configurado para JWT Bearer.

**Solução:** Implementado um **Policy Scheme "Smart"** que escolhe automaticamente o esquema correto:
- **APIs** (com header `Authorization`): Usa JWT Bearer
- **Admin UI** (sem header `Authorization`): Usa Cookie (Identity)

```csharp
services.AddAuthentication(options =>
{
    options.DefaultScheme = "Smart";
    options.DefaultAuthenticateScheme = "Smart";
    options.DefaultChallengeScheme = "Smart";
})
.AddPolicyScheme("Smart", "Smart Auth Scheme", options =>
{
    options.ForwardDefaultSelector = context =>
    {
        if (context.Request.Headers.ContainsKey("Authorization"))
            return JwtBearerDefaults.AuthenticationScheme;
        
        return IdentityConstants.ApplicationScheme;
    };
})
```

## URLs de Acesso

### Painel Admin
- **HTTP:** `http://localhost:7298/Admin`
- **HTTPS:** `https://localhost:7299/Admin`

### Login
- **HTTP:** `http://localhost:7298/Admin/Account/Login`
- **HTTPS:** `https://localhost:7299/Admin/Account/Login`

### Logout
- **HTTP:** `http://localhost:7298/Admin/Account/Logout`
- **HTTPS:** `https://localhost:7299/Admin/Account/Logout`

## Fluxo de Autenticação

### 1. Acesso Não Autenticado
```
User -> /Admin
  ↓
Sistema detecta não autenticado
  ↓
Redirect -> /Admin/Account/Login
```

### 2. Login Bem-Sucedido
```
User -> /Admin/Account/Login (POST)
  ↓
SignInManager valida credenciais
  ↓
Cria cookie de autenticação
  ↓
Redirect -> /Admin (dashboard)
```

### 3. Acesso Autenticado
```
User -> /Admin
  ↓
Cookie válido detectado
  ↓
Autorização verificada (Role: Administrator)
  ↓
Renderiza dashboard
```

## Configurações Importantes

### Identity Cookie
- **Nome:** `AdminPanel.Auth`
- **Expiração:** 8 horas
- **Sliding Expiration:** Sim
- **HttpOnly:** Sim

### Autorização
- **Política:** `AdminOnly`
- **Role Requerida:** `Administrator`
- **Aplicada a:** Todas as páginas em `/Admin/*` exceto Login/Logout

### APIs (JWT Bearer)
- **Esquema:** JWT Bearer Token
- **Header:** `Authorization: Bearer {token}`
- **Expiração:** 3 horas (configurável em `appsettings.json`)

## Arquivos Modificados

1. **`src/Modules/Auth/Auth.Infrastructure/DependencyInjection.cs`**
   - Adicionado Policy Scheme "Smart"
   - Corrigidos paths de Cookie authentication
   - Configurado `ConfigureApplicationCookie` com paths corretos

## Testes Recomendados

### ✅ Teste 1: Redirect para Login
1. Abrir navegador em modo anónimo
2. Navegar para `http://localhost:7298/Admin`
3. **Esperado:** Redirect automático para `/Admin/Account/Login`

### ✅ Teste 2: Login e Redirect para Painel
1. Na página de login, inserir credenciais válidas
2. Clicar em "Entrar"
3. **Esperado:** Redirect para `/Admin` (dashboard)

### ✅ Teste 3: Proteção de Páginas
1. Fazer logout
2. Tentar aceder diretamente `/Admin/Users`
3. **Esperado:** Redirect para `/Admin/Account/Login`

### ✅ Teste 4: API com JWT (não deve ser afetado)
1. Fazer login via API `/api/authenticate/login`
2. Obter token JWT
3. Fazer request para API protegida com header `Authorization: Bearer {token}`
4. **Esperado:** Request bem-sucedido

## Notas Técnicas

### Por que Policy Scheme?
O Policy Scheme permite ter **dois sistemas de autenticação simultâneos**:
- **Cookie:** Para Admin UI (páginas web)
- **JWT Bearer:** Para APIs (clientes externos, mobile, etc.)

O "Smart selector" decide automaticamente qual usar baseado no contexto da requisição.

### Alternativas Consideradas
1. **Opção A:** Usar Cookie como padrão
   - ❌ Problema: APIs param de funcionar sem ajustes
   
2. **Opção B:** Usar JWT como padrão
   - ❌ Problema: Admin UI não funciona (Identity precisa de Cookie)
   
3. **Opção C (Escolhida):** Policy Scheme Smart
   - ✅ Melhor dos dois mundos
   - ✅ Sem mudanças nos controllers
   - ✅ Funciona automaticamente

## Troubleshooting

### Problema: "Access Denied" após login
**Causa:** Utilizador não tem role `Administrator`
**Solução:** Adicionar role via seeding ou SQL:
```sql
INSERT INTO AspNetUserRoles (UserId, RoleId)
SELECT u.Id, r.Id
FROM AspNetUsers u, AspNetRoles r
WHERE u.UserName = 'admin' AND r.Name = 'Administrator'
```

### Problema: Cookie não persiste
**Causa:** HTTPS redirection pode limpar cookies
**Solução:** Usar sempre a mesma URL (HTTP ou HTTPS)

### Problema: API retorna 401 com token válido
**Causa:** Token expirado ou header incorreto
**Solução:** Verificar formato: `Authorization: Bearer {token}` (sem aspas)

---

**Data da Correção:** 15/02/2026  
**Autor:** GitHub Copilot  
**Módulos Afetados:** Auth.Infrastructure, Admin.UI
