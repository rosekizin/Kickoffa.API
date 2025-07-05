using Kickoffa.API.Data.EntityFramework.Context;
using Kickoffa.API.Data.Repositories.Base;
using Kickoffa.API.Domain.Models.Items;
using Kickoffa.API.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.Repositories
{
	/// <summary>
	/// Implementação do repositório de TextItem
	/// </summary>
	public class TextItemRepository : BaseRepository<TextItem>, ITextItemRepository
	{
		public TextItemRepository(KickoffaDbContext context) : base(context)
		{
		}

		/// <summary>
		/// Busca itens de texto por seção
		/// </summary>
		public async Task<IEnumerable<TextItem>> GetBySectionIdAsync(long sectionId, CancellationToken cancellationToken)
		{
			return await _context.TextItems
				.Where(t => t.SectionId == sectionId)
				.OrderBy(t => t.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca itens de texto por placeholder
		/// </summary>
		public async Task<IEnumerable<TextItem>> GetByPlaceholderAsync(string placeholder, CancellationToken cancellationToken)
		{
			return await _context.TextItems
				.Where(t => t.Placeholder == placeholder)
				.OrderBy(t => t.SectionId)
				.ThenBy(t => t.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca itens de texto por tamanho máximo
		/// </summary>
		public async Task<IEnumerable<TextItem>> GetByMaxLengthAsync(int? maxLength, CancellationToken cancellationToken)
		{
			return await _context.TextItems
				.Where(t => t.MaxLength == maxLength)
				.OrderBy(t => t.SectionId)
				.ThenBy(t => t.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca itens de texto obrigatórios por seção
		/// </summary>
		public async Task<IEnumerable<TextItem>> GetRequiredBySectionIdAsync(long sectionId, CancellationToken cancellationToken)
		{
			return await _context.TextItems
				.Where(t => t.SectionId == sectionId && t.IsRequired)
				.OrderBy(t => t.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca itens de texto por checklist
		/// </summary>
		public async Task<IEnumerable<TextItem>> GetByChecklistIdAsync(long checklistId, CancellationToken cancellationToken)
		{
			return await _context.TextItems
				.Include(t => t.Section)
				.Where(t => t.Section.ChecklistId == checklistId)
				.OrderBy(t => t.Section.Order)
				.ThenBy(t => t.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Conta itens de texto por seção
		/// </summary>
		public async Task<int> CountBySectionIdAsync(long sectionId, CancellationToken cancellationToken)
		{
			return await _context.TextItems
				.CountAsync(t => t.SectionId == sectionId, cancellationToken);
		}

		/// <summary>
		/// Busca itens de texto por múltiplas seções
		/// </summary>
		public async Task<IEnumerable<TextItem>> GetBySectionIdsAsync(IEnumerable<long> sectionIds, CancellationToken cancellationToken)
		{
			return await _context.TextItems
				.Where(t => sectionIds.Contains(t.SectionId))
				.OrderBy(t => t.SectionId)
				.ThenBy(t => t.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca itens de texto que contêm o placeholder especificado
		/// </summary>
		public async Task<IEnumerable<TextItem>> SearchByPlaceholderAsync(string searchText, CancellationToken cancellationToken)
		{
			return await _context.TextItems
				.Where(t => t.Placeholder != null && t.Placeholder.Contains(searchText))
				.OrderBy(t => t.SectionId)
				.ThenBy(t => t.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Override para incluir relacionamentos por padrão
		/// </summary>
		public override async Task<TextItem?> GetByIdAsync(long id, CancellationToken cancellationToken)
		{
			return await _context.TextItems
				.Include(t => t.Section)
				.Include(t => t.Status)
				.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
		}

		/// <summary>
		/// Override para incluir relacionamentos por padrão
		/// </summary>
		public override async Task<IEnumerable<TextItem>> GetAllAsync(CancellationToken cancellationToken)
		{
			return await _context.TextItems
				.Include(t => t.Section)
				.Include(t => t.Status)
				.OrderBy(t => t.SectionId)
				.ThenBy(t => t.Order)
				.ToListAsync(cancellationToken);
		}
	}
}
