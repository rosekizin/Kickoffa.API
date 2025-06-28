# 🔐 Sistema de Alteração Segura de Email

## Visão Geral

O sistema de alteração de email do Kickoffa.API implementa um processo seguro em duas etapas usando tokens de confirmação do ASP.NET Core Identity para garantir que o usuário realmente possui o novo endereço de email.

## 🔧 Como Funciona

### Processo de Duas Etapas

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   1. Solicitar  │───▶│  2. Confirmar   │───▶│  3. Concluído   │
│   Alteração     │    │  via Email      │    │                 │
└─────────────────┘    └─────────────────┘    └─────────────────┘
```

#### Etapa 1: Solicitação de Alteração
1. Usuário envia novo email via `POST /api/user/change-email/initiate`
2. Sistema valida se email não está em uso
3. `UserManager.GenerateChangeEmailTokenAsync()` gera token seguro
4. Email de confirmação enviado para o **novo endereço**
5. Token armazenado temporariamente pelo Identity

#### Etapa 2: Confirmação
1. Usuário clica no link do email ou insere token manualmente
2. `POST /api/user/change-email/confirm` com token
3. `UserManager.ChangeEmailAsync()` valida token e altera email
4. Email de notificação enviado para o **endereço antigo**
5. Sessão mantida (usuário não precisa fazer login novamente)

## 📡 Endpoints da API

### `POST /api/user/change-email/initiate`
Inicia o processo de alteração de email.

**Headers:**
```http
Authorization: Bearer <token>
Content-Type: application/json
```

**Request Body:**
```json
{
  "newEmail": "novo@example.com"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Email de confirmação enviado com sucesso",
  "emailSentTo": "novo@example.com"
}
```

**Response (400 Bad Request):**
```json
{
  "success": false,
  "message": "Este email já está sendo usado por outro usuário"
}
```

### `POST /api/user/change-email/confirm`
Confirma a alteração usando o token recebido por email.

**Headers:**
```http
Authorization: Bearer <token>
Content-Type: application/json
```

**Request Body:**
```json
{
  "newEmail": "novo@example.com",
  "confirmationToken": "CfDJ8Abc123..."
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Email alterado com sucesso"
}
```

**Response (400 Bad Request):**
```json
{
  "success": false,
  "message": "Token de confirmação inválido ou expirado"
}
```

### `GET /api/user/check-email-availability`
Verifica se um email está disponível.

**Query Parameters:**
- `email` (string): Email a ser verificado

**Response:**
```json
{
  "available": true
}
```

## 🛡️ Segurança Implementada

### Validações de Segurança

#### 1. Token Criptográfico
```csharp
// Geração segura pelo ASP.NET Core Identity
var token = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);
```

#### 2. Verificação de Duplicidade
```csharp
// Impede uso de email já existente
var existingUser = await _userManager.FindByEmailAsync(newEmail);
if (existingUser != null && existingUser.Id != user.Id)
    return IdentityResult.Failed("Email já está em uso");
```

#### 3. Validação Automática de Token
```csharp
// O ChangeEmailAsync valida o token internamente
var result = await _userManager.ChangeEmailAsync(user, newEmail, token);
// Se token inválido/expirado, retorna erro automaticamente
```

#### 4. Autenticação Obrigatória
```csharp
[Authorize] // Usuário deve estar logado
public async Task<ActionResult> InitiateEmailChange(...)
```

### Proteções Contra Ataques

| Tipo de Ataque | Proteção Implementada |
|----------------|----------------------|
| **Email Hijacking** | Token enviado apenas para novo email |
| **Token Replay** | Token expira automaticamente |
| **Brute Force** | Rate limiting (recomendado) |
| **CSRF** | Autenticação obrigatória |
| **Session Hijacking** | Validação de usuário autenticado |

## 🔧 Configuração

### Serviços Necessários

#### 1. UserService
```csharp
// Registrado em ApplicationServiceCollectionExtensions.cs
services.AddScoped<IUserService, UserService>();
```

#### 2. EmailService
```csharp
// Interface abstrata para diferentes provedores
services.AddScoped<IEmailService, EmailService>();
```

#### 3. Wrappers para Testabilidade
```csharp
services.AddScoped<IUserManagerWrapper, UserManagerWrapper>();
services.AddScoped<ISignInManagerWrapper, SignInManagerWrapper>();
```

### Configuração de Email

#### Implementação Atual (Mock)
```csharp
public class EmailService : IEmailService
{
    // TODO: Implementar com provedor real (SendGrid, AWS SES, etc.)
    public async Task<bool> SendEmailChangeConfirmationAsync(...)
    {
        _logger.LogInformation("Email enviado para {Email}", email);
        // Simula envio para desenvolvimento
        return true;
    }
}
```

#### Implementação Recomendada (Produção)
```csharp
// Usar SendGrid, AWS SES, ou similar
public class SendGridEmailService : IEmailService
{
    private readonly ISendGridClient _sendGridClient;
    
