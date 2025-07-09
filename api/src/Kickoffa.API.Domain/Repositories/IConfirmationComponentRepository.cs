using Kickoffa.API.Domain.Interfaces.Models.Components;
using Kickoffa.API.Domain.Models.Components;

namespace Kickoffa.API.Domain.Repositories
{
	/// <summary>
	/// Interface para repositório de ConfirmationComponent
	/// </summary>
	public interface IConfirmationComponentRepository : IBaseRepository<IConfirmationComponent, ConfirmationComponent>
	{
		/// <summary>
		/// Busca componentes de confirmação por seção
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de componentes de confirmação da seção</returns>
		Task<IEnumerable<IConfirmationComponent>> GetBySectionIdAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca componentes de confirmação por texto de confirmação
		/// </summary>
		/// <param name="confirmationText">Texto de confirmação</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de componentes com o texto de confirmação especificado</returns>
		Task<IEnumerable<IConfirmationComponent>> GetByConfirmationTextAsync(string confirmationText, CancellationToken cancellationToken);

		/// <summary>
		/// Busca componentes de confirmação que contêm o texto especificado
		/// </summary>
		/// <param name="searchText">Texto a buscar</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de componentes que contêm o texto</returns>
		Task<IEnumerable<IConfirmationComponent>> SearchByConfirmationTextAsync(string searchText, CancellationToken cancellationToken);

		/// <summary>
		/// Busca componentes de confirmação obrigatórios por seção
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de componentes de confirmação obrigatórios</returns>
		Task<IEnumerable<IConfirmationComponent>> GetRequiredBySectionIdAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca componentes de confirmação por checklist
		/// </summary>
		/// <param name="checklistId">ID do checklist</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de componentes de confirmação do checklist</returns>
		Task<IEnumerable<IConfirmationComponent>> GetByChecklistIdAsync(long checklistId, CancellationToken cancellationToken);

		/// <summary>
		/// Conta componentes de confirmação por seção
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Número de componentes de confirmação na seção</returns>
		Task<int> CountBySectionIdAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca componentes de confirmação por múltiplas seções
		/// </summary>
		/// <param name="sectionIds">IDs das seções</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de componentes de confirmação das seções</returns>
		Task<IEnumerable<IConfirmationComponent>> GetBySectionIdsAsync(IEnumerable<long> sectionIds, CancellationToken cancellationToken);

		/// <summary>
		/// Conta componentes de confirmação obrigatórios por seção
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Número de componentes de confirmação obrigatórios na seção</returns>
		Task<int> CountRequiredBySectionIdAsync(long sectionId, CancellationToken cancellationToken);
	}
}