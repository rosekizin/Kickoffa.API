
using Kickoffa.API.Domain.Models.Base;

namespace Kickoffa.API.Domain.Models
{
	public class Checklist : BaseEntity
	{
		/// <summary>
		/// Construtor para criação de novo checklist
		/// </summary>
		public Checklist(long ownerId, string title, string slug, string? description, DateTime? dueDate)
		{
			OwnerId = ownerId;
			Title = title ?? throw new ArgumentNullException(nameof(title));
			Slug = slug ?? throw new ArgumentNullException(nameof(slug));
			Description = description;
			DueDate = dueDate;
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
		public string Title { get; private set; }
		public string Slug { get; private set; } // url amigável para o checklist
		public string? Description { get; private set; }
		public DateTime? DueDate { get; private set; }
		public string? AccessToken { get; private set; } // Token único para compartilhar
		public bool IsPublished { get; private set; }

		// Relacionamentos
		public virtual ICollection<Section> Sections { get; private set; }

		/// <summary>
		/// Atualiza o título do checklist
		/// </summary>
		public void UpdateTitle(string title)
		{
			Title = title ?? throw new ArgumentNullException(nameof(title));
			UpdateLastUpdatedDate();
		}

		/// <summary>
		/// Atualiza a descrição do checklist
		/// </summary>
		public void UpdateDescription(string? description)
		{
			Description = description;
			UpdateLastUpdatedDate();
		}

		/// <summary>
		/// Atualiza a data de vencimento
		/// </summary>
		public void UpdateDueDate(DateTime? dueDate)
		{
			DueDate = dueDate;
			UpdateLastUpdatedDate();
		}

		/// <summary>
		/// Publica o checklist
		/// </summary>
		public void Publish()
		{
			IsPublished = true;
			UpdateLastUpdatedDate();
		}

		/// <summary>
		/// Despublica o checklist
		/// </summary>
		public void Unpublish()
		{
			IsPublished = false;
			UpdateLastUpdatedDate();
		}

		/// <summary>
		/// Regenera o token de acesso
		/// </summary>
		public void RegenerateAccessToken()
		{
			AccessToken = GenerateAccessToken();
			UpdateLastUpdatedDate();
		}

		/// <summary>
		/// Adiciona uma seção ao checklist
		/// </summary>
		public void AddSection(Section section)
		{
			ArgumentNullException.ThrowIfNull(section);

			section.SetChecklist(this);
			Sections.Add(section);
			UpdateLastUpdatedDate();
		}

		/// <summary>
		/// Remove uma seção do checklist
		/// </summary>
		public void RemoveSection(Section section)
		{
			ArgumentNullException.ThrowIfNull(section);

			Sections.Remove(section);
			UpdateLastUpdatedDate();
		}

		/// <summary>
		/// Gera um token de acesso único
		/// </summary>
		private static string GenerateAccessToken()
		{
			return Guid.NewGuid().ToString("N")[..16]; // 16 caracteres
		}
	}
}