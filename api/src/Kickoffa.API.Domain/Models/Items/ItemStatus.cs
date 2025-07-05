using Kickoffa.API.Domain.Models.Base;
using Kickoffa.API.Domain.Models.Items.Base;

namespace Kickoffa.API.Domain.Models.Items
{
	public class ItemStatus : BaseEntity<ItemStatus>
	{
		public ItemStatus(long itemId, bool isCompleted, DateTime? completedAt, string? response)
		{
			ItemId = itemId;
			IsCompleted = isCompleted;
			CompletedAt = completedAt;
			Response = response;
		}

		public long ItemId { get; private set; }
		public bool IsCompleted { get; private set; }
		public DateTime? CompletedAt { get; private set; }
		public string? Response { get; private set; } // Resposta textual quando aplicável

		// Relacionamento
		public Item Item { get; set; } = null!;
	}
}