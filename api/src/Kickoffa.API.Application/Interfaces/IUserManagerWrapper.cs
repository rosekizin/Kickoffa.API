using Kickoffa.API.Domain.Models.AppUser;
using Microsoft.AspNetCore.Identity;

namespace Kickoffa.API.Application.Interfaces;

/// <summary>
/// Wrapper para UserManager para facilitar testes unitários
/// </summary>
public interface IUserManagerWrapper
{
    /// <summary>
    /// Busca usuário por email
    /// </summary>
    /// <param name="email">Email do usuário</param>
    /// <returns>Usuário encontrado ou null</returns>
    Task<User?> FindByEmailAsync(string email);

    /// <summary>
    /// Busca usuário por ID
    /// </summary>
    /// <param name="userId">ID do usuário</param>
    /// <returns>Usuário encontrado ou null</returns>
    Task<User?> FindByIdAsync(string userId);

    /// <summary>
    /// Cria um novo usuário
    /// </summary>
    /// <param name="user">Usuário a ser criado</param>
    /// <param name="password">Senha do usuário</param>
    /// <returns>Resultado da criação</returns>
    Task<IdentityResult> CreateAsync(User user, string password);

    /// <summary>
    /// Atualiza um usuário
    /// </summary>
    /// <param name="user">Usuário a ser atualizado</param>
    /// <returns>Resultado da atualização</returns>
    Task<IdentityResult> UpdateAsync(User user);

    /// <summary>
    /// Altera senha do usuário
    /// </summary>
    /// <param name="user">Usuário</param>
    /// <param name="currentPassword">Senha atual</param>
    /// <param name="newPassword">Nova senha</param>
    /// <returns>Resultado da alteração</returns>
    Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword);

    /// <summary>
    /// Adiciona usuário a uma role
    /// </summary>
    /// <param name="user">Usuário</param>
    /// <param name="role">Nome da role</param>
    /// <returns>Resultado da operação</returns>
    Task<IdentityResult> AddToRoleAsync(User user, string role);

    /// <summary>
    /// Remove usuário de uma role
    /// </summary>
    /// <param name="user">Usuário</param>
    /// <param name="role">Nome da role</param>
    /// <returns>Resultado da operação</returns>
    Task<IdentityResult> RemoveFromRoleAsync(User user, string role);

    /// <summary>
    /// Verifica se usuário está em uma role
    /// </summary>
    /// <param name="user">Usuário</param>
    /// <param name="role">Nome da role</param>
    /// <returns>True se está na role</returns>
    Task<bool> IsInRoleAsync(User user, string role);

    /// <summary>
    /// Obtém todas as roles do usuário
    /// </summary>
    /// <param name="user">Usuário</param>
    /// <returns>Lista de roles</returns>
    Task<IList<string>> GetRolesAsync(User user);

    /// <summary>
    /// Define email do usuário
    /// </summary>
    /// <param name="user">Usuário</param>
    /// <param name="email">Novo email</param>
    /// <returns>Resultado da operação</returns>
    Task<IdentityResult> SetEmailAsync(User user, string email);

    /// <summary>
    /// Define username do usuário
    /// </summary>
    /// <param name="user">Usuário</param>
    /// <param name="userName">Novo username</param>
    /// <returns>Resultado da operação</returns>
    Task<IdentityResult> SetUserNameAsync(User user, string userName);

    /// <summary>
    /// Gera token para alteração de email
    /// </summary>
    /// <param name="user">Usuário</param>
    /// <param name="newEmail">Novo email</param>
    /// <returns>Token gerado</returns>
    Task<string> GenerateChangeEmailTokenAsync(User user, string newEmail);

    /// <summary>
    /// Altera email do usuário usando token
    /// </summary>
    /// <param name="user">Usuário</param>
    /// <param name="newEmail">Novo email</param>
    /// <param name="token">Token de confirmação</param>
    /// <returns>Resultado da alteração</returns>
    Task<IdentityResult> ChangeEmailAsync(User user, string newEmail, string token);
}
