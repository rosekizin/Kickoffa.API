using Kickoffa.API.Data.EntityFramework.Context;
using Kickoffa.API.Data.Repositories.Base;
using Kickoffa.API.Domain.Interfaces.Models.Components;
using Kickoffa.API.Domain.Models.Components;
using Kickoffa.API.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.Repositories
{
	/// <summary>
	/// Implementação do repositório de CheckboxComponent
	/// </summary>
	public class CheckboxComponentRepository : BaseRepository<ICheckboxComponent, CheckboxComponent>, ICheckboxComponentRepository
	{
		public CheckboxComponentRepository(KickoffaDbContext context) : base(context)
		{
		}

		/// <summary>
		/// Busca components de checkbox por seção
		/// </summary>
		public async Task<IEnumerable<ICheckboxComponent>> GetBySectionIdAsync(long sectionId, CancellationToken cancellationToken)
		{
			return await _context.CheckboxComponents
				.Where(c => c.SectionId == sectionId)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca components de checkbox ordenados por seção
		/// </summary>
		public async Task<IEnumerable<ICheckboxComponent>> GetBySectionIdOrderedAsync(long sectionId, CancellationToken cancellationToken)
		{
			return await _context.CheckboxComponents
				.Where(c => c.SectionId == sectionId)
				.OrderBy(c => c.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca components de checkbox obrigatórios por seção
		/// </summary>
		public async Task<IEnumerable<ICheckboxComponent>> GetRequiredBySectionIdAsync(long sectionId, CancellationToken cancellationToken)
		{
			return await _context.CheckboxComponents
				.Where(c => c.SectionId == sectionId && c.IsRequired)
				.OrderBy(c => c.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca components de checkbox por checklist
		/// </summary>
		public async Task<IEnumerable<ICheckboxComponent>> GetByChecklistIdAsync(long checklistId, CancellationToken cancellationToken)
		{
			return await _context.CheckboxComponents
				.Include(c => c.Section)
				.Where(c => c.Section.ChecklistId == checklistId)
				.OrderBy(c => c.Section.Order)
				.ThenBy(c => c.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca components de checkbox por múltiplas seções
		/// </summary>
		public async Task<IEnumerable<ICheckboxComponent>> GetBySectionIdsAsync(IEnumerable<long> sectionIds, CancellationToken cancellationToken)
		{
			return await _context.CheckboxComponents
				.Where(c => sectionIds.Contains(c.SectionId))
				.OrderBy(c => c.SectionId)
				.ThenBy(c => c.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Conta components de checkbox por seção
		/// </summary>
		public async Task<int> CountBySectionIdAsync(long sectionId, CancellationToken cancellationToken)
		{
			return await _context.CheckboxComponents
				.CountAsync(c => c.SectionId == sectionId, cancellationToken);
		}

		/// <summary>
		/// Conta components de checkbox obrigatórios por seção
		/// </summary>
		public async Task<int> CountRequiredBySectionIdAsync(long sectionId, CancellationToken cancellationToken)
		{
			return await _context.CheckboxComponents
				.CountAsync(c => c.SectionId == sectionId && c.IsRequired, cancellationToken);
		}

		/// <summary>
		/// Busca components de checkbox por status de obrigatoriedade
		/// </summary>
		public async Task<IEnumerable<ICheckboxComponent>> GetByRequiredStatusAsync(bool isRequired, CancellationToken cancellationToken)
		{
			return await _context.CheckboxComponents
				.Where(c => c.IsRequired == isRequired)
				.OrderBy(c => c.SectionId)
				.ThenBy(c => c.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca components de checkbox completados por seção
		/// </summary>
		public async Task<IEnumerable<ICheckboxComponent>> GetCompletedBySectionIdAsync(long sectionId, CancellationToken cancellationToken)
		{
			return await _context.CheckboxComponents
				.Include(c => c.Status)
				.Where(c => c.SectionId == sectionId && c.Status != null && c.Status.IsCompleted)
				.OrderBy(c => c.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Conta components de checkbox completados por seção
		/// </summary>
		public async Task<int> CountCompletedBySectionIdAsync(long sectionId, CancellationToken cancellationToken)
		{
			return await _context.CheckboxComponents
				.Include(c => c.Status)
				.CountAsync(c => c.SectionId == sectionId && c.Status != null && c.Status.IsCompleted, cancellationToken);
		}

		/// <summary>
		/// Override para incluir relacionamentos por padrão
		/// </summary>
		public override async Task<ICheckboxComponent?> GetByIdAsync(long id, CancellationToken cancellationToken)
		{
			return await _context.CheckboxComponents
				.Include(c => c.Section)
				.Include(c => c.Status)
				.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
		}

		/// <summary>
		/// Override para incluir relacionamentos por padrão
		/// </summary>
		public override async Task<IEnumerable<ICheckboxComponent>> GetAllAsync(CancellationToken cancellationToken)
		{
			return await _context.CheckboxComponents
				.Include(c => c.Section)
				.Include(c => c.Status)
				.OrderBy(c => c.SectionId)
				.ThenBy(c => c.Order)
				.ToListAsync(cancellationToken);
		}
	}
}