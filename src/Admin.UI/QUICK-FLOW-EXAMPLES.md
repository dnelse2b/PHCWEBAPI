# 🎯 GUIA RÁPIDO: Fluxo Login e Lista de Users

## 🔐 EXEMPLO 1: LOGIN

### Código Simplificado

```csharp
// 📁 Admin.UI/Pages/Account/Login.cshtml.cs

public class LoginModel : PageModel
{
    private readonly SignInManager<IdentityUser> _signInManager;  // ← INJETADO
    
    // 🏗️ Constructor Injection (ASP.NET Core faz automaticamente)
    public LoginModel(SignInManager<IdentityUser> signInManager)
    {
        _signInManager = signInManager;
    }
    
    [BindProperty]  // ← Popula automaticamente com dados do formulário
    public InputModel Input { get; set; } = new();
    
    // 📝 Modelo de dados do formulário
    public class InputModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
    
    // 🌐 GET Request: Carregar página de login
    public async Task OnGetAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl ?? "/Admin/Users";
        // Razor renderiza Login.cshtml → envia HTML para browser
    }
    
    // 📮 POST Request: Processar login
    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        // ✅ 1. Tentar autenticar
        var result = await _signInManager.PasswordSignInAsync(
            Input.Username,    // Ex: "admin@phcapi.com"
            Input.Password,    // Ex: "Admin@123"
            Input.RememberMe,  // Ex: true
            lockoutOnFailure: false
        );
        
        // ✅ 2. Verificar resultado
        if (result.Succeeded)
        {
            // 🎉 Sucesso! Cookie criado automaticamente
            return LocalRedirect(returnUrl ?? "/Admin/Users");
        }
        else
        {
            // ❌ Falhou
            ErrorMessage = "Credenciais inválidas";
            return Page();  // Re-renderiza formulário com erro
        }
    }
}
```

### Fluxo Visual

```
👤 Utilizador preenche formulário
          ↓
    [admin@phcapi.com]
    [Admin@123]
    ☑️ Lembrar-me
          ↓
    Clica "Iniciar sessão"
          ↓
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    POST /Admin/Account/Login
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
          ↓
┌─────────────────────────────────────┐
│  LoginModel.OnPostAsync()           │
│                                     │
│  1. Recebe Input do formulário      │
│     - Username: admin@phcapi.com    │
│     - Password: Admin@123           │
│     - RememberMe: true              │
└──────────┬──────────────────────────┘
           ↓
┌─────────────────────────────────────┐
│  SignInManager.PasswordSignInAsync()│
│                                     │
│  2. Busca user na base de dados     │
│     SELECT * FROM AspNetUsers       │
│     WHERE NormalizedUserName = ...  │
│                                     │
│  3. Verifica password hash          │
│     BCrypt.Verify(input, dbHash)    │
│                                     │
│  4. Password correto?               │
└──────────┬──────────────────────────┘
           ↓ SIM
┌─────────────────────────────────────┐
│  SignInManager.SignInAsync()        │
│                                     │
│  5. Cria cookie de autenticação     │
│     .AspNetCore.Identity.Application│
│     (válido por 30 minutos)         │
└──────────┬──────────────────────────┘
           ↓
┌─────────────────────────────────────┐
│  return LocalRedirect("/Admin/Users")│
└──────────┬──────────────────────────┘
           ↓
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    HTTP 302 Redirect
    Location: /Admin/Users
    Set-Cookie: .AspNetCore.Identity...
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
           ↓
    Browser segue redirect
           ↓
    GET /Admin/Users (com cookie)
           ↓
    ✅ Listagem de utilizadores
```

### O que o SignInManager faz?

