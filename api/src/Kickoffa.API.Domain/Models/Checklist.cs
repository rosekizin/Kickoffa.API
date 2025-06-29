
namespace Kickoffa.API.Domain.Models
{
	public class Checklist
	{
		public long Id { get; set; }
		public long OwnerId { get; set; } // ID do usuário que criou o checklist
		public string Title { get; set; } = string.Empty;
		public string Slug { get; set; } = string.Empty;
		public string? Description { get; set; }
		public DateTime? DueDate { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }
		public string? AccessToken { get; set; } // Token único para compartilhar
		public bool IsPublished { get; set; }

		// Relacionamentos
		public ICollection<Section> Sections { get; set; } = new List<Section>();
	}
}