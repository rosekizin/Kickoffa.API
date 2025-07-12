using Kickoffa.API.Data.EntityFramework.Context;
using Kickoffa.API.Data.Repositories.Base;
using Kickoffa.API.Domain.Interfaces.Models.Components;
using Kickoffa.API.Domain.Models.Components;
using Kickoffa.API.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.Repositories
{
	/// <summary>
	/// Implementação do repositório de ComponentStatus
	/// </summary>
	public class ComponentStatusRepository : BaseRepository<IComponentStatus, ComponentStatus>, IComponentStatusRepository
	{
		public ComponentStatusRepository(KickoffaDbContext context) : base(context)
		{
		}

		/// <summary>
		/// Busca status por componente
		/// </summary>
		public async Task<IComponentStatus?> GetByComponentIdAsync(long componentId, CancellationToken cancellationToken)
		{
			return await _context.ComponentStatuses
				.FirstOrDefaultAsync(s => s.ComponentId == componentId, cancellationToken);
		}

		/// <summary>
		/// Busca status por múltiplos componentes
		/// </summary>
		public async Task<IEnumerable<IComponentStatus>> GetByComponentIdsAsync(IEnumerable<long> componentIds, CancellationToken cancellationToken)
		{
			return await _context.ComponentStatuses
				.Where(s => componentIds.Contains(s.ComponentId))
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca status completados por seção
		/// </summary>
		public async Task<IEnumerable<IComponentStatus>> GetCompletedBySectionIdAsync(long sectionId, CancellationToken cancellationToken)
		{
			return await _context.ComponentStatuses
				.Include(s => s.Component)
				.Where(s => s.Component.SectionId == sectionId && s.IsCompleted)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca status completados por checklist
		/// </summary>
		public async Task<IEnumerable<IComponentStatus>> GetCompletedByChecklistIdAsync(long checklistId, CancellationToken cancellationToken)
		{
			return await _context.ComponentStatuses
				.Include(s => s.Component)
					.ThenInclude(i => i.Section)
				.Where(s => s.Component.Section.ChecklistId == checklistId && s.IsCompleted)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Conta componentes completados por checklist
		/// </summary>
		public async Task<int> CountCompletedByChecklistIdAsync(long checklistId, CancellationToken cancellationToken)
		{
			return await _context.ComponentStatuses
				.Include(s => s.Component)
					.ThenInclude(i => i.Section)
				.CountAsync(s => s.Component.Section.ChecklistId == checklistId && s.IsCompleted, cancellationToken);
		}

		/// <summary>
		/// Conta total de componentes por checklist
		/// </summary>
		public async Task<int> CountTotalByChecklistIdAsync(long checklistId, CancellationToken cancellationToken)
		{
			return await _context.Components
				.Include(i => i.Section)
				.CountAsync(i => i.Section.ChecklistId == checklistId, cancellationToken);
		}

		/// <summary>
		/// Override para incluir relacionamentos por padrão
		/// </summary>
		public override async Task<IComponentStatus?> GetByIdAsync(long id, CancellationToken cancellationToken)
		{
			return await _context.ComponentStatuses
				.Include(s => s.Component)
				.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
		}

		/// <summary>
		/// Override para incluir relacionamentos por padrão
		/// </summary>
		public override async Task<IEnumerable<IComponentStatus>> GetAllAsync(CancellationToken cancellationToken)
		{
			return await _context.ComponentStatuses
				.Include(s => s.Component)
				.OrderByDescending(s => s.CreatedDateUtc)
				.ToListAsync(cancellationToken);
		}
	}
}