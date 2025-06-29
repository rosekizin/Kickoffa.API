using Kickoffa.API.Contracts.FileType;
using Kickoffa.API.Domain.Models.Enums;

namespace Kickoffa.API.Application.Interfaces
{
	/// <summary>
	/// Serviço para gerenciar tipos de arquivo
	/// </summary>
	public interface IFileTypeService
	{
		/// <summary>
		/// Obtém todos os tipos de arquivo ativos
		/// </summary>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Resposta com lista de tipos de arquivo</returns>
		Task<FileTypesSearchResponse> GetActiveFileTypesAsync(CancellationToken cancellationToken);

		/// <summary>
		/// Busca tipos de arquivo por termo
		/// </summary>
		/// <param name="searchTerm">Termo para buscar</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Resposta com tipos de arquivo encontrados</returns>
		Task<FileTypesSearchResponse> SearchFileTypesAsync(string? searchTerm, CancellationToken cancellationToken);

		/// <summary>
		/// Obtém tipos de arquivo por categoria
		/// </summary>
		/// <param name="category">Categoria dos arquivos</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Resposta com tipos de arquivo da categoria</returns>
		Task<FileTypesSearchResponse> GetFileTypesByCategoryAsync(FileTypeCategory category, CancellationToken cancellationToken);
	}
}
