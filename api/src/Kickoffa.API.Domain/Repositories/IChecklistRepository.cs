using Kickoffa.API.Domain.Interfaces.Models;
using Kickoffa.API.Domain.Models;

namespace Kickoffa.API.Domain.Repositories
{
	/// <summary>
	/// Interface para repositório de Checklist
	/// </summary>
	public interface IChecklistRepository : IBaseRepository<IChecklist, Checklist>
	{
		/// <summary>
		/// Busca checklist por slug
		/// </summary>
		/// <param name="slug">Slug do checklist</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Checklist encontrado ou null</returns>
		Task<IChecklist?> GetBySlugAsync(string slug, CancellationToken cancellationToken);

		/// <summary>
		/// Busca checklist por token de acesso
		/// </summary>
		/// <param name="accessToken">Token de acesso</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Checklist encontrado ou null</returns>
		Task<IChecklist?> GetByAccessTokenAsync(string accessToken, CancellationToken cancellationToken);

		/// <summary>
		/// Busca checklists por proprietário
		/// </summary>
		/// <param name="ownerId">ID do proprietário</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de checklists do proprietário</returns>
		Task<IEnumerable<IChecklist>> GetByOwnerIdAsync(long ownerId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca checklists publicados por proprietário
		/// </summary>
		/// <param name="ownerId">ID do proprietário</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de checklists publicados do proprietário</returns>
		Task<IEnumerable<IChecklist>> GetPublishedByOwnerIdAsync(long ownerId, CancellationToken cancellationToken);

		/// <summary>
		/// Verifica se existe checklist com o slug especificado
		/// </summary>
		/// <param name="slug">Slug a verificar</param>
		/// <param name="excludeId">ID do checklist a excluir da verificação (para updates)</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>True se existe, false caso contrário</returns>
		Task<bool> ExistsBySlugAsync(string slug, long? excludeId, CancellationToken cancellationToken);
	}
}