using Newtonsoft.Json;

namespace Kickoffa.API.Contracts.Checklist.Components.Request
{
	/// <summary>
	/// Response para configuração de tamanho máximo por tipo de arquivo
	/// </summary>
	public sealed record FileTypeSizeConfigResponse
	{
		/// <summary>
		/// ID da configuração
		/// </summary>
		[JsonProperty(PropertyName = "id", Required = Required.Always)]
		public required long Id { get; init; }

		/// <summary>
		/// ID do componente de upload
		/// </summary>
		[JsonProperty(PropertyName = "uploadComponentId", Required = Required.Always)]
		public required long UploadComponentId { get; init; }

		/// <summary>
		/// ID do tipo de arquivo
		/// </summary>
		[JsonProperty(PropertyName = "fileTypeId", Required = Required.Always)]
		public required long FileTypeId { get; init; }

		/// <summary>
		/// Tamanho máximo personalizado em MB para este tipo de arquivo
		/// </summary>
		[JsonProperty(PropertyName = "maxSizeMB", Required = Required.Always)]
		public required int MaxSizeMB { get; init; }

		/// <summary>
		/// Data de criação
		/// </summary>
		[JsonProperty(PropertyName = "createdDateUtc", Required = Required.Always)]
		public required DateTime CreatedDateUtc { get; init; }

		/// <summary>
		/// Data da última atualização
		/// </summary>
		[JsonProperty(PropertyName = "lastUpdatedDateUtc", Required = Required.Always)]
		public required DateTime LastUpdatedDateUtc { get; init; }
	}
}