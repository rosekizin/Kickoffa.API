using Kickoffa.API.Domain.Models.Items;

namespace Kickoffa.API.Domain.Repositories
{
	/// <summary>
	/// Interface para repositório de TextItem
	/// </summary>
	public interface ITextItemRepository
	{
		/// <summary>
		/// Busca itens de texto por seção
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de itens de texto da seção</returns>
		Task<IEnumerable<TextItem>> GetBySectionIdAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca itens de texto por placeholder
		/// </summary>
		/// <param name="placeholder">Texto do placeholder</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de itens com o placeholder especificado</returns>
		Task<IEnumerable<TextItem>> GetByPlaceholderAsync(string placeholder, CancellationToken cancellationToken);

		/// <summary>
		/// Busca itens de texto por tamanho máximo
		/// </summary>
		/// <param name="maxLength">Tamanho máximo</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de itens com o tamanho máximo especificado</returns>
		Task<IEnumerable<TextItem>> GetByMaxLengthAsync(int? maxLength, CancellationToken cancellationToken);

		/// <summary>
		/// Busca itens de texto obrigatórios por seção
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de itens de texto obrigatórios</returns>
		Task<IEnumerable<TextItem>> GetRequiredBySectionIdAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca itens de texto por checklist
		/// </summary>
		/// <param name="checklistId">ID do checklist</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de itens de texto do checklist</returns>
		Task<IEnumerable<TextItem>> GetByChecklistIdAsync(long checklistId, CancellationToken cancellationToken);

		/// <summary>
		/// Conta itens de texto por seção
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Número de itens de texto na seção</returns>
		Task<int> CountBySectionIdAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca itens de texto por múltiplas seções
		/// </summary>
		/// <param name="sectionIds">IDs das seções</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de itens de texto das seções</returns>
		Task<IEnumerable<TextItem>> GetBySectionIdsAsync(IEnumerable<long> sectionIds, CancellationToken cancellationToken);

		/// <summary>
		/// Busca itens de texto que contêm o placeholder especificado
		/// </summary>
		/// <param name="searchText">Texto a buscar no placeholder</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de itens que contêm o texto no placeholder</returns>
		Task<IEnumerable<TextItem>> SearchByPlaceholderAsync(string searchText, CancellationToken cancellationToken);
	}
}
