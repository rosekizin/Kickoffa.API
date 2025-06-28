using Kickoffa.API.Domain.Models.AppUser;
using Microsoft.AspNetCore.Identity;

namespace Kickoffa.API.Application.Services.AppUser;

/// <summary>
/// Interface para serviços relacionados a User usando ASP.NET Core Identity
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Autentica um usuário por email e senha
    /// </summary>
    /// <param name="email">Email do usuário</param>
    /// <param name="password">Senha do usuário</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Usuário autenticado ou null se credenciais inválidas</returns>
    Task<User?> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca um usuário por email
    /// </summary>
    /// <param name="email">Email do usuário</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Usuário encontrado ou null</returns>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca um usuário por ID
    /// </summary>
    /// <param name="userId">ID do usuário</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Usuário encontrado ou null</returns>
    Task<User?> GetByIdAsync(long userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cria um novo usuário
    /// </summary>
    /// <param name="email">Email do usuário</param>
    /// <param name="password">Senha do usuário</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da criação</returns>
    Task<IdentityResult> CreateUserAsync(string email, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Inicia o processo de alteração de email (envia token de confirmação)
    /// </summary>
    /// <param name="userId">ID do usuário</param>
    /// <param name="newEmail">Novo email</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação</returns>
    Task<IdentityResult> InitiateEmailChangeAsync(long userId, string newEmail, CancellationToken cancellationToken = default);

    /// <summary>
    /// Confirma a alteração de email usando token
    /// </summary>
    /// <param name="userId">ID do usuário</param>
    /// <param name="newEmail">Novo email</param>
    /// <param name="confirmationToken">Token de confirmação</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da confirmação</returns>
    Task<IdentityResult> ConfirmEmailChangeAsync(long userId, string newEmail, string confirmationToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Altera a senha do usuário
    /// </summary>
    /// <param name="userId">ID do usuário</param>
    /// <param name="currentPassword">Senha atual</param>
    /// <param name="newPassword">Nova senha</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da alteração</returns>
    Task<IdentityResult> ChangePasswordAsync(long userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica se um email já existe
    /// </summary>
    /// <param name="email">Email a ser verificado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>True se o email já existe, false caso contrário</returns>
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
}
