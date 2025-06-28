using Microsoft.Extensions.Logging;

namespace Kickoffa.API.Application.Services.Email;

/// <summary>
/// Implementação básica do serviço de email (mock para desenvolvimento)
/// TODO: Implementar com provedor real (SendGrid, AWS SES, etc.)
/// </summary>
public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<bool> SendEmailChangeConfirmationAsync(string email, string userName, string confirmationToken, CancellationToken cancellationToken = default)
    {
        try
        {
            // TODO: Implementar envio real de email
            // Por enquanto, apenas log para desenvolvimento
            _logger.LogInformation(
                "📧 Email de confirmação de alteração enviado para {Email}. " +
                "Usuário: {UserName}, Token: {Token}", 
                email, userName, confirmationToken);

            // Simular delay de envio
            await Task.Delay(100, cancellationToken);

            // TODO: Remover este log em produção (token sensível)
            _logger.LogWarning(
                "🔗 DESENVOLVIMENTO: Link de confirmação seria: " +
                "https://app.kickoffa.com/confirm-email-change?token={Token}&email={Email}", 
                confirmationToken, email);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao enviar email de confirmação para {Email}", email);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<bool> SendEmailChangeNotificationAsync(string oldEmail, string newEmail, string userName, CancellationToken cancellationToken = default)
    {
        try
        {
            // TODO: Implementar envio real de email
            _logger.LogInformation(
                "📧 Notificação de alteração de email enviada. " +
                "Email antigo: {OldEmail}, Novo email: {NewEmail}, Usuário: {UserName}", 
                oldEmail, newEmail, userName);

            // Simular delay de envio
            await Task.Delay(100, cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao enviar notificação de alteração de email para {OldEmail}", oldEmail);
            return false;
        }
    }
}
