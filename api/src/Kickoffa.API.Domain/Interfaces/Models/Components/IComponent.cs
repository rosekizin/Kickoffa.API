using Kickoffa.API.Domain.Models;
using Kickoffa.API.Domain.Models.Enums;

namespace Kickoffa.API.Domain.Interfaces.Models.Components
{
	public interface IComponent : IBaseEntity
	{
		long SectionId { get; }
		int Order { get; }
		string Title { get; }
		string? Description { get; }
		bool IsRequired { get; }

		/// <summary>
		/// Tipo do componente definido pela classe concreta
		/// </summary>
		ComponentType Type { get; }

		// Relacionamentos
		IChecklistSection Section { get; }
		IComponentStatus? Status { get; }

		/// <summary>
		/// Atualiza o título do componente
		/// </summary>
		void UpdateTitle(string title);

		/// <summary>
		/// Atualiza a descrição do componente
		/// </summary>
		void UpdateDescription(string? description);

		/// <summary>
		/// Atualiza se o componente é obrigatório
		/// </summary>
		void UpdateRequired(bool isRequired);

		/// <summary>
		/// Atualiza a ordem do componente (usado internamente pela seção)
		/// </summary>
		void UpdateOrder(int order);

		/// <summary>
		/// Atualiza o ID da seção (usado internamente pela seção)
		/// </summary>
		void UpdateSectionId(long sectionId);

		/// <summary>
		/// Define o relacionamento com a seção (usado pelo EF)
		/// </summary>
		void SetSection(ChecklistSection section);
	}
}