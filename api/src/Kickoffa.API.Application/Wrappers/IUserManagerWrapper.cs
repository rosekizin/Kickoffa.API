using Kickoffa.API.Domain.Models.AppUser;
using Microsoft.AspNetCore.Identity;

namespace Kickoffa.API.Application.Wrappers;

/// <summary>
/// Interface wrapper para UserManager para facilitar testes unitários
/// </summary>
public interface IUserManagerWrapper
{
    /// <summary>
    /// Busca um usuário por email
    /// </summary>
    /// <param name="email">Email do usuário</param>
    /// <returns>Usuário encontrado ou null</returns>
    Task<User?> FindByEmailAsync(string email);

    /// <summary>
    /// Busca um usuário por ID
    /// </summary>
    /// <param name="userId">ID do usuário</param>
    /// <returns>Usuário encontrado ou null</returns>
    Task<User?> FindByIdAsync(string userId);

    /// <summary>
    /// Cria um novo usuário
    /// </summary>
    /// <param name="user">Usuário a ser criado</param>
    /// <param name="password">Senha do usuário</param>
    /// <returns>Resultado da operação</returns>
    Task<IdentityResult> CreateAsync(User user, string password);

    /// <summary>
    /// Atualiza um usuário
    /// </summary>
    /// <param name="user">Usuário a ser atualizado</param>
    /// <returns>Resultado da operação</returns>
    Task<IdentityResult> UpdateAsync(User user);

    /// <summary>
    /// Altera a senha de um usuário
    /// </summary>
    /// <param name="user">Usuário</param>
    /// <param name="currentPassword">Senha atual</param>
    /// <param name="newPassword">Nova senha</param>
    /// <returns>Resultado da operação</returns>
    Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword);

    /// <summary>
    /// Adiciona um usuário a uma role
    /// </summary>
    /// <param name="user">Usuário</param>
    /// <param name="role">Nome da role</param>
    /// <returns>Resultado da operação</returns>
    Task<IdentityResult> AddToRoleAsync(User user, string role);

    /// <summary>
    /// Remove um usuário de uma role
    /// </summary>
    /// <param name="user">Usuário</param>
    /// <param name="role">Nome da role</param>
    /// <returns>Resultado da operação</returns>
    Task<IdentityResult> RemoveFromRoleAsync(User user, string role);

    /// <summary>
    /// Verifica se um usuário está em uma role
    /// </summary>
    /// <param name="user">Usuário</param>
    /// <param name="role">Nome da role</param>
    /// <returns>True se o usuário está na role</returns>
    Task<bool> IsInRoleAsync(User user, string role);

    /// <summary>
    /// Obtém as roles de um usuário
    /// </summary>
    /// <param name="user">Usuário</param>
    /// <returns>Lista de roles</returns>
    Task<IList<string>> GetRolesAsync(User user);

    /// <summary>
    /// Define o email de um usuário
    /// </summary>
    /// <param name="user">Usuário</param>
    /// <param name="email">Novo email</param>
    /// <returns>Resultado da operação</returns>
    Task<IdentityResult> SetEmailAsync(User user, string email);

    /// <summary>
    /// Define o nome de usuário
    /// </summary>
    /// <param name="user">Usuário</param>
    /// <param name="userName">Novo nome de usuário</param>
    /// <returns>Resultado da operação</returns>
    Task<IdentityResult> SetUserNameAsync(User user, string userName);

    /// <summary>
    /// Gera um token para alteração de email
    /// </summary>
    /// <param name="user">Usuário</param>
    /// <param name="newEmail">Novo email</param>
    /// <returns>Token de confirmação</returns>
    Task<string> GenerateChangeEmailTokenAsync(User user, string newEmail);

    /// <summary>
    /// Altera o email usando token de confirmação
    /// </summary>
    /// <param name="user">Usuário</param>
    /// <param name="newEmail">Novo email</param>
    /// <param name="token">Token de confirmação</param>
    /// <returns>Resultado da operação</returns>
    Task<IdentityResult> ChangeEmailAsync(User user, string newEmail, string token);
}
