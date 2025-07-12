using Kickoffa.API.Domain.Interfaces.Models;
using Kickoffa.API.Domain.Models.Enums;

namespace Kickoffa.API.Domain.Models
{
	public class BriefingSection : Section, IBriefingSection
	{
		/// <summary>
		/// Construtor para criação de nova seção de briefing
		/// </summary>
		public BriefingSection(long checklistId, string title, int order, string? contentJson, string? contentHtml)
			: base(checklistId, title, order)
		{
			Media = [];

			if (!string.IsNullOrWhiteSpace(contentJson) || !string.IsNullOrWhiteSpace(contentHtml))
			{
				UpdateContent(contentJson, contentHtml);
			}
		}

		/// <summary>
		/// Construtor sem parâmetros para EF
		/// </summary>
		protected BriefingSection() : base()
		{
			Media = [];
		}

		/// <inheritdoc/>
		public override SectionType Type => SectionType.Briefing;

		/// <inheritdoc/>
		public string? ContentJson { get; private set; }

		/// <inheritdoc/>
		public string? ContentHtml { get; private set; }

		/// <inheritdoc/>
		public DateTime? ContentLastUpdated { get; private set; }

		/// <inheritdoc/>
		public virtual ICollection<BriefingMedia> Media { get; private set; }

		IEnumerable<IBriefingMedia> IBriefingSection.Media => Media;

		/// <inheritdoc/>
		public void UpdateContent(string? contentJson, string? contentHtml)
		{
			ContentJson = contentJson;
			ContentHtml = contentHtml;
			ContentLastUpdated = DateTime.UtcNow;
			UpdateLastUpdatedDate();
		}

		/// <inheritdoc/>
		public void AddMedia(IBriefingMedia media)
		{
			ArgumentNullException.ThrowIfNull(media);

			Media.Add((BriefingMedia)media);
			UpdateLastUpdatedDate();
		}

		/// <inheritdoc/>
		public void RemoveMedia(IBriefingMedia media)
		{
			ArgumentNullException.ThrowIfNull(media);

			Media.Remove((BriefingMedia)media);
			UpdateLastUpdatedDate();
		}

		/// <inheritdoc/>
		public bool HasContent()
		{
			return !string.IsNullOrWhiteSpace(ContentJson) || !string.IsNullOrWhiteSpace(ContentHtml);
		}

		/// <inheritdoc/>
		public bool HasMedia()
		{
			return Media.Count != 0;
		}
	}
}