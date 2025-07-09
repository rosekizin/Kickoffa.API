using Kickoffa.API.Data.EntityFramework.Context;
using Kickoffa.API.Data.Repositories.Base;
using Kickoffa.API.Domain.Interfaces.Models;
using Kickoffa.API.Domain.Models;
using Kickoffa.API.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.Repositories
{
	/// <summary>
	/// Implementação do repositório de Section
	/// </summary>
	public class SectionRepository : BaseRepository<ISection, Section>, ISectionRepository
	{
		public SectionRepository(KickoffaDbContext context) : base(context)
		{
		}

		/// <summary>
		/// Busca seções por checklist
		/// </summary>
		public async Task<IEnumerable<ISection>> GetByChecklistIdAsync(long checklistId, CancellationToken cancellationToken)
		{
			return await _context.Sections
				.Where(s => s.ChecklistId == checklistId)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca seções por checklist ordenadas por Order
		/// </summary>
		public async Task<IEnumerable<ISection>> GetByChecklistIdOrderedAsync(long checklistId, CancellationToken cancellationToken)
		{
			return await _context.Sections
				.Where(s => s.ChecklistId == checklistId)
				.OrderBy(s => s.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Obtém a próxima ordem disponível para uma nova seção
		/// </summary>
		public async Task<int> GetNextOrderAsync(long checklistId, CancellationToken cancellationToken)
		{
			var maxOrder = await _context.Sections
				.Where(s => s.ChecklistId == checklistId)
				.MaxAsync(s => (int?)s.Order, cancellationToken);

			return (maxOrder ?? 0) + 1;
		}

		/// <summary>
		/// Reordena seções de um checklist
		/// </summary>
		public async Task ReorderSectionsAsync(long checklistId, Dictionary<long, int> sectionOrders, CancellationToken cancellationToken)
		{
			var sections = await _context.Sections
				.Where(s => s.ChecklistId == checklistId && sectionOrders.Keys.Contains(s.Id))
				.ToListAsync(cancellationToken);

			foreach (var section in sections)
			{
				if (sectionOrders.TryGetValue(section.Id, out var newOrder))
				{
					section.UpdateOrder(newOrder);
				}
			}

			await _context.SaveChangesAsync(cancellationToken);
		}

		/// <summary>
		/// Override para incluir relacionamentos por padrão
		/// </summary>
		public override async Task<ISection?> GetByIdAsync(long id, CancellationToken cancellationToken)
		{
			return await _context.Sections
				.Include(s => s.Checklist)
				.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
		}

		/// <summary>
		/// Override para incluir relacionamentos por padrão
		/// </summary>
		public override async Task<IEnumerable<ISection>> GetAllAsync(CancellationToken cancellationToken)
		{
			return await _context.Sections
				.Include(s => s.Checklist)
				.OrderBy(s => s.ChecklistId)
				.ThenBy(s => s.Order)
				.ToListAsync(cancellationToken);
		}
	}
}