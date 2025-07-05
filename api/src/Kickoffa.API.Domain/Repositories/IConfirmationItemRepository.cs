using Kickoffa.API.Domain.Models.Items;

namespace Kickoffa.API.Domain.Repositories
{
	/// <summary>
	/// Interface para repositório de ConfirmationItem
	/// </summary>
	public interface IConfirmationItemRepository : IBaseRepository<ConfirmationItem>
	{
		/// <summary>
		/// Busca itens de confirmação por seção
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de itens de confirmação da seção</returns>
		Task<IEnumerable<ConfirmationItem>> GetBySectionIdAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca itens de confirmação por texto de confirmação
		/// </summary>
		/// <param name="confirmationText">Texto de confirmação</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de itens com o texto de confirmação especificado</returns>
		Task<IEnumerable<ConfirmationItem>> GetByConfirmationTextAsync(string confirmationText, CancellationToken cancellationToken);

		/// <summary>
		/// Busca itens de confirmação que contêm o texto especificado
		/// </summary>
		/// <param name="searchText">Texto a buscar</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de itens que contêm o texto</returns>
		Task<IEnumerable<ConfirmationItem>> SearchByConfirmationTextAsync(string searchText, CancellationToken cancellationToken);

		/// <summary>
		/// Busca itens de confirmação obrigatórios por seção
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de itens de confirmação obrigatórios</returns>
		Task<IEnumerable<ConfirmationItem>> GetRequiredBySectionIdAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca itens de confirmação por checklist
		/// </summary>
		/// <param name="checklistId">ID do checklist</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de itens de confirmação do checklist</returns>
		Task<IEnumerable<ConfirmationItem>> GetByChecklistIdAsync(long checklistId, CancellationToken cancellationToken);

		/// <summary>
		/// Conta itens de confirmação por seção
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Número de itens de confirmação na seção</returns>
		Task<int> CountBySectionIdAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca itens de confirmação por múltiplas seções
		/// </summary>
		/// <param name="sectionIds">IDs das seções</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de itens de confirmação das seções</returns>
		Task<IEnumerable<ConfirmationItem>> GetBySectionIdsAsync(IEnumerable<long> sectionIds, CancellationToken cancellationToken);

		/// <summary>
		/// Busca itens de confirmação por tamanho do texto
		/// </summary>
		/// <param name="minLength">Tamanho mínimo do texto</param>
		/// <param name="maxLength">Tamanho máximo do texto</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de itens na faixa de tamanho especificada</returns>
		Task<IEnumerable<ConfirmationItem>> GetByTextLengthRangeAsync(int minLength, int maxLength, CancellationToken cancellationToken);

		/// <summary>
		/// Conta itens de confirmação obrigatórios por seção
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Número de itens de confirmação obrigatórios na seção</returns>
		Task<int> CountRequiredBySectionIdAsync(long sectionId, CancellationToken cancellationToken);
	}
}
