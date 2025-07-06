using Kickoffa.API.Domain.Models.Components;

namespace Kickoffa.API.Domain.Repositories
{
	/// <summary>
	/// Interface para repositório de ComponentStatus
	/// </summary>
	public interface IComponentStatusRepository : IBaseRepository<ComponentStatus>
	{
		/// <summary>
		/// Busca status por component
		/// </summary>
		/// <param name="componentId">ID do component</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Status do component ou null</returns>
		Task<ComponentStatus?> GetByComponentIdAsync(long componentId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca status por múltiplos componentes
		/// </summary>
		/// <param name="componentIds">IDs dos componentes</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de status dos componentes</returns>
		Task<IEnumerable<ComponentStatus>> GetByComponentIdsAsync(IEnumerable<long> componentIds, CancellationToken cancellationToken);

		/// <summary>
		/// Busca status completados por seção
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de status completados</returns>
		Task<IEnumerable<ComponentStatus>> GetCompletedBySectionIdAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca status completados por checklist
		/// </summary>
		/// <param name="checklistId">ID do checklist</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de status completados</returns>
		Task<IEnumerable<ComponentStatus>> GetCompletedByChecklistIdAsync(long checklistId, CancellationToken cancellationToken);

		/// <summary>
		/// Conta componentes completados por checklist
		/// </summary>
		/// <param name="checklistId">ID do checklist</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Número de componentes completados</returns>
		Task<int> CountCompletedByChecklistIdAsync(long checklistId, CancellationToken cancellationToken);

		/// <summary>
		/// Conta total de componentes por checklist
		/// </summary>
		/// <param name="checklistId">ID do checklist</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Número total de componentes</returns>
		Task<int> CountTotalByChecklistIdAsync(long checklistId, CancellationToken cancellationToken);
	}
}
