using Kickoffa.API.Domain.Models.Components;

namespace Kickoffa.API.Domain.Repositories
{
	/// <summary>
	/// Interface para repositório de CheckboxComponent
	/// </summary>
	public interface ICheckboxComponentRepository : IBaseRepository<CheckboxComponent>
	{
		/// <summary>
		/// Busca itens de checkbox por seção
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de itens de checkbox da seção</returns>
		Task<IEnumerable<CheckboxComponent>> GetBySectionIdAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca itens de checkbox ordenados por seção
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de itens de checkbox ordenados</returns>
		Task<IEnumerable<CheckboxComponent>> GetBySectionIdOrderedAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca itens de checkbox obrigatórios por seção
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de itens de checkbox obrigatórios</returns>
		Task<IEnumerable<CheckboxComponent>> GetRequiredBySectionIdAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca itens de checkbox por checklist
		/// </summary>
		/// <param name="checklistId">ID do checklist</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de itens de checkbox do checklist</returns>
		Task<IEnumerable<CheckboxComponent>> GetByChecklistIdAsync(long checklistId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca itens de checkbox por múltiplas seções
		/// </summary>
		/// <param name="sectionIds">IDs das seções</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de itens de checkbox das seções</returns>
		Task<IEnumerable<CheckboxComponent>> GetBySectionIdsAsync(IEnumerable<long> sectionIds, CancellationToken cancellationToken);

		/// <summary>
		/// Conta itens de checkbox por seção
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Número de itens de checkbox na seção</returns>
		Task<int> CountBySectionIdAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Conta itens de checkbox obrigatórios por seção
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Número de itens de checkbox obrigatórios na seção</returns>
		Task<int> CountRequiredBySectionIdAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca itens de checkbox por status de obrigatoriedade
		/// </summary>
		/// <param name="isRequired">Se é obrigatório ou não</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de itens de checkbox com o status especificado</returns>
		Task<IEnumerable<CheckboxComponent>> GetByRequiredStatusAsync(bool isRequired, CancellationToken cancellationToken);

		/// <summary>
		/// Busca itens de checkbox completados por seção
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de itens de checkbox completados</returns>
		Task<IEnumerable<CheckboxComponent>> GetCompletedBySectionIdAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Conta itens de checkbox completados por seção
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Número de itens de checkbox completados na seção</returns>
		Task<int> CountCompletedBySectionIdAsync(long sectionId, CancellationToken cancellationToken);
	}
}