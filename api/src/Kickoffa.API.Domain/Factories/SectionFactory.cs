using Kickoffa.API.Domain.Models;
using Kickoffa.API.Domain.Models.Enums;

namespace Kickoffa.API.Domain.Factories
{
	/// <summary>
	/// Factory para criação de seções
	/// </summary>
	public static class SectionFactory
	{
		/// <summary>
		/// Cria uma seção de briefing
		/// </summary>
		/// <param name="checklistId">ID do checklist</param>
		/// <param name="title">Título da seção</param>
		/// <param name="order">Ordem da seção</param>
		/// <param name="contentJson">Conteúdo JSON inicial (opcional)</param>
		/// <param name="contentHtml">Conteúdo HTML inicial (opcional)</param>
		/// <returns>Instância de BriefingSection</returns>
		public static BriefingSection CreateBriefingSection(
			long checklistId,
			string title,
			int order,
			string? contentJson = null,
			string? contentHtml = null)
		{
			return new BriefingSection(checklistId, title, order, contentJson, contentHtml);
		}

		/// <summary>
		/// Cria uma seção de checklist
		/// </summary>
		/// <param name="checklistId">ID do checklist</param>
		/// <param name="title">Título da seção</param>
		/// <param name="order">Ordem da seção</param>
		/// <returns>Instância de ChecklistSection</returns>
		public static ChecklistSection CreateChecklistSection(long checklistId, string title, int order)
		{
			return new ChecklistSection(checklistId, title, order);
		}

		/// <summary>
		/// Verifica se um tipo de seção é válido
		/// </summary>
		/// <param name="type">Tipo a ser verificado</param>
		/// <returns>True se válido, false caso contrário</returns>
		public static bool IsValidSectionType(SectionType type)
		{
			return Enum.IsDefined(typeof(SectionType), type);
		}

		/// <summary>
		/// Obtém todos os tipos de seção disponíveis
		/// </summary>
		/// <returns>Array com todos os tipos</returns>
		public static SectionType[] GetAvailableSectionTypes()
		{
			return Enum.GetValues<SectionType>();
		}
	}
}