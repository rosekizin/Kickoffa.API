using Kickoffa.API.Domain.Models.Enums;
using Kickoffa.API.Domain.Models.Items.Base;

namespace Kickoffa.API.Domain.Models
{
	public class Section
	{
		public long Id { get; set; }
		public long ChecklistId { get; set; }
		public int Order { get; set; }
		public string Title { get; set; } = string.Empty;
		public SectionType Type { get; set; }

		// Apenas para seções do tipo Briefing
		public string? ContentJson { get; set; } // Conteúdo TipTap serializado como JSON
		public string? ContentHtml { get; set; } // Versão HTML do mesmo conteúdo
		public DateTime? ContentLastUpdated { get; set; }

		// Relacionamentos
		public Checklist Checklist { get; set; } = null!;
		public ICollection<Item> Items { get; set; } = new List<Item>();
		public ICollection<BriefingMedia> Media { get; set; } = new List<BriefingMedia>();
	}
}