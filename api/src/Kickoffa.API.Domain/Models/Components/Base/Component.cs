using Kickoffa.API.Domain.Interfaces.Models;
using Kickoffa.API.Domain.Interfaces.Models.Components;
using Kickoffa.API.Domain.Models.Base;
using Kickoffa.API.Domain.Models.Enums;

namespace Kickoffa.API.Domain.Models.Components.Base
{
	/// <inheritdoc/>
	public abstract class Component : BaseEntity, IComponent
	{
		public long SectionId { get; private set; }
		public int Order { get; private set; }
		public string Title { get; private set; }
		public string? Description { get; private set; }
		public bool IsRequired { get; private set; }

		/// <inheritdoc/>
		public abstract ComponentType Type { get; }

		// Relacionamentos
		public ChecklistSection Section { get; private set; } = null!;
		IChecklistSection IComponent.Section => Section;

		public ComponentStatus? Status { get; private set; }
		IComponentStatus? IComponent.Status => Status;

		/// <summary>
		/// Construtor protegido para uso pelas classes derivadas
		/// </summary>
		protected Component(long sectionId, string title, int order, string? description = null, bool isRequired = false)
		{
			SectionId = sectionId;
			Title = title ?? throw new ArgumentNullException(nameof(title));
			Order = order;
			Description = description;
			IsRequired = isRequired;
		}

		/// <summary>
		/// Construtor protegido sem parâmetros para EF
		/// </summary>
		protected Component()
		{
			Title = string.Empty;
		}

		/// <inheritdoc/>
		public void UpdateTitle(string title)
		{
			Title = title ?? throw new ArgumentNullException(nameof(title));
			UpdateLastUpdatedDate();
		}

		/// <inheritdoc/>
		public void UpdateDescription(string? description)
		{
			Description = description;
			UpdateLastUpdatedDate();
		}

		/// <inheritdoc/>
		public void UpdateRequired(bool isRequired)
		{
			IsRequired = isRequired;
			UpdateLastUpdatedDate();
		}

		/// <inheritdoc/>
		public void UpdateOrder(int order)
		{
			Order = order;
			UpdateLastUpdatedDate();
		}

		/// <inheritdoc/>
		public void UpdateSectionId(long sectionId)
		{
			SectionId = sectionId;
			UpdateLastUpdatedDate();
		}

		/// <inheritdoc/>
		public void SetSection(ChecklistSection section)
		{
			Section = section;
		}

		/// <inheritdoc/>
		public void SetStatus(ComponentStatus? status)
		{
			Status = status;
		}
	}
}