using Kickoffa.API.Data.EntityFramework.Context;
using Kickoffa.API.Data.Repositories.Base;
using Kickoffa.API.Domain.Interfaces.Models;
using Kickoffa.API.Domain.Models;
using Kickoffa.API.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.Repositories
{
	/// <summary>
	/// Implementação do repositório de Checklist
	/// </summary>
	public class ChecklistRepository : BaseRepository<IChecklist, Checklist>, IChecklistRepository
	{
		public ChecklistRepository(KickoffaDbContext context) : base(context)
		{
		}

		/// <summary>
		/// Busca checklist por slug
		/// </summary>
		public async Task<IChecklist?> GetBySlugAsync(string slug, CancellationToken cancellationToken)
		{
			return await _context.Checklists
				.Include(c => c.Sections)
				.FirstOrDefaultAsync(c => c.Slug == slug, cancellationToken);
		}

		/// <summary>
		/// Busca checklist por token de acesso
		/// </summary>
		public async Task<IChecklist?> GetByAccessTokenAsync(string accessToken, CancellationToken cancellationToken)
		{
			return await _context.Checklists
				.Include(c => c.Sections)
				.FirstOrDefaultAsync(c => c.AccessToken == accessToken, cancellationToken);
		}

		/// <summary>
		/// Busca checklists por proprietário
		/// </summary>
		public async Task<IEnumerable<IChecklist>> GetByOwnerIdAsync(long ownerId, CancellationToken cancellationToken)
		{
			return await _context.Checklists
				.Include(c => c.Sections)
				.Where(c => c.OwnerId == ownerId)
				.OrderByDescending(c => c.CreatedDateUtc)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca checklists publicados por proprietário
		/// </summary>
		public async Task<IEnumerable<IChecklist>> GetPublishedByOwnerIdAsync(long ownerId, CancellationToken cancellationToken)
		{
			return await _context.Checklists
				.Include(c => c.Sections)
				.Where(c => c.OwnerId == ownerId && c.IsPublished)
				.OrderByDescending(c => c.CreatedDateUtc)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Verifica se existe checklist com o slug especificado
		/// </summary>
		public async Task<bool> ExistsBySlugAsync(string slug, long? excludeId, CancellationToken cancellationToken)
		{
			var query = _context.Checklists.Where(c => c.Slug == slug);
			
			if (excludeId.HasValue)
			{
				query = query.Where(c => c.Id != excludeId.Value);
			}

			return await query.AnyAsync(cancellationToken);
		}

		/// <summary>
		/// Override para incluir seções por padrão
		/// </summary>
		public override async Task<IChecklist?> GetByIdAsync(long id, CancellationToken cancellationToken)
		{
			return await _context.Checklists
				.Include(c => c.Sections)
				.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
		}

		/// <summary>
		/// Override para incluir seções por padrão
		/// </summary>
		public override async Task<IEnumerable<IChecklist>> GetAllAsync(CancellationToken cancellationToken)
		{
			return await _context.Checklists
				.Include(c => c.Sections)
				.OrderByDescending(c => c.CreatedDateUtc)
				.ToListAsync(cancellationToken);
		}
	}
}