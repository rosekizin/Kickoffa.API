using Kickoffa.API.Data.EntityFramework.Context;
using Kickoffa.API.Data.Repositories.Base;
using Kickoffa.API.Domain.Models.Items;
using Kickoffa.API.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.Repositories
{
	/// <summary>
	/// Implementação do repositório de CheckboxItem
	/// </summary>
	public class CheckboxItemRepository : BaseRepository<CheckboxItem>, ICheckboxItemRepository
	{
		public CheckboxItemRepository(KickoffaDbContext context) : base(context)
		{
		}

		/// <summary>
		/// Busca itens de checkbox por seção
		/// </summary>
		public async Task<IEnumerable<CheckboxItem>> GetBySectionIdAsync(long sectionId, CancellationToken cancellationToken)
		{
			return await _context.CheckboxItems
				.Where(c => c.SectionId == sectionId)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca itens de checkbox ordenados por seção
		/// </summary>
		public async Task<IEnumerable<CheckboxItem>> GetBySectionIdOrderedAsync(long sectionId, CancellationToken cancellationToken)
		{
			return await _context.CheckboxItems
				.Where(c => c.SectionId == sectionId)
				.OrderBy(c => c.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca itens de checkbox obrigatórios por seção
		/// </summary>
		public async Task<IEnumerable<CheckboxItem>> GetRequiredBySectionIdAsync(long sectionId, CancellationToken cancellationToken)
		{
			return await _context.CheckboxItems
				.Where(c => c.SectionId == sectionId && c.IsRequired)
				.OrderBy(c => c.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca itens de checkbox por checklist
		/// </summary>
		public async Task<IEnumerable<CheckboxItem>> GetByChecklistIdAsync(long checklistId, CancellationToken cancellationToken)
		{
			return await _context.CheckboxItems
				.Include(c => c.Section)
				.Where(c => c.Section.ChecklistId == checklistId)
				.OrderBy(c => c.Section.Order)
				.ThenBy(c => c.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca itens de checkbox por múltiplas seções
		/// </summary>
		public async Task<IEnumerable<CheckboxItem>> GetBySectionIdsAsync(IEnumerable<long> sectionIds, CancellationToken cancellationToken)
		{
			return await _context.CheckboxItems
				.Where(c => sectionIds.Contains(c.SectionId))
				.OrderBy(c => c.SectionId)
				.ThenBy(c => c.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Conta itens de checkbox por seção
		/// </summary>
		public async Task<int> CountBySectionIdAsync(long sectionId, CancellationToken cancellationToken)
		{
			return await _context.CheckboxItems
				.CountAsync(c => c.SectionId == sectionId, cancellationToken);
		}

		/// <summary>
		/// Conta itens de checkbox obrigatórios por seção
		/// </summary>
		public async Task<int> CountRequiredBySectionIdAsync(long sectionId, CancellationToken cancellationToken)
		{
			return await _context.CheckboxItems
				.CountAsync(c => c.SectionId == sectionId && c.IsRequired, cancellationToken);
		}

		/// <summary>
		/// Busca itens de checkbox por status de obrigatoriedade
		/// </summary>
		public async Task<IEnumerable<CheckboxItem>> GetByRequiredStatusAsync(bool isRequired, CancellationToken cancellationToken)
		{
			return await _context.CheckboxItems
				.Where(c => c.IsRequired == isRequired)
				.OrderBy(c => c.SectionId)
				.ThenBy(c => c.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca itens de checkbox completados por seção
		/// </summary>
		public async Task<IEnumerable<CheckboxItem>> GetCompletedBySectionIdAsync(long sectionId, CancellationToken cancellationToken)
		{
			return await _context.CheckboxItems
				.Include(c => c.Status)
				.Where(c => c.SectionId == sectionId && c.Status != null && c.Status.IsCompleted)
				.OrderBy(c => c.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Conta itens de checkbox completados por seção
		/// </summary>
		public async Task<int> CountCompletedBySectionIdAsync(long sectionId, CancellationToken cancellationToken)
		{
			return await _context.CheckboxItems
				.Include(c => c.Status)
				.CountAsync(c => c.SectionId == sectionId && c.Status != null && c.Status.IsCompleted, cancellationToken);
		}

		/// <summary>
		/// Override para incluir relacionamentos por padrão
		/// </summary>
		public override async Task<CheckboxItem?> GetByIdAsync(long id, CancellationToken cancellationToken)
		{
			return await _context.CheckboxItems
				.Include(c => c.Section)
				.Include(c => c.Status)
				.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
		}

		/// <summary>
		/// Override para incluir relacionamentos por padrão
		/// </summary>
		public override async Task<IEnumerable<CheckboxItem>> GetAllAsync(CancellationToken cancellationToken)
		{
			return await _context.CheckboxItems
				.Include(c => c.Section)
				.Include(c => c.Status)
				.OrderBy(c => c.SectionId)
				.ThenBy(c => c.Order)
				.ToListAsync(cancellationToken);
		}
	}
}
