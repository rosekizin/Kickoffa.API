namespace Kickoffa.API.Domain.Models
{
	public class BriefingMedia
	{
		public long Id { get; set; }
		public long SectionId { get; set; }
		public string FileName { get; set; } = string.Empty;
		public string StoragePath { get; set; } = string.Empty;
		public string Url { get; set; } = string.Empty;
		public string ContentType { get; set; } = string.Empty;
		public long FileSize { get; set; }
		public int? Width { get; set; }
		public int? Height { get; set; }
		public string? AltText { get; set; }
		public DateTime UploadedAt { get; set; }

		// Relacionamento
		public Section Section { get; set; } = null!;
	}
}