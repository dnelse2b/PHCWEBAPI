# Admin.UI - Preview Visual

## 🎨 Como Fica o Design

### Dashboard Principal (`/admin`)
```
┌─────────────────────────────────────────────────────────────────┐
│  SIDEBAR              │  ADMIN PANEL - Dashboard                │
│  ┌─────────┐         │  ┌──────┐ ┌──────┐ ┌──────┐ ┌──────┐   │
│  │ 👑 Admin│         │  │ 👥   │ │ ✓    │ │ 🛡️   │ │ 🖥️   │   │
│  │  Panel  │         │  │ 150  │ │ 105  │ │  4   │ │Active│   │
│  └─────────┘         │  │Users │ │Active│ │Roles │ │ 99.9%│   │
│                      │  └──────┘ └──────┘ └──────┘ └──────┘   │
│  🏠 Dashboard        │                                         │
│  👥 Utilizadores     │  ┌─────────────────────────────────┐   │
│  ➕ Criar User       │  │ Criar Utilizador                │   │
│  🛡️ Roles            │  │ Adicione novos users...         │   │
│  ⚙️  Settings         │  │ [Criar Agora →]                 │   │
│  🚪 Sair             │  └─────────────────────────────────┘   │
│                      │                                         │
│  ┌──────────────┐   │  ┌─────────────────────────────────┐   │
│  │ 👤 admin     │   │  │ Utilizadores Recentes           │   │
│  │ Administrator│   │  │ • João Silva (online)           │   │
│  └──────────────┘   │  │ • Maria Costa (2h ago)          │   │
└─────────────────────┴─────────────────────────────────────────┘
```

### Lista de Utilizadores (`/admin/users`)
```
┌─────────────────────────────────────────────────────────────────┐
│  Gestão de Utilizadores                    [🔄] [➕ Novo User] │
├─────────────────────────────────────────────────────────────────┤
│  🔍 [Procurar utilizadores...]                                  │
│  Filtrar: [Todas Roles ▼] [Todos Estados ▼] [✖ Limpar]        │
├─────────────────────────────────────────────────────────────────┤
│  Utilizador         │ Email              │ Roles      │ Ações   │
├─────────────────────┼────────────────────┼────────────┼─────────┤
│  👤 admin           │ admin@phcapi.local │ 👑 Admin   │ 👁️ 🛡️ ✏️ 🗑️│
│  ID: abc123...      │ ✅ Confirmado      │            │         │
├─────────────────────┼────────────────────┼────────────┼─────────┤
│  👤 joao.silva      │ joao@emp.pt        │ 👔 User    │ 👁️ 🛡️ ✏️ 🗑️│
│  ID: def456...      │ ⚠️ Pendente        │            │         │
└─────────────────────────────────────────────────────────────────┘
```

### Criar Utilizador (`/admin/users/create`)
```
┌─────────────────────────────────────────────────────────────────┐
│  ← Voltar                                                       │
│  ┌────────────────────────────────────────────────────────────┐│
│  │ 👤  Novo Utilizador                                        ││
│  │     Preencha os dados abaixo                              ││
│  ├────────────────────────────────────────────────────────────┤│
│  │                                                            ││
│  │  Nome de Utilizador *                                     ││
│  │  👤 [joao.silva________________]                          ││
│  │  ℹ️ 3-20 caracteres alfanuméricos                         ││
│  │                                                            ││
│  │  Email *                                                  ││
│  │  ✉️ [joao.silva@empresa.pt_____]                          ││
│  │                                                            ││
│  │  Password *                                               ││
│  │  🔒 [••••••••••••••••••••••____] 👁️                      ││
│  │  ○ Mínimo 6 caracteres                                    ││
│  │  ✓ Uma letra maiúscula                                    ││
│  │  ✓ Uma letra minúscula                                    ││
│  │  ✓ Um número                                              ││
│  │                                                            ││
│  │  Roles (opcional)                                         ││
│  │  ☐ Administrator - Acesso total                          ││
│  │  ☑ InternalUser - Utilizador interno                     ││
│  │  ☐ ApiUser - Acesso via API                              ││
│  │                                                            ││
│  │  ☑ Email já confirmado                                    ││
│  │                                                            ││
│  │  [Cancelar]              [➕ Criar Utilizador]            ││
│  └────────────────────────────────────────────────────────────┘│
└─────────────────────────────────────────────────────────────────┘
```