```csharp
// Simplificação do que acontece internamente
await _signInManager.PasswordSignInAsync(username, password, rememberMe, false);

// ↓ Internamente:

// 1. Buscar utilizador
var user = await _userManager.FindByNameAsync(username);
// SQL: SELECT * FROM AspNetUsers WHERE NormalizedUserName = 'ADMIN@PHCAPI.COM'

// 2. Verificar password
var isValid = await _userManager.CheckPasswordAsync(user, password);
// BCrypt hash comparison

if (!isValid)
    return SignInResult.Failed;

// 3. Criar cookie de autenticação (claims-based)
var principal = await _signInManager.CreateUserPrincipalAsync(user);
await HttpContext.SignInAsync(
    IdentityConstants.ApplicationScheme,
    principal,
    new AuthenticationProperties { IsPersistent = rememberMe }
);
// Cookie contém: UserId, Username, Roles, etc.

return SignInResult.Success;
```

---

## 👥 EXEMPLO 2: LISTA DE UTILIZADORES

### Código Simplificado

```csharp
// 📁 Admin.UI/Pages/Users/Index.cshtml.cs

[Authorize(Roles = "Administrator")]  // ← Só Admin pode aceder
public class IndexModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;    // ← INJETADO
    private readonly RoleManager<IdentityRole> _roleManager;    // ← INJETADO
    
    // 🏗️ Constructor Injection
    public IndexModel(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }
    
    // 📊 Dados que vão para a View
    public List<UserViewModel> Users { get; set; } = new();
    
    // 🌐 GET Request: Carregar lista de users
    public async Task OnGetAsync()
    {
        // ✅ 1. Buscar todos os utilizadores
        var users = _userManager.Users.ToList();
        // SQL: SELECT * FROM AspNetUsers
        
        // ✅ 2. Para cada utilizador, buscar suas roles
        Users = new List<UserViewModel>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            // SQL: SELECT r.Name FROM AspNetUserRoles ur
            //      JOIN AspNetRoles r ON ur.RoleId = r.Id
            //      WHERE ur.UserId = @userId
            
            Users.Add(new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                Roles = roles.ToList()  // ["Administrator", "InternalUser"]
            });
        }
        
        // ✅ 3. Razor renderiza Index.cshtml com estes dados
        // e envia HTML completo para o browser
    }
}

// 📝 ViewModel para a View
public class UserViewModel
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public List<string> Roles { get; set; }
}
```

### Fluxo Visual

