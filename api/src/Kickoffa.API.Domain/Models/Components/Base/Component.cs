using Kickoffa.API.Domain.Models.Base;
using Kickoffa.API.Domain.Models.Enums;

namespace Kickoffa.API.Domain.Models.Components.Base
{
	/// <summary>
	/// Classe base abstrata para todos os tipos de componentes de checklist
	/// </summary>
	public abstract class Component : BaseEntity
	{
		public long SectionId { get; private set; }
		public int Order { get; private set; }
		public string Title { get; private set; }
		public string? Description { get; private set; }
		public bool IsRequired { get; private set; }

		/// <summary>
		/// Tipo do componente definido pela classe concreta
		/// </summary>
		public abstract ComponentType Type { get; }

		// Relacionamentos
		public ChecklistSection Section { get; private set; } = null!;
		public ComponentStatus? Status { get; private set; }

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

		/// <summary>
		/// Atualiza o título do componente
		/// </summary>
		public void UpdateTitle(string title)
		{
			Title = title ?? throw new ArgumentNullException(nameof(title));
			UpdateLastUpdatedDate();
		}

		/// <summary>
		/// Atualiza a descrição do componente
		/// </summary>
		public void UpdateDescription(string? description)
		{
			Description = description;
			UpdateLastUpdatedDate();
		}

		/// <summary>
		/// Atualiza se o componente é obrigatório
		/// </summary>
		public void UpdateRequired(bool isRequired)
		{
			IsRequired = isRequired;
			UpdateLastUpdatedDate();
		}

		/// <summary>
		/// Atualiza a ordem do componente (usado internamente pela seção)
		/// </summary>
		public void UpdateOrder(int order)
		{
			Order = order;
			UpdateLastUpdatedDate();
		}

		/// <summary>
		/// Atualiza o ID da seção (usado internamente pela seção)
		/// </summary>
		public void UpdateSectionId(long sectionId)
		{
			SectionId = sectionId;
			UpdateLastUpdatedDate();
		}

		/// <summary>
		/// Define o relacionamento com a seção (usado pelo EF)
		/// </summary>
		public void SetSection(ChecklistSection section)
		{
			Section = section;
		}

		/// <summary>
		/// Define o status do componente (usado pelo EF)
		/// </summary>
		public void SetStatus(ComponentStatus? status)
		{
			Status = status;
		}
	}
}