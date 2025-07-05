using Kickoffa.API.Domain.Models.Items;

namespace Kickoffa.API.Domain.Repositories
{
	/// <summary>
	/// Interface para repositório de UploadItemFile
	/// </summary>
	public interface IUploadItemFileRepository : IBaseRepository<UploadItemFile>
	{
		/// <summary>
		/// Busca arquivos por item de upload
		/// </summary>
		/// <param name="uploadItemId">ID do item de upload</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de arquivos do item</returns>
		Task<IEnumerable<UploadItemFile>> GetByUploadItemIdAsync(long uploadItemId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca arquivo por nome
		/// </summary>
		/// <param name="fileName">Nome do arquivo</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Arquivo encontrado ou null</returns>
		Task<UploadItemFile?> GetByFileNameAsync(string fileName, CancellationToken cancellationToken);

		/// <summary>
		/// Busca arquivo por hash SHA256
		/// </summary>
		/// <param name="sha256Hash">Hash SHA256 do arquivo</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Arquivo encontrado ou null</returns>
		Task<UploadItemFile?> GetBySha256HashAsync(string sha256Hash, CancellationToken cancellationToken);

		/// <summary>
		/// Busca arquivos por múltiplos itens de upload
		/// </summary>
		/// <param name="uploadItemIds">IDs dos itens de upload</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de arquivos dos itens</returns>
		Task<IEnumerable<UploadItemFile>> GetByUploadItemIdsAsync(IEnumerable<long> uploadItemIds, CancellationToken cancellationToken);

		/// <summary>
		/// Busca arquivos por tipo de conteúdo
		/// </summary>
		/// <param name="contentType">Tipo de conteúdo (MIME type)</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de arquivos do tipo especificado</returns>
		Task<IEnumerable<UploadItemFile>> GetByContentTypeAsync(string contentType, CancellationToken cancellationToken);

		/// <summary>
		/// Busca arquivos por seção (através do relacionamento com UploadItem)
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de arquivos da seção</returns>
		Task<IEnumerable<UploadItemFile>> GetBySectionIdAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca arquivos por checklist (através do relacionamento com UploadItem e Section)
		/// </summary>
		/// <param name="checklistId">ID do checklist</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de arquivos do checklist</returns>
		Task<IEnumerable<UploadItemFile>> GetByChecklistIdAsync(long checklistId, CancellationToken cancellationToken);

		/// <summary>
		/// Calcula o tamanho total de arquivos por item de upload
		/// </summary>
		/// <param name="uploadItemId">ID do item de upload</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Tamanho total em bytes</returns>
		Task<long> GetTotalFileSizeByUploadItemIdAsync(long uploadItemId, CancellationToken cancellationToken);

		/// <summary>
		/// Conta arquivos por item de upload
		/// </summary>
		/// <param name="uploadItemId">ID do item de upload</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Número de arquivos</returns>
		Task<int> CountByUploadItemIdAsync(long uploadItemId, CancellationToken cancellationToken);

		/// <summary>
		/// Remove arquivos órfãos (sem item de upload associado)
		/// </summary>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Número de arquivos removidos</returns>
		Task<int> RemoveOrphanedFilesAsync(CancellationToken cancellationToken);

		/// <summary>
		/// Verifica se existe arquivo com o hash especificado
		/// </summary>
		/// <param name="sha256Hash">Hash SHA256 a verificar</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>True se existe, false caso contrário</returns>
		Task<bool> ExistsBySha256HashAsync(string sha256Hash, CancellationToken cancellationToken);
	}
}
