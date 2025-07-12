using Kickoffa.API.Domain.Models.AppUser;
using Microsoft.AspNetCore.Identity;

namespace Kickoffa.API.Application.Interfaces
{
	/// <summary>
	/// Interface para serviços relacionados a usuários
	/// </summary>
	public interface IUserService
	{
		/// <summary>
		/// Busca um usuário por email
		/// </summary>
		/// <param name="email">Email do usuário</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Usuário encontrado ou null</returns>
		Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);

		/// <summary>
		/// Busca um usuário por ID
		/// </summary>
		/// <param name="id">ID do usuário</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Usuário encontrado ou null</returns>
		Task<User?> GetByIdAsync(long id, CancellationToken cancellationToken);

		/// <summary>
		/// Autentica um usuário com email e senha
		/// </summary>
		/// <param name="email">Email do usuário</param>
		/// <param name="password">Senha do usuário</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Usuário autenticado ou null se credenciais inválidas</returns>
		Task<User?> AuthenticateAsync(string email, string password, CancellationToken cancellationToken);

		/// <summary>
		/// Cria um novo usuário
		/// </summary>
		/// <param name="email">Email do usuário</param>
		/// <param name="password">Senha do usuário</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Resultado da criação</returns>
		Task<IdentityResult> CreateUserAsync(string email, string password, CancellationToken cancellationToken);

		/// <summary>
		/// Verifica se um email já existe
		/// </summary>
		/// <param name="email">Email a ser verificado</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>True se email existe</returns>
		Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken);

		/// <summary>
		/// Inicia o processo de alteração de email
		/// </summary>
		/// <param name="userId">ID do usuário</param>
		/// <param name="newEmail">Novo email</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Resultado da operação</returns>
		Task<IdentityResult> InitiateEmailChangeAsync(long userId, string newEmail, CancellationToken cancellationToken);

		/// <summary>
		/// Confirma a alteração de email usando token
		/// </summary>
		/// <param name="userId">ID do usuário</param>
		/// <param name="newEmail">Novo email</param>
		/// <param name="token">Token de confirmação</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Resultado da operação</returns>
		Task<IdentityResult> ConfirmEmailChangeAsync(long userId, string newEmail, string token, CancellationToken cancellationToken);
	}
}