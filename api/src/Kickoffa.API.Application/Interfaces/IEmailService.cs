namespace Kickoffa.API.Application.Interfaces;

/// <summary>
/// Interface para serviços de email
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Envia email de confirmação para alteração de email
    /// </summary>
    /// <param name="email">Email de destino</param>
    /// <param name="userName">Nome do usuário</param>
    /// <param name="confirmationToken">Token de confirmação</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>True se enviado com sucesso</returns>
    Task<bool> SendEmailChangeConfirmationAsync(string email, string userName, string confirmationToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Envia notificação de alteração de email para o email antigo
    /// </summary>
    /// <param name="oldEmail">Email antigo</param>
    /// <param name="newEmail">Novo email</param>
    /// <param name="userName">Nome do usuário</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>True se enviado com sucesso</returns>
    Task<bool> SendEmailChangeNotificationAsync(string oldEmail, string newEmail, string userName, CancellationToken cancellationToken = default);
}
