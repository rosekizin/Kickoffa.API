using Kickoffa.API.Domain.Models;
using Kickoffa.API.Domain.Models.Enums;

namespace Kickoffa.API.Domain.Extensions
{
	/// <summary>
	/// Extensões para trabalhar com seções
	/// </summary>
	public static class SectionExtensions
	{
		/// <summary>
		/// Verifica se a seção é do tipo Briefing
		/// </summary>
		/// <param name="section">Seção a ser verificada</param>
		/// <returns>True se for BriefingSection</returns>
		public static bool IsBriefingSection(this Section section)
		{
			return section is BriefingSection;
		}

		/// <summary>
		/// Verifica se a seção é do tipo Checklist
		/// </summary>
		/// <param name="section">Seção a ser verificada</param>
		/// <returns>True se for ChecklistSection</returns>
		public static bool IsChecklistSection(this Section section)
		{
			return section is ChecklistSection;
		}

		/// <summary>
		/// Converte para BriefingSection se possível
		/// </summary>
		/// <param name="section">Seção a ser convertida</param>
		/// <returns>BriefingSection ou null se não for do tipo correto</returns>
		public static BriefingSection? AsBriefingSection(this Section section)
		{
			return section as BriefingSection;
		}

		/// <summary>
		/// Converte para ChecklistSection se possível
		/// </summary>
		/// <param name="section">Seção a ser convertida</param>
		/// <returns>ChecklistSection ou null se não for do tipo correto</returns>
		public static ChecklistSection? AsChecklistSection(this Section section)
		{
			return section as ChecklistSection;
		}

		/// <summary>
		/// Obtém o nome amigável do tipo da seção
		/// </summary>
		/// <param name="section">Seção</param>
		/// <returns>Nome amigável do tipo</returns>
		public static string GetTypeFriendlyName(this Section section)
		{
			return section.Type switch
			{
				SectionType.Briefing => "Briefing",
				SectionType.Checklist => "Lista de Verificação",
				_ => section.Type.ToString()
			};
		}

		/// <summary>
		/// Verifica se a seção tem conteúdo (independente do tipo)
		/// </summary>
		/// <param name="section">Seção a ser verificada</param>
		/// <returns>True se tem conteúdo</returns>
		public static bool HasContent(this Section section)
		{
			return section switch
			{
				BriefingSection briefing => briefing.HasContent() || briefing.HasMedia(),
				ChecklistSection checklist => checklist.HasItems(),
				_ => false
			};
		}

		/// <summary>
		/// Obtém uma descrição do conteúdo da seção
		/// </summary>
		/// <param name="section">Seção</param>
		/// <returns>Descrição do conteúdo</returns>
		public static string GetContentDescription(this Section section)
		{
			return section switch
			{
				BriefingSection briefing => GetBriefingDescription(briefing),
				ChecklistSection checklist => GetChecklistDescription(checklist),
				_ => "Seção sem conteúdo"
			};
		}

		/// <summary>
		/// Verifica se a seção está vazia
		/// </summary>
		/// <param name="section">Seção a ser verificada</param>
		/// <returns>True se estiver vazia</returns>
		public static bool IsEmpty(this Section section)
		{
			return !section.HasContent();
		}

		/// <summary>
		/// Obtém estatísticas da seção
		/// </summary>
		/// <param name="section">Seção</param>
		/// <returns>Dicionário com estatísticas</returns>
		public static Dictionary<string, object> GetStatistics(this Section section)
		{
			var stats = new Dictionary<string, object>
			{
				["Type"] = section.GetTypeFriendlyName(),
				["Title"] = section.Title,
				["Order"] = section.Order,
				["HasContent"] = section.HasContent()
			};

			switch (section)
			{
				case BriefingSection briefing:
					stats["MediaCount"] = briefing.Media.Count;
					stats["HasTextContent"] = briefing.HasContent();
					stats["LastContentUpdate"] = briefing.ContentLastUpdated?.ToString("yyyy-MM-dd HH:mm:ss") ?? "Nunca";
					break;

				case ChecklistSection checklist:
					stats["ItemCount"] = checklist.Items.Count;
					stats["Progress"] = $"{checklist.CalculateProgress():F1}%";
					stats["RequiredItemsCompleted"] = checklist.AreRequiredItemsCompleted();
					break;
			}

			return stats;
		}

		private static string GetBriefingDescription(BriefingSection briefing)
		{
			var parts = new List<string>();

			if (briefing.HasContent())
				parts.Add("com conteúdo de texto");

			if (briefing.HasMedia())
				parts.Add($"{briefing.Media.Count} mídia(s)");

			return parts.Any() 
				? $"Briefing {string.Join(" e ", parts)}"
				: "Briefing vazio";
		}

		private static string GetChecklistDescription(ChecklistSection checklist)
		{
			if (!checklist.HasItems())
				return "Lista de verificação vazia";

			var progress = checklist.CalculateProgress();
			return $"Lista com {checklist.Items.Count} item(s) - {progress:F1}% concluído";
		}
	}
}
