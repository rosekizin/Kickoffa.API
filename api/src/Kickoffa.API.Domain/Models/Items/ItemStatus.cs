using Kickoffa.API.Domain.Models.Items.Base;

namespace Kickoffa.API.Domain.Models.Items
{
	public class ItemStatus
	{
		public long Id { get; set; }
		public long ItemId { get; set; }
		public bool Completed { get; set; }
		public DateTime? CompletedAt { get; set; }
		public string? Response { get; set; } // Resposta textual quando aplicável

		// Para uploads
		public string? FileName { get; set; }
		public string? StoragePath { get; set; }
		public long? FileSize { get; set; }
		public string? ContentType { get; set; }
		public string? Sha256Hash { get; set; }

		// Relacionamento
		public Item Item { get; set; } = null!;
	}
}