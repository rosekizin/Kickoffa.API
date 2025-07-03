using Kickoffa.API.Domain.Models;
using System.Text.RegularExpressions;

namespace Kickoffa.API.Domain.Factories
{
	/// <summary>
	/// Factory para criação de checklists
	/// </summary>
	public static class ChecklistFactory
	{
		/// <summary>
		/// Cria um novo checklist
		/// </summary>
		public static Checklist CreateChecklist(
			long ownerId,
			string title,
			string? description = null,
			DateTime? dueDate = null,
			string? customSlug = null)
		{
			var slug = !string.IsNullOrWhiteSpace(customSlug) 
				? NormalizeSlug(customSlug) 
				: GenerateSlugFromTitle(title);

			return new Checklist(ownerId, title, slug, description, dueDate);
		}

		/// <summary>
		/// Cria um checklist com seções iniciais
		/// </summary>
		public static Checklist CreateChecklistWithSections(
			long ownerId,
			string title,
			string? description = null,
			DateTime? dueDate = null,
			string? customSlug = null,
			params (string sectionTitle, bool isBriefing)[] sections)
		{
			var checklist = CreateChecklist(ownerId, title, description, dueDate, customSlug);

			for (int i = 0; i < sections.Length; i++)
			{
				var (sectionTitle, isBriefing) = sections[i];
				var section = isBriefing
					? SectionFactory.CreateBriefingSection(checklist.Id, sectionTitle, i + 1)
					: (Section)SectionFactory.CreateChecklistSection(checklist.Id, sectionTitle, i + 1);

				checklist.AddSection(section);
			}

			return checklist;
		}

		/// <summary>
		/// Cria um checklist padrão para onboarding
		/// </summary>
		public static Checklist CreateOnboardingChecklist(long ownerId, string clientName)
		{
			var title = $"Onboarding - {clientName}";
			var description = $"Processo de onboarding para o cliente {clientName}";

			return CreateChecklistWithSections(
				ownerId,
				title,
				description,
				DateTime.UtcNow.AddDays(30), // 30 dias para completar
				null,
				("Briefing do Projeto", true),
				("Informações do Cliente", true),
				("Documentos Necessários", false),
				("Aprovações", false)
			);
		}

		/// <summary>
		/// Cria um checklist padrão para desenvolvimento web
		/// </summary>
		public static Checklist CreateWebDevelopmentChecklist(long ownerId, string projectName)
		{
			var title = $"Desenvolvimento Web - {projectName}";
			var description = $"Checklist para desenvolvimento do projeto {projectName}";

			return CreateChecklistWithSections(
				ownerId,
				title,
				description,
				DateTime.UtcNow.AddDays(60), // 60 dias para completar
				null,
				("Briefing Técnico", true),
				("Design e Layout", true),
				("Funcionalidades", false),
				("Testes", false),
				("Deploy", false)
			);
		}

		/// <summary>
		/// Cria um checklist padrão para design
		/// </summary>
		public static Checklist CreateDesignChecklist(long ownerId, string projectName)
		{
			var title = $"Design - {projectName}";
			var description = $"Processo de design para o projeto {projectName}";

			return CreateChecklistWithSections(
				ownerId,
				title,
				description,
				DateTime.UtcNow.AddDays(45), // 45 dias para completar
				null,
				("Briefing Criativo", true),
				("Referências e Inspirações", true),
				("Conceitos Iniciais", false),
				("Refinamentos", false),
				("Entrega Final", false)
			);
		}

