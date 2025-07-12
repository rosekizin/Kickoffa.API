using Kickoffa.API.Data.EntityFramework.Context;
using Kickoffa.API.Data.Repositories.Base;
using Kickoffa.API.Domain.Interfaces.Models.Components;
using Kickoffa.API.Domain.Models.Components;
using Kickoffa.API.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.Repositories
{
	/// <summary>
	/// Implementação do repositório de ConfirmationComponent
	/// </summary>
	public class ConfirmationComponentRepository : BaseRepository<IConfirmationComponent, ConfirmationComponent>, IConfirmationComponentRepository
	{
		public ConfirmationComponentRepository(KickoffaDbContext context) : base(context)
		{
		}

		/// <summary>
		/// Busca components de confirmação por seção
		/// </summary>
		public async Task<IEnumerable<IConfirmationComponent>> GetBySectionIdAsync(long sectionId, CancellationToken cancellationToken)
		{
			return await _context.ConfirmationComponents
				.Where(c => c.SectionId == sectionId)
				.OrderBy(c => c.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca components de confirmação por texto de confirmação
		/// </summary>
		public async Task<IEnumerable<IConfirmationComponent>> GetByConfirmationTextAsync(string confirmationText, CancellationToken cancellationToken)
		{
			return await _context.ConfirmationComponents
				.Where(c => c.ConfirmationText == confirmationText)
				.OrderBy(c => c.SectionId)
				.ThenBy(c => c.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca components de confirmação que contêm o texto especificado
		/// </summary>
		public async Task<IEnumerable<IConfirmationComponent>> SearchByConfirmationTextAsync(string searchText, CancellationToken cancellationToken)
		{
			return await _context.ConfirmationComponents
				.Where(c => c.ConfirmationText != null)
				.Where(c => c.ConfirmationText!.Contains(searchText, StringComparison.OrdinalIgnoreCase))
				.OrderBy(c => c.SectionId)
				.ThenBy(c => c.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca components de confirmação obrigatórios por seção
		/// </summary>
		public async Task<IEnumerable<IConfirmationComponent>> GetRequiredBySectionIdAsync(long sectionId, CancellationToken cancellationToken)
		{
			return await _context.ConfirmationComponents
				.Where(c => c.SectionId == sectionId && c.IsRequired)
				.OrderBy(c => c.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca components de confirmação por checklist
		/// </summary>
		public async Task<IEnumerable<IConfirmationComponent>> GetByChecklistIdAsync(long checklistId, CancellationToken cancellationToken)
		{
			return await _context.ConfirmationComponents
				.Include(c => c.Section)
				.Where(c => c.Section.ChecklistId == checklistId)
				.OrderBy(c => c.Section.Order)
				.ThenBy(c => c.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Conta components de confirmação por seção
		/// </summary>
		public async Task<int> CountBySectionIdAsync(long sectionId, CancellationToken cancellationToken)
		{
			return await _context.ConfirmationComponents
				.CountAsync(c => c.SectionId == sectionId, cancellationToken);
		}

		/// <summary>
		/// Busca components de confirmação por múltiplas seções
		/// </summary>
		public async Task<IEnumerable<IConfirmationComponent>> GetBySectionIdsAsync(IEnumerable<long> sectionIds, CancellationToken cancellationToken)
		{
			return await _context.ConfirmationComponents
				.Where(c => sectionIds.Contains(c.SectionId))
				.OrderBy(c => c.SectionId)
				.ThenBy(c => c.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Conta components de confirmação obrigatórios por seção
		/// </summary>
		public async Task<int> CountRequiredBySectionIdAsync(long sectionId, CancellationToken cancellationToken)
		{
			return await _context.ConfirmationComponents
				.CountAsync(c => c.SectionId == sectionId && c.IsRequired, cancellationToken);
		}

		/// <summary>
		/// Override para incluir relacionamentos por padrão
		/// </summary>
		public override async Task<IConfirmationComponent?> GetByIdAsync(long id, CancellationToken cancellationToken)
		{
			return await _context.ConfirmationComponents
				.Include(c => c.Section)
				.Include(c => c.Status)
				.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
		}

		/// <summary>
		/// Override para incluir relacionamentos por padrão
		/// </summary>
		public override async Task<IEnumerable<IConfirmationComponent>> GetAllAsync(CancellationToken cancellationToken)
		{
			return await _context.ConfirmationComponents
				.Include(c => c.Section)
				.Include(c => c.Status)
				.OrderBy(c => c.SectionId)
				.ThenBy(c => c.Order)
				.ToListAsync(cancellationToken);
		}
	}
}