using Kickoffa.API.Domain.Models.Enums;

namespace Kickoffa.API.Domain.Models
{
	/// <summary>
	/// Seção do tipo Briefing - contém conteúdo rico e mídias
	/// </summary>
	public class BriefingSection : Section
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
		/// <summary>
		/// Tipo da seção (sempre Briefing)
		/// </summary>
		public override SectionType Type => SectionType.Briefing;

		/// <summary>
		/// Conteúdo da seção serializado como JSON (formato TipTap)
		/// </summary>
		public string? ContentJson { get; private set; }

		/// <summary>
		/// Versão HTML do conteúdo para exibição
		/// </summary>
		public string? ContentHtml { get; private set; }

		/// <summary>
		/// Data da última atualização do conteúdo
		/// </summary>
		public DateTime? ContentLastUpdated { get; private set; }

		/// <summary>
		/// Mídias associadas à seção de briefing
		/// </summary>
		public virtual ICollection<BriefingMedia> Media { get; private set; }

		/// <summary>
		/// Atualiza o conteúdo da seção
		/// </summary>
		/// <param name="contentJson">Conteúdo em formato JSON</param>
		/// <param name="contentHtml">Conteúdo em formato HTML</param>
		public void UpdateContent(string? contentJson, string? contentHtml)
		{
			ContentJson = contentJson;
			ContentHtml = contentHtml;
			ContentLastUpdated = DateTime.UtcNow;
			UpdateLastUpdatedDate();
		}

		/// <summary>
		/// Adiciona uma mídia à seção
		/// </summary>
		/// <param name="media">Mídia a ser adicionada</param>
		public void AddMedia(BriefingMedia media)
		{
			ArgumentNullException.ThrowIfNull(media);

			Media.Add(media);
			UpdateLastUpdatedDate();
		}

		/// <summary>
		/// Remove uma mídia da seção
		/// </summary>
		/// <param name="media">Mídia a ser removida</param>
		public void RemoveMedia(BriefingMedia media)
		{
			ArgumentNullException.ThrowIfNull(media);

			Media.Remove(media);
			UpdateLastUpdatedDate();
		}

		/// <summary>
		/// Verifica se a seção tem conteúdo
		/// </summary>
		/// <returns>True se tem conteúdo, false caso contrário</returns>
		public bool HasContent()
		{
			return !string.IsNullOrWhiteSpace(ContentJson) || !string.IsNullOrWhiteSpace(ContentHtml);
		}

		/// <summary>
		/// Verifica se a seção tem mídias
		/// </summary>
		/// <returns>True se tem mídias, false caso contrário</returns>
		public bool HasMedia()
		{
			return Media.Count != 0;
		}
	}
}