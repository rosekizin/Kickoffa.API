using Kickoffa.API.Data.EntityFramework.Context;
using Kickoffa.API.Data.Repositories.Base;
using Kickoffa.API.Domain.Models.Items;
using Kickoffa.API.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.Repositories
{
	/// <summary>
	/// Implementação do repositório de ConfirmationItem
	/// </summary>
	public class ConfirmationItemRepository : BaseRepository<ConfirmationItem>, IConfirmationItemRepository
	{
		public ConfirmationItemRepository(KickoffaDbContext context) : base(context)
		{
		}

		/// <summary>
		/// Busca itens de confirmação por seção
		/// </summary>
		public async Task<IEnumerable<ConfirmationItem>> GetBySectionIdAsync(long sectionId, CancellationToken cancellationToken)
		{
			return await _context.ConfirmationItems
				.Where(c => c.SectionId == sectionId)
				.OrderBy(c => c.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca itens de confirmação por texto de confirmação
		/// </summary>
		public async Task<IEnumerable<ConfirmationItem>> GetByConfirmationTextAsync(string confirmationText, CancellationToken cancellationToken)
		{
			return await _context.ConfirmationItems
				.Where(c => c.ConfirmationText == confirmationText)
				.OrderBy(c => c.SectionId)
				.ThenBy(c => c.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca itens de confirmação que contêm o texto especificado
		/// </summary>
		public async Task<IEnumerable<ConfirmationItem>> SearchByConfirmationTextAsync(string searchText, CancellationToken cancellationToken)
		{
			return await _context.ConfirmationItems
				.Where(c => c.ConfirmationText.Contains(searchText))
				.OrderBy(c => c.SectionId)
				.ThenBy(c => c.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca itens de confirmação obrigatórios por seção
		/// </summary>
		public async Task<IEnumerable<ConfirmationItem>> GetRequiredBySectionIdAsync(long sectionId, CancellationToken cancellationToken)
		{
			return await _context.ConfirmationItems
				.Where(c => c.SectionId == sectionId && c.IsRequired)
				.OrderBy(c => c.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca itens de confirmação por checklist
		/// </summary>
		public async Task<IEnumerable<ConfirmationItem>> GetByChecklistIdAsync(long checklistId, CancellationToken cancellationToken)
		{
			return await _context.ConfirmationItems
				.Include(c => c.Section)
				.Where(c => c.Section.ChecklistId == checklistId)
				.OrderBy(c => c.Section.Order)
				.ThenBy(c => c.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Conta itens de confirmação por seção
		/// </summary>
		public async Task<int> CountBySectionIdAsync(long sectionId, CancellationToken cancellationToken)
		{
			return await _context.ConfirmationItems
				.CountAsync(c => c.SectionId == sectionId, cancellationToken);
		}

		/// <summary>
		/// Busca itens de confirmação por múltiplas seções
		/// </summary>
		public async Task<IEnumerable<ConfirmationItem>> GetBySectionIdsAsync(IEnumerable<long> sectionIds, CancellationToken cancellationToken)
		{
			return await _context.ConfirmationItems
				.Where(c => sectionIds.Contains(c.SectionId))
				.OrderBy(c => c.SectionId)
				.ThenBy(c => c.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca itens de confirmação por tamanho do texto
		/// </summary>
		public async Task<IEnumerable<ConfirmationItem>> GetByTextLengthRangeAsync(int minLength, int maxLength, CancellationToken cancellationToken)
		{
			return await _context.ConfirmationItems
				.Where(c => c.ConfirmationText.Length >= minLength && c.ConfirmationText.Length <= maxLength)
				.OrderBy(c => c.SectionId)
				.ThenBy(c => c.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Conta itens de confirmação obrigatórios por seção
		/// </summary>
		public async Task<int> CountRequiredBySectionIdAsync(long sectionId, CancellationToken cancellationToken)
		{
			return await _context.ConfirmationItems
				.CountAsync(c => c.SectionId == sectionId && c.IsRequired, cancellationToken);
		}

		/// <summary>
		/// Override para incluir relacionamentos por padrão
		/// </summary>
		public override async Task<ConfirmationItem?> GetByIdAsync(long id, CancellationToken cancellationToken)
		{
			return await _context.ConfirmationItems
				.Include(c => c.Section)
				.Include(c => c.Status)
				.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
		}

		/// <summary>
		/// Override para incluir relacionamentos por padrão
		/// </summary>
		public override async Task<IEnumerable<ConfirmationItem>> GetAllAsync(CancellationToken cancellationToken)
		{
			return await _context.ConfirmationItems
				.Include(c => c.Section)
				.Include(c => c.Status)
				.OrderBy(c => c.SectionId)
				.ThenBy(c => c.Order)
				.ToListAsync(cancellationToken);
		}
	}
}
