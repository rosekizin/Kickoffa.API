using Kickoffa.API.Application.Interfaces.Checkilists;
using Kickoffa.API.Application.Interfaces.Factories;
using Kickoffa.API.Contracts.Checklist;
using Kickoffa.API.Domain.Models;
using Kickoffa.API.Domain.Repositories;
using System.Globalization;
using System.Text;

namespace Kickoffa.API.Application.Services.Checklists
{
	public class CreateChecklistService : ICreateChecklistService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly ISectionFactory _sectionFactory;
		private readonly IChecklistFactory _checklistFactory;
		private readonly IChecklistRepository _checklistRepository;
		private readonly IMapChecklistToResponse _mapChecklistToResponse;

		/// <summary>
		/// Inicializa uma nova instância do ChecklistService
		/// </summary>
		/// <param name="checklistRepository">Repositório de checklists</param>
		/// <param name="unitOfWork">Unit of Work para transações</param>
		public CreateChecklistService(
			IUnitOfWork unitOfWork,
			ISectionFactory sectionFactory,
			IChecklistFactory checklistFactory,
			IChecklistRepository checklistRepository,
			IMapChecklistToResponse mapChecklistToResponse)
		{
			_unitOfWork = unitOfWork;
			_sectionFactory = sectionFactory;
			_checklistFactory = checklistFactory;
			_checklistRepository = checklistRepository;
			_mapChecklistToResponse = mapChecklistToResponse;
		}

		/// <inheritdoc />
		public async Task<ChecklistResponse> CreateAsync(long ownerId, ChecklistRequest request, CancellationToken cancellationToken)
		{
			// Gerar slug único baseado no título
			var slug = await GenerateUniqueSlugAsync(request.Title, cancellationToken);

			// Criar checklist
			var checklist = _checklistFactory.CreateChecklist(
				ownerId: ownerId,
				title: request.Title,
				customSlug: slug,
				description: request.Description,
				dueDate: request.Deadline
			);

			// Adicionar seções
			foreach (var sectionRequest in request.Sections.OrderBy(s => s.Order))
			{
				var section = await _sectionFactory.CreateSection(sectionRequest, cancellationToken);
				checklist.AddSection(section);
			}

			// Salvar no repositório
			await _checklistRepository.AddAsync((Checklist)checklist, cancellationToken);
			await _unitOfWork.SaveChangesAsync(cancellationToken);

			return _mapChecklistToResponse.MapToResponse(checklist);
		}

		/// <summary>
		/// Gera um slug único baseado no título
		/// </summary>
		private async Task<string> GenerateUniqueSlugAsync(string title, CancellationToken cancellationToken)
		{
			var baseSlug = GenerateSlugFromTitle(title);
			var slug = baseSlug;
			var counter = 1;

			while (await _checklistRepository.ExistsBySlugAsync(slug, null, cancellationToken))
			{
				slug = $"{baseSlug}-{counter}";
				counter++;
			}

			return slug;
		}
		/*
		/// <summary>
		/// Gera slug a partir do título
		/// </summary>
		private static string GenerateSlugFromTitle(string title)
		{
			return title
				.ToLowerInvariant()
				.Replace(" ", "-")
				.Replace("ã", "a")
				.Replace("á", "a")
				.Replace("à", "a")
				.Replace("â", "a")
				.Replace("é", "e")
				.Replace("ê", "e")
				.Replace("í", "i")
				.Replace("ó", "o")
				.Replace("ô", "o")
				.Replace("õ", "o")
				.Replace("ú", "u")
				.Replace("ü", "u")
				.Replace("ç", "c")
				.Replace("ñ", "n")
				.Where(c => char.IsLetterOrDigit(c) || c == '-')
				.Aggregate("", (current, c) => current + c)
				.Trim('-');
		}*/

		/// <summary>
		/// Gera slug a partir do título
		/// </summary>
		private static string GenerateSlugFromTitle(string title)
		{
			if (string.IsNullOrWhiteSpace(title))
				return string.Empty;

			string normalized = title.Normalize(NormalizationForm.FormD);
			var builder = new StringBuilder(capacity: normalized.Length);
			bool lastWasHyphen = false;

			foreach (var c in normalized)
			{
				var category = CharUnicodeInfo.GetUnicodeCategory(c);

				if (category == UnicodeCategory.NonSpacingMark)
					continue;

				char lower = char.ToLowerInvariant(c);

				if (char.IsLetterOrDigit(lower))
				{
					builder.Append(lower);
					lastWasHyphen = false;
				}
				else if ((lower == ' ' || lower == '-' || lower == '_') && !lastWasHyphen)
				{
					builder.Append('-');
					lastWasHyphen = true;
				}
				// ignora outros caracteres especiais
			}

			// Remove hífens do início e fim
			return builder.ToString().Trim('-');
		}
	}
}