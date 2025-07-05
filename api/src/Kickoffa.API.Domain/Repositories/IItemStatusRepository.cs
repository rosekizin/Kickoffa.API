using Kickoffa.API.Domain.Models;
using Kickoffa.API.Domain.Models.Items;

namespace Kickoffa.API.Domain.Repositories
{
	/// <summary>
	/// Interface para repositório de ItemStatus
	/// </summary>
	public interface IItemStatusRepository : IBaseRepository<ItemStatus>
	{
		/// <summary>
		/// Busca status por item
		/// </summary>
		/// <param name="itemId">ID do item</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Status do item ou null</returns>
		Task<ItemStatus?> GetByItemIdAsync(long itemId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca status por múltiplos itens
		/// </summary>
		/// <param name="itemIds">IDs dos itens</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de status dos itens</returns>
		Task<IEnumerable<ItemStatus>> GetByItemIdsAsync(IEnumerable<long> itemIds, CancellationToken cancellationToken);

		/// <summary>
		/// Busca status completados por seção
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de status completados</returns>
		Task<IEnumerable<ItemStatus>> GetCompletedBySectionIdAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca status completados por checklist
		/// </summary>
		/// <param name="checklistId">ID do checklist</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de status completados</returns>
		Task<IEnumerable<ItemStatus>> GetCompletedByChecklistIdAsync(long checklistId, CancellationToken cancellationToken);

		/// <summary>
		/// Conta itens completados por checklist
		/// </summary>
		/// <param name="checklistId">ID do checklist</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Número de itens completados</returns>
		Task<int> CountCompletedByChecklistIdAsync(long checklistId, CancellationToken cancellationToken);

		/// <summary>
		/// Conta total de itens por checklist
		/// </summary>
		/// <param name="checklistId">ID do checklist</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Número total de itens</returns>
		Task<int> CountTotalByChecklistIdAsync(long checklistId, CancellationToken cancellationToken);
	}
}
