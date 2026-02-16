# Admin.UI - Painel de Administração

Interface de administração moderna estilo SPA para gestão completa de utilizadores e roles do sistema.

## 🎨 Características

### ✨ Design Moderno
- **Interface SPA** - Experiência fluida sem recarregar página
- **Tailwind CSS** - Design responsivo e bonito
- **Animações suaves** - Transições e efeitos visuais agradáveis
- **Dark sidebar** - Menu lateral com gradientes modernos
- **Cards interativos** - Componentes com hover effects

### 🚀 Funcionalidades

#### Dashboard
- Estatísticas em tempo real
- Cards com métricas de utilizadores e roles
- Atividade recente do sistema
- Quick actions para tarefas comuns

#### Gestão de Utilizadores
- ✅ **Listar** todos os utilizadores com pesquisa e filtros
- ✅ **Criar** novos utilizadores com validação de password
- ✅ **Ver detalhes** completos de cada utilizador
- ✅ **Gerir roles** - atribuir e remover roles
- ✅ **Eliminar** utilizadores (com confirmação)
- ✅ **Filtrar** por role e estado

#### Gestão de Roles
- ✅ **Listar** todas as roles do sistema
- ✅ **Criar** novas roles personalizadas
- ✅ **Ver utilizadores** associados a cada role
- ✅ **Eliminar** roles personalizadas
- ✅ Roles do sistema protegidas (Administrator, InternalUser, etc.)

## 🏗️ Arquitetura

```
Admin.UI/
├── Admin.UI.csproj
├── DependencyInjection.cs          # Configuração modular
├── Pages/
│   ├── _Layout.cshtml              # Layout base com sidebar
│   ├── _ViewStart.cshtml
│   ├── _ViewImports.cshtml
│   ├── Index.cshtml                # Dashboard principal
│   ├── Index.cshtml.cs
│   ├── Users/
│   │   ├── Index.cshtml            # Lista de utilizadores
│   │   ├── Index.cshtml.cs
│   │   ├── Create.cshtml           # Criar utilizador
│   │   └── Create.cshtml.cs
│   └── Roles/
│       ├── Index.cshtml            # Gestão de roles
│       └── Index.cshtml.cs
├── wwwroot/
│   └── js/
│       └── admin.js                # JavaScript SPA-like
└── README.md
```

## 🚀 Instalação e Configuração

### 1. Adicionar ao Projeto Host

Edite `src/SGOFAPI.Host/SGOFAPI.Host.csproj`:

```xml
<ItemGroup>
  <ProjectReference Include="..\Admin.UI\Admin.UI.csproj" />
</ItemGroup>
```

### 2. Configurar no Program.cs

```csharp
using Admin.UI;

// ... outras configurações

// Adicionar Admin UI (após AddAuth e AddAuthUI)
builder.Services.AddAdminUI();

// ... no middleware pipeline

app.UseAdminUI();

// ... antes de app.Run()

app.MapAdminUI();
```

### 3. Executar a Aplicação

```bash
cd src/SGOFAPI.Host
dotnet run
```

Aceda ao painel:
```
https://localhost:7298/admin
```

## 🔐 Autenticação e Autorização

### Requisitos de Acesso
- **Role necessária**: `Administrator`
- **Tipo de autenticação**: Cookie-based (para browsers)

### Login
1. Aceda a `/Identity/Account/Login`
2. Utilize as credenciais de admin:
   - **Username**: `admin`
   - **Password**: `Admin@123`

3. Após login, aceda `/admin`

### Rotas Protegidas
Todas as páginas do Admin.UI requerem a role `Administrator`:

```
/admin                  → Dashboard
/admin/users            → Lista de utilizadores
/admin/users/create     → Criar utilizador
/admin/roles            → Gestão de roles
```

## 📱 Páginas Disponíveis

### Dashboard (`/admin`)
**Métricas e Visão Geral**
- Total de utilizadores
- Utilizadores ativos
- Roles configuradas
- Estado do sistema
- Utilizadores recentes
- Atividade recente
- Quick actions

### Utilizadores (`/admin/users`)
**Lista Completa de Utilizadores**
- Tabela com todos os utilizadores
- Pesquisa por nome ou email
- Filtros por role e estado
- Ações: Ver, Editar, Gerir Roles, Eliminar
- Paginação automática

### Criar Utilizador (`/admin/users/create`)
**Formulário de Criação**
- Nome de utilizador (validação)
- Email (validação)
- Password (com indicador de força)
- Confirmar password
- Atribuir roles (múltipla seleção)
- Email confirmado (checkbox)

### Gestão de Roles (`/admin/roles`)
**Visualização de Roles**
- Cards com todas as roles
- Contagem de utilizadores por role
- Criar nova role personalizada
- Eliminar roles personalizadas
- Informações das roles do sistema

## 🎯 Funcionalidades JavaScript

