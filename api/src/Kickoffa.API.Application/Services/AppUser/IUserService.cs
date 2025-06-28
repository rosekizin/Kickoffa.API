using Kickoffa.API.Domain.Models.AppUser;

namespace Kickoffa.API.Application.Services.AppUser;

/// <summary>
/// Interface para serviços relacionados a User
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Busca um usuário por email e senha
    /// </summary>
    /// <param name="email">Email do usuário</param>
    /// <param name="password">Senha do usuário</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Usuário encontrado ou null</returns>
    Task<User?> GetByEmailAndPasswordAsync(string email, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca um usuário por email
    /// </summary>
    /// <param name="email">Email do usuário</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Usuário encontrado ou null</returns>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica se um email já existe
    /// </summary>
    /// <param name="email">Email a ser verificado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>True se o email já existe, false caso contrário</returns>
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
}
