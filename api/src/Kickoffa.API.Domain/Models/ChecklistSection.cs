using Kickoffa.API.Domain.Models.Enums;
using Kickoffa.API.Domain.Models.Components.Base;

namespace Kickoffa.API.Domain.Models
{
	/// <summary>
	/// Seção do tipo Checklist - contém componentes interativos
	/// </summary>
	public class ChecklistSection : Section
	{
		/// <summary>
		/// Tipo da seção (sempre Checklist)
		/// </summary>
		public override SectionType Type => SectionType.Checklist;

		/// <summary>
		/// Componentes da seção de checklist
		/// </summary>
		public virtual ICollection<Component> Components { get; private set; }

		/// <summary>
		/// Construtor para criação de nova seção de checklist
		/// </summary>
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

		/// <summary>
		/// Adiciona um componente à seção
		/// </summary>
		/// <param name="component">Componente a ser adicionado</param>
		public void AddComponent(Component component)
		{
			ArgumentNullException.ThrowIfNull(component);

			component.UpdateSectionId(Id);
			component.UpdateOrder(Components.Count + 1);
			Components.Add(component);
			UpdateLastUpdatedDate();
		}

		/// <summary>
		/// Remove um componente da seção
		/// </summary>
		/// <param name="component">Componente a ser removido</param>
		public void RemoveComponent(Component component)
		{
			ArgumentNullException.ThrowIfNull(component);

			Components.Remove(component);
			ReorderComponents();
			UpdateLastUpdatedDate();
		}

		/// <summary>
		/// Reordena os componentes da seção
		/// </summary>
		public void ReorderComponents()
		{
			var orderedComponents = Components.OrderBy(i => i.Order).ToList();
			for (int i = 0; i < orderedComponents.Count; i++)
			{
				orderedComponents[i].UpdateOrder(i + 1);
			}
			UpdateLastUpdatedDate();
		}

		/// <summary>
		/// Move um componente para uma nova posição
		/// </summary>
		/// <param name="component">Componente a ser movido</param>
		/// <param name="newOrder">Nova posição</param>
		public void MoveComponent(Component component, int newOrder)
		{
			ArgumentNullException.ThrowIfNull(component);
			if (!Components.Contains(component))
				return;

			component.UpdateOrder(newOrder);
			ReorderComponents();
		}

		/// <summary>
		/// Obtém componentes por tipo
		/// </summary>
		/// <param name="componenteType">Tipo do componente</param>
		/// <returns>Lista de componentes do tipo especificado</returns>
		public IEnumerable<Component> GetComponentsByType(ComponentType componenteType)
		{
			return Components.Where(i => i.Type == componenteType).OrderBy(i => i.Order);
		}

		/// <summary>
		/// Verifica se a seção tem componentes
		/// </summary>
		/// <returns>True se tem componentes, false caso contrário</returns>
		public bool HasComponents()
		{
			return Components.Count != 0;
		}

		/// <summary>
		/// Calcula o progresso da seção (percentual de componentes completados)
		/// </summary>
		/// <returns>Percentual de progresso (0-100)</returns>
		public decimal CalculateProgress()
		{
			if (Components.Count == 0)
				return 0;

			var completedComponents = Components.Count(i => i.Status?.IsCompleted == true);
			return Math.Round((decimal)completedComponents / Components.Count * 100, 2);
		}

		/// <summary>
		/// Verifica se todos os componentes obrigatórios foram completados
		/// </summary>
		/// <returns>True se todos os obrigatórios estão completos, false caso contrário</returns>
		public bool AreRequiredComponentsCompleted()
		{
			var requiredComponents = Components.Where(i => i.IsRequired);
			return requiredComponents.All(i => i.Status?.IsCompleted == true);
		}
	}
}