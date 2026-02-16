# Admin UI - Frontend Structure (Enterprise Level)

## 📁 Estrutura de Pastas

```
Admin.UI/
├── Pages/
│   ├── Shared/                          # ✅ Componentes Reutilizáveis
│   │   ├── _ValidationScriptsPartial.cshtml    # Validação client-side
│   │   ├── _Alert.cshtml                       # Alertas/Notificações
│   │   ├── _LoadingSpinner.cshtml              # Loading states
│   │   ├── _EmptyState.cshtml                  # Estados vazios
│   │   ├── _FormInput.cshtml                   # Input component
│   │   └── _Breadcrumb.cshtml                  # Navegação breadcrumb
│   ├── _Layout.cshtml                   # Layout principal
│   ├── _ViewImports.cshtml              # Imports globais
│   ├── _ViewStart.cshtml                # Configuração inicial
│   ├── Account/                         # Módulo de autenticação
│   │   ├── Login.cshtml
│   │   ├── Logout.cshtml
│   │   └── AccessDenied.cshtml
│   ├── Users/                           # Módulo de utilizadores
│   │   ├── Index.cshtml
│   │   ├── Index.cshtml.cs
│   │   ├── Create.cshtml
│   │   └── Create.cshtml.cs
│   └── Roles/                           # Módulo de roles
│       └── Index.cshtml
├── wwwroot/                             # Assets estáticos
│   ├── css/                             # (future: custom styles)
│   └── js/                              # (future: custom scripts)
└── DependencyInjection.cs               # Configuração do módulo
```

## 🎨 Design System

### Cores (Tailwind CSS)
- **Primary:** `#667eea` (Purple-blue)
- **Secondary:** `#764ba2` (Purple)
- **Accent:** `#f093fb` (Pink)

### Componentes Base

#### 1. **Alerts** (`_Alert.cshtml`)
```csharp
// Usage em Razor Pages
@{
    ViewData["Type"] = "success"; // success, error, warning, info
    ViewData["Message"] = "Operação realizada com sucesso!";
}
<partial name="_Alert" />
```

#### 2. **Loading Spinner** (`_LoadingSpinner.cshtml`)
```csharp
@{
    ViewData["Message"] = "A processar...";
}
<partial name="_LoadingSpinner" />
```

#### 3. **Empty State** (`_EmptyState.cshtml`)
```csharp
@{
    ViewData["Icon"] = "fa-users";
    ViewData["Title"] = "Nenhum utilizador encontrado";
    ViewData["Message"] = "Comece por criar o primeiro utilizador.";
    ViewData["ActionUrl"] = "/Admin/Users/Create";
    ViewData["ActionText"] = "Criar Utilizador";
}
<partial name="_EmptyState" />
```

#### 4. **Form Input** (`_FormInput.cshtml`)
```csharp
@{
    ViewData["Id"] = "username";
    ViewData["Name"] = "username";
    ViewData["Label"] = "Nome de Utilizador";
    ViewData["Type"] = "text";
    ViewData["Icon"] = "fa-user";
    ViewData["Required"] = true;
    ViewData["Placeholder"] = "joao.silva";
    ViewData["HelpText"] = "3-20 caracteres alfanuméricos";
}
<partial name="_FormInput" />
```

#### 5. **Breadcrumb** (`_Breadcrumb.cshtml`)
```csharp
@{
    ViewData["Items"] = new List<(string Text, string? Url)>
    {
        ("Utilizadores", "/Admin/Users"),
        ("Criar", null)
    };
}
<partial name="_Breadcrumb" />
```

## 🔧 Validação de Formulários

### Server-Side (Razor Pages)
```csharp
[Required(ErrorMessage = "Campo obrigatório")]
[StringLength(20, MinimumLength = 3, ErrorMessage = "Entre 3 e 20 caracteres")]
public string UserName { get; set; } = string.Empty;
```

### Client-Side (jQuery Validation)
```html
@section Scripts {
    <partial name="_ValidationScriptsPartial" />
}
```

**Features:**
- ✅ Validação em tempo real
- ✅ Mensagens em Português
- ✅ Integração com Tailwind CSS
- ✅ Highlight automático de erros

## 📐 Layout Pattern

### Page Structure
```html
@page "/Admin/Module/Action"
@model NamespaceModel
@{
    ViewData["Title"] = "Título da Página";
    ViewData["Subtitle"] = "Descrição";
}

<!-- Alert Messages -->
@if (Model.SuccessMessage != null)
{
    @{
        ViewData["Type"] = "success";
        ViewData["Message"] = Model.SuccessMessage;
    }
    <partial name="_Alert" />
}

<!-- Breadcrumb -->
@{
    ViewData["Items"] = new List<(string, string?)> { ... };
}
<partial name="_Breadcrumb" />

<!-- Main Content -->
<div class="max-w-7xl mx-auto">
    <!-- Content here -->
</div>
```

