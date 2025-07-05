using Kickoffa.API.Data.EntityFramework.Context;
using Kickoffa.API.Data.Repositories.Base;
using Kickoffa.API.Domain.Models.Items.Base;
using Kickoffa.API.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.Repositories
{
	/// <summary>
	/// Implementação do repositório de Item
	/// </summary>
	public class ItemRepository : BaseRepository<Item>, IItemRepository
	{
		public ItemRepository(KickoffaDbContext context) : base(context)
		{
		}

		/// <summary>
		/// Busca itens por seção
		/// </summary>
		public async Task<IEnumerable<Item>> GetBySectionIdAsync(long sectionId, CancellationToken cancellationToken)
		{
			return await _context.Items
				.Where(i => i.SectionId == sectionId)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca itens por seção ordenados por Order
		/// </summary>
		public async Task<IEnumerable<Item>> GetBySectionIdOrderedAsync(long sectionId, CancellationToken cancellationToken)
		{
			return await _context.Items
				.Where(i => i.SectionId == sectionId)
				.OrderBy(i => i.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Obtém a próxima ordem disponível para um novo item
		/// </summary>
		public async Task<int> GetNextOrderAsync(long sectionId, CancellationToken cancellationToken)
		{
			var maxOrder = await _context.Items
				.Where(i => i.SectionId == sectionId)
				.MaxAsync(i => (int?)i.Order, cancellationToken);

			return (maxOrder ?? 0) + 1;
		}

		/// <summary>
		/// Reordena itens de uma seção
		/// </summary>
		public async Task ReorderItemsAsync(long sectionId, Dictionary<long, int> itemOrders, CancellationToken cancellationToken)
		{
			var items = await _context.Items
				.Where(i => i.SectionId == sectionId && itemOrders.Keys.Contains(i.Id))
				.ToListAsync(cancellationToken);

			foreach (var item in items)
			{
				if (itemOrders.TryGetValue(item.Id, out var newOrder))
				{
					item.UpdateOrder(newOrder);
				}
			}

			await _context.SaveChangesAsync(cancellationToken);
		}

		/// <summary>
		/// Busca itens obrigatórios por seção
		/// </summary>
		public async Task<IEnumerable<Item>> GetRequiredBySectionIdAsync(long sectionId, CancellationToken cancellationToken)
		{
			return await _context.Items
				.Where(i => i.SectionId == sectionId && i.IsRequired)
				.OrderBy(i => i.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Override para incluir relacionamentos por padrão
		/// </summary>
		public override async Task<Item?> GetByIdAsync(long id, CancellationToken cancellationToken)
		{
			return await _context.Items
				.Include(i => i.Section)
				.Include(i => i.Status)
				.FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
		}

		/// <summary>
		/// Override para incluir relacionamentos por padrão
		/// </summary>
		public override async Task<IEnumerable<Item>> GetAllAsync(CancellationToken cancellationToken)
		{
			return await _context.Items
				.Include(i => i.Section)
				.Include(i => i.Status)
				.OrderBy(i => i.SectionId)
				.ThenBy(i => i.Order)
				.ToListAsync(cancellationToken);
		}
	}
}
