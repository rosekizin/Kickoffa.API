using Kickoffa.API.Domain.Interfaces.Models;
using Kickoffa.API.Domain.Interfaces.Models.Components;
using Kickoffa.API.Domain.Models.Components.Base;
using Kickoffa.API.Domain.Models.Enums;

namespace Kickoffa.API.Domain.Models
{
	/// <inheritdoc/>
	public class ChecklistSection : Section, IChecklistSection
	{
		/// <inheritdoc/>
		public override SectionType Type => SectionType.Checklist;

		/// <inheritdoc/>
		public virtual ICollection<Component> Components { get; private set; }
		IEnumerable<IComponent> IChecklistSection.Components => Components;

		/// <inheritdoc/>
		public ChecklistSection(long checklistId, string title, int order)
			: base(checklistId, title, order)
		{
			Components = [];
		}

		/// <summary>
		/// Construtor sem parâmetros para EF
		/// </summary>
		protected ChecklistSection() : base()
		{
			Components = [];
		}

		/// <inheritdoc/>
		public void AddComponent(IComponent component)
		{
			ArgumentNullException.ThrowIfNull(component);

			component.UpdateSectionId(Id);
			if(component.Order == default)
			{
				// Se a ordem não foi definida, atribui a próxima ordem disponível
				// Isso é útil para componentes criados sem ordem específica
				component.UpdateOrder(Components.Count + 1);
			}
			//else if (Components.Any(i => i.Order == component.Order))
			//{
			//	// Se a ordem já existe, ajusta para a próxima ordem disponível
			//	int newOrder = Components.Max(i => i.Order) + 1;
			//	component.UpdateOrder(newOrder);
			//}
			Components.Add((Component)component);
			UpdateLastUpdatedDate();
		}

		/// <inheritdoc/>
		public void RemoveComponent(IComponent component)
		{
			ArgumentNullException.ThrowIfNull(component);

			Components.Remove((Component)component);
			ReorderComponents();
			UpdateLastUpdatedDate();
		}

		/// <inheritdoc/>
		public void ReorderComponents()
		{
			var orderedComponents = Components.OrderBy(i => i.Order).ToList();
			for (int i = 0; i < orderedComponents.Count; i++)
			{
				orderedComponents[i].UpdateOrder(i + 1);
			}
			UpdateLastUpdatedDate();
		}

		/// <inheritdoc/>
		public void MoveComponent(IComponent component, int newOrder)
		{
			ArgumentNullException.ThrowIfNull(component);
			if (!Components.Contains(component))
				return;

			component.UpdateOrder(newOrder);
			ReorderComponents();
		}

		/// <inheritdoc/>
		public IEnumerable<IComponent> GetComponentsByType(ComponentType componenteType)
		{
			return Components.Where(i => i.Type == componenteType).OrderBy(i => i.Order);
		}

		/// <inheritdoc/>
		public bool HasComponents()
		{
			return Components.Count != 0;
		}

		/// <inheritdoc/>
		public decimal CalculateProgress()
		{
			if (Components.Count == 0)
				return 0;

			var completedComponents = Components.Count(i => i.Status?.IsCompleted == true);
			return Math.Round((decimal)completedComponents / Components.Count * 100, 2);
		}

		/// <inheritdoc/>
		public bool AreRequiredComponentsCompleted()
		{
			var requiredComponents = Components.Where(i => i.IsRequired);
			return requiredComponents.All(i => i.Status?.IsCompleted == true);
		}
	}
}