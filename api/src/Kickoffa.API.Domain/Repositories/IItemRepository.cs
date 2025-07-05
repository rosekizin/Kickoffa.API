using Kickoffa.API.Domain.Models.Items.Base;

namespace Kickoffa.API.Domain.Repositories
{
	/// <summary>
	/// Interface para repositório de Item
	/// </summary>
	public interface IItemRepository : IBaseRepository<Item>
	{
		/// <summary>
		/// Busca itens por seção
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de itens ordenados</returns>
		Task<IEnumerable<Item>> GetBySectionIdAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca itens por seção ordenados por Order
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de itens ordenados</returns>
		Task<IEnumerable<Item>> GetBySectionIdOrderedAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Obtém a próxima ordem disponível para um novo item
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Próximo número de ordem</returns>
		Task<int> GetNextOrderAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Reordena itens de uma seção
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="itemOrders">Dicionário com ID do item e nova ordem</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		Task ReorderItemsAsync(long sectionId, Dictionary<long, int> itemOrders, CancellationToken cancellationToken);

		/// <summary>
		/// Busca itens obrigatórios por seção
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de itens obrigatórios</returns>
		Task<IEnumerable<Item>> GetRequiredBySectionIdAsync(long sectionId, CancellationToken cancellationToken);
	}
}
