using Kickoffa.API.Domain.Models.Components;

namespace Kickoffa.API.Domain.Repositories
{
	/// <summary>
	/// Interface para repositório de UploadComponent
	/// </summary>
	public interface IUploadComponentRepository : IBaseRepository<UploadComponent>
	{
		/// <summary>
		/// Busca componentes de upload por seção
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de componentes de upload da seção</returns>
		Task<IEnumerable<UploadComponent>> GetBySectionIdAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca componentes de upload por tamanho máximo
		/// </summary>
		/// <param name="maxSizeMB">Tamanho máximo em MB</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de componentes com o tamanho máximo especificado</returns>
		Task<IEnumerable<UploadComponent>> GetByMaxSizeAsync(int? maxSizeMB, CancellationToken cancellationToken);

		/// <summary>
		/// Busca componentes de upload por placeholder
		/// </summary>
		/// <param name="placeholder">Texto do placeholder</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de componentes com o placeholder especificado</returns>
		Task<IEnumerable<UploadComponent>> GetByPlaceholderAsync(string placeholder, CancellationToken cancellationToken);

		/// <summary>
		/// Busca componentes de upload obrigatórios por seção
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de componentes de upload obrigatórios</returns>
		Task<IEnumerable<UploadComponent>> GetRequiredBySectionIdAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca componentes de upload por checklist
		/// </summary>
		/// <param name="checklistId">ID do checklist</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de componentes de upload do checklist</returns>
		Task<IEnumerable<UploadComponent>> GetByChecklistIdAsync(long checklistId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca componentes de upload por tipo de arquivo permitido
		/// </summary>
		/// <param name="fileTypeId">ID do tipo de arquivo</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de componentes que permitem o tipo de arquivo especificado</returns>
		Task<IEnumerable<UploadComponent>> GetByAllowedFileTypeAsync(long fileTypeId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca componentes de upload com arquivos enviados
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de componentes que possuem arquivos enviados</returns>
		Task<IEnumerable<UploadComponent>> GetWithUploadedFilesAsync(long sectionId, CancellationToken cancellationToken);

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
		Task<IEnumerable<UploadComponent>> GetBySectionIdsAsync(IEnumerable<long> sectionIds, CancellationToken cancellationToken);

		/// <summary>
		/// Busca componentes de upload por faixa de tamanho
		/// </summary>
		/// <param name="minSizeMB">Tamanho mínimo em MB</param>
		/// <param name="maxSizeMB">Tamanho máximo em MB</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de componentes na faixa de tamanho especificada</returns>
		Task<IEnumerable<UploadComponent>> GetBySizeRangeAsync(int? minSizeMB, int? maxSizeMB, CancellationToken cancellationToken);
	}
}