### Gestão de Roles (`/admin/roles`)
```
┌─────────────────────────────────────────────────────────────────┐
│  Gestão de Roles                              [➕ Nova Role]    │
├─────────────────────────────────────────────────────────────────┤
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐         │
│  │ 👑           │  │ 👔           │  │ 💻           │         │
│  │ Administrator│  │ InternalUser │  │ ApiUser      │         │
│  │              │  │              │  │              │         │
│  │ Acesso total │  │ User interno │  │ Acesso API   │         │
│  │              │  │              │  │              │         │
│  │ 👥 3 users   │  │ 👥 45 users  │  │ 👥 12 users  │         │
│  │      [Ver →] │  │      [Ver →] │  │      [Ver →] │         │
│  └──────────────┘  └──────────────┘  └──────────────┘         │
│                                                                 │
│  ┌──────────────┐                                              │
│  │ 👁️           │                                              │
│  │ AuditViewer  │                                              │
│  │              │                                              │
│  │ Ver audits   │                                              │
│  │              │                                              │
│  │ 👥 8 users   │                                              │
│  │      [Ver →] │                                              │
│  └──────────────┘                                              │
└─────────────────────────────────────────────────────────────────┘
```

## 🎨 Paleta de Cores

### Gradientes Principais
- **Primary**: #667eea → #764ba2 (Roxo)
- **Success**: #48bb78 → #38a169 (Verde)
- **Error**: #f56565 → #e53e3e (Vermelho)
- **Warning**: #ed8936 → #dd6b20 (Laranja)

### Sidebar
- Background: Linear gradient (Gray-900 → Gray-800 → Gray-900)
- Hover: Gray-700 com transição suave
- Active: Primary gradient

### Cards
- Background: White
- Border: Gray-100
- Hover: Elevação com shadow
- Transition: 0.3s cubic-bezier

### Badges/Tags
- Administrator: Red-Pink gradient
- InternalUser: Blue-Cyan gradient
- ApiUser: Green-Emerald gradient
- AuditViewer: Yellow-Orange gradient

## ✨ Animações

### Page Load
- Fade in + Slide down (0.3s ease-out)

### Hover Effects
- Cards: Lift up 4px + shadow increase
- Buttons: Lift up 2px + shadow glow
- Sidebar links: Slide right 5px

### Transitions
- All interactive: 0.2s ease
- Modals: Fade in background + slide in content
- Toasts: Slide in from right

## 📱 Responsive

### Desktop (> 1024px)
- Sidebar fixo à esquerda (256px)
- Conteúdo com padding-left: 256px
- Grid de 4 colunas para stats

### Tablet (768px - 1024px)
- Sidebar mantém-se
- Grid de 2 colunas para stats
- Tabelas com scroll horizontal

### Mobile (< 768px)
- Sidebar oculto (hamburger menu)
- Grid de 1 coluna
- Touch-friendly buttons (44px min)
- Stack vertical de elementos

## 🎭 Estados Visuais

### Loading
- Skeleton screens com pulse animation
- Spinner inline em botões
- Overlay para ações críticas

### Empty States
- Ícone grande + mensagem
- Botão de ação quando aplicável
- Design amigável e informativo

### Error States
- Toast vermelho com ícone
- Mensagens inline em formulários
- Retry buttons quando apropriado

### Success States
- Toast verde com ícone
- Feedback imediato
- Redirect automático quando aplicável
