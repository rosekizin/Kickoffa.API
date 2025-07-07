using Kickoffa.API.Data.EntityFramework.Context;
using Kickoffa.API.Data.Repositories.Base;
using Kickoffa.API.Domain.Models;
using Kickoffa.API.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.Repositories
{
	/// <summary>
	/// Implementação do repositório de BriefingMedia
	/// </summary>
	public class BriefingMediaRepository : BaseRepository<BriefingMedia>, IBriefingMediaRepository
	{
		public BriefingMediaRepository(KickoffaDbContext context) : base(context)
		{
		}

		/// <summary>
		/// Busca mídias por seção de briefing
		/// </summary>
		public async Task<IEnumerable<BriefingMedia>> GetBySectionIdAsync(long sectionId, CancellationToken cancellationToken)
		{
			return await _context.BriefingMedias
				.Where(m => m.SectionId == sectionId)
				.OrderBy(m => m.CreatedDateUtc)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca mídia por nome do arquivo
		/// </summary>
		public async Task<BriefingMedia?> GetByFileNameAsync(string fileName, CancellationToken cancellationToken)
		{
			return await _context.BriefingMedias
				.FirstOrDefaultAsync(m => m.FileName == fileName, cancellationToken);
		}

		/// <summary>
		/// Override para incluir relacionamentos por padrão
		/// </summary>
		public override async Task<BriefingMedia?> GetByIdAsync(long id, CancellationToken cancellationToken)
		{
			return await _context.BriefingMedias
				.Include(m => m.Section)
				.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
		}

		/// <summary>
		/// Override para incluir relacionamentos por padrão
		/// </summary>
		public override async Task<IEnumerable<BriefingMedia>> GetAllAsync(CancellationToken cancellationToken)
		{
			return await _context.BriefingMedias
				.Include(m => m.Section)
				.OrderByDescending(m => m.CreatedDateUtc)
				.ToListAsync(cancellationToken);
		}
	}
}