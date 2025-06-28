using Kickoffa.API.Application.Interfaces;
using Kickoffa.API.Domain.Models.AppUser;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Kickoffa.API.Application.Services.AppUser;

/// <summary>
/// Serviço para operações relacionadas a User usando ASP.NET Core Identity
/// </summary>
public sealed class UserService : IUserService
{
    private readonly IUserManagerWrapper _userManagerWrapper;
    private readonly ISignInManagerWrapper _signInManagerWrapper;
    private readonly IEmailService _emailService;
    private readonly ILogger<UserService> _logger;

    /// <summary>
    /// Inicializa uma nova instância do UserService
    /// </summary>
    /// <param name="userManagerWrapper">Wrapper do UserManager</param>
    /// <param name="signInManagerWrapper">Wrapper do SignInManager</param>
    /// <param name="emailService">Serviço de email</param>
    /// <param name="logger">Logger</param>
    public UserService(
        IUserManagerWrapper userManagerWrapper,
        ISignInManagerWrapper signInManagerWrapper,
        IEmailService emailService,
        ILogger<UserService> logger)
    {
        _userManagerWrapper = userManagerWrapper ?? throw new ArgumentNullException(nameof(userManagerWrapper));
        _signInManagerWrapper = signInManagerWrapper ?? throw new ArgumentNullException(nameof(signInManagerWrapper));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<User?> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return null;

        var user = await _userManagerWrapper.FindByEmailAsync(email);
        if (user == null)
            return null;

        var result = await _signInManagerWrapper.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);
        return result.Succeeded ? user : null;
    }

    /// <inheritdoc />
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            return null;

        return await _userManagerWrapper.FindByEmailAsync(email);
    }

    /// <inheritdoc />
    public async Task<User?> GetByIdAsync(long userId, CancellationToken cancellationToken = default)
    {
        return await _userManagerWrapper.FindByIdAsync(userId.ToString());
    }

    /// <inheritdoc />
    public async Task<IdentityResult> CreateUserAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = new User(email);
        var result = await _userManagerWrapper.CreateAsync(user, password);

        if (result.Succeeded)
        {
            // Adicionar role padrão de freelancer
            await _userManagerWrapper.AddToRoleAsync(user, "freelancer");
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<IdentityResult> InitiateEmailChangeAsync(long userId, string newEmail, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(newEmail))
            return IdentityResult.Failed(new IdentityError { Description = "Email é obrigatório" });

        var user = await _userManagerWrapper.FindByIdAsync(userId.ToString());
        if (user == null)
            return IdentityResult.Failed(new IdentityError { Description = "Usuário não encontrado" });

        // Verificar se o novo email já está em uso
        var existingUser = await _userManagerWrapper.FindByEmailAsync(newEmail);
        if (existingUser != null && existingUser.Id != user.Id)
            return IdentityResult.Failed(new IdentityError { Description = "Este email já está sendo usado por outro usuário" });

        try
        {
            // Gerar token de confirmação
            var token = await _userManagerWrapper.GenerateChangeEmailTokenAsync(user, newEmail);

            // Enviar email de confirmação
            var emailSent = await _emailService.SendEmailChangeConfirmationAsync(
                newEmail,
                user.Email ?? user.UserName ?? "Usuário",
                token,
                cancellationToken);

            if (!emailSent)
            {
                _logger.LogError("Falha ao enviar email de confirmação para {NewEmail} (usuário {UserId})", newEmail, userId);
                return IdentityResult.Failed(new IdentityError { Description = "Erro ao enviar email de confirmação" });
            }

            _logger.LogInformation("Token de alteração de email gerado para usuário {UserId}. Novo email: {NewEmail}", userId, newEmail);

            return IdentityResult.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao iniciar processo de alteração de email para usuário {UserId}", userId);
            return IdentityResult.Failed(new IdentityError { Description = "Erro interno ao processar solicitação" });
        }
    }

    /// <inheritdoc />
    public async Task<IdentityResult> ConfirmEmailChangeAsync(long userId, string newEmail, string confirmationToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(newEmail))
            return IdentityResult.Failed(new IdentityError { Description = "Email é obrigatório" });

        if (string.IsNullOrWhiteSpace(confirmationToken))
            return IdentityResult.Failed(new IdentityError { Description = "Token de confirmação é obrigatório" });

        var user = await _userManagerWrapper.FindByIdAsync(userId.ToString());
        if (user == null)
            return IdentityResult.Failed(new IdentityError { Description = "Usuário não encontrado" });

        try
        {
            // Verificar novamente se o email não está em uso (pode ter mudado desde a solicitação)
            var existingUser = await _userManagerWrapper.FindByEmailAsync(newEmail);
            if (existingUser != null && existingUser.Id != user.Id)
                return IdentityResult.Failed(new IdentityError { Description = "Este email já está sendo usado por outro usuário" });

            var oldEmail = user.Email;

            // Alterar email usando o método seguro do Identity
            // O ChangeEmailAsync valida internamente o token e falha se for inválido/expirado
            // Esta é a abordagem recomendada pelo ASP.NET Core Identity
            var result = await _userManagerWrapper.ChangeEmailAsync(user, newEmail, confirmationToken);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Falha ao alterar email para usuário {UserId}. Token pode ser inválido ou expirado. Erros: {Errors}",
                    userId, string.Join(", ", result.Errors.Select(e => e.Description)));
                return result;
            }

            // Atualizar username para ser igual ao email
            var userNameResult = await _userManagerWrapper.SetUserNameAsync(user, newEmail);
            if (!userNameResult.Succeeded)
            {
                _logger.LogError("Falha ao atualizar username para usuário {UserId}. Erros: {Errors}",
                    userId, string.Join(", ", userNameResult.Errors.Select(e => e.Description)));
                // Não falhar aqui, pois o email já foi alterado
            }

            // Atualizar propriedades customizadas
            user.UpdateEmail(newEmail);
            await _userManagerWrapper.UpdateAsync(user);

            // Enviar notificação para o email antigo
            if (!string.IsNullOrEmpty(oldEmail))
            {
                await _emailService.SendEmailChangeNotificationAsync(
                    oldEmail,
                    newEmail,
                    user.UserName ?? "Usuário",
                    cancellationToken);
            }

            _logger.LogInformation("Email alterado com sucesso para usuário {UserId}. Email antigo: {OldEmail}, Novo email: {NewEmail}",
                userId, oldEmail, newEmail);

            return IdentityResult.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao confirmar alteração de email para usuário {UserId}", userId);
            return IdentityResult.Failed(new IdentityError { Description = "Erro interno ao processar confirmação" });
        }
    }

    /// <inheritdoc />
    public async Task<IdentityResult> ChangePasswordAsync(long userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
    {
        var user = await _userManagerWrapper.FindByIdAsync(userId.ToString());
        if (user == null)
            return IdentityResult.Failed(new IdentityError { Description = "Usuário não encontrado" });

        return await _userManagerWrapper.ChangePasswordAsync(user, currentPassword, newPassword);
    }

    /// <inheritdoc />
    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var user = await _userManagerWrapper.FindByEmailAsync(email);
        return user != null;
    }
}
