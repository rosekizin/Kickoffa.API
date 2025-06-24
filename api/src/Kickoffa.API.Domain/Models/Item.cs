using Kickoffa.API.Domain.Models.Enums;

namespace Kickoffa.API.Domain.Models
{
	public class Item
	{
		public Guid Id { get; set; }
		public Guid SectionId { get; set; }
		public int Order { get; set; }
		public string Title { get; set; } = string.Empty;
		public string? Description { get; set; }
		public ItemType Type { get; set; }
		public bool IsRequired { get; set; }

		// Para itens do tipo Upload
		public string? AllowedMimeTypes { get; set; }
		public int? MaxSizeMB { get; set; }

		// Para itens do tipo TextInput
		public string? Placeholder { get; set; }
		public int? MaxLength { get; set; }

		// Para itens do tipo Confirmation
		public string? ConfirmationText { get; set; }

		// Relacionamentos
		public Section Section { get; set; } = null!;
		public ItemStatus? Status { get; set; }
	}
}