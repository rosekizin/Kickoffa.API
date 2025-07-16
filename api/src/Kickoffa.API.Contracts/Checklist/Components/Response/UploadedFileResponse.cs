namespace Kickoffa.API.Contracts.Checklist.Components.Response
{
	/// <summary>
	/// Response de um arquivo enviado (relacionado ao status do componente)
	/// </summary>
	public sealed record UploadedFileResponse
	{
		public required long Id { get; init; }
		public required long ComponentStatusId { get; init; }
		public required string FileName { get; init; }
		public required string OriginalName { get; init; }
		public required string MimeType { get; init; }
		public required long Size { get; init; }
		public required string Url { get; init; }
		public required DateTime CreatedDateUtc { get; init; }
	}
}