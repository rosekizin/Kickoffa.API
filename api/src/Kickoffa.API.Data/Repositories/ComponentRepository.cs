using Kickoffa.API.Data.EntityFramework.Context;
using Kickoffa.API.Data.Repositories.Base;
using Kickoffa.API.Domain.Interfaces.Models.Components;
using Kickoffa.API.Domain.Models.Components.Base;
using Kickoffa.API.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.Repositories
{
	/// <summary>
	/// Implementação do repositório de Component
	/// </summary>
	public class ComponentRepository : BaseRepository<IComponent, Component>, IComponentRepository
	{
		public ComponentRepository(KickoffaDbContext context) : base(context)
		{
		}

		/// <summary>
		/// Busca components por seção
		/// </summary>
		public async Task<IEnumerable<Component>> GetBySectionIdAsync(long sectionId, CancellationToken cancellationToken)
		{
			return await _context.Components
				.Where(i => i.SectionId == sectionId)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca components por seção ordenados por Order
		/// </summary>
		public async Task<IEnumerable<Component>> GetBySectionIdOrderedAsync(long sectionId, CancellationToken cancellationToken)
		{
			return await _context.Components
				.Where(i => i.SectionId == sectionId)
				.OrderBy(i => i.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Obtém a próxima ordem disponível para um novo componente
		/// </summary>
		public async Task<int> GetNextOrderAsync(long sectionId, CancellationToken cancellationToken)
		{
			var maxOrder = await _context.Components
				.Where(i => i.SectionId == sectionId)
				.MaxAsync(i => (int?)i.Order, cancellationToken);

			return (maxOrder ?? 0) + 1;
		}

		/// <summary>
		/// Reordena components de uma seção
		/// </summary>
		public async Task ReorderComponentsAsync(long sectionId, Dictionary<long, int> componentOrders, CancellationToken cancellationToken)
		{
			var components = await _context.Components
				.Where(i => i.SectionId == sectionId && componentOrders.Keys.Contains(i.Id))
				.ToListAsync(cancellationToken);

			foreach (var component in components)
			{
				if (componentOrders.TryGetValue(component.Id, out var newOrder))
				{
					component.UpdateOrder(newOrder);
				}
			}

			await _context.SaveChangesAsync(cancellationToken);
		}

		/// <summary>
		/// Busca components obrigatórios por seção
		/// </summary>
		public async Task<IEnumerable<Component>> GetRequiredBySectionIdAsync(long sectionId, CancellationToken cancellationToken)
		{
			return await _context.Components
				.Where(i => i.SectionId == sectionId && i.IsRequired)
				.OrderBy(i => i.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca components por tipo específico e seção (método genérico)
		/// </summary>
		public async Task<IEnumerable<T>> GetBySectionIdAsync<T>(long sectionId, CancellationToken cancellationToken) where T : Component
		{
			return await _context.Components
				.OfType<T>()
				.Where(i => i.SectionId == sectionId)
				.OrderBy(i => i.Order)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Conta components por tipo específico e seção (método genérico)
		/// </summary>
		public async Task<int> CountBySectionIdAsync<T>(long sectionId, CancellationToken cancellationToken) where T : Component
		{
			return await _context.Components
				.OfType<T>()
				.CountAsync(i => i.SectionId == sectionId, cancellationToken);
		}

		/// <summary>
		/// Override para incluir relacionamentos por padrão
		/// </summary>
		public override async Task<IComponent?> GetByIdAsync(long id, CancellationToken cancellationToken)
		{
			return await _context.Components
				.Include(i => i.Section)
				.Include(i => i.Status)
				.FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
		}

		/// <summary>
		/// Override para incluir relacionamentos por padrão
		/// </summary>
		public override async Task<IEnumerable<IComponent>> GetAllAsync(CancellationToken cancellationToken)
		{
			return await _context.Components
				.Include(i => i.Section)
				.Include(i => i.Status)
				.OrderBy(i => i.SectionId)
				.ThenBy(i => i.Order)
				.ToListAsync(cancellationToken);
		}
	}
}