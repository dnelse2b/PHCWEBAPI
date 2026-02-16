# 🚀 GUIA RÁPIDO: Testar Personalização do Tema

## ✅ Passo a Passo para Alterar o Tema

### 1️⃣ Abrir Configuração
```
📁 src/SGOFAPI.Host/appsettings.json
```

### 2️⃣ Escolher um Tema (exemplo: Tema Verde)

Encontre a secção `"AdminUI"` → `"Colors"` e altere para:

```json
"Colors": {
  "Primary": "#1e8e3e",
  "PrimaryHover": "#188038",
  "PrimaryLight": "#e6f4ea",
  "Success": "#1e8e3e",
  "Error": "#d93025",
  "Warning": "#f9ab00",
  "Info": "#1e8e3e",
  "TextPrimary": "#202124",
  "TextSecondary": "#5f6368",
  "SidebarBackground": "#ffffff",
  "SidebarText": "#5f6368",
  "SidebarActive": "#e6f4ea",
  "SidebarActiveText": "#1e8e3e"
}
```

### 3️⃣ Executar a Aplicação

```powershell
cd c:\Users\dbarreto\source\repos\PHCWEBAPI
dotnet run --project src/SGOFAPI.Host
```

### 4️⃣ Abrir no Browser

```
https://localhost:[porta]/Admin/Account/Login
```

**Credenciais padrão:**
- Username: `admin@phcapi.com`
- Password: `Admin@123`

### 5️⃣ Verificar Alterações

Verá:
- ✅ Logo da 2Business no topo da sidebar
- ✅ Botões com a nova cor (verde neste exemplo)
- ✅ Links ativos com a nova cor
- ✅ Elementos interativos com as cores configuradas

---

## 🎨 Testar Outros Temas Rapidamente

### Tema Roxo (Moderno)
```json
"Primary": "#9334e9",
"PrimaryHover": "#7c3aed",
"PrimaryLight": "#f3e8ff",
"SidebarActive": "#f3e8ff",
"SidebarActiveText": "#9334e9"
```

### Tema Laranja (Energético)
```json
"Primary": "#ea580c",
"PrimaryHover": "#c2410c",
"PrimaryLight": "#ffedd5",
"SidebarActive": "#ffedd5",
"SidebarActiveText": "#ea580c"
```

### Tema Azul Escuro (Elegante)
```json
"Primary": "#1e40af",
"PrimaryHover": "#1e3a8a",
"PrimaryLight": "#dbeafe",
"SidebarActive": "#dbeafe",
"SidebarActiveText": "#1e40af"
```

---

## 🖼️ Testar Logo Diferente

### Logo Local (SVG ou PNG)

1. **Criar pasta wwwroot:**
```powershell
mkdir src/Admin.UI/wwwroot/images
```

2. **Copiar seu logo:**
```powershell
copy "C:\caminho\para\seu\logo.svg" src/Admin.UI/wwwroot/images/
```

3. **Configurar em appsettings.json:**
```json
"AdminUI": {
  "LogoUrl": "/images/logo.svg",
  "LogoAlt": "Minha Empresa",
  "LogoWidth": 200,
  "LogoHeight": 60
}
```

### Logo de URL Externa

```json
"LogoUrl": "https://exemplo.com/logo.png"
```

---

## ⚡ Dicas de Produtividade

### 1. Hot Reload (sem reiniciar)
```powershell
dotnet watch run --project src/SGOFAPI.Host
```
Agora quando alterar `appsettings.json`, a configuração recarrega automaticamente!

### 2. Cache do Browser
Se as cores não mudarem, limpe o cache:
- **Windows**: `Ctrl + Shift + Delete`
- **Ou abra em aba anónima**: `Ctrl + Shift + N` (Chrome/Edge)

### 3. Testar Múltiplos Temas
Crie ficheiros de configuração separados:
```powershell
# Tema verde
copy appsettings.json appsettings.Verde.json

# Tema roxo
copy appsettings.json appsettings.Roxo.json

# Depois execute com:
dotnet run --project src/SGOFAPI.Host --environment Verde
```

---

## 🐛 Troubleshooting Rápido

### Problema: Build falha
```powershell
# Limpar e rebuild
dotnet clean
dotnet build
```

### Problema: Logo não aparece
1. Verifique a consola do browser (F12)
2. Confirme o URL está acessível
3. Tente com logo padrão primeiro:
```json
"LogoUrl": "https://www.2business-si.com/wp-content/uploads/2024/12/2b_logo_ca_03.svg"
```

### Problema: Cores não mudam
1. Confirme que editou `src/SGOFAPI.Host/appsettings.json`
2. Reinicie completamente a aplicação (Ctrl+C e run novamente)
3. Limpe cache do browser

---

## 📊 Exemplo Visual de Mudança

**ANTES (Tema Azul Google):**
```
🔵 Botões azuis (#1a73e8)
🔵 Links azuis
🔵 Items ativos azul claro
```

**DEPOIS (Tema Verde):**
```
🟢 Botões verdes (#1e8e3e)
🟢 Links verdes
🟢 Items ativos verde claro
```

**Mudança necessária:** Apenas 3 linhas no `appsettings.json`! ✨

---

## 📝 Checklist de Teste

- [ ] Logo aparece no topo da sidebar
- [ ] Botões "Guardar", "Criar" têm a cor primária
- [ ] Hover nos botões mostra cor mais escura
- [ ] Links na sidebar ficam destacados ao clicar
- [ ] Alerts (success, error, warning) têm cores apropriadas
- [ ] Texto é legível (contraste adequado)

---

## 🎯 Próximo Nível

### Adicionar Logo no Login
Edite `src/Admin.UI/Pages/Account/Login.cshtml` linha ~30:
```razor
@inject IOptions<AdminUISettings> AdminSettings
@{
    var settings = AdminSettings.Value;
}

<!-- Na secção do logo -->
<img src="@settings.LogoUrl" alt="@settings.LogoAlt" class="mx-auto mb-6" style="max-width: 200px;" />
```

### Sidebar com Cor de Fundo Personalizada
Em `appsettings.json`:
```json
"SidebarBackground": "#1e293b",  // Azul escuro
"SidebarText": "#cbd5e1"         // Texto claro
```

---

## 💡 Recursos Adicionais

📚 Documentação completa: [THEME-CUSTOMIZATION.md](THEME-CUSTOMIZATION.md)
🎨 Gerador de paletas: https://coolors.co/
🔍 Teste de contraste: https://webaim.org/resources/contrastchecker/

---

**Tempo estimado:** 5 minutos ⏱️  
**Dificuldade:** ⭐ Fácil
