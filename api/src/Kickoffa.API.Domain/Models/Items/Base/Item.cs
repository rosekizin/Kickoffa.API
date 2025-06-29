using Kickoffa.API.Domain.Models.Base;
using Kickoffa.API.Domain.Models.Enums;

namespace Kickoffa.API.Domain.Models.Items.Base
{
	/// <summary>
	/// Classe base abstrata para todos os tipos de itens de checklist
	/// </summary>
	public abstract class Item : BaseEntity<Item>
	{
		public long SectionId { get; set; }
		public int Order { get; set; }
		public string Title { get; set; } = string.Empty;
		public string? Description { get; set; }
		public bool IsRequired { get; set; }

		/// <summary>
		/// Tipo do item definido pela classe concreta
		/// </summary>
		public abstract ItemType Type { get; }

		// Relacionamentos
		public Section Section { get; set; } = null!;
		public ItemStatus? Status { get; set; }
	}
}