# 🔐 Autenticação com ASP.NET Core Identity

## Visão Geral

O sistema de autenticação do Kickoffa.API utiliza **ASP.NET Core Identity** com **cookies HttpOnly** para máxima segurança e simplicidade.

## 🍪 Como Funciona

### Autenticação por Cookies
- **Sem JWT customizado**: O Identity gerencia tokens internamente
- **Cookies HttpOnly**: Proteção contra ataques XSS
- **Secure & SameSite**: Proteção contra CSRF
- **Sliding Expiration**: Sessão renovada automaticamente

### Fluxo de Autenticação

```
1. POST /api/auth/login
   ├── Valida credenciais
   ├── SignInManager.PasswordSignInAsync()
   ├── Cookie automático criado
   └── Retorna dados do usuário

2. Requisições subsequentes
   ├── Cookie enviado automaticamente
   ├── Identity valida automaticamente
   └── [Authorize] funciona

3. POST /api/auth/logout
   ├── SignInManager.SignOutAsync()
   └── Cookie removido automaticamente
```

## 📡 Endpoints

### `POST /api/auth/login`
Autentica o usuário e cria sessão.

**Request:**
```json
{
  "email": "user@example.com",
  "password": "password123",
  "rememberMe": false
}
```

**Response:**
```json
{
  "userId": "123",
  "email": "user@example.com",
  "success": true,
  "message": "Login realizado com sucesso",
  "loginAt": "2024-01-01T10:00:00Z",
  "session": {
    "expiresInSeconds": 3600,
    "expiresAt": "2024-01-01T11:00:00Z",
    "isPersistent": false
  }
}
```

### `GET /api/auth/validate`
Valida se o usuário está autenticado.

**Response:**
```json
{
  "userId": "123",
  "email": "user@example.com",
  "isAuthenticated": true,
  "validatedAt": "2024-01-01T10:30:00Z",
  "sessionInfo": {
    "authenticationMethod": "Cookie"
  }
}
```

### `GET /api/auth/me`
Obtém dados completos do usuário atual.

**Response:**
```json
{
  "id": 123,
  "email": "user@example.com",
  "userName": "user@example.com",
  "emailConfirmed": true,
  "createdAt": "2024-01-01T09:00:00Z",
  "lastUpdated": "2024-01-01T09:00:00Z"
}
```

### `POST /api/auth/logout`
Encerra a sessão do usuário.

**Response:**
```json
{
  "message": "Logout realizado com sucesso"
}
```

## 🔧 Configuração

### Cookies (ApplicationServiceCollectionExtensions.cs)
```csharp
services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/api/auth/login";
    options.LogoutPath = "/api/auth/logout";
    options.ExpireTimeSpan = TimeSpan.FromHours(1);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.Name = "KickoffaAuth";
});
```

### CORS (Program.cs)
```csharp
options.AddPolicy("AllowFrontend", policy =>
{
    policy.WithOrigins("http://localhost:3000")
          .AllowAnyHeader()
          .AllowAnyMethod()
          .AllowCredentials(); // ✅ Essencial para cookies
});
```

## 🛡️ Segurança

### Proteções Implementadas
- **HttpOnly**: Cookies não acessíveis via JavaScript
- **Secure**: Apenas HTTPS em produção
- **SameSite Strict**: Proteção CSRF
- **Sliding Expiration**: Sessão renovada com atividade
- **Lockout**: Bloqueio após tentativas falhadas

### Headers de Segurança
```http
Set-Cookie: KickoffaAuth=...; HttpOnly; Secure; SameSite=Strict
```

## 🔄 Frontend Integration

### Axios Configuration
```javascript
// Sempre enviar cookies
axios.defaults.withCredentials = true;

// Interceptor para tratar 401
axios.interceptors.response.use(
  response => response,
  error => {
    if (error.response?.status === 401) {
      // Redirecionar para login
      window.location.href = '/auth/login';
    }
    return Promise.reject(error);
  }
);
```

### Verificação de Autenticação
```javascript
const checkAuth = async () => {
  try {
    const response = await axios.get('/api/auth/validate');
    return response.data.isAuthenticated;
  } catch {
    return false;
  }
};
```

## 🚀 Vantagens da Abordagem

- **✅ Simplicidade**: Sem gerenciamento manual de tokens
- **✅ Segurança**: Framework testado e maduro
- **✅ Performance**: Cookies mais eficientes que JWT
- **✅ Recursos**: Lockout, 2FA, etc. prontos
- **✅ Manutenção**: Atualizações automáticas com .NET

## 🔍 Debugging

### Verificar Cookies no Browser
```javascript
// Console do navegador
document.cookie; // Não mostrará HttpOnly cookies (correto!)

// Network tab: verificar headers Set-Cookie
```

### Logs do Servidor
```csharp
// AuthController logs automáticos
_logger.LogInformation("Login realizado para usuário: {Email}", user.Email);
_logger.LogWarning("Tentativa de login inválida: {Email}", request.Email);
```

## 📝 Notas Importantes

1. **Sem JWT no Response**: Tokens gerenciados internamente
2. **Cookies Automáticos**: Enviados em todas as requisições
3. **CORS Credentials**: `withCredentials: true` obrigatório
4. **HTTPS Recomendado**: Para cookies Secure em produção
