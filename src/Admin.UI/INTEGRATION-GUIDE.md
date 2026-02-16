# Admin.UI - Guia de Integração

## 📋 Passo a Passo para Integrar no Host

### 1. Adicionar Referência ao Projeto

Edite `src/SGOFAPI.Host/SGOFAPI.Host.csproj` e adicione:

```xml
<ItemGroup>
  <ProjectReference Include="..\Admin.UI\Admin.UI.csproj" />
  <ProjectReference Include="..\Modules\Auth\Auth.Infrastructure\Auth.Infrastructure.csproj" />
</ItemGroup>
```

### 2. Configurar Program.cs

Adicione a configuração no `Program.cs` do host:

```csharp
using Admin.UI;
using Auth.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ... outras configurações

// ===== ADICIONAR AUTENTICAÇÃO (se ainda não tiver) =====
builder.Services.AddAuthInfrastructure(builder.Configuration);

// ===== ADICIONAR ADMIN UI =====
builder.Services.AddAdminUI();

// ===== ADICIONAR RAZOR PAGES (se ainda não tiver) =====
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // IMPORTANTE para CSS/JS

// ===== AUTENTICAÇÃO E AUTORIZAÇÃO =====
app.UseAuthentication();
app.UseAuthorization();

// ===== ADMIN UI MIDDLEWARE =====
app.UseAdminUI();

app.MapControllers();

// ===== MAPEAR ADMIN UI =====
app.MapAdminUI();

app.Run();
```

### 3. Testar a Instalação

1. **Compile o projeto:**
   ```bash
   dotnet build src/SGOFAPI.Host
   ```

2. **Execute a aplicação:**
   ```bash
   dotnet run --project src/SGOFAPI.Host
   ```

3. **Aceda ao login:**
   ```
   https://localhost:7298/Identity/Account/Login
   ```

4. **Faça login como admin:**
   - Username: `admin`
   - Password: `Admin@123`

5. **Aceda ao painel:**
   ```
   https://localhost:7298/admin
   ```

### 4. Verificar Funcionamento

Após login, deverá ver:
- ✅ Dashboard com estatísticas
- ✅ Menu lateral com navegação
- ✅ Acesso a todas as páginas de gestão

## 🔧 Configurações Adicionais (Opcional)

### CORS (se necessário para API)

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AdminUI", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// No pipeline
app.UseCors("AdminUI");
```

### Session Timeout Customizado

No `Admin.UI/DependencyInjection.cs`, já está configurado:
```csharp
services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Altere aqui
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
```

### Configurar URL Base da API

Se a API estiver noutro domínio, edite `admin.js`:

```javascript
// Em wwwroot/js/admin.js
window.api = new APIClient('https://api.empresa.pt/api/authenticate');
```

## 🐛 Troubleshooting

### Erro: "Cannot resolve service for type 'UserManager'"

**Solução:** Certifique-se que `AddAuthInfrastructure()` está antes de `AddAdminUI()`

### Erro: "Static files not found"

**Solução:** Adicione `app.UseStaticFiles()` no pipeline

### Erro: "Unauthorized" ao aceder /admin

**Solução:** 
1. Faça login primeiro em `/Identity/Account/Login`
2. Verifique se o utilizador tem a role `Administrator`

### Páginas não carregam CSS/JS

**Solução:** Verifique se `wwwroot/js/admin.js` existe e se `UseStaticFiles()` está ativo

## 📁 Estrutura Final do Projeto

```
src/
├── Admin.UI/                    ← Novo módulo
│   ├── Pages/
│   ├── wwwroot/
│   └── README.md
├── Modules/
│   └── Auth/
│       ├── Auth.Infrastructure/
│       ├── Auth.Presentation/
│       └── Auth.UI/
└── SGOFAPI.Host/               ← Projeto host
    ├── Program.cs              ← Editar aqui
    └── SGOFAPI.Host.csproj     ← Adicionar referência
```

## ✅ Checklist de Integração

- [ ] Adicionar referência ao projeto no `.csproj`
- [ ] Importar namespace `Admin.UI` no `Program.cs`
- [ ] Adicionar `AddAdminUI()` nos serviços
- [ ] Adicionar `UseAdminUI()` no middleware
- [ ] Adicionar `MapAdminUI()` nos endpoints
- [ ] Compilar sem erros
- [ ] Executar aplicação
- [ ] Fazer login como admin
- [ ] Aceder `/admin` com sucesso
- [ ] Testar criação de utilizador
- [ ] Testar criação de role

## 🎉 Pronto!

Após seguir estes passos, o painel de administração estará totalmente funcional!
