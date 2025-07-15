using Kickoffa.API.Domain.Interfaces.Models;

namespace Kickoffa.API.Application.Interfaces.Factories
{
	/// <summary>
	/// Factory para criação de checklists
	/// </summary>
	public interface IChecklistFactory
	{
		/// <summary>
		/// Cria um novo checklist
		/// </summary>
		public IChecklist CreateChecklist(
			long ownerId,
			long customerId,
			string title,
			string? description = null,
			DateTime? dueDate = null,
			string? customSlug = null);

		/// <summary>
		/// Cria um checklist com seções iniciais
		/// </summary>
		public IChecklist CreateChecklistWithSections(
			long ownerId,
			long customerId,
			string title,
			string? description = null,
			DateTime? dueDate = null,
			string? customSlug = null,
			params (string sectionTitle, bool isBriefing)[] sections);

		/// <summary>
		/// Cria um checklist padrão para onboarding
		/// </summary>
		public IChecklist CreateOnboardingChecklist(long ownerId, long customerId, string clientName);

		/// <summary>
		/// Cria um checklist padrão para desenvolvimento web
		/// </summary>
		public IChecklist CreateWebDevelopmentChecklist(long ownerId, long customerId, string projectName);

		/// <summary>
		/// Cria um checklist padrão para design
		/// </summary>
		public IChecklist CreateDesignChecklist(long ownerId, long customerId, string projectName);

		/// <summary>
		/// Valida os dados de um checklist antes da criação
		/// </summary>
		public bool ValidateChecklistData(
			string title,
			string? slug,
			out string? errorMessage);

		/// <summary>
		/// Gera um slug a partir do título
		/// </summary>
		public string GenerateSlugFromTitle(string title);

		/// <summary>
		/// Normaliza um slug personalizado
		/// </summary>
		public string NormalizeSlug(string slug);

		/// <summary>
		/// Verifica se um slug é válido
		/// </summary>
		public bool IsValidSlug(string slug);

		/// <summary>
		/// Gera um slug único adicionando sufixo numérico se necessário
		/// </summary>
		public string GenerateUniqueSlug(string baseSlug, Func<string, bool> slugExistsCheck);

		/// <summary>
		/// Obtém templates de checklist disponíveis
		/// </summary>
		public Dictionary<string, Func<long, long, string, IChecklist>> GetAvailableTemplates();

		/// <summary>
		/// Cria um checklist a partir de um template
		/// </summary>
		public IChecklist? CreateFromTemplate(string templateName, long ownerId, long customerId, string projectName);
	}
}