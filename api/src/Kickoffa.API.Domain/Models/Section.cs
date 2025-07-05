using Kickoffa.API.Domain.Models.Base;
using Kickoffa.API.Domain.Models.Enums;

namespace Kickoffa.API.Domain.Models
{
	/// <summary>
	/// Classe base para seções de checklist
	/// </summary>
	public abstract class Section : BaseEntity
	{

		/// <summary>
		/// Construtor protegido para uso pelas classes derivadas
		/// </summary>
		protected Section(long checklistId, string title, int order)
		{
			ChecklistId = checklistId;
			Title = title ?? throw new ArgumentNullException(nameof(title));
			Order = order;
		}

		/// <summary>
		/// Construtor protegido sem parâmetros para EF
		/// </summary>
		protected Section()
		{
			Title = string.Empty;
		}

		/// <summary>
		/// ID do checklist ao qual esta seção pertence
		/// </summary>
		public long ChecklistId { get; private set; }

		/// <summary>
		/// Ordem da seção dentro do checklist
		/// </summary>
		public int Order { get; private set; }

		/// <summary>
		/// Título da seção
		/// </summary>
		public string Title { get; private set; }

		/// <summary>
		/// Tipo da seção (usado para discriminação no EF)
		/// </summary>
		public abstract SectionType Type { get; }

		/// <summary>
		/// Relacionamento com o checklist
		/// </summary>
		public Checklist Checklist { get; private set; } = null!;

		/// <summary>
		/// Atualiza o título da seção
		/// </summary>
		public void UpdateTitle(string title)
		{
			Title = title ?? throw new ArgumentNullException(nameof(title));
			UpdateLastUpdatedDate();
		}

		/// <summary>
		/// Atualiza a ordem da seção
		/// </summary>
		public void UpdateOrder(int order)
		{
			Order = order;
			UpdateLastUpdatedDate();
		}

		/// <summary>
		/// Define o relacionamento com o checklist (usado pelo EF)
		/// </summary>
		internal void SetChecklist(Checklist checklist)
		{
			Checklist = checklist;
		}
	}
}