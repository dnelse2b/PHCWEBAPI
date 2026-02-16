# 🎨 Personalização do Tema Admin UI

## 📍 Localização das Configurações

Todas as configurações visuais estão centralizadas em:
```
src/SGOFAPI.Host/appsettings.json
```

Secção: `"AdminUI"`

---

## 🖼️ Alterar o Logo

### Opção 1: URL Externa (Recomendado)
```json
"AdminUI": {
  "LogoUrl": "https://www.2business-si.com/wp-content/uploads/2024/12/2b_logo_ca_03.svg",
  "LogoAlt": "2Business Solutions",
  "LogoWidth": 180,
  "LogoHeight": 50
}
```

### Opção 2: Logo Local
1. Coloque a imagem em `src/Admin.UI/wwwroot/images/logo.svg`
2. Configure:
```json
"LogoUrl": "/images/logo.svg"
```

### Propriedades do Logo
- **LogoUrl**: Caminho completo (URL ou `/caminho/local`)
- **LogoAlt**: Texto alternativo para acessibilidade
- **LogoWidth**: Largura máxima em pixels
- **LogoHeight**: Altura máxima em pixels

---

## 🎨 Alterar Cores do Tema

### Cores Disponíveis

```json
"Colors": {
  "Primary": "#1a73e8",           // Cor principal (botões, links)
  "PrimaryHover": "#1557b0",      // Cor ao passar o mouse
  "PrimaryLight": "#e8f0fe",      // Fundo dos elementos ativos
  "Success": "#1e8e3e",           // Mensagens de sucesso
  "Error": "#d93025",             // Mensagens de erro
  "Warning": "#f9ab00",           // Mensagens de aviso
  "Info": "#1a73e8",              // Mensagens informativas
  "TextPrimary": "#202124",       // Texto principal
  "TextSecondary": "#5f6368",     // Texto secundário
  "SidebarBackground": "#ffffff", // Fundo da sidebar
  "SidebarText": "#5f6368",       // Texto da sidebar
  "SidebarActive": "#e8f0fe",     // Fundo do item ativo
  "SidebarActiveText": "#1a73e8"  // Texto do item ativo
}
```

---

## 🎯 Exemplos de Temas Prontos

### Tema Azul (Google - Padrão)
```json
"Primary": "#1a73e8",
"PrimaryHover": "#1557b0",
"PrimaryLight": "#e8f0fe"
```

### Tema Verde (Sucesso)
```json
"Primary": "#1e8e3e",
"PrimaryHover": "#188038",
"PrimaryLight": "#e6f4ea"
```

### Tema Roxo (Moderno)
```json
"Primary": "#9334e9",
"PrimaryHover": "#7c3aed",
"PrimaryLight": "#f3e8ff"
```

### Tema Laranja (Energético)
```json
"Primary": "#ea580c",
"PrimaryHover": "#c2410c",
"PrimaryLight": "#ffedd5"
```

### Tema Vermelho (Corporativo)
```json
"Primary": "#dc2626",
"PrimaryHover": "#b91c1c",
"PrimaryLight": "#fee2e2"
```

### Tema Escuro (Profissional)
```json
"Primary": "#0ea5e9",
"PrimaryHover": "#0284c7",
"PrimaryLight": "#e0f2fe",
"SidebarBackground": "#1e293b",
"SidebarText": "#cbd5e1",
"TextPrimary": "#f8fafc",
"TextSecondary": "#cbd5e1"
```

---

## ⚙️ Como Aplicar Alterações

### 1. Editar Configurações
Abra `src/SGOFAPI.Host/appsettings.json` e modifique a secção `AdminUI`

### 2. Reiniciar Aplicação
```powershell
# Parar a aplicação (Ctrl+C se estiver em modo watch)
# Depois executar:
dotnet run --project src/SGOFAPI.Host
```

### 3. Limpar Cache do Browser
- **Chrome/Edge**: `Ctrl + Shift + Delete` → Limpar cache
- **Firefox**: `Ctrl + Shift + Delete` → Limpar cache
- Ou abrir em modo anónimo/incógnito

---

## 🔧 Validação de Cores

### Formato Aceite
- Hexadecimal: `#1a73e8` ✅
- RGB: `rgb(26, 115, 232)` ✅
- RGBA: `rgba(26, 115, 232, 0.9)` ✅

### Ferramentas Úteis
- [Coolors.co](https://coolors.co/) - Gerador de paletas
- [Material Palette](https://www.materialpalette.com/) - Paletas Material Design
- [Adobe Color](https://color.adobe.com/) - Criar harmonia de cores

---

## 📱 Acessibilidade

### Contraste Mínimo Recomendado
- **Texto Normal**: Ratio 4.5:1
- **Texto Grande**: Ratio 3:1
- **UI Elements**: Ratio 3:1

### Testar Contraste
[WebAIM Contrast Checker](https://webaim.org/resources/contrastchecker/)

---

## 🚀 Configurações Avançadas

### Logo Responsivo
Configure larguras diferentes para desktop e mobile editando `_Layout.cshtml`:
```razor
<img src="@settings.LogoUrl" 
     alt="@settings.LogoAlt"
     class="hidden md:block object-contain"
     style="max-width: @(settings.LogoWidth)px; max-height: @(settings.LogoHeight)px;" />
<img src="@settings.LogoUrl" 
     alt="@settings.LogoAlt"
     class="md:hidden object-contain"
     style="max-width: 120px; max-height: 40px;" />
```

### CSS Variables Personalizadas
Todas as cores estão disponíveis como CSS variables em `_Layout.cshtml`:
```css
:root {
    --color-primary: #1a73e8;
    --color-primary-hover: #1557b0;
    /* etc... */
}
```

Pode criar componentes personalizados usando:
```css
.meu-botao {
    background: var(--color-primary);
    color: white;
}
```

---

## 📝 Ambiente de Desenvolvimento

Para testes rápidos sem reiniciar, pode editar temporariamente as cores em:
```
src/Admin.UI/Pages/_Layout.cshtml
```

Mas lembre-se: alterações manuais no `_Layout.cshtml` serão substituídas pelas do `appsettings.json`.

---

## ⚠️ Troubleshooting

### As cores não mudaram
1. Confirme que editou `appsettings.json` correto (em `SGOFAPI.Host`, não na raiz)
2. Reinicie a aplicação completamente
3. Limpe o cache do browser
4. Verifique se não há erros na consola do browser

### Logo não aparece
1. Verifique se o URL está correto e acessível
2. Se for logo local, confirme o caminho em `wwwroot`
3. Verifique a consola do browser para erros 404

### Layout quebrado
1. Verifique se todos os valores em `appsettings.json` têm formato válido
2. Confirme que não removeu propriedades obrigatórias
3. Compare com o exemplo padrão neste documento

---

## 📞 Suporte

Para questões técnicas ou customizações avançadas:
- Documentação completa: `docs/`
- Frontend structure: `src/Admin.UI/FRONTEND-STRUCTURE.md`
