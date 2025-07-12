using Kickoffa.API.Domain.Models.AppUser;
using Microsoft.AspNetCore.Identity;

namespace Kickoffa.API.Application.Interfaces
{
	/// <summary>
	/// Wrapper para SignInManager para facilitar testes unitários
	/// </summary>
	public interface ISignInManagerWrapper
	{
		/// <summary>
		/// Verifica senha sem fazer login
		/// </summary>
		/// <param name="user">Usuário</param>
		/// <param name="password">Senha</param>
		/// <param name="lockoutOnFailure">Se deve bloquear em caso de falha</param>
		/// <returns>Resultado da verificação</returns>
		Task<SignInResult> CheckPasswordSignInAsync(User user, string password, bool lockoutOnFailure);

		/// <summary>
		/// Realiza login com senha usando objeto User
		/// </summary>
		/// <param name="user">Usuário</param>
		/// <param name="password">Senha</param>
		/// <param name="isPersistent">Se deve manter login persistente</param>
		/// <param name="lockoutOnFailure">Se deve bloquear em caso de falha</param>
		/// <returns>Resultado do login</returns>
		Task<SignInResult> PasswordSignInAsync(User user, string password, bool isPersistent, bool lockoutOnFailure);

		/// <summary>
		/// Realiza login com senha usando email
		/// </summary>
		/// <param name="email">Email do usuário</param>
		/// <param name="password">Senha</param>
		/// <param name="isPersistent">Se deve manter login persistente</param>
		/// <param name="lockoutOnFailure">Se deve bloquear em caso de falha</param>
		/// <returns>Resultado do login</returns>
		Task<SignInResult> PasswordSignInAsync(string email, string password, bool isPersistent, bool lockoutOnFailure);

		/// <summary>
		/// Realiza logout
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
}