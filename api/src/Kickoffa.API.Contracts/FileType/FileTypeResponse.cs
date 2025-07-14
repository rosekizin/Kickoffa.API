

using Newtonsoft.Json;

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
	public sealed record FileTypeResponse
	{
		[JsonProperty(PropertyName = "id", Required = Required.Always)]
		public long Id { get; init; }

		[JsonProperty(PropertyName = "mimeType", Required = Required.Always)]
		public required string MimeType { get; init; }

		[JsonProperty(PropertyName = "extension", Required = Required.Always)]
		public required string Extension { get; init; }

		[JsonProperty(PropertyName = "displayName", Required = Required.Always)]
		public required string DisplayName { get; init; }

		[JsonProperty(PropertyName = "description", Required = Required.Default)]
		public string? Description { get; init; }

		[JsonProperty(PropertyName = "category", Required = Required.Default)]
		public string? Category { get; init; }

		[JsonProperty(PropertyName = "recommendedMaxSizeMB", Required = Required.Default)]
		public int? RecommendedMaxSizeMB { get; init; }
	}

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