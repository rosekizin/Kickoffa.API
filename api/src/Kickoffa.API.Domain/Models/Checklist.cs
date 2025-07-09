
using Kickoffa.API.Domain.Interfaces.Models;
using Kickoffa.API.Domain.Models.Base;

namespace Kickoffa.API.Domain.Models
{
	public class Checklist : BaseEntity, IChecklist
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
			DueDate = dueDate;
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
	}
}