```
👤 Utilizador autenticado visita /Admin/Users
          ↓
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    GET /Admin/Users
    Cookie: .AspNetCore.Identity.Application=...
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
          ↓
┌─────────────────────────────────────────┐
│  Authentication Middleware              │
│                                         │
│  1. Lê cookie                           │
│  2. Valida assinatura                   │
│  3. Desserializa claims:                │
│     - UserId: "abc123"                  │
│     - UserName: "admin@phcapi.com"      │
│     - Role: "Administrator"             │
│  4. Popula HttpContext.User             │
└──────────┬──────────────────────────────┘
           ↓
┌─────────────────────────────────────────┐
│  Authorization Middleware               │
│                                         │
│  5. Verifica [Authorize(Roles = "...")]│
│     User tem role "Administrator"? ✅    │
│     → Permite continuar                 │
└──────────┬──────────────────────────────┘
           ↓
┌─────────────────────────────────────────┐
│  IndexModel.OnGetAsync()                │
│                                         │
│  6. Buscar todos os utilizadores        │
└──────────┬──────────────────────────────┘
           ↓
┌─────────────────────────────────────────┐
│  UserManager.Users.ToList()             │
│                                         │
│  7. Entity Framework gera SQL:          │
│     SELECT Id, UserName, Email, ...     │
│     FROM AspNetUsers                    │
│                                         │
│  Resultado:                             │
│  - User 1: admin@phcapi.com             │
│  - User 2: john.doe@example.com         │
│  - User 3: jane.smith@example.com       │
└──────────┬──────────────────────────────┘
           ↓
┌─────────────────────────────────────────┐
│  Para cada user:                        │
│    UserManager.GetRolesAsync(user)      │
│                                         │
│  8. Para User 1:                        │
│     SELECT r.Name FROM AspNetUserRoles  │
│     WHERE UserId = 'user1-id'           │
│     → ["Administrator"]                 │
│                                         │
│  9. Para User 2:                        │
│     SELECT r.Name FROM AspNetUserRoles  │
│     WHERE UserId = 'user2-id'           │
│     → ["InternalUser", "AuditViewer"]   │
│                                         │
│  10. Para User 3:                       │
│      SELECT r.Name FROM AspNetUserRoles │
│      WHERE UserId = 'user3-id'          │
│      → ["ExternalUser"]                 │
└──────────┬──────────────────────────────┘
           ↓
┌─────────────────────────────────────────┐
│  Criar ViewModels                       │
│                                         │
│  Users = [                              │
│    {                                    │
│      UserName: "admin@phcapi.com",      │
│      Roles: ["Administrator"]           │
│    },                                   │
│    {                                    │
│      UserName: "john.doe@example.com",  │
│      Roles: ["InternalUser", "Audit..."]│
│    },                                   │
│    ...                                  │
│  ]                                      │
└──────────┬──────────────────────────────┘
           ↓
┌─────────────────────────────────────────┐
│  Razor Engine renderiza Index.cshtml    │
│                                         │
│  11. Processa loops:                    │
│      @foreach (var user in Model.Users) │
│      {                                  │
│        <tr>                             │
│          <td>@user.UserName</td>        │
│          <td>                           │
│            @foreach (var role in        │
│                      user.Roles)        │
│            { ... }                      │
│          </td>                          │
│        </tr>                            │
│      }                                  │
└──────────┬──────────────────────────────┘
           ↓
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    HTML completo gerado:
    
    <table>
      <tr>
        <td>admin@phcapi.com</td>
        <td>
          <span>Administrator</span>
        </td>
      </tr>
      <tr>
        <td>john.doe@example.com</td>
        <td>
          <span>InternalUser</span>
          <span>AuditViewer</span>
        </td>
      </tr>
      ...
    </table>
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
           ↓
    Enviado para o Browser
           ↓
    ✅ Tabela renderizada
```

### View (Razor)

```razor
@* 📁 Admin.UI/Pages/Users/Index.cshtml *@

@page "/Admin/Users"
@model Admin.UI.Pages.Users.IndexModel

<h1>Utilizadores (@Model.Users.Count)</h1>

<table class="table">
    <thead>
        <tr>
            <th>Username</th>
            <th>Email</th>
            <th>Roles</th>
            <th>Ações</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var user in Model.Users)  @* ← Dados vêm do PageModel.Users *@
        {
            <tr>
                <td>@user.UserName</td>
                <td>@user.Email</td>
                <td>
                    @foreach (var role in user.Roles)
                    {
                        <span class="badge">@role</span>
                    }
                </td>
                <td>
                    <a href="/Admin/Users/Edit?id=@user.Id">Editar</a>
                </td>
            </tr>
        }
    </tbody>
</table>
```

---

## 🔑 Diferenças Chave vs API REST

### ❌ Como NÃO funciona (API REST + SPA):

```javascript
// Frontend JavaScript (React/Vue/Angular)
useEffect(() => {
    fetch('/api/users', {
        headers: { 'Authorization': 'Bearer ' + localStorage.getItem('token') }
    })
    .then(res => res.json())
    .then(users => setUsers(users));
}, []);

return (
    <table>
        {users.map(user => (
            <tr key={user.id}>
                <td>{user.userName}</td>
                <td>{user.email}</td>
            </tr>
        ))}
    </table>
);
```

**Problemas:**
- 🚫 Precisa criar API separada
- 🚫 Token JWT no localStorage (vulnerável a XSS)
- 🚫 CORS configuration necessária
- 🚫 Mais código para manter (frontend + backend)

### ✅ Como funciona (Razor Pages SSR):

