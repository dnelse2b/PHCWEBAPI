# Providers Module - Test Guide

## 📋 Resumo dos Endpoints

### 1. **POST /api/providers** - Criar Provider com todos os valores
**Authorization**: Bearer Token (ApiUser ou Administrator)
**Rate Limit**: parameters-create

### 2. **GET /api/providers** - Listar todos os providers
**Authorization**: Bearer Token (Administrator)
**Rate Limit**: parameters-query
**Query Parameters**:
- `includeInactive` (bool, default: false)
- `environment` (string, opcional: "Development", "Staging", "Production")

### 3. **GET /api/providers/{stamp}** - Obter provider por stamp
**Authorization**: Bearer Token (Administrator)
**Rate Limit**: parameters-query

### 4. **GET /api/providers/config/{provedor}/{operationCode}?environment=Development** - ⭐ Obter configuração específica
**Authorization**: Bearer Token (ApiUser, InternalUser, Administrator)
**Rate Limit**: parameters-query
**Path Parameters**:
- `provedor`: TFR, MPDC, etc.
- `operationCode`: AUTH, GET_ALL_CONSIGNMENTS, NOTIFYCONSIGMENTS, etc.
**Query Parameters**:
- `environment`: Development, Staging ou Production (default: Development)

### 5. **PUT /api/providers/{stamp}** - Atualizar provider (replace completo das linhas)
**Authorization**: Bearer Token (ApiUser ou Administrator)
**Rate Limit**: parameters-update

### 6. **DELETE /api/providers/{stamp}** - Eliminar provider
**Authorization**: Bearer Token (Administrator)
**Rate Limit**: parameters-delete

---

## 🚀 Testando no Postman

### Passo 1: Criar TFR Provider

**Endpoint**: `POST https://localhost:7298/api/providers`

**Headers**:
```
Authorization: Bearer {seu_token_jwt}
Content-Type: application/json
```

**Body**: Usar o conteúdo de `scripts/SeedProviders_TFR_Development.json`

Resultado esperado:
- Status: `201 Created`
- Response com `uProviderStamp` gerado

---

### Passo 2: Criar MPDC Provider

**Endpoint**: `POST https://localhost:7298/api/providers`

**Headers**:
```
Authorization: Bearer {seu_token_jwt}
Content-Type: application/json
```

**Body**: Usar o conteúdo de `scripts/SeedProviders_MPDC_Development.json`

Resultado esperado:
- Status: `201 Created`
- Response com `uProviderStamp` gerado

---

### Passo 3: Listar Todos os Providers

**Endpoint**: `GET https://localhost:7298/api/providers`

**Headers**:
```
Authorization: Bearer {seu_token_jwt}
```

Resultado esperado:
- Status: `200 OK`
- Array com TFR e MPDC

**Filtrar por Environment**:
```
GET https://localhost:7298/api/providers?environment=Development
```

---

### Passo 4: Obter Configuração de um Endpoint (⭐ Main Feature)

**Exemplo 1 - TFR AUTH**:
```
GET https://localhost:7298/api/providers/config/TFR/AUTH?environment=Development
```

Resposta esperada:
```json
{
  "success": true,
  "data": {
    "provedor": "TFR",
    "operationCode": "AUTH",
    "environment": "Development",
    "properties": {
      "Method": "POST",
      "Url": "http://online.transnetfreightrail.co.za/api/transnetOnlineAPI/authentication/portalauth",
      "Port": "8080",
      "ContentType": "application/json",
      "Username": "Sap3008",
      "Password": "Cfmapi2022#"  // ✅ Automaticamente desencriptado!
    }
  }
}
```

**Exemplo 2 - MPDC AUTH**:
```
GET https://localhost:7298/api/providers/config/MPDC/AUTH?environment=Development
```

**Exemplo 3 - MPDC NOTIFYCONSIGMENTS**:
```
GET https://localhost:7298/api/providers/config/MPDC/NOTIFYCONSIGMENTS?environment=Development
```

---

### Passo 5: Atualizar Provider (Adicionar novos endpoints ou modificar existentes)

**Endpoint**: `PUT https://localhost:7298/api/providers/{uProviderStamp}`

**Body**:
```json
{
  "codigo": 1,
  "provedor": "TFR",
  "environment": "Development",
  "descricao": "TFR - Development - Updated",
  "ativo": true,
  "values": [
    // ⚠️ REPLACE COMPLETO: Adicionar TODAS as linhas antigas + novas
    {
      "operationCode": "AUTH",
      "chave": "Method",
      "valor": "POST",
      "encriptado": false,
      "ordem": 1,
      "ativo": true
    },
    {
      "operationCode": "NOVO_ENDPOINT",
      "chave": "Url",
      "valor": "https://api.example.com/novo",
      "encriptado": false,
      "ordem": 100,
      "ativo": true
    }
  ]
}
```

⚠️ **Importante**: No UPDATE, as linhas antigas são ELIMINADAS e substituídas pelas novas. Envie TODAS as linhas que quer manter!

---

### Passo 6: Eliminar Provider

**Endpoint**: `DELETE https://localhost:7298/api/providers/{uProviderStamp}`

**Headers**:
```
Authorization: Bearer {seu_token_jwt}
```

Resultado esperado:
- Status: `200 OK`
- Cascade delete: Provider + todas as linhas (ProviderValues)

---

## 🔐 Encriptação

Valores marcados com `"encriptado": true` são automaticamente:
- **Encriptados** ao criar/atualizar (usando AES com chave do appsettings)
- **Desencriptados** ao consultar via `/config/` endpoint

**Exemplos de campos para encriptar**:
- Passwords
- API Keys
- Tokens sensíveis
- Certificados

---

## 🎯 Códigos de Providers Definidos

| Código | Provider | Descrição |
|--------|----------|-----------|
| 1      | TFR      | Transnet Freight Rail |
| 2      | MPDC     | Maputo Port Development Company |

---

## 📝 Response Codes do Módulo

| Código | Mensagem |
|--------|----------|
| 1101   | Provider not found |
| 1102   | Provider configuration not found |
| 1103   | Provider already exists for this environment |
| 1104   | Provider created successfully |
| 1105   | Provider updated successfully |
| 1106   | Provider deleted successfully |

---

## ✅ Validação Automática (FluentValidation)

Campos obrigatórios validados antes de chegar ao handler:
- ✅ Provedor não vazio (max 50 chars)
- ✅ Environment deve ser: "Development", "Staging" ou "Production"
- ✅ Descrição obrigatória (max 200 chars)
- ✅ Cada Value deve ter: OperationCode, Chave, Valor preenchidos
- ✅ Ordem ≥ 0

Erro de validação retorna `400 Bad Request` com detalhes dos erros.

---

## 🗄️ Structured Logging

Todas as operações são auditadas com:
- Timestamp da operação
- Usuário que executou (CriadoPor/AtualizadoPor)
- Correlation ID para rastreamento
- Campos audit: DataCriacao, DataActualizacao, Oculto

---

## 🔄 Próximos Passos

1. ✅ Executar script SQL: `scripts/CreateExternalApiProviderTables.sql`
2. ✅ Testar criação dos 2 providers (TFR e MPDC)
3. ✅ Testar endpoint `/config/` para obter configurações
4. ✅ Validar encriptação/desencriptação de passwords
5. 🔜 Criar UI de administração (Admin.UI)
6. 🔜 Migrar código existente para usar este módulo
