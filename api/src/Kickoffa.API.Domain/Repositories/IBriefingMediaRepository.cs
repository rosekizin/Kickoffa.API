using Kickoffa.API.Domain.Models;

namespace Kickoffa.API.Domain.Repositories
{
	/// <summary>
	/// Interface para repositório de BriefingMedia
	/// </summary>
	public interface IBriefingMediaRepository : IBaseRepository<BriefingMedia>
	{
		/// <summary>
		/// Busca mídias por seção de briefing
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de mídias da seção</returns>
		Task<IEnumerable<BriefingMedia>> GetBySectionIdAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca mídia por nome do arquivo
		/// </summary>
		/// <param name="fileName">Nome do arquivo</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Mídia encontrada ou null</returns>
		Task<BriefingMedia?> GetByFileNameAsync(string fileName, CancellationToken cancellationToken);
	}
}