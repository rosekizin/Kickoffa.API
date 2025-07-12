using Kickoffa.API.Domain.Interfaces.Models;
using Kickoffa.API.Domain.Models.Base;
using Kickoffa.API.Domain.Models.Enums;

namespace Kickoffa.API.Domain.Models
{
	/// <inheritdoc/>
	public abstract class Section : BaseEntity, ISection
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

		/// <inheritdoc/>
		public long ChecklistId { get; private set; }

		/// <inheritdoc/>
		public int Order { get; private set; }

		/// <inheritdoc/>
		public string Title { get; private set; }

		/// <inheritdoc/>
		public abstract SectionType Type { get; }

		/// <inheritdoc/>
		public virtual Checklist Checklist { get; private set; } = null!;
		IChecklist ISection.Checklist => Checklist;

		/// <inheritdoc/>
		public void UpdateTitle(string title)
		{
			Title = title ?? throw new ArgumentNullException(nameof(title));
			UpdateLastUpdatedDate();
		}

		/// <inheritdoc/>
		public void UpdateOrder(int order)
		{
			Order = order;
			UpdateLastUpdatedDate();
		}

		/// <inheritdoc/>
		public void SetChecklist(IChecklist checklist)
		{
			Checklist = (Checklist)checklist;
		}
	}
}