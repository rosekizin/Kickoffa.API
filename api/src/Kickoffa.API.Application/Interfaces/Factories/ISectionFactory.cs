using Kickoffa.API.Contracts.Checklist;
using Kickoffa.API.Domain.Interfaces.Models;
using Kickoffa.API.Domain.Models;
using Kickoffa.API.Domain.Models.Enums;

namespace Kickoffa.API.Application.Interfaces.Factories
{
	/// <summary>
	/// Factory para criação de seções
	/// </summary>
	public interface ISectionFactory
	{
		/// <summary>
		/// Cria uma seção de briefing ou checklist
		/// </summary>
		/// <param name="sectionRequest">Request</param>
		/// <returns>Instância de Section</returns>
		Task<ISection> CreateSection(SectionRequest sectionRequest, CancellationToken cancellationToken);

		/// <summary>
		/// Cria uma seção de briefing
		/// </summary>
		/// <param name="checklistId">ID do checklist</param>
		/// <param name="title">Título da seção</param>
		/// <param name="order">Ordem da seção</param>
		/// <param name="contentJson">Conteúdo JSON inicial (opcional)</param>
		/// <param name="contentHtml">Conteúdo HTML inicial (opcional)</param>
		/// <returns>Instância de BriefingSection</returns>
		public IBriefingSection CreateBriefingSection(
			long checklistId,
			string title,
			int order,
			string? contentJson = null,
			string? contentHtml = null);

		/// <summary>
		/// Cria uma seção de checklist
		/// </summary>
		/// <param name="checklistId">ID do checklist</param>
		/// <param name="title">Título da seção</param>
		/// <param name="order">Ordem da seção</param>
		/// <returns>Instância de ChecklistSection</returns>
		public IChecklistSection CreateChecklistSection(long checklistId, string title, int order);

		/// <summary>
		/// Verifica se um tipo de seção é válido
		/// </summary>
		/// <param name="type">Tipo a ser verificado</param>
		/// <returns>True se válido, false caso contrário</returns>
		public bool IsValidSectionType(SectionType type);

		/// <summary>
		/// Obtém todos os tipos de seção disponíveis
		/// </summary>
		/// <returns>Array com todos os tipos</returns>
		public SectionType[] GetAvailableSectionTypes();
	}
}