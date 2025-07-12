using Kickoffa.API.Domain.Models.Enums;

namespace Kickoffa.API.Domain.Interfaces.Models
{
	/// <summary>
	/// Classe base para seções de checklist
	/// </summary>
	public interface ISection : IBaseEntity
	{
		/// <summary>	 
		/// ID do checklist ao qual esta seção pertence	 
		/// </summary>
		long ChecklistId { get; }

		/// <summary>
		/// Ordem da seção dentro do checklist
		/// </summary>
		int Order { get; }

		/// <summary>
		/// Título da seção
		/// </summary>
		string Title { get; }

		/// <summary>
		/// Tipo da seção (usado para discriminação no EF)
		/// </summary>
		SectionType Type { get; }

		/// <summary>
		/// Relacionamento com o checklist
		/// </summary>
		IChecklist Checklist { get; }

		/// <summary>
		/// Atualiza o título da seção
		/// </summary>
		void UpdateTitle(string title);

		/// <summary>
		/// Atualiza a ordem da seção
		/// </summary>
		void UpdateOrder(int order);

		/// <summary>
		/// Define o relacionamento com o checklist (usado pelo EF)
		/// </summary>
		void SetChecklist(IChecklist checklist);
	}
}