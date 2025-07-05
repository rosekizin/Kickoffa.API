using Kickoffa.API.Data.EntityFramework.Context;
using Kickoffa.API.Data.Repositories.Base;
using Kickoffa.API.Domain.Models.Items;
using Kickoffa.API.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.Repositories
{
	/// <summary>
	/// Implementação do repositório de UploadItemFile
	/// </summary>
	public class UploadItemFileRepository : BaseRepository<UploadItemFile>, IUploadItemFileRepository
	{
		public UploadItemFileRepository(KickoffaDbContext context) : base(context)
		{
		}

		/// <summary>
		/// Busca arquivos por item de upload
		/// </summary>
		public async Task<IEnumerable<UploadItemFile>> GetByUploadItemIdAsync(long uploadItemId, CancellationToken cancellationToken)
		{
			return await _context.UploadItemFiles
				.Where(f => f.UploadItem.Id == uploadItemId)
				.OrderBy(f => f.CreatedDateUtc)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca arquivo por nome
		/// </summary>
		public async Task<UploadItemFile?> GetByFileNameAsync(string fileName, CancellationToken cancellationToken)
		{
			return await _context.UploadItemFiles
				.Include(f => f.UploadItem)
				.FirstOrDefaultAsync(f => f.FileName == fileName, cancellationToken);
		}

		/// <summary>
		/// Busca arquivo por hash SHA256
		/// </summary>
		public async Task<UploadItemFile?> GetBySha256HashAsync(string sha256Hash, CancellationToken cancellationToken)
		{
			return await _context.UploadItemFiles
				.Include(f => f.UploadItem)
				.FirstOrDefaultAsync(f => f.Sha256Hash == sha256Hash, cancellationToken);
		}

		/// <summary>
		/// Busca arquivos por múltiplos itens de upload
		/// </summary>
		public async Task<IEnumerable<UploadItemFile>> GetByUploadItemIdsAsync(IEnumerable<long> uploadItemIds, CancellationToken cancellationToken)
		{
			return await _context.UploadItemFiles
				.Include(f => f.UploadItem)
				.Where(f => uploadItemIds.Contains(f.UploadItem.Id))
				.OrderBy(f => f.UploadItem.Id)
				.ThenBy(f => f.CreatedDateUtc)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca arquivos por tipo de conteúdo
		/// </summary>
		public async Task<IEnumerable<UploadItemFile>> GetByContentTypeAsync(string contentType, CancellationToken cancellationToken)
		{
			return await _context.UploadItemFiles
				.Include(f => f.UploadItem)
				.Where(f => f.ContentType == contentType)
				.OrderByDescending(f => f.CreatedDateUtc)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca arquivos por seção (através do relacionamento com UploadItem)
		/// </summary>
		public async Task<IEnumerable<UploadItemFile>> GetBySectionIdAsync(long sectionId, CancellationToken cancellationToken)
		{
			return await _context.UploadItemFiles
				.Include(f => f.UploadItem)
					.ThenInclude(u => u.Section)
				.Where(f => f.UploadItem.SectionId == sectionId)
				.OrderBy(f => f.UploadItem.Order)
				.ThenBy(f => f.CreatedDateUtc)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca arquivos por checklist (através do relacionamento com UploadItem e Section)
		/// </summary>
		public async Task<IEnumerable<UploadItemFile>> GetByChecklistIdAsync(long checklistId, CancellationToken cancellationToken)
		{
			return await _context.UploadItemFiles
				.Include(f => f.UploadItem)
					.ThenInclude(u => u.Section)
				.Where(f => f.UploadItem.Section.ChecklistId == checklistId)
				.OrderBy(f => f.UploadItem.Section.Order)
				.ThenBy(f => f.UploadItem.Order)
				.ThenBy(f => f.CreatedDateUtc)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Calcula o tamanho total de arquivos por item de upload
		/// </summary>
		public async Task<long> GetTotalFileSizeByUploadItemIdAsync(long uploadItemId, CancellationToken cancellationToken)
		{
			return await _context.UploadItemFiles
				.Where(f => f.UploadItem.Id == uploadItemId)
				.SumAsync(f => f.FileSize, cancellationToken);
		}

		/// <summary>
		/// Conta arquivos por item de upload
		/// </summary>
		public async Task<int> CountByUploadItemIdAsync(long uploadItemId, CancellationToken cancellationToken)
		{
			return await _context.UploadItemFiles
				.CountAsync(f => f.UploadItem.Id == uploadItemId, cancellationToken);
		}

		/// <summary>
		/// Remove arquivos órfãos (sem item de upload associado)
		/// </summary>
		public async Task<int> RemoveOrphanedFilesAsync(CancellationToken cancellationToken)
		{
			var orphanedFiles = await _context.UploadItemFiles
				.Where(f => f.UploadItem == null)
				.ToListAsync(cancellationToken);

			if (orphanedFiles.Any())
			{
				_context.UploadItemFiles.RemoveRange(orphanedFiles);
				await _context.SaveChangesAsync(cancellationToken);
			}

			return orphanedFiles.Count;
		}

		/// <summary>
		/// Verifica se existe arquivo com o hash especificado
		/// </summary>
		public async Task<bool> ExistsBySha256HashAsync(string sha256Hash, CancellationToken cancellationToken)
		{
			return await _context.UploadItemFiles
				.AnyAsync(f => f.Sha256Hash == sha256Hash, cancellationToken);
		}

		/// <summary>
		/// Override para incluir relacionamentos por padrão
		/// </summary>
		public override async Task<UploadItemFile?> GetByIdAsync(long id, CancellationToken cancellationToken)
		{
			return await _context.UploadItemFiles
				.Include(f => f.UploadItem)
					.ThenInclude(u => u.Section)
				.FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
		}

		/// <summary>
		/// Override para incluir relacionamentos por padrão
		/// </summary>
		public override async Task<IEnumerable<UploadItemFile>> GetAllAsync(CancellationToken cancellationToken)
		{
			return await _context.UploadItemFiles
				.Include(f => f.UploadItem)
					.ThenInclude(u => u.Section)
				.OrderByDescending(f => f.CreatedDateUtc)
				.ToListAsync(cancellationToken);
		}
	}
}
