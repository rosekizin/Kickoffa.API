using Kickoffa.API.Domain.Interfaces.Models.Customer;
using Kickoffa.API.Domain.Models.Enums;

namespace Kickoffa.API.Domain.Interfaces.Models
{
	public interface IChecklist : IBaseEntity
	{
		long OwnerId { get;} // ID do usuário que criou o checklist
		long CustomerId { get; } // ID do cliente associado ao checklist
		string Title { get; }
		string Slug { get; } // url amigável para o checklist
		string? Description { get; }
		DateTime? DueDate { get; }
		string? AccessToken { get; } // Token único para compartilhar
		ChecklistStatus Status { get; }

		// Relacionamentos
		ICustomer Customer { get; }
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
		/// Marca o checklist como concluído
		/// </summary>
		void MarkAsCompleted();

		/// <summary>
		/// Arquiva o checklist
		/// </summary>
		void Archive();

		/// <summary>
		/// Atualiza o status do checklist
		/// </summary>
		void UpdateStatus(ChecklistStatus status);

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

		/// <summary>
		/// Atualiza o cliente associado ao checklist
		/// </summary>
		void UpdateCustomer(long customerId);
	}
}