		/// <summary>
		/// Valida os dados de um checklist antes da criação
		/// </summary>
		public static bool ValidateChecklistData(
			string title,
			string? slug,
			out string? errorMessage)
		{
			errorMessage = null;

			if (string.IsNullOrWhiteSpace(title))
			{
				errorMessage = "Título é obrigatório";
				return false;
			}

			if (title.Length > 200)
			{
				errorMessage = "Título não pode ter mais de 200 caracteres";
				return false;
			}

			if (!string.IsNullOrWhiteSpace(slug))
			{
				if (!IsValidSlug(slug))
				{
					errorMessage = "Slug deve conter apenas letras, números e hífens";
					return false;
				}

				if (slug.Length > 100)
				{
					errorMessage = "Slug não pode ter mais de 100 caracteres";
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Gera um slug a partir do título
		/// </summary>
		public static string GenerateSlugFromTitle(string title)
		{
			if (string.IsNullOrWhiteSpace(title))
				throw new ArgumentException("Título não pode ser vazio", nameof(title));

			// Converter para minúsculas
			var slug = title.ToLowerInvariant();

			// Remover acentos
			slug = RemoveAccents(slug);

			// Substituir espaços e caracteres especiais por hífens
			slug = Regex.Replace(slug, @"[^a-z0-9\-]", "-");

			// Remover hífens duplicados
			slug = Regex.Replace(slug, @"-+", "-");

			// Remover hífens do início e fim
			slug = slug.Trim('-');

			// Limitar tamanho
			if (slug.Length > 100)
			{
				slug = slug[..100].TrimEnd('-');
			}

			// Se ficou vazio, usar um padrão
			if (string.IsNullOrEmpty(slug))
			{
				slug = "checklist";
			}

			return slug;
		}

		/// <summary>
		/// Normaliza um slug personalizado
		/// </summary>
		public static string NormalizeSlug(string slug)
		{
			if (string.IsNullOrWhiteSpace(slug))
				throw new ArgumentException("Slug não pode ser vazio", nameof(slug));

			return GenerateSlugFromTitle(slug);
		}

		/// <summary>
		/// Verifica se um slug é válido
		/// </summary>
		public static bool IsValidSlug(string slug)
		{
			if (string.IsNullOrWhiteSpace(slug))
				return false;

			// Deve conter apenas letras minúsculas, números e hífens
			// Não pode começar ou terminar com hífen
			return Regex.IsMatch(slug, @"^[a-z0-9]+(-[a-z0-9]+)*$");
		}

		/// <summary>
		/// Gera um slug único adicionando sufixo numérico se necessário
		/// </summary>
		public static string GenerateUniqueSlug(string baseSlug, Func<string, bool> slugExistsCheck)
		{
			var slug = NormalizeSlug(baseSlug);
			var originalSlug = slug;
			var counter = 1;

			while (slugExistsCheck(slug))
			{
				slug = $"{originalSlug}-{counter}";
				counter++;
			}

			return slug;
		}

		/// <summary>
		/// Remove acentos de uma string
		/// </summary>
		private static string RemoveAccents(string text)
		{
			var normalizedString = text.Normalize(System.Text.NormalizationForm.FormD);
			var stringBuilder = new System.Text.StringBuilder();

			foreach (var c in normalizedString)
			{
				var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
				if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
				{
					stringBuilder.Append(c);
				}
			}

			return stringBuilder.ToString().Normalize(System.Text.NormalizationForm.FormC);
		}

		/// <summary>
		/// Obtém templates de checklist disponíveis
		/// </summary>
		public static Dictionary<string, Func<long, string, Checklist>> GetAvailableTemplates()
		{
			return new Dictionary<string, Func<long, string, Checklist>>
			{
				["onboarding"] = (ownerId, name) => CreateOnboardingChecklist(ownerId, name),
				["web-development"] = (ownerId, name) => CreateWebDevelopmentChecklist(ownerId, name),
				["design"] = (ownerId, name) => CreateDesignChecklist(ownerId, name)
			};
		}

		/// <summary>
		/// Cria um checklist a partir de um template
		/// </summary>
		public static Checklist? CreateFromTemplate(string templateName, long ownerId, string projectName)
		{
			var templates = GetAvailableTemplates();
			
			if (templates.TryGetValue(templateName.ToLowerInvariant(), out var templateFactory))
			{
				return templateFactory(ownerId, projectName);
			}

			return null;
		}
	}
}
