using Kickoffa.API.Data.EntityFramework.Context;
using Kickoffa.API.Data.Repositories.Base;
using Kickoffa.API.Domain.Models.Items;
using Kickoffa.API.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.Repositories
{
	/// <summary>
	/// Implementação do repositório de UploadItem
	/// </summary>
	public class UploadItemRepository : BaseRepository<UploadItem>, IUploadItemRepository
	{
		public UploadItemRepository(KickoffaDbContext context) : base(context)
		{
		}

		/// <summary>
		/// Busca itens de upload por seção
		/// </summary>
		public async Task<IEnumerable<UploadItem>> GetBySectionIdAsync(long sectionId, CancellationToken cancellationToken)
		{
			return await _context.UploadItems
				.Where(u => u.SectionId == sectionId)
				.OrderBy(u => u.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca itens de upload por tamanho máximo
		/// </summary>
		public async Task<IEnumerable<UploadItem>> GetByMaxSizeAsync(int? maxSizeMB, CancellationToken cancellationToken)
		{
			return await _context.UploadItems
				.Where(u => u.MaxSizeMB == maxSizeMB)
				.OrderBy(u => u.SectionId)
				.ThenBy(u => u.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca itens de upload por placeholder
		/// </summary>
		public async Task<IEnumerable<UploadItem>> GetByPlaceholderAsync(string placeholder, CancellationToken cancellationToken)
		{
			return await _context.UploadItems
				.Where(u => u.Placeholder == placeholder)
				.OrderBy(u => u.SectionId)
				.ThenBy(u => u.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca itens de upload obrigatórios por seção
		/// </summary>
		public async Task<IEnumerable<UploadItem>> GetRequiredBySectionIdAsync(long sectionId, CancellationToken cancellationToken)
		{
			return await _context.UploadItems
				.Where(u => u.SectionId == sectionId && u.IsRequired)
				.OrderBy(u => u.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca itens de upload por checklist
		/// </summary>
		public async Task<IEnumerable<UploadItem>> GetByChecklistIdAsync(long checklistId, CancellationToken cancellationToken)
		{
			return await _context.UploadItems
				.Include(u => u.Section)
				.Where(u => u.Section.ChecklistId == checklistId)
				.OrderBy(u => u.Section.Order)
				.ThenBy(u => u.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca itens de upload por tipo de arquivo permitido
		/// </summary>
		public async Task<IEnumerable<UploadItem>> GetByAllowedFileTypeAsync(long fileTypeId, CancellationToken cancellationToken)
		{
			return await _context.UploadItems
				.Include(u => u.AllowedFileTypes)
				.Where(u => u.AllowedFileTypes.Any(ft => ft.Id == fileTypeId))
				.OrderBy(u => u.SectionId)
				.ThenBy(u => u.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca itens de upload com arquivos enviados
		/// </summary>
		public async Task<IEnumerable<UploadItem>> GetWithUploadedFilesAsync(long sectionId, CancellationToken cancellationToken)
		{
			return await _context.UploadItems
				.Include(u => u.ItemFiles)
				.Where(u => u.SectionId == sectionId && u.ItemFiles.Any())
				.OrderBy(u => u.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Conta itens de upload por seção
		/// </summary>
		public async Task<int> CountBySectionIdAsync(long sectionId, CancellationToken cancellationToken)
		{
			return await _context.UploadItems
				.CountAsync(u => u.SectionId == sectionId, cancellationToken);
		}

		/// <summary>
		/// Busca itens de upload por múltiplas seções
		/// </summary>
		public async Task<IEnumerable<UploadItem>> GetBySectionIdsAsync(IEnumerable<long> sectionIds, CancellationToken cancellationToken)
		{
			return await _context.UploadItems
				.Where(u => sectionIds.Contains(u.SectionId))
				.OrderBy(u => u.SectionId)
				.ThenBy(u => u.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca itens de upload por faixa de tamanho
		/// </summary>
		public async Task<IEnumerable<UploadItem>> GetBySizeRangeAsync(int? minSizeMB, int? maxSizeMB, CancellationToken cancellationToken)
		{
			var query = _context.UploadItems.AsQueryable();

			if (minSizeMB.HasValue)
				query = query.Where(u => u.MaxSizeMB >= minSizeMB.Value);

			if (maxSizeMB.HasValue)
				query = query.Where(u => u.MaxSizeMB <= maxSizeMB.Value);

			return await query
				.OrderBy(u => u.SectionId)
				.ThenBy(u => u.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Override para incluir relacionamentos por padrão
		/// </summary>
		public override async Task<UploadItem?> GetByIdAsync(long id, CancellationToken cancellationToken)
		{
			return await _context.UploadItems
				.Include(u => u.Section)
				.Include(u => u.Status)
				.Include(u => u.AllowedFileTypes)
				.Include(u => u.ItemFiles)
				.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
		}

		/// <summary>
		/// Override para incluir relacionamentos por padrão
		/// </summary>
		public override async Task<IEnumerable<UploadItem>> GetAllAsync(CancellationToken cancellationToken)
		{
			return await _context.UploadItems
				.Include(u => u.Section)
				.Include(u => u.Status)
				.Include(u => u.AllowedFileTypes)
				.Include(u => u.ItemFiles)
				.OrderBy(u => u.SectionId)
				.ThenBy(u => u.Order)
				.ToListAsync(cancellationToken);
		}
	}
}
