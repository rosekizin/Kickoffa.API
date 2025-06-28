namespace Kickoffa.API.Application.Services.Email;

/// <summary>
/// Interface para serviços de envio de email
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Envia email de confirmação de alteração de email
    /// </summary>
    /// <param name="email">Email de destino</param>
    /// <param name="userName">Nome do usuário (pode ser email atual)</param>
    /// <param name="confirmationToken">Token de confirmação</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>True se enviado com sucesso</returns>
    Task<bool> SendEmailChangeConfirmationAsync(string email, string userName, string confirmationToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Envia email de notificação de alteração de email concluída
    /// </summary>
    /// <param name="oldEmail">Email antigo</param>
    /// <param name="newEmail">Novo email</param>
    /// <param name="userName">Nome do usuário</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>True se enviado com sucesso</returns>
    Task<bool> SendEmailChangeNotificationAsync(string oldEmail, string newEmail, string userName, CancellationToken cancellationToken = default);
}
