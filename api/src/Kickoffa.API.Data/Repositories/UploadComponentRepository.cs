using Kickoffa.API.Data.EntityFramework.Context;
using Kickoffa.API.Data.Repositories.Base;
using Kickoffa.API.Domain.Interfaces.Models.Components;
using Kickoffa.API.Domain.Models.Components;
using Kickoffa.API.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.Repositories
{
	/// <summary>
	/// Implementação do repositório de UploadComponent
	/// </summary>
	public class UploadComponentRepository : BaseRepository<IUploadComponent, UploadComponent>, IUploadComponentRepository
	{
		public UploadComponentRepository(KickoffaDbContext context) : base(context)
		{
		}

		/// <summary>
		/// Busca componentes de upload por seção
		/// </summary>
		public async Task<IEnumerable<IUploadComponent>> GetBySectionIdAsync(long sectionId, CancellationToken cancellationToken)
		{
			return await _context.UploadComponents
				.Where(u => u.SectionId == sectionId)
				.OrderBy(u => u.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca componentes de upload por placeholder
		/// </summary>
		public async Task<IEnumerable<IUploadComponent>> GetByPlaceholderAsync(string placeholder, CancellationToken cancellationToken)
		{
			return await _context.UploadComponents
				.Where(u => u.Placeholder == placeholder)
				.OrderBy(u => u.SectionId)
				.ThenBy(u => u.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca componentes de upload obrigatórios por seção
		/// </summary>
		public async Task<IEnumerable<IUploadComponent>> GetRequiredBySectionIdAsync(long sectionId, CancellationToken cancellationToken)
		{
			return await _context.UploadComponents
				.Where(u => u.SectionId == sectionId && u.IsRequired)
				.OrderBy(u => u.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca componentes de upload por checklist
		/// </summary>
		public async Task<IEnumerable<IUploadComponent>> GetByChecklistIdAsync(long checklistId, CancellationToken cancellationToken)
		{
			return await _context.UploadComponents
				.Include(u => u.Section)
				.Where(u => u.Section.ChecklistId == checklistId)
				.OrderBy(u => u.Section.Order)
				.ThenBy(u => u.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca componentes de upload por tipo de arquivo permitido
		/// </summary>
		public async Task<IEnumerable<IUploadComponent>> GetByAllowedFileTypeAsync(long fileTypeId, CancellationToken cancellationToken)
		{
			return await _context.UploadComponents
				.Include(u => u.AllowedFileTypes)
				.Where(u => u.AllowedFileTypes.Any(ft => ft.Id == fileTypeId))
				.OrderBy(u => u.SectionId)
				.ThenBy(u => u.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca componentes de upload com arquivos enviados
		/// </summary>
		public async Task<IEnumerable<IUploadComponent>> GetWithUploadedFilesAsync(long sectionId, CancellationToken cancellationToken)
		{
			return await _context.UploadComponents
				.Include(u => u.ComponentFiles)
				.Where(u => u.SectionId == sectionId && u.ComponentFiles.Any())
				.OrderBy(u => u.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Conta componentes de upload por seção
		/// </summary>
		public async Task<int> CountBySectionIdAsync(long sectionId, CancellationToken cancellationToken)
		{
			return await _context.UploadComponents
				.CountAsync(u => u.SectionId == sectionId, cancellationToken);
		}

		/// <summary>
		/// Busca componentes de upload por múltiplas seções
		/// </summary>
		public async Task<IEnumerable<IUploadComponent>> GetBySectionIdsAsync(IEnumerable<long> sectionIds, CancellationToken cancellationToken)
		{
			return await _context.UploadComponents
				.Where(u => sectionIds.Contains(u.SectionId))
				.OrderBy(u => u.SectionId)
				.ThenBy(u => u.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Override para incluir relacionamentos por padrão
		/// </summary>
		public override async Task<IUploadComponent?> GetByIdAsync(long id, CancellationToken cancellationToken)
		{
			return await _context.UploadComponents
				.Include(u => u.Section)
				.Include(u => u.Status)
				.Include(u => u.AllowedFileTypes)
				.Include(u => u.ComponentFiles)
				.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
		}

		/// <summary>
		/// Override para incluir relacionamentos por padrão
		/// </summary>
		public override async Task<IEnumerable<IUploadComponent>> GetAllAsync(CancellationToken cancellationToken)
		{
			return await _context.UploadComponents
				.Include(u => u.Section)
				.Include(u => u.Status)
				.Include(u => u.AllowedFileTypes)
				.Include(u => u.ComponentFiles)
				.OrderBy(u => u.SectionId)
				.ThenBy(u => u.Order)
				.ToListAsync(cancellationToken);
		}
	}
}