using Kickoffa.API.Domain.Interfaces.Models.Components;
using Kickoffa.API.Domain.Models.Components;

namespace Kickoffa.API.Domain.Repositories
{
	/// <summary>
	/// Interface para repositório de UploadComponent
	/// </summary>
	public interface IUploadComponentRepository : IBaseRepository<IUploadComponent, UploadComponent>
	{
		/// <summary>
		/// Busca componentes de upload por seção
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de componentes de upload da seção</returns>
		Task<IEnumerable<IUploadComponent>> GetBySectionIdAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca componentes de upload por placeholder
		/// </summary>
		/// <param name="placeholder">Texto do placeholder</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de componentes com o placeholder especificado</returns>
		Task<IEnumerable<IUploadComponent>> GetByPlaceholderAsync(string placeholder, CancellationToken cancellationToken);

		/// <summary>
		/// Busca componentes de upload obrigatórios por seção
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de componentes de upload obrigatórios</returns>
		Task<IEnumerable<IUploadComponent>> GetRequiredBySectionIdAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca componentes de upload por checklist
		/// </summary>
		/// <param name="checklistId">ID do checklist</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de componentes de upload do checklist</returns>
		Task<IEnumerable<IUploadComponent>> GetByChecklistIdAsync(long checklistId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca componentes de upload por tipo de arquivo permitido
		/// </summary>
		/// <param name="fileTypeId">ID do tipo de arquivo</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de componentes que permitem o tipo de arquivo especificado</returns>
		Task<IEnumerable<IUploadComponent>> GetByAllowedFileTypeAsync(long fileTypeId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca componentes de upload com arquivos enviados
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de componentes que possuem arquivos enviados</returns>
		Task<IEnumerable<IUploadComponent>> GetWithUploadedFilesAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Conta componentes de upload por seção
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Número de componentes de upload na seção</returns>
		Task<int> CountBySectionIdAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca componentes de upload por múltiplas seções
		/// </summary>
		/// <param name="sectionIds">IDs das seções</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de componentes de upload das seções</returns>
		Task<IEnumerable<IUploadComponent>> GetBySectionIdsAsync(IEnumerable<long> sectionIds, CancellationToken cancellationToken);
	}
}