    public async Task<bool> SendEmailChangeConfirmationAsync(...)
    {
        var msg = new SendGridMessage()
        {
            From = new EmailAddress("noreply@kickoffa.com"),
            Subject = "Confirme sua alteração de email",
            HtmlContent = GenerateEmailTemplate(confirmationToken)
        };
        
        var response = await _sendGridClient.SendEmailAsync(msg);
        return response.IsSuccessStatusCode;
    }
}
```

## 🚀 Vantagens da Implementação

### ✅ Segurança
- **Token criptográfico** gerado pelo framework
- **Validação automática** de expiração
- **Verificação de posse** do novo email
- **Notificação** para email antigo

### ✅ Usabilidade
- **Processo claro** em duas etapas
- **Sessão mantida** após alteração
- **Feedback imediato** sobre disponibilidade
- **Mensagens descritivas** de erro

### ✅ Manutenibilidade
- **Framework padrão** do .NET
- **Código testável** com wrappers
- **Logs detalhados** para auditoria
- **Interface abstrata** para email

### ✅ Escalabilidade
- **Assíncrono** e não bloqueante
- **Stateless** (token no email)
- **Configurável** por ambiente
- **Extensível** para outros provedores

## 📝 Notas Importantes

### ⚠️ Considerações de Produção

1. **Provedor de Email Real**
   ```csharp
   // Substituir EmailService mock por implementação real
   services.AddScoped<IEmailService, SendGridEmailService>();
   ```

2. **Rate Limiting**
   ```csharp
   // Implementar para prevenir spam
   [EnableRateLimiting("EmailChangePolicy")]
   public async Task<ActionResult> InitiateEmailChange(...)
   ```

3. **Templates de Email**
   ```html
   <!-- Criar templates profissionais -->
   <h1>Confirme sua alteração de email</h1>
   <a href="https://app.kickoffa.com/confirm-email?token={{token}}">
     Confirmar Alteração
   </a>
   ```

4. **Monitoramento**
   ```csharp
   // Logs para auditoria
   _logger.LogInformation("Email alterado: {UserId} {OldEmail} -> {NewEmail}", 
       userId, oldEmail, newEmail);
   ```

### 🔍 Troubleshooting

#### Problema: Token Inválido
```
Erro: "Token de confirmação inválido ou expirado"
```
**Soluções:**
- Verificar se token não foi modificado
- Confirmar que email corresponde ao token
- Verificar expiração (padrão: 24h)

#### Problema: Email Não Enviado
```
Erro: "Erro ao enviar email de confirmação"
```
**Soluções:**
- Verificar configuração do provedor de email
- Validar credenciais de SMTP/API
- Verificar logs do EmailService

#### Problema: Email Já Existe
```
Erro: "Este email já está sendo usado por outro usuário"
```
**Soluções:**
- Verificar se usuário não está tentando usar próprio email
- Confirmar unicidade no banco de dados
- Validar normalização de email

### 🧪 Testando o Sistema

#### Teste Manual
1. Login no sistema
2. POST `/api/user/change-email/initiate` com novo email
3. Verificar logs para token gerado
4. POST `/api/user/change-email/confirm` com token
5. Verificar alteração no banco de dados

#### Teste Automatizado
```csharp
[Fact]
public async Task InitiateEmailChange_WithValidEmail_ShouldSendConfirmation()
{
    // Arrange
    var newEmail = "new@example.com";
    
    // Act
    var result = await _userService.InitiateEmailChangeAsync(userId, newEmail);
    
    // Assert
    Assert.True(result.Succeeded);
    await _emailService.Received(1).SendEmailChangeConfirmationAsync(newEmail, Arg.Any<string>(), Arg.Any<string>());
}
```

---

📧 **Próximos Passos**: Implementar provedor de email real e templates profissionais para produção.
