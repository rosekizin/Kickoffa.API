using Kickoffa.API.Domain.Interfaces.Models.Components;
using Kickoffa.API.Domain.Models.Components;

namespace Kickoffa.API.Domain.Repositories
{
	/// <summary>
	/// Interface para repositório de UploadComponentFileTypeSize
	/// </summary>
	public interface IUploadComponentFileTypeSizeRepository : IBaseRepository<IUploadComponentFileTypeSize, UploadComponentFileTypeSize>
	{
		new Task<IUploadComponentFileTypeSize?> GetByIdAsync(long id, CancellationToken cancellationToken);

		new Task<IEnumerable<IUploadComponentFileTypeSize>> GetAllAsync(CancellationToken cancellationToken);

		/// <summary>
		/// Busca configurações de tamanho por componente de upload
		/// </summary>
		/// <param name="uploadComponentId">ID do componente de upload</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de configurações de tamanho do componente</returns>
		Task<IEnumerable<IUploadComponentFileTypeSize>> GetByUploadComponentIdAsync(long uploadComponentId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca configuração específica por componente e tipo de arquivo
		/// </summary>
		/// <param name="uploadComponentId">ID do componente de upload</param>
		/// <param name="fileTypeId">ID do tipo de arquivo</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Configuração encontrada ou null</returns>
		Task<IUploadComponentFileTypeSize?> GetByUploadComponentAndFileTypeAsync(long uploadComponentId, long fileTypeId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca configurações por tipo de arquivo
		/// </summary>
		/// <param name="fileTypeId">ID do tipo de arquivo</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de configurações para o tipo de arquivo</returns>
		Task<IEnumerable<IUploadComponentFileTypeSize>> GetByFileTypeIdAsync(long fileTypeId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca configurações por múltiplos componentes de upload
		/// </summary>
		/// <param name="uploadComponentIds">IDs dos componentes de upload</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de configurações dos componentes</returns>
		Task<IEnumerable<IUploadComponentFileTypeSize>> GetByUploadComponentIdsAsync(IEnumerable<long> uploadComponentIds, CancellationToken cancellationToken);

		/// <summary>
		/// Remove todas as configurações de um componente de upload
		/// </summary>
		/// <param name="uploadComponentId">ID do componente de upload</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Número de configurações removidas</returns>
		Task<int> RemoveByUploadComponentIdAsync(long uploadComponentId, CancellationToken cancellationToken);

		/// <summary>
		/// Remove configuração específica por componente e tipo de arquivo
		/// </summary>
		/// <param name="uploadComponentId">ID do componente de upload</param>
		/// <param name="fileTypeId">ID do tipo de arquivo</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>True se removeu, false se não encontrou</returns>
		Task<bool> RemoveByUploadComponentAndFileTypeAsync(long uploadComponentId, long fileTypeId, CancellationToken cancellationToken);

		/// <summary>
		/// Verifica se existe configuração para o componente e tipo de arquivo
		/// </summary>
		/// <param name="uploadComponentId">ID do componente de upload</param>
		/// <param name="fileTypeId">ID do tipo de arquivo</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>True se existe, false caso contrário</returns>
		Task<bool> ExistsByUploadComponentAndFileTypeAsync(long uploadComponentId, long fileTypeId, CancellationToken cancellationToken);

		/// <summary>
		/// Atualiza ou cria configuração de tamanho
		/// </summary>
		/// <param name="uploadComponentId">ID do componente de upload</param>
		/// <param name="fileTypeId">ID do tipo de arquivo</param>
		/// <param name="maxSizeMB">Tamanho máximo em MB</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Configuração atualizada ou criada</returns>
		Task<IUploadComponentFileTypeSize> UpsertAsync(long uploadComponentId, long fileTypeId, int maxSizeMB, CancellationToken cancellationToken);
	}
}
