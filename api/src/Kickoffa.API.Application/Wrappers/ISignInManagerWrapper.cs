using Kickoffa.API.Domain.Models.AppUser;
using Microsoft.AspNetCore.Identity;

namespace Kickoffa.API.Application.Wrappers;

/// <summary>
/// Interface wrapper para SignInManager para facilitar testes unitários
/// </summary>
public interface ISignInManagerWrapper
{
    /// <summary>
    /// Verifica a senha de um usuário sem fazer login
    /// </summary>
    /// <param name="user">Usuário</param>
    /// <param name="password">Senha a ser verificada</param>
    /// <param name="lockoutOnFailure">Se deve bloquear em caso de falha</param>
    /// <returns>Resultado da verificação</returns>
    Task<SignInResult> CheckPasswordSignInAsync(User user, string password, bool lockoutOnFailure);

    /// <summary>
    /// Faz login do usuário
    /// </summary>
    /// <param name="user">Usuário</param>
    /// <param name="password">Senha</param>
    /// <param name="isPersistent">Se o login deve ser persistente</param>
    /// <param name="lockoutOnFailure">Se deve bloquear em caso de falha</param>
    /// <returns>Resultado do login</returns>
    Task<SignInResult> PasswordSignInAsync(User user, string password, bool isPersistent, bool lockoutOnFailure);

    /// <summary>
    /// Faz login do usuário por email
    /// </summary>
    /// <param name="email">Email do usuário</param>
    /// <param name="password">Senha</param>
    /// <param name="isPersistent">Se o login deve ser persistente</param>
    /// <param name="lockoutOnFailure">Se deve bloquear em caso de falha</param>
    /// <returns>Resultado do login</returns>
    Task<SignInResult> PasswordSignInAsync(string email, string password, bool isPersistent, bool lockoutOnFailure);

    /// <summary>
    /// Faz logout do usuário
    /// </summary>
    /// <returns>Task</returns>
    Task SignOutAsync();

    /// <summary>
    /// Verifica se o usuário pode fazer login
    /// </summary>
    /// <param name="user">Usuário</param>
    /// <returns>True se pode fazer login</returns>
    Task<bool> CanSignInAsync(User user);
}
