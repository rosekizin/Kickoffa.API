using Kickoffa.API.Domain.Models.Enums;
using Kickoffa.API.Domain.Models.Items.Base;

namespace Kickoffa.API.Domain.Models
{
	/// <summary>
	/// Seção do tipo Checklist - contém itens interativos
	/// </summary>
	public class ChecklistSection : Section
	{
		/// <summary>
		/// Tipo da seção (sempre Checklist)
		/// </summary>
		public override SectionType Type => SectionType.Checklist;

		/// <summary>
		/// Itens da seção de checklist
		/// </summary>
		public ICollection<Item> Items { get; private set; }

		/// <summary>
		/// Construtor para criação de nova seção de checklist
		/// </summary>
		public ChecklistSection(long checklistId, string title, int order)
			: base(checklistId, title, order)
		{
			Items = [];
		}

		/// <summary>
		/// Construtor sem parâmetros para EF
		/// </summary>
		protected ChecklistSection() : base()
		{
			Items = [];
		}

		/// <summary>
		/// Adiciona um item à seção
		/// </summary>
		/// <param name="item">Item a ser adicionado</param>
		public void AddItem(Item item)
		{
			ArgumentNullException.ThrowIfNull(item);

			item.UpdateSectionId(Id);
			item.UpdateOrder(Items.Count + 1);
			Items.Add(item);
			UpdateLastUpdatedDate();
		}

		/// <summary>
		/// Remove um item da seção
		/// </summary>
		/// <param name="item">Item a ser removido</param>
		public void RemoveItem(Item item)
		{
			ArgumentNullException.ThrowIfNull(item);

			Items.Remove(item);
			ReorderItems();
			UpdateLastUpdatedDate();
		}

		/// <summary>
		/// Reordena os itens da seção
		/// </summary>
		public void ReorderItems()
		{
			var orderedItems = Items.OrderBy(i => i.Order).ToList();
			for (int i = 0; i < orderedItems.Count; i++)
			{
				orderedItems[i].UpdateOrder(i + 1);
			}
			UpdateLastUpdatedDate();
		}

		/// <summary>
		/// Move um item para uma nova posição
		/// </summary>
		/// <param name="item">Item a ser movido</param>
		/// <param name="newOrder">Nova posição</param>
		public void MoveItem(Item item, int newOrder)
		{
			ArgumentNullException.ThrowIfNull(item);
			if (!Items.Contains(item))
				return;

			item.UpdateOrder(newOrder);
			ReorderItems();
		}

		/// <summary>
		/// Obtém itens por tipo
		/// </summary>
		/// <param name="itemType">Tipo do item</param>
		/// <returns>Lista de itens do tipo especificado</returns>
		public IEnumerable<Item> GetItemsByType(ItemType itemType)
		{
			return Items.Where(i => i.Type == itemType).OrderBy(i => i.Order);
		}

		/// <summary>
		/// Verifica se a seção tem itens
		/// </summary>
		/// <returns>True se tem itens, false caso contrário</returns>
		public bool HasItems()
		{
			return Items.Count != 0;
		}

		/// <summary>
		/// Calcula o progresso da seção (percentual de itens completados)
		/// </summary>
		/// <returns>Percentual de progresso (0-100)</returns>
		public decimal CalculateProgress()
		{
			if (Items.Count == 0)
				return 0;

			var completedItems = Items.Count(i => i.Status?.IsCompleted == true);
			return Math.Round((decimal)completedItems / Items.Count * 100, 2);
		}

		/// <summary>
		/// Verifica se todos os itens obrigatórios foram completados
		/// </summary>
		/// <returns>True se todos os obrigatórios estão completos, false caso contrário</returns>
		public bool AreRequiredItemsCompleted()
		{
			var requiredItems = Items.Where(i => i.IsRequired);
			return requiredItems.All(i => i.Status?.IsCompleted == true);
		}
	}
}