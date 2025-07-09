using Kickoffa.API.Domain.Interfaces.Models;
using Kickoffa.API.Domain.Models.Components;
using Kickoffa.API.Domain.Models.Enums;

namespace Kickoffa.API.Domain.Repositories
{
	/// <summary>
	/// Repositório para gerenciar tipos de arquivo
	/// </summary>
	public interface IFileTypeRepository : IBaseRepository<IFileType, FileType>
	{
		/// <summary>
		/// Obtém todos os tipos de arquivo ativos
		/// </summary>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de tipos de arquivo ativos</returns>
		Task<IEnumerable<IFileType>> GetActiveFileTypesAsync(CancellationToken cancellationToken);

		/// <summary>
		/// Busca tipos de arquivo por termo de pesquisa
		/// </summary>
		/// <param name="searchTerm">Termo para buscar em DisplayName, Extension ou Description</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de tipos de arquivo que correspondem à busca</returns>
		Task<IEnumerable<IFileType>> SearchFileTypesAsync(string searchTerm, CancellationToken cancellationToken);

		/// <summary>
		/// Obtém tipos de arquivo por categoria
		/// </summary>
		/// <param name="category">Categoria dos arquivos</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de tipos de arquivo da categoria especificada</returns>
		Task<IEnumerable<IFileType>> GetFileTypesByCategoryAsync(FileTypeCategory category, CancellationToken cancellationToken);

		/// <summary>
		/// Obtém um tipo de arquivo por ID
		/// </summary>
		/// <param name="id">ID do tipo de arquivo</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Tipo de arquivo ou null se não encontrado</returns>
		Task<IFileType?> GetByIdAsync(long id, CancellationToken cancellationToken);

		/// <summary>
		/// Obtém tipos de arquivo por lista de IDs
		/// </summary>
		/// <param name="ids">Lista de IDs dos tipos de arquivo</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de tipos de arquivo encontrados</returns>
		Task<IEnumerable<IFileType>> GetByIdsAsync(IEnumerable<long> ids, CancellationToken cancellationToken);
	}
}