using Newtonsoft.Json;

namespace Kickoffa.API.Contracts.Checklist.Components.Response
{
	/// <summary>
	/// Response do status de um componente
	/// </summary>
	public sealed record ComponentStatusResponse
	{
		[JsonProperty(PropertyName = "id", Required = Required.Always)]
		public required long Id { get; init; }

		[JsonProperty(PropertyName = "componentId", Required = Required.Always)]
		public required long ComponentId { get; init; }

		[JsonProperty(PropertyName = "isCompleted", Required = Required.Always)]
		public required bool IsCompleted { get; init; }

		[JsonProperty(PropertyName = "completedAt", Required = Required.Default)]
		public DateTime? CompletedAt { get; init; }

		// Dados da resposta

		[JsonProperty(PropertyName = "textResponse", Required = Required.Default)]
		public string? TextResponse { get; init; }

		[JsonProperty(PropertyName = "signatureData", Required = Required.Default)]
		public string? SignatureData { get; init; }

		[JsonProperty(PropertyName = "uploadedFiles", Required = Required.Default)]
		public ICollection<UploadedFileResponse>? UploadedFiles { get; init; }

		[JsonProperty(PropertyName = "createdDateUtc", Required = Required.Default)]
		public required DateTime CreatedDateUtc { get; init; }

		[JsonProperty(PropertyName = "lastUpdatedDateUtc", Required = Required.Default)]
		public required DateTime LastUpdatedDateUtc { get; init; }
	}
}