## 🎯 Best Practices

### 1. **Separation of Concerns**
- Backend: `.cshtml.cs` (PageModel)
- Frontend: `.cshtml` (View)
- Components: `Shared/*.cshtml`

### 2. **Component Reusability**
- Use partial views para componentes repetitivos
- ViewData para passar parâmetros
- Nomenclatura clara e consistente

### 3. **Responsive Design**
- Tailwind CSS utility classes
- Mobile-first approach
- Breakpoints: sm, md, lg, xl

### 4. **Accessibility**
- Semantic HTML
- ARIA labels
- Keyboard navigation
- Screen reader support

### 5. **Performance**
- Server-side rendering (SSR)
- Minimal JavaScript
- CDN para libraries externas
- Lazy loading quando necessário

## 🔐 Security

### CSRF Protection
```html
<form method="post" asp-antiforgery="true">
    <!-- Automatically includes CSRF token -->
</form>
```

### Input Validation
- Server-side: Data Annotations
- Client-side: jQuery Validation
- XSS Protection: Razor automatic encoding

### Authorization
```csharp
[Authorize(Roles = "Administrator")]
public class PageModel : PageModel { }
```

## 📊 State Management

### TempData (Redirect Messages)
```csharp
[TempData]
public string? SuccessMessage { get; set; }

// Set
SuccessMessage = "Utilizador criado com sucesso!";
return RedirectToPage("./Index");

// Display (automatic via _Alert partial)
```

### ViewData (Page-specific)
```csharp
ViewData["Title"] = "Página Título";
ViewData["CustomData"] = someValue;
```

### Model Binding (Forms)
```csharp
[BindProperty]
public InputModel Input { get; set; } = new();
```

## 🎨 UI Components Library

### Buttons
```html
<!-- Primary -->
<button class="px-6 py-3 btn-primary text-white rounded-lg font-medium hover:shadow-lg transition">
    <i class="fas fa-save mr-2"></i>Guardar
</button>

<!-- Secondary -->
<button class="px-6 py-3 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition">
    Cancelar
</button>
```

### Cards
```html
<div class="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
    <div class="p-6">
        <!-- Content -->
    </div>
</div>
```

### Badges
```html
<span class="px-2 py-1 text-xs font-medium rounded-full bg-green-100 text-green-700">
    <i class="fas fa-check-circle"></i> Ativo
</span>
```

### Tables
```html
<table class="min-w-full divide-y divide-gray-200">
    <thead class="bg-gray-50">
        <tr>
            <th class="px-6 py-4 text-left text-xs font-bold text-gray-700 uppercase">
                Header
            </th>
        </tr>
    </thead>
    <tbody class="bg-white divide-y divide-gray-200">
        <tr class="hover:bg-gray-50 transition">
            <td class="px-6 py-4 whitespace-nowrap">Data</td>
        </tr>
    </tbody>
</table>
```

## 🚀 Future Enhancements

### Phase 2
- [ ] Pagination component
- [ ] Search/Filter component
- [ ] Modal dialog component
- [ ] Toast notifications (JS)
- [ ] Data tables with sorting

### Phase 3
- [ ] Dark mode support
- [ ] Internationalization (i18n)
- [ ] Custom CSS build (Tailwind)
- [ ] Performance monitoring
- [ ] Analytics integration

## 📚 Dependencies

### CDN (Production Ready)
- **Tailwind CSS:** v3.x (via CDN)
- **Font Awesome:** v6.5.1
- **jQuery:** v3.7.1
- **jQuery Validation:** v1.21.0

### Future: NPM/Webpack
```json
{
  "dependencies": {
    "tailwindcss": "^3.4.0",
    "@fortawesome/fontawesome-free": "^6.5.0",
    "jquery": "^3.7.1",
    "jquery-validation": "^1.21.0"
  }
}
```

## 🧪 Testing Strategy

### Unit Tests (Backend)
- PageModel logic
- Validation rules
- Business logic

### Integration Tests
- Form submissions
- Authentication flow
- Authorization checks

### E2E Tests (Future)
- Selenium/Playwright
- User workflows
- Accessibility testing

---

**Última Atualização:** 15/02/2026  
**Nível:** Enterprise Senior  
**Autor:** GitHub Copilot
