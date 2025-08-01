namespace Kickoffa.API.Domain.Interfaces.Models
{
	public interface IBriefingMedia : IBaseEntity
	{
		long SectionId { get; }
		string FileName { get; }
		string StoragePath { get; }
		string S3FileKey { get; }
		string Url { get; }
		string ContentType { get; }
		long FileSize { get; }
		int? Width { get; }
		int? Height { get; }
		string? AltText { get; }
		DateTime UploadedAt { get; }

		// Relacionamento
		IBriefingSection Section { get; }

		/// <summary>
		/// Atualiza o texto alternativo
		/// </summary>
		void UpdateAltText(string? altText);

		/// <summary>
		/// Atualiza a URL da mídia
		/// </summary>
		void UpdateUrl(string url);
	}
}