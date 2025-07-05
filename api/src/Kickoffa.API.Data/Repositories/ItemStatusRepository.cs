using Kickoffa.API.Data.EntityFramework.Context;
using Kickoffa.API.Data.Repositories.Base;
using Kickoffa.API.Domain.Models.Items;
using Kickoffa.API.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.Repositories
{
	/// <summary>
	/// Implementação do repositório de ItemStatus
	/// </summary>
	public class ItemStatusRepository : BaseRepository<ItemStatus>, IItemStatusRepository
	{
		public ItemStatusRepository(KickoffaDbContext context) : base(context)
		{
		}

		/// <summary>
		/// Busca status por item
		/// </summary>
		public async Task<ItemStatus?> GetByItemIdAsync(long itemId, CancellationToken cancellationToken)
		{
			return await _context.ItemStatuses
				.FirstOrDefaultAsync(s => s.ItemId == itemId, cancellationToken);
		}

		/// <summary>
		/// Busca status por múltiplos itens
		/// </summary>
		public async Task<IEnumerable<ItemStatus>> GetByItemIdsAsync(IEnumerable<long> itemIds, CancellationToken cancellationToken)
		{
			return await _context.ItemStatuses
				.Where(s => itemIds.Contains(s.ItemId))
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca status completados por seção
		/// </summary>
		public async Task<IEnumerable<ItemStatus>> GetCompletedBySectionIdAsync(long sectionId, CancellationToken cancellationToken)
		{
			return await _context.ItemStatuses
				.Include(s => s.Item)
				.Where(s => s.Item.SectionId == sectionId && s.IsCompleted)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca status completados por checklist
		/// </summary>
		public async Task<IEnumerable<ItemStatus>> GetCompletedByChecklistIdAsync(long checklistId, CancellationToken cancellationToken)
		{
			return await _context.ItemStatuses
				.Include(s => s.Item)
					.ThenInclude(i => i.Section)
				.Where(s => s.Item.Section.ChecklistId == checklistId && s.IsCompleted)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Conta itens completados por checklist
		/// </summary>
		public async Task<int> CountCompletedByChecklistIdAsync(long checklistId, CancellationToken cancellationToken)
		{
			return await _context.ItemStatuses
				.Include(s => s.Item)
					.ThenInclude(i => i.Section)
				.CountAsync(s => s.Item.Section.ChecklistId == checklistId && s.IsCompleted, cancellationToken);
		}

		/// <summary>
		/// Conta total de itens por checklist
		/// </summary>
		public async Task<int> CountTotalByChecklistIdAsync(long checklistId, CancellationToken cancellationToken)
		{
			return await _context.Items
				.Include(i => i.Section)
				.CountAsync(i => i.Section.ChecklistId == checklistId, cancellationToken);
		}

		/// <summary>
		/// Override para incluir relacionamentos por padrão
		/// </summary>
		public override async Task<ItemStatus?> GetByIdAsync(long id, CancellationToken cancellationToken)
		{
			return await _context.ItemStatuses
				.Include(s => s.Item)
				.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
		}

		/// <summary>
		/// Override para incluir relacionamentos por padrão
		/// </summary>
		public override async Task<IEnumerable<ItemStatus>> GetAllAsync(CancellationToken cancellationToken)
		{
			return await _context.ItemStatuses
				.Include(s => s.Item)
				.OrderByDescending(s => s.CreatedDateUtc)
				.ToListAsync(cancellationToken);
		}
	}
}