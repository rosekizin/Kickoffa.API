namespace Kickoffa.API.Domain.Interfaces.Models
{
	public interface IChecklist : IBaseEntity
	{
		long OwnerId { get;} // ID do usuário que criou o checklist
		string Title { get; }
		string Slug { get; } // url amigável para o checklist
		string? Description { get; }
		DateTime? DueDate { get; }
		string? AccessToken { get; } // Token único para compartilhar
		bool IsPublished { get; }

		// Relacionamentos
		IEnumerable<ISection> Sections { get; }

		/// <summary>
		/// Atualiza o título do checklist
		/// </summary>
		void UpdateTitle(string title);

		/// <summary>
		/// Atualiza a descrição do checklist
		/// </summary>
		void UpdateDescription(string? description);

		/// <summary>
		/// Atualiza a data de vencimento
		/// </summary>
		void UpdateDueDate(DateTime? dueDate);

		/// <summary>
		/// Publica o checklist
		/// </summary>
		void Publish();

		/// <summary>
		/// Despublica o checklist
		/// </summary>
		void Unpublish();

		/// <summary>
		/// Regenera o token de acesso
		/// </summary>
		void RegenerateAccessToken();

		/// <summary>
		/// Adiciona uma seção ao checklist
		/// </summary>
		void AddSection(ISection section);

		/// <summary>
		/// Remove uma seção do checklist
		/// </summary>
		void RemoveSection(ISection section);
	}
}