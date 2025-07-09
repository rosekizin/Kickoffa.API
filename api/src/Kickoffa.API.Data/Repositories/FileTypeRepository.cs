using Kickoffa.API.Data.EntityFramework.Context;
using Kickoffa.API.Data.Repositories.Base;
using Kickoffa.API.Domain.Interfaces.Models;
using Kickoffa.API.Domain.Models.Components;
using Kickoffa.API.Domain.Models.Enums;
using Kickoffa.API.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.Repositories
{
	/// <summary>
	/// Implementação do repositório para tipos de arquivo
	/// </summary>
	public class FileTypeRepository : BaseRepository<IFileType, FileType>, IFileTypeRepository
	{
		public FileTypeRepository(KickoffaDbContext context) : base(context)
		{
		}

		public async Task<IEnumerable<IFileType>> GetActiveFileTypesAsync(CancellationToken cancellationToken)
		{
			return await _context.FileTypes
				.Where(ft => ft.IsActive)
				.OrderBy(ft => ft.DisplayOrder)
				.ThenBy(ft => ft.DisplayName)
				.ToListAsync(cancellationToken);
		}

		public async Task<IEnumerable<IFileType>> SearchFileTypesAsync(string searchTerm, CancellationToken cancellationToken)
		{
			if (string.IsNullOrWhiteSpace(searchTerm))
			{
				return await GetActiveFileTypesAsync(cancellationToken);
			}

			var normalizedSearchTerm = searchTerm.ToLower().Trim();

			return await _context.FileTypes
				.Where(ft => ft.IsActive && (
					ft.DisplayName.ToLower().Contains(normalizedSearchTerm) ||
					ft.Extension.ToLower().Contains(normalizedSearchTerm) ||
					(ft.Description != null && ft.Description.ToLower().Contains(normalizedSearchTerm)) ||
					ft.MimeType.ToLower().Contains(normalizedSearchTerm)
				))
				.OrderBy(ft => ft.DisplayOrder)
				.ThenBy(ft => ft.DisplayName)
				.ToListAsync(cancellationToken);
		}

		public async Task<IEnumerable<IFileType>> GetFileTypesByCategoryAsync(FileTypeCategory category, CancellationToken cancellationToken)
		{
			return await _context.FileTypes
				.Where(ft => ft.IsActive && ft.Category == category)
				.OrderBy(ft => ft.DisplayOrder)
				.ThenBy(ft => ft.DisplayName)
				.ToListAsync(cancellationToken);
		}

		public async Task<IFileType?> GetByIdAsync(long id, CancellationToken cancellationToken)
		{
			return await _context.FileTypes
				.FirstOrDefaultAsync(ft => ft.Id == id, cancellationToken);
		}

		public async Task<IEnumerable<IFileType>> GetByIdsAsync(IEnumerable<long> ids, CancellationToken cancellationToken)
		{
			return await _context.FileTypes
				.Where(ft => ids.Contains(ft.Id))
				.OrderBy(ft => ft.DisplayOrder)
				.ThenBy(ft => ft.DisplayName)
				.ToListAsync(cancellationToken);
		}
	}
}