using Kickoffa.API.Domain.Models.Enums;

namespace Kickoffa.API.Domain.Models
{
	public class Section
	{
		public Guid Id { get; set; }
		public Guid ChecklistId { get; set; }
		public int Order { get; set; }
		public string Title { get; set; } = string.Empty;
		public SectionType Type { get; set; }

		// Apenas para seções do tipo Briefing
		public string? ContentJson { get; set; } // Conteúdo TipTap serializado como JSON
		public string? ContentHtml { get; set; } // Versão HTML do mesmo conteúdo
		public DateTime? ContentLastUpdated { get; set; }

		// Metadados para UI
		public bool IsCollapsed { get; set; } = false;
		public string? IconName { get; set; }
		public string? CustomCssClass { get; set; }

		// Relacionamentos
		public Checklist Checklist { get; set; } = null!;
		public ICollection<Item> Items { get; set; } = new List<Item>();
		public ICollection<BriefingMedia> Media { get; set; } = new List<BriefingMedia>();
	}
}