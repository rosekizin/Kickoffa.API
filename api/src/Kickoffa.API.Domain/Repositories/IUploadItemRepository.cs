using Kickoffa.API.Domain.Models.Items;

namespace Kickoffa.API.Domain.Repositories
{
	/// <summary>
	/// Interface para repositório de UploadItem
	/// </summary>
	public interface IUploadItemRepository : IBaseRepository<UploadItem>
	{
		/// <summary>
		/// Busca itens de upload por seção
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de itens de upload da seção</returns>
		Task<IEnumerable<UploadItem>> GetBySectionIdAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca itens de upload por tamanho máximo
		/// </summary>
		/// <param name="maxSizeMB">Tamanho máximo em MB</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de itens com o tamanho máximo especificado</returns>
		Task<IEnumerable<UploadItem>> GetByMaxSizeAsync(int? maxSizeMB, CancellationToken cancellationToken);

		/// <summary>
		/// Busca itens de upload por placeholder
		/// </summary>
		/// <param name="placeholder">Texto do placeholder</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de itens com o placeholder especificado</returns>
		Task<IEnumerable<UploadItem>> GetByPlaceholderAsync(string placeholder, CancellationToken cancellationToken);

		/// <summary>
		/// Busca itens de upload obrigatórios por seção
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de itens de upload obrigatórios</returns>
		Task<IEnumerable<UploadItem>> GetRequiredBySectionIdAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca itens de upload por checklist
		/// </summary>
		/// <param name="checklistId">ID do checklist</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de itens de upload do checklist</returns>
		Task<IEnumerable<UploadItem>> GetByChecklistIdAsync(long checklistId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca itens de upload por tipo de arquivo permitido
		/// </summary>
		/// <param name="fileTypeId">ID do tipo de arquivo</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de itens que permitem o tipo de arquivo especificado</returns>
		Task<IEnumerable<UploadItem>> GetByAllowedFileTypeAsync(long fileTypeId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca itens de upload com arquivos enviados
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de itens que possuem arquivos enviados</returns>
		Task<IEnumerable<UploadItem>> GetWithUploadedFilesAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Conta itens de upload por seção
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Número de itens de upload na seção</returns>
		Task<int> CountBySectionIdAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca itens de upload por múltiplas seções
		/// </summary>
		/// <param name="sectionIds">IDs das seções</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de itens de upload das seções</returns>
		Task<IEnumerable<UploadItem>> GetBySectionIdsAsync(IEnumerable<long> sectionIds, CancellationToken cancellationToken);

		/// <summary>
		/// Busca itens de upload por faixa de tamanho
		/// </summary>
		/// <param name="minSizeMB">Tamanho mínimo em MB</param>
		/// <param name="maxSizeMB">Tamanho máximo em MB</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de itens na faixa de tamanho especificada</returns>
		Task<IEnumerable<UploadItem>> GetBySizeRangeAsync(int? minSizeMB, int? maxSizeMB, CancellationToken cancellationToken);
	}
}
