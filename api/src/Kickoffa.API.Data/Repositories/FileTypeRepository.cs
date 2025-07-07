using Kickoffa.API.Data.EntityFramework.Context;
using Kickoffa.API.Domain.Models.Components;
using Kickoffa.API.Domain.Models.Enums;
using Kickoffa.API.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.Repositories
{
	/// <summary>
	/// Implementação do repositório para tipos de arquivo
	/// </summary>
	public class FileTypeRepository : IFileTypeRepository
	{
		private readonly IKickoffaDbContext _context;

		public FileTypeRepository(IKickoffaDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<FileType>> GetActiveFileTypesAsync(CancellationToken cancellationToken)
		{
			return await _context.FileTypes
				.Where(ft => ft.IsActive)
				.OrderBy(ft => ft.DisplayOrder)
				.ThenBy(ft => ft.DisplayName)
				.ToListAsync(cancellationToken);
		}

		public async Task<IEnumerable<FileType>> SearchFileTypesAsync(string searchTerm, CancellationToken cancellationToken)
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

		public async Task<IEnumerable<FileType>> GetFileTypesByCategoryAsync(FileTypeCategory category, CancellationToken cancellationToken)
		{
			return await _context.FileTypes
				.Where(ft => ft.IsActive && ft.Category == category)
				.OrderBy(ft => ft.DisplayOrder)
				.ThenBy(ft => ft.DisplayName)
				.ToListAsync(cancellationToken);
		}

		public async Task<FileType?> GetByIdAsync(long id, CancellationToken cancellationToken)
		{
			return await _context.FileTypes
				.FirstOrDefaultAsync(ft => ft.Id == id, cancellationToken);
		}

		public async Task<IEnumerable<FileType>> GetByIdsAsync(IEnumerable<long> ids, CancellationToken cancellationToken)
		{
			return await _context.FileTypes
				.Where(ft => ids.Contains(ft.Id))
				.OrderBy(ft => ft.DisplayOrder)
				.ThenBy(ft => ft.DisplayName)
				.ToListAsync(cancellationToken);
		}
	}
}
