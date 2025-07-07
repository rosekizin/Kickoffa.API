using Kickoffa.API.Domain.Models;

namespace Kickoffa.API.Domain.Repositories
{
	/// <summary>
	/// Interface para repositório de Section
	/// </summary>
	public interface ISectionRepository : IBaseRepository<Section>
	{
		/// <summary>
		/// Busca seções por checklist
		/// </summary>
		/// <param name="checklistId">ID do checklist</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de seções ordenadas</returns>
		Task<IEnumerable<Section>> GetByChecklistIdAsync(long checklistId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca seções por checklist ordenadas por Order
		/// </summary>
		/// <param name="checklistId">ID do checklist</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de seções ordenadas</returns>
		Task<IEnumerable<Section>> GetByChecklistIdOrderedAsync(long checklistId, CancellationToken cancellationToken);

		/// <summary>
		/// Obtém a próxima ordem disponível para uma nova seção
		/// </summary>
		/// <param name="checklistId">ID do checklist</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Próximo número de ordem</returns>
		Task<int> GetNextOrderAsync(long checklistId, CancellationToken cancellationToken);

		/// <summary>
		/// Reordena seções de um checklist
		/// </summary>
		/// <param name="checklistId">ID do checklist</param>
		/// <param name="sectionOrders">Dicionário com ID da seção e nova ordem</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		Task ReorderSectionsAsync(long checklistId, Dictionary<long, int> sectionOrders, CancellationToken cancellationToken);
	}
}
