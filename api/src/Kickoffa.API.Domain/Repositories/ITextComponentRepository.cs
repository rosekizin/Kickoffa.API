using Kickoffa.API.Domain.Interfaces.Models.Components;
using Kickoffa.API.Domain.Models.Components;

namespace Kickoffa.API.Domain.Repositories
{
	/// <summary>
	/// Interface para repositório de TextComponent
	/// </summary>
	public interface ITextComponentRepository : IBaseRepository<ITextComponent, TextComponent>
	{
		/// <summary>
		/// Busca componentes de texto por seção
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de componentes de texto da seção</returns>
		Task<IEnumerable<ITextComponent>> GetBySectionIdAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca componentes de texto por placeholder
		/// </summary>
		/// <param name="placeholder">Texto do placeholder</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de componentes com o placeholder especificado</returns>
		Task<IEnumerable<ITextComponent>> GetByPlaceholderAsync(string placeholder, CancellationToken cancellationToken);

		/// <summary>
		/// Busca componentes de texto por tamanho máximo
		/// </summary>
		/// <param name="maxLength">Tamanho máximo</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de componentes com o tamanho máximo especificado</returns>
		Task<IEnumerable<ITextComponent>> GetByMaxLengthAsync(int? maxLength, CancellationToken cancellationToken);

		/// <summary>
		/// Busca componentes de texto obrigatórios por seção
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de componentes de texto obrigatórios</returns>
		Task<IEnumerable<ITextComponent>> GetRequiredBySectionIdAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca componentes de texto por checklist
		/// </summary>
		/// <param name="checklistId">ID do checklist</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de componentes de texto do checklist</returns>
		Task<IEnumerable<ITextComponent>> GetByChecklistIdAsync(long checklistId, CancellationToken cancellationToken);

		/// <summary>
		/// Conta componentes de texto por seção
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Número de componentes de texto na seção</returns>
		Task<int> CountBySectionIdAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca componentes de texto por múltiplas seções
		/// </summary>
		/// <param name="sectionIds">IDs das seções</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de componentes de texto das seções</returns>
		Task<IEnumerable<ITextComponent>> GetBySectionIdsAsync(IEnumerable<long> sectionIds, CancellationToken cancellationToken);

		/// <summary>
		/// Busca componentes de texto que contêm o placeholder especificado
		/// </summary>
		/// <param name="searchText">Texto a buscar no placeholder</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de componentes que contêm o texto no placeholder</returns>
		Task<IEnumerable<ITextComponent>> SearchByPlaceholderAsync(string searchText, CancellationToken cancellationToken);
	}
}