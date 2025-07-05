using Kickoffa.API.Data.EntityFramework.Context;
using Kickoffa.API.Data.Repositories.Base;
using Kickoffa.API.Domain.Models.Items;
using Kickoffa.API.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.Repositories
{
	/// <summary>
	/// Implementação do repositório de SignatureItem
	/// </summary>
	public class SignatureItemRepository : BaseRepository<SignatureItem>, ISignatureItemRepository
	{
		public SignatureItemRepository(KickoffaDbContext context) : base(context)
		{
		}

		/// <summary>
		/// Busca itens de assinatura por seção
		/// </summary>
		public async Task<IEnumerable<SignatureItem>> GetBySectionIdAsync(long sectionId, CancellationToken cancellationToken)
		{
			return await _context.SignatureItems
				.Where(s => s.SectionId == sectionId)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca itens de assinatura ordenados por seção
		/// </summary>
		public async Task<IEnumerable<SignatureItem>> GetBySectionIdOrderedAsync(long sectionId, CancellationToken cancellationToken)
		{
			return await _context.SignatureItems
				.Where(s => s.SectionId == sectionId)
				.OrderBy(s => s.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca itens de assinatura obrigatórios por seção
		/// </summary>
		public async Task<IEnumerable<SignatureItem>> GetRequiredBySectionIdAsync(long sectionId, CancellationToken cancellationToken)
		{
			return await _context.SignatureItems
				.Where(s => s.SectionId == sectionId && s.IsRequired)
				.OrderBy(s => s.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca itens de assinatura por checklist
		/// </summary>
		public async Task<IEnumerable<SignatureItem>> GetByChecklistIdAsync(long checklistId, CancellationToken cancellationToken)
		{
			return await _context.SignatureItems
				.Include(s => s.Section)
				.Where(s => s.Section.ChecklistId == checklistId)
				.OrderBy(s => s.Section.Order)
				.ThenBy(s => s.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca itens de assinatura por múltiplas seções
		/// </summary>
		public async Task<IEnumerable<SignatureItem>> GetBySectionIdsAsync(IEnumerable<long> sectionIds, CancellationToken cancellationToken)
		{
			return await _context.SignatureItems
				.Where(s => sectionIds.Contains(s.SectionId))
				.OrderBy(s => s.SectionId)
				.ThenBy(s => s.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Conta itens de assinatura por seção
		/// </summary>
		public async Task<int> CountBySectionIdAsync(long sectionId, CancellationToken cancellationToken)
		{
			return await _context.SignatureItems
				.CountAsync(s => s.SectionId == sectionId, cancellationToken);
		}

		/// <summary>
		/// Conta itens de assinatura obrigatórios por seção
		/// </summary>
		public async Task<int> CountRequiredBySectionIdAsync(long sectionId, CancellationToken cancellationToken)
		{
			return await _context.SignatureItems
				.CountAsync(s => s.SectionId == sectionId && s.IsRequired, cancellationToken);
		}

		/// <summary>
		/// Busca itens de assinatura por status de obrigatoriedade
		/// </summary>
		public async Task<IEnumerable<SignatureItem>> GetByRequiredStatusAsync(bool isRequired, CancellationToken cancellationToken)
		{
			return await _context.SignatureItems
				.Where(s => s.IsRequired == isRequired)
				.OrderBy(s => s.SectionId)
				.ThenBy(s => s.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca itens de assinatura completados por seção
		/// </summary>
		public async Task<IEnumerable<SignatureItem>> GetCompletedBySectionIdAsync(long sectionId, CancellationToken cancellationToken)
		{
			return await _context.SignatureItems
				.Include(s => s.Status)
				.Where(s => s.SectionId == sectionId && s.Status != null && s.Status.IsCompleted)
				.OrderBy(s => s.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Conta itens de assinatura completados por seção
		/// </summary>
		public async Task<int> CountCompletedBySectionIdAsync(long sectionId, CancellationToken cancellationToken)
		{
			return await _context.SignatureItems
				.Include(s => s.Status)
				.CountAsync(s => s.SectionId == sectionId && s.Status != null && s.Status.IsCompleted, cancellationToken);
		}

		/// <summary>
		/// Busca itens de assinatura pendentes por checklist
		/// </summary>
		public async Task<IEnumerable<SignatureItem>> GetPendingByChecklistIdAsync(long checklistId, CancellationToken cancellationToken)
		{
			return await _context.SignatureItems
				.Include(s => s.Section)
				.Include(s => s.Status)
				.Where(s => s.Section.ChecklistId == checklistId && 
				           (s.Status == null || !s.Status.IsCompleted))
				.OrderBy(s => s.Section.Order)
				.ThenBy(s => s.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Override para incluir relacionamentos por padrão
		/// </summary>
		public override async Task<SignatureItem?> GetByIdAsync(long id, CancellationToken cancellationToken)
		{
			return await _context.SignatureItems
				.Include(s => s.Section)
				.Include(s => s.Status)
				.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
		}

		/// <summary>
		/// Override para incluir relacionamentos por padrão
		/// </summary>
		public override async Task<IEnumerable<SignatureItem>> GetAllAsync(CancellationToken cancellationToken)
		{
			return await _context.SignatureItems
				.Include(s => s.Section)
				.Include(s => s.Status)
				.OrderBy(s => s.SectionId)
				.ThenBy(s => s.Order)
				.ToListAsync(cancellationToken);
		}
	}
}
