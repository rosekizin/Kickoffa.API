using Kickoffa.API.Application.Interfaces.Factories;
using Kickoffa.API.Domain.Interfaces.Models;
using Kickoffa.API.Domain.Models;
using System.Text.RegularExpressions;

namespace Kickoffa.API.Application.Factories
{
	///<inheritdoc/>
	public partial class ChecklistFactory : IChecklistFactory
	{
		private readonly ISectionFactory _sectionFactory;

		public ChecklistFactory(ISectionFactory sectionFactory)
		{
			_sectionFactory = sectionFactory;
		}

		///<inheritdoc/>
		public IChecklist CreateChecklist(
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

		///<inheritdoc/>
		public IChecklist CreateChecklistWithSections(
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
				ISection section = isBriefing
					? _sectionFactory.CreateBriefingSection(checklist.Id, sectionTitle, i + 1)
					: _sectionFactory.CreateChecklistSection(checklist.Id, sectionTitle, i + 1);

				checklist.AddSection(section);
			}

			return checklist;
		}

		///<inheritdoc/>
		public IChecklist CreateOnboardingChecklist(long ownerId, string clientName)
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

		///<inheritdoc/>
		public IChecklist CreateWebDevelopmentChecklist(long ownerId, string projectName)
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

		///<inheritdoc/>
		public IChecklist CreateDesignChecklist(long ownerId, string projectName)
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

		///<inheritdoc/>
		public bool ValidateChecklistData(
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

		///<inheritdoc/>
		public string GenerateSlugFromTitle(string title)
		{
			if (string.IsNullOrWhiteSpace(title))
				throw new ArgumentException("Título não pode ser vazio", nameof(title));

			// Converter para minúsculas
			var slug = title.ToLowerInvariant();

			// Remover acentos
			slug = RemoveAccents(slug);

			// Substituir espaços e caracteres especiais por hífens
			slug = SpecialCharsAndSpaces().Replace(slug, "-");

			// Remover hífens duplicados
			slug = ConsecutiveHyphens().Replace(slug, "-");

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

		///<inheritdoc/>
		public string NormalizeSlug(string slug)
		{
			if (string.IsNullOrWhiteSpace(slug))
				throw new ArgumentException("Slug não pode ser vazio", nameof(slug));

			return GenerateSlugFromTitle(slug);
		}

		///<inheritdoc/>
		public bool IsValidSlug(string slug)
		{
			if (string.IsNullOrWhiteSpace(slug))
				return false;

			// Deve conter apenas letras minúsculas, números e hífens
			// Não pode começar ou terminar com hífen
			return AllowOnlyAlphaNumericAndDashes().IsMatch(slug);
		}

		///<inheritdoc/>
		public string GenerateUniqueSlug(string baseSlug, Func<string, bool> slugExistsCheck)
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

		///<inheritdoc/>
		private string RemoveAccents(string text)
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

		///<inheritdoc/>
		public Dictionary<string, Func<long, string, IChecklist>> GetAvailableTemplates()
		{
			return new Dictionary<string, Func<long, string, IChecklist>>
			{
				["onboarding"] = (ownerId, name) => CreateOnboardingChecklist(ownerId, name),
				["web-development"] = (ownerId, name) => CreateWebDevelopmentChecklist(ownerId, name),
				["design"] = (ownerId, name) => CreateDesignChecklist(ownerId, name)
			};
		}

		///<inheritdoc/>
		public IChecklist? CreateFromTemplate(string templateName, long ownerId, string projectName)
		{
			var templates = GetAvailableTemplates();
			
			if (templates.TryGetValue(templateName.ToLowerInvariant(), out var templateFactory))
			{
				return templateFactory(ownerId, projectName);
			}

			return null;
		}

		[GeneratedRegex(@"[^a-z0-9\-]")]
		private static partial Regex SpecialCharsAndSpaces();

		[GeneratedRegex(@"-+")]
		private static partial Regex ConsecutiveHyphens();

		[GeneratedRegex(@"^[a-z0-9]+(-[a-z0-9]+)*$")]
		private static partial Regex AllowOnlyAlphaNumericAndDashes();
	}
}