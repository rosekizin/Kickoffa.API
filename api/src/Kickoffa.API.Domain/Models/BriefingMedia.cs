using Kickoffa.API.Domain.Models.Base;

namespace Kickoffa.API.Domain.Models
{
	public class BriefingMedia : BaseEntity
	{
		/// <summary>
		/// Construtor para criação de nova mídia
		/// </summary>
		public BriefingMedia(long sectionId, string fileName, string storagePath, string url,
			string contentType, long fileSize, int? width = null, int? height = null, string? altText = null)
		{
			SectionId = sectionId;
			FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
			StoragePath = storagePath ?? throw new ArgumentNullException(nameof(storagePath));
			Url = url ?? throw new ArgumentNullException(nameof(url));
			ContentType = contentType ?? throw new ArgumentNullException(nameof(contentType));
			FileSize = fileSize;
			Width = width;
			Height = height;
			AltText = altText;
			UploadedAt = DateTime.UtcNow;
		}

		/// <summary>
		/// Construtor sem parâmetros para EF
		/// </summary>
		protected BriefingMedia()
		{
			FileName = string.Empty;
			StoragePath = string.Empty;
			Url = string.Empty;
			ContentType = string.Empty;
		}

		public long SectionId { get; private set; }
		public string FileName { get; private set; }
		public string StoragePath { get; private set; }
		public string Url { get; private set; }
		public string ContentType { get; private set; }
		public long FileSize { get; private set; }
		public int? Width { get; private set; }
		public int? Height { get; private set; }
		public string? AltText { get; private set; }
		public DateTime UploadedAt { get; private set; }

		// Relacionamento
		public BriefingSection Section { get; private set; } = null!;

		/// <summary>
		/// Atualiza o texto alternativo
		/// </summary>
		public void UpdateAltText(string? altText)
		{
			AltText = altText;
			UpdateLastUpdatedDate();
		}

		/// <summary>
		/// Atualiza a URL da mídia
		/// </summary>
		public void UpdateUrl(string url)
		{
			Url = url ?? throw new ArgumentNullException(nameof(url));
			UpdateLastUpdatedDate();
		}
	}
}