using Kickoffa.API.Domain.Models.Components;

namespace Kickoffa.API.Domain.Repositories
{
	/// <summary>
	/// Interface para repositório de UploadComponentFile
	/// </summary>
	public interface IUploadComponentFileRepository : IBaseRepository<UploadComponentFile>
	{
		/// <summary>
		/// Busca arquivos por componente de upload
		/// </summary>
		/// <param name="uploadComponentId">ID do componente de upload</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de arquivos do componente</returns>
		Task<IEnumerable<UploadComponentFile>> GetByUploadComponentIdAsync(long uploadComponentId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca arquivo por nome
		/// </summary>
		/// <param name="fileName">Nome do arquivo</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Arquivo encontrado ou null</returns>
		Task<UploadComponentFile?> GetByFileNameAsync(string fileName, CancellationToken cancellationToken);

		/// <summary>
		/// Busca arquivo por hash SHA256
		/// </summary>
		/// <param name="sha256Hash">Hash SHA256 do arquivo</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Arquivo encontrado ou null</returns>
		Task<UploadComponentFile?> GetBySha256HashAsync(string sha256Hash, CancellationToken cancellationToken);

		/// <summary>
		/// Busca arquivos por múltiplos componentes de upload
		/// </summary>
		/// <param name="uploadComponentIds">IDs dos componentes de upload</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de arquivos dos componentes</returns>
		Task<IEnumerable<UploadComponentFile>> GetByUploadComponentIdsAsync(IEnumerable<long> uploadComponentIds, CancellationToken cancellationToken);

		/// <summary>
		/// Busca arquivos por tipo de conteúdo
		/// </summary>
		/// <param name="contentType">Tipo de conteúdo (MIME type)</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de arquivos do tipo especificado</returns>
		Task<IEnumerable<UploadComponentFile>> GetByContentTypeAsync(string contentType, CancellationToken cancellationToken);

		/// <summary>
		/// Busca arquivos por seção (através do relacionamento com UploadComponent)
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de arquivos da seção</returns>
		Task<IEnumerable<UploadComponentFile>> GetBySectionIdAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca arquivos por checklist (através do relacionamento com UploadComponent e Section)
		/// </summary>
		/// <param name="checklistId">ID do checklist</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de arquivos do checklist</returns>
		Task<IEnumerable<UploadComponentFile>> GetByChecklistIdAsync(long checklistId, CancellationToken cancellationToken);

		/// <summary>
		/// Calcula o tamanho total de arquivos por componente de upload
		/// </summary>
		/// <param name="uploadComponentId">ID do componente de upload</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Tamanho total em bytes</returns>
		Task<long> GetTotalFileSizeByUploadComponentIdAsync(long uploadComponentId, CancellationToken cancellationToken);

		/// <summary>
		/// Conta arquivos por componente de upload
		/// </summary>
		/// <param name="uploadComponentId">ID do componente de upload</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Número de arquivos</returns>
		Task<int> CountByUploadComponentIdAsync(long uploadComponentId, CancellationToken cancellationToken);

		/// <summary>
		/// Remove arquivos órfãos (sem componente de upload associado)
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
