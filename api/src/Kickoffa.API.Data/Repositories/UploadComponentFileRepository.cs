using Kickoffa.API.Data.EntityFramework.Context;
using Kickoffa.API.Data.Repositories.Base;
using Kickoffa.API.Domain.Models.Components;
using Kickoffa.API.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.Repositories
{
	/// <summary>
	/// Implementação do repositório de UploadComponentFile
	/// </summary>
	public class UploadComponentFileRepository : BaseRepository<UploadComponentFile>, IUploadComponentFileRepository
	{
		public UploadComponentFileRepository(KickoffaDbContext context) : base(context)
		{
		}

		/// <summary>
		/// Busca arquivos por componente de upload
		/// </summary>
		public async Task<IEnumerable<UploadComponentFile>> GetByUploadComponentIdAsync(long uploadComponentId, CancellationToken cancellationToken)
		{
			return await _context.UploadComponentFiles
				.Where(f => f.UploadComponent.Id == uploadComponentId)
				.OrderBy(f => f.CreatedDateUtc)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca arquivo por nome
		/// </summary>
		public async Task<UploadComponentFile?> GetByFileNameAsync(string fileName, CancellationToken cancellationToken)
		{
			return await _context.UploadComponentFiles
				.Include(f => f.UploadComponent)
				.FirstOrDefaultAsync(f => f.FileName == fileName, cancellationToken);
		}

		/// <summary>
		/// Busca arquivo por hash SHA256
		/// </summary>
		public async Task<UploadComponentFile?> GetBySha256HashAsync(string sha256Hash, CancellationToken cancellationToken)
		{
			return await _context.UploadComponentFiles
				.Include(f => f.UploadComponent)
				.FirstOrDefaultAsync(f => f.Sha256Hash == sha256Hash, cancellationToken);
		}

		/// <summary>
		/// Busca arquivos por múltiplos componentes de upload
		/// </summary>
		public async Task<IEnumerable<UploadComponentFile>> GetByUploadComponentIdsAsync(IEnumerable<long> uploadComponentIds, CancellationToken cancellationToken)
		{
			return await _context.UploadComponentFiles
				.Include(f => f.UploadComponent)
				.Where(f => uploadComponentIds.Contains(f.UploadComponent.Id))
				.OrderBy(f => f.UploadComponent.Id)
				.ThenBy(f => f.CreatedDateUtc)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca arquivos por tipo de conteúdo
		/// </summary>
		public async Task<IEnumerable<UploadComponentFile>> GetByContentTypeAsync(string contentType, CancellationToken cancellationToken)
		{
			return await _context.UploadComponentFiles
				.Include(f => f.UploadComponent)
				.Where(f => f.ContentType == contentType)
				.OrderByDescending(f => f.CreatedDateUtc)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca arquivos por seção (através do relacionamento com UploadComponent)
		/// </summary>
		public async Task<IEnumerable<UploadComponentFile>> GetBySectionIdAsync(long sectionId, CancellationToken cancellationToken)
		{
			return await _context.UploadComponentFiles
				.Include(f => f.UploadComponent)
					.ThenInclude(u => u.Section)
				.Where(f => f.UploadComponent.SectionId == sectionId)
				.OrderBy(f => f.UploadComponent.Order)
				.ThenBy(f => f.CreatedDateUtc)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca arquivos por checklist (através do relacionamento com UploadComponent e Section)
		/// </summary>
		public async Task<IEnumerable<UploadComponentFile>> GetByChecklistIdAsync(long checklistId, CancellationToken cancellationToken)
		{
			return await _context.UploadComponentFiles
				.Include(f => f.UploadComponent)
					.ThenInclude(u => u.Section)
				.Where(f => f.UploadComponent.Section.ChecklistId == checklistId)
				.OrderBy(f => f.UploadComponent.Section.Order)
				.ThenBy(f => f.UploadComponent.Order)
				.ThenBy(f => f.CreatedDateUtc)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Calcula o tamanho total de arquivos por componente de upload
		/// </summary>
		public async Task<long> GetTotalFileSizeByUploadComponentIdAsync(long uploadComponentId, CancellationToken cancellationToken)
		{
			return await _context.UploadComponentFiles
				.Where(f => f.UploadComponent.Id == uploadComponentId)
				.SumAsync(f => f.FileSize, cancellationToken);
		}

		/// <summary>
		/// Conta arquivos por componente de upload
		/// </summary>
		public async Task<int> CountByUploadComponentIdAsync(long uploadComponentId, CancellationToken cancellationToken)
		{
			return await _context.UploadComponentFiles
				.CountAsync(f => f.UploadComponent.Id == uploadComponentId, cancellationToken);
		}

		/// <summary>
		/// Remove arquivos órfãos (sem componente de upload associado)
		/// </summary>
		public async Task<int> RemoveOrphanedFilesAsync(CancellationToken cancellationToken)
		{
			var orphanedFiles = await _context.UploadComponentFiles
				.Where(f => f.UploadComponent == null)
				.ToListAsync(cancellationToken);

			if (orphanedFiles.Any())
			{
				_context.UploadComponentFiles.RemoveRange(orphanedFiles);
				await _context.SaveChangesAsync(cancellationToken);
			}

			return orphanedFiles.Count;
		}

		/// <summary>
		/// Verifica se existe arquivo com o hash especificado
		/// </summary>
		public async Task<bool> ExistsBySha256HashAsync(string sha256Hash, CancellationToken cancellationToken)
		{
			return await _context.UploadComponentFiles
				.AnyAsync(f => f.Sha256Hash == sha256Hash, cancellationToken);
		}

		/// <summary>
		/// Override para incluir relacionamentos por padrão
		/// </summary>
		public override async Task<UploadComponentFile?> GetByIdAsync(long id, CancellationToken cancellationToken)
		{
			return await _context.UploadComponentFiles
				.Include(f => f.UploadComponent)
					.ThenInclude(u => u.Section)
				.FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
		}

		/// <summary>
		/// Override para incluir relacionamentos por padrão
		/// </summary>
		public override async Task<IEnumerable<UploadComponentFile>> GetAllAsync(CancellationToken cancellationToken)
		{
			return await _context.UploadComponentFiles
				.Include(f => f.UploadComponent)
					.ThenInclude(u => u.Section)
				.OrderByDescending(f => f.CreatedDateUtc)
				.ToListAsync(cancellationToken);
		}
	}
}