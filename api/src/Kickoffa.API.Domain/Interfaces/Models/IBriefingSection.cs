using Kickoffa.API.Domain.Models.Enums;

namespace Kickoffa.API.Domain.Interfaces.Models
{
	/// <summary>
	/// Seção do tipo Briefing - contém conteúdo rico e mídias
	/// </summary>
	public interface IBriefingSection : IBaseEntity, ISection
	{
		/// <summary>
		/// Conteúdo da seção serializado como JSON (formato TipTap)
		/// </summary>
		string? ContentJson { get; }

		/// <summary>
		/// Versão HTML do conteúdo para exibição
		/// </summary>
		string? ContentHtml { get; }

		/// <summary>
		/// Data da última atualização do conteúdo
		/// </summary>
		DateTime? ContentLastUpdated { get; }

		/// <summary>
		/// Mídias associadas à seção de briefing
		/// </summary>
		IEnumerable<IBriefingMedia> Media { get; }

		/// <summary>
		/// Atualiza o conteúdo da seção
		/// </summary>
		/// <param name="contentJson">Conteúdo em formato JSON</param>
		/// <param name="contentHtml">Conteúdo em formato HTML</param>
		void UpdateContent(string? contentJson, string? contentHtml);

		/// <summary>
		/// Adiciona uma mídia à seção
		/// </summary>
		/// <param name="media">Mídia a ser adicionada</param>
		void AddMedia(IBriefingMedia media);

		/// <summary>
		/// Remove uma mídia da seção
		/// </summary>
		/// <param name="media">Mídia a ser removida</param>
		void RemoveMedia(IBriefingMedia media);

		/// <summary>
		/// Verifica se a seção tem conteúdo
		/// </summary>
		/// <returns>True se tem conteúdo, false caso contrário</returns>
		bool HasContent();

		/// <summary>
		/// Verifica se a seção tem mídias
		/// </summary>
		/// <returns>True se tem mídias, false caso contrário</returns>
		bool HasMedia();
	}
}