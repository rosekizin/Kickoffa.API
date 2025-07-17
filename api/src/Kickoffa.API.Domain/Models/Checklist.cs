
using Kickoffa.API.Domain.Interfaces.Models;
using Kickoffa.API.Domain.Interfaces.Models.Customer;
using Kickoffa.API.Domain.Models.Base;
using Kickoffa.API.Domain.Models.FreelancerCustomer;

namespace Kickoffa.API.Domain.Models
{
	public class Checklist : BaseEntity, IChecklist
	{
		/// <summary>
		/// Construtor para criação de novo checklist
		/// </summary>
		public Checklist(long ownerId, long customerId, string title, string slug, string? description, DateTime? dueDate)
		{
			OwnerId = ownerId;
			CustomerId = customerId;
			Title = title ?? throw new ArgumentNullException(nameof(title));
			Slug = slug ?? throw new ArgumentNullException(nameof(slug));
			Description = description;
			DueDate = CalculateDueDate(dueDate);
			IsPublished = false;
			AccessToken = GenerateAccessToken();
			Sections = [];
		}

		/// <summary>
		/// Construtor sem parâmetros para EF
		/// </summary>
		protected Checklist()
		{
			Title = string.Empty;
			Slug = string.Empty;
			Sections = [];
		}

		public long OwnerId { get; private set; } // ID do usuário que criou o checklist
		public long CustomerId { get; private set; } // ID do cliente associado ao checklist
		public string Title { get; private set; }
		public string Slug { get; private set; } // url amigável para o checklist
		public string? Description { get; private set; }
		public DateTime? DueDate { get; private set; }
		public string? AccessToken { get; private set; } // Token único para compartilhar
		public bool IsPublished { get; private set; }

		// Relacionamentos
		public virtual Customer Customer { get; private set; }
		ICustomer  IChecklist.Customer => Customer;

		public virtual ICollection<Section> Sections { get; private set; }
		IEnumerable<ISection> IChecklist.Sections => Sections;

		/// <inheritdoc/>
		public void UpdateTitle(string title)
		{
			Title = title ?? throw new ArgumentNullException(nameof(title));
			UpdateLastUpdatedDate();
		}

		/// <inheritdoc/>
		public void UpdateDescription(string? description)
		{
			Description = description;
			UpdateLastUpdatedDate();
		}

		/// <inheritdoc/>
		public void UpdateDueDate(DateTime? dueDate)
		{
			DueDate = DateTime.SpecifyKind(dueDate ?? DateTime.Now.AddDays(7), DateTimeKind.Utc);
			UpdateLastUpdatedDate();
		}

		/// <summary>
		/// Atualiza o cliente associado ao checklist
		/// </summary>
		/// <param name="customerId">ID do novo cliente</param>
		public void UpdateCustomer(long customerId)
		{
			CustomerId = customerId;
			UpdateLastUpdatedDate();
		}

		/// <inheritdoc/>
		public void Publish()
		{
			IsPublished = true;
			UpdateLastUpdatedDate();
		}

		/// <inheritdoc/>
		public void Unpublish()
		{
			IsPublished = false;
			UpdateLastUpdatedDate();
		}

		/// <inheritdoc/>
		public void RegenerateAccessToken()
		{
			AccessToken = GenerateAccessToken();
			UpdateLastUpdatedDate();
		}

		/// <inheritdoc/>
		public void AddSection(ISection section)
		{
			ArgumentNullException.ThrowIfNull(section);

			section.SetChecklist(this);
			Sections.Add((Section)section);
			UpdateLastUpdatedDate();
		}

		/// <inheritdoc/>
		public void RemoveSection(ISection section)
		{
			ArgumentNullException.ThrowIfNull(section);

			Sections.Remove((Section)section);
			UpdateLastUpdatedDate();
		}

		/// <summary>
		/// Gera um token de acesso único
		/// </summary>
		private static string GenerateAccessToken()
		{
			return Guid.NewGuid().ToString("N")[..16]; // 16 caracteres
		}

		/// <summary>
		/// Se não informado o due date terá o padrão de 7 dias.
		/// </summary>
		/// <param name="dueDate"></param>
		/// <returns></returns>
		private static DateTime CalculateDueDate(DateTime? dueDate)
		{
			return DateTime.SpecifyKind(dueDate ?? DateTime.Now.AddDays(7), DateTimeKind.Utc);
		}
	}
}