```csharp
// Backend C# (tudo no servidor)
public async Task OnGetAsync()
{
    Users = _userManager.Users.ToList();  // ← Acesso direto aos serviços!
    // Razor renderiza HTML automaticamente
}
```

**Vantagens:**
- ✅ Sem API separada necessária
- ✅ Cookie HttpOnly (seguro contra XSS)
- ✅ Sem CORS issues
- ✅ Uma única codebase

---

## 🏗️ Dependency Injection Explicado

### Como os serviços chegam ao PageModel?

```
Program.cs (Startup)
    ↓ Registar serviços no DI Container
    
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AuthDbContext>();

    ↓ Container agora conhece:
    - UserManager<IdentityUser>
    - RoleManager<IdentityRole>
    - SignInManager<IdentityUser>
    - AuthDbContext
    
    ↓ Request chega
    
ASP.NET Core vê: "IndexModel precisa de UserManager e RoleManager"
    ↓
Container: "Tenho isso! Vou criar instâncias e injetar"
    ↓
new IndexModel(
    userManager: new UserManager(...),  // ← Criado automaticamente
    roleManager: new RoleManager(...)   // ← Criado automaticamente
)
    ↓
PageModel executa normalmente
```

### Lifetime dos Serviços

```csharp
// ⏱️ SCOPED: Uma instância por request HTTP
services.AddDbContext<AuthDbContext>();  // ← Scoped por padrão
// Novo request → Novo DbContext
// Evita problemas de concorrência

// 🌍 SINGLETON: Uma instância para toda a aplicação
services.AddSingleton<ICacheService, RedisCacheService>();
// Criado uma vez, usado por todos

// ⚡ TRANSIENT: Nova instância sempre
services.AddTransient<IEmailService, EmailService>();
// Cada vez que alguém pede, cria novo
```

**No Admin UI:**
- `UserManager` → **Scoped** ✅
- `SignInManager` → **Scoped** ✅
- `AuthDbContext` → **Scoped** ✅

---

## 📊 SQL Queries Geradas (Entity Framework)

### Login

```sql
-- FindByNameAsync(username)
SELECT TOP(1) 
    [u].[Id], 
    [u].[UserName], 
    [u].[Email], 
    [u].[PasswordHash],
    [u].[SecurityStamp],
    [u].[EmailConfirmed],
    [u].[LockoutEnd],
    [u].[AccessFailedCount]
FROM [AspNetUsers] AS [u]
WHERE [u].[NormalizedUserName] = N'ADMIN@PHCAPI.COM'
```

### Listar Users + Roles

```sql
-- 1. Buscar todos os users
SELECT 
    [u].[Id],
    [u].[UserName],
    [u].[Email],
    [u].[EmailConfirmed],
    [u].[LockoutEnd]
FROM [AspNetUsers] AS [u]

-- 2. Para cada user, buscar roles (N+1 queries)
SELECT [r].[Name]
FROM [AspNetUserRoles] AS [ur]
INNER JOIN [AspNetRoles] AS [r] ON [ur].[RoleId] = [r].[Id]
WHERE [ur].[UserId] = @userId  -- Executado para cada user
```

**Nota:** Este é um problema clássico de **N+1 queries**. Em produção, use eager loading:

```csharp
// Melhor performance (uma query só)
var users = await _context.Users
    .Include(u => u.UserRoles)
    .ThenInclude(ur => ur.Role)
    .ToListAsync();
```

---

## 🎯 Resumo

### Login Flow
```
Form POST → LoginModel → SignInManager → Database → Cookie criado → Redirect
```

### List Users Flow
```
GET Request → Check Authorization → IndexModel → UserManager → Database → 
→ Razor renderiza HTML → Send to Browser
```

### Comunicação
```
NUNCA há API calls JavaScript!
Tudo acontece no servidor (C#).
Browser apenas envia forms e recebe HTML pronto.
```

**Documentação completa:** [ARCHITECTURE-COMMUNICATION.md](ARCHITECTURE-COMMUNICATION.md)
