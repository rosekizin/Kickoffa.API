

namespace Kickoffa.API.Contracts.FileType
{
	/// <summary>
	/// Resposta contendo informações de um tipo de arquivo
	/// </summary>
	/// <param name="Id">Identificador único do tipo de arquivo</param>
	/// <param name="MimeType">Tipo MIME do arquivo (ex: "image/jpeg")</param>
	/// <param name="Extension">Extensão do arquivo (ex: ".jpg")</param>
	/// <param name="DisplayName">Nome amigável para exibição (ex: "JPEG Image")</param>
	/// <param name="Description">Descrição detalhada do tipo de arquivo</param>
	/// <param name="Category">Categoria do arquivo para organização</param>
	/// <param name="RecommendedMaxSizeMB">Tamanho máximo recomendado em MB</param>
	public record FileTypeResponse(
		long Id,
		string MimeType,
		string Extension,
		string DisplayName,
		string? Description,
		string? Category,
		int? RecommendedMaxSizeMB
	);

	/// <summary>
	/// Resposta para busca de tipos de arquivo
	/// </summary>
	/// <param name="FileTypes">Lista de tipos de arquivo encontrados</param>
	/// <param name="TotalCount">Total de tipos de arquivo disponíveis</param>
	public record FileTypesSearchResponse(
		IEnumerable<FileTypeResponse> FileTypes,
		int TotalCount
	);
}
