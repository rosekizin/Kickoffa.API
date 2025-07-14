using Kickoffa.API.Data.EntityFramework.Context;
using Kickoffa.API.Data.Repositories.Base;
using Kickoffa.API.Domain.Interfaces.Models.Components;
using Kickoffa.API.Domain.Models.Components;
using Kickoffa.API.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.Repositories
{
	/// <inheritdoc/>
	public class UploadComponentFileTypeSizeRepository : BaseRepository<IUploadComponentFileTypeSize, UploadComponentFileTypeSize>, IUploadComponentFileTypeSizeRepository
	{
		public UploadComponentFileTypeSizeRepository(KickoffaDbContext context) : base(context)
		{
		}

		/// <inheritdoc/>
		public async Task<IEnumerable<IUploadComponentFileTypeSize>> GetByUploadComponentIdAsync(long uploadComponentId, CancellationToken cancellationToken)
		{
			return await _context.UploadComponentFileTypeSizes
				.Include(s => s.FileType)
				.Include(s => s.UploadComponent)
				.Where(s => s.UploadComponentId == uploadComponentId)
				.OrderBy(s => s.FileType.DisplayName)
				.ToListAsync(cancellationToken);
		}

		/// <inheritdoc/>
		public async Task<IUploadComponentFileTypeSize?> GetByUploadComponentAndFileTypeAsync(long uploadComponentId, long fileTypeId, CancellationToken cancellationToken)
		{
			return await _context.UploadComponentFileTypeSizes
				.Include(s => s.FileType)
				.Include(s => s.UploadComponent)
				.FirstOrDefaultAsync(s => s.UploadComponentId == uploadComponentId && s.FileTypeId == fileTypeId, cancellationToken);
		}

		/// <inheritdoc/>
		public async Task<IEnumerable<IUploadComponentFileTypeSize>> GetByFileTypeIdAsync(long fileTypeId, CancellationToken cancellationToken)
		{
			return await _context.UploadComponentFileTypeSizes
				.Include(s => s.FileType)
				.Include(s => s.UploadComponent)
				.Where(s => s.FileTypeId == fileTypeId)
				.OrderBy(s => s.UploadComponent.Title)
				.ToListAsync(cancellationToken);
		}

		/// <inheritdoc/>
		public async Task<IEnumerable<IUploadComponentFileTypeSize>> GetByUploadComponentIdsAsync(IEnumerable<long> uploadComponentIds, CancellationToken cancellationToken)
		{
			return await _context.UploadComponentFileTypeSizes
				.Include(s => s.FileType)
				.Include(s => s.UploadComponent)
				.Where(s => uploadComponentIds.Contains(s.UploadComponentId))
				.OrderBy(s => s.UploadComponentId)
				.ThenBy(s => s.FileType.DisplayName)
				.ToListAsync(cancellationToken);
		}

		/// <inheritdoc/>
		public async Task<int> RemoveByUploadComponentIdAsync(long uploadComponentId, CancellationToken cancellationToken)
		{
			var configs = await _context.UploadComponentFileTypeSizes
				.Where(s => s.UploadComponentId == uploadComponentId)
				.ToListAsync(cancellationToken);

			if (configs.Any())
			{
				_context.UploadComponentFileTypeSizes.RemoveRange(configs);
				await _context.SaveChangesAsync(cancellationToken);
			}

			return configs.Count;
		}

		/// <inheritdoc/>
		public async Task<bool> RemoveByUploadComponentAndFileTypeAsync(long uploadComponentId, long fileTypeId, CancellationToken cancellationToken)
		{
			var config = await _context.UploadComponentFileTypeSizes
				.FirstOrDefaultAsync(s => s.UploadComponentId == uploadComponentId && s.FileTypeId == fileTypeId, cancellationToken);

			if (config != null)
			{
				_context.UploadComponentFileTypeSizes.Remove(config);
				await _context.SaveChangesAsync(cancellationToken);
				return true;
			}

			return false;
		}

		/// <inheritdoc/>
		public async Task<bool> ExistsByUploadComponentAndFileTypeAsync(long uploadComponentId, long fileTypeId, CancellationToken cancellationToken)
		{
			return await _context.UploadComponentFileTypeSizes
				.AnyAsync(s => s.UploadComponentId == uploadComponentId && s.FileTypeId == fileTypeId, cancellationToken);
		}

		/// <inheritdoc/>
		public async Task<IUploadComponentFileTypeSize> UpsertAsync(long uploadComponentId, long fileTypeId, int maxSizeMB, CancellationToken cancellationToken)
		{
			var existingConfig = await _context.UploadComponentFileTypeSizes
				.FirstOrDefaultAsync(s => s.UploadComponentId == uploadComponentId && s.FileTypeId == fileTypeId, cancellationToken);

			if (existingConfig != null)
			{
				existingConfig.UpdateMaxSize(maxSizeMB);
				await _context.SaveChangesAsync(cancellationToken);
				return existingConfig;
			}
			else
			{
				var newConfig = new UploadComponentFileTypeSize(uploadComponentId, fileTypeId, maxSizeMB);
				_context.UploadComponentFileTypeSizes.Add(newConfig);
				await _context.SaveChangesAsync(cancellationToken);
				return newConfig;
			}
		}

		/// <summary>
		/// Override para incluir relacionamentos por padrão
		/// </summary>
		public override async Task<IUploadComponentFileTypeSize?> GetByIdAsync(long id, CancellationToken cancellationToken)
		{
			return await _context.UploadComponentFileTypeSizes
				.Include(s => s.FileType)
				.Include(s => s.UploadComponent)
				.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
		}

		/// <summary>
		/// Override para incluir relacionamentos por padrão
		/// </summary>
		public override async Task<IEnumerable<IUploadComponentFileTypeSize>> GetAllAsync(CancellationToken cancellationToken)
		{
			return await _context.UploadComponentFileTypeSizes
				.Include(s => s.FileType)
				.Include(s => s.UploadComponent)
				.OrderBy(s => s.UploadComponent.Title)
				.ThenBy(s => s.FileType.DisplayName)
				.ToListAsync(cancellationToken);
		}
	}
}