### API Client
Cliente JavaScript para comunicação com a API:

```javascript
const api = window.api;

// Utilizadores
await api.getUsers();
await api.createUser({ username, email, password });
await api.deleteUser(userId);

// Roles
await api.getRoles();
await api.createRole(roleName);
await api.getUserRoles(username);
await api.addRoleToUser(username, role);
```

### Toast Notifications
Sistema de notificações elegante:

```javascript
ToastManager.show('Mensagem', 'success'); // success, error, warning, info
```

### Modals
Modais bonitos para confirmações:

```javascript
ModalManager.confirm('Título', 'Mensagem', async () => {
    // Ação ao confirmar
});
```

### Loading States
Gestão de estados de carregamento:

```javascript
LoadingManager.show(button);
LoadingManager.hide(button);
```

### Form Validation
Validação client-side:

```javascript
FormValidator.validateEmail(email);
FormValidator.validatePassword(password);
FormValidator.validateUsername(username);
```

## 🎨 Customização

### Cores do Tema
As cores principais são definidas via Tailwind:

```javascript
// No _Layout.cshtml
tailwind.config = {
    theme: {
        extend: {
            colors: {
                primary: '#667eea',    // Roxo principal
                secondary: '#764ba2',  // Roxo secundário
                accent: '#f093fb',     // Rosa accent
            }
        }
    }
}
```

### Sidebar
Para adicionar novos itens ao menu, edite `_Layout.cshtml`:

```html
<a href="/admin/nova-pagina" class="sidebar-link ...">
    <i class="fas fa-icon w-6"></i>
    <span class="ml-3 font-medium">Nova Página</span>
</a>
```

## 🔗 Integração com API

O Admin.UI comunica com os endpoints da `Auth.Presentation`:

| Endpoint | Método | Descrição |
|----------|--------|-----------|
| `/api/authenticate/login` | POST | Login |
| `/api/authenticate/register` | POST | Criar utilizador |
| `/api/authenticate/roles` | GET | Listar roles |
| `/api/authenticate/roles` | POST | Criar role |
| `/api/authenticate/users/add-role` | POST | Atribuir role |
| `/api/authenticate/users/{username}/roles` | GET | Ver roles do utilizador |

## 🛡️ Segurança

### Validações
- **Client-side**: JavaScript com validação em tempo real
- **Server-side**: ASP.NET Core Identity (validação de password, unicidade, etc.)

### Password Requirements
- Mínimo 6 caracteres
- Pelo menos 1 letra maiúscula
- Pelo menos 1 letra minúscula
- Pelo menos 1 número

### Proteção CSRF
- Integrado via ASP.NET Core
- Tokens automáticos em formulários

### Autorização
- Todas as páginas requerem role `Administrator`
- Validação server-side em cada PageModel

## 📊 Tecnologias Utilizadas

- **ASP.NET Core 8.0** - Framework backend
- **Razor Pages** - Rendering server-side
- **Tailwind CSS** - Framework CSS moderno
- **Font Awesome 6.5** - Ícones
- **Vanilla JavaScript** - Sem dependências pesadas
- **Fetch API** - Comunicação com backend
- **CSS3 Animations** - Transições suaves

## 🚀 Próximos Passos

### Melhorias Futuras
- [ ] Edição inline de utilizadores
- [ ] Exportar lista de utilizadores (CSV/Excel)
- [ ] Logs de auditoria de ações admin
- [ ] Gestão de permissões granulares
- [ ] Dashboard com gráficos (Chart.js)
- [ ] Notificações em tempo real (SignalR)
- [ ] Temas claro/escuro
- [ ] Pesquisa avançada com múltiplos filtros

## 📝 Notas Importantes

### Produção
⚠️ **Em produção:**
1. Altere a password do admin padrão
2. Configure HTTPS
3. Adicione rate limiting
4. Configure CORS adequadamente
5. Use secrets manager para JWT keys

### Desenvolvimento
- O módulo é **independente** - pode ser usado em outros projetos
- Segue padrões de **Clean Architecture**
- Totalmente **desacoplado** dos outros módulos
- Pronto para **escalar** com novas funcionalidades

## 🤝 Contribuir

Este módulo faz parte da arquitetura modular. Para adicionar novas funcionalidades:

1. Crie novas páginas em `Pages/`
2. Adicione rotas no sidebar (`_Layout.cshtml`)
3. Implemente funcionalidades JavaScript em `admin.js`
4. Comunique com a API existente ou crie novos endpoints

## 📚 Documentação Relacionada

- [Auth Module](../Modules/Auth/README.md)
- [Auth.UI Module](../Modules/Auth/Auth.UI/README.md)
- [Clean Architecture Guide](../../docs/Arquitecture-Build-Guide.md)

---

**Desenvolvido com ❤️ para facilitar a gestão de utilizadores e roles**
