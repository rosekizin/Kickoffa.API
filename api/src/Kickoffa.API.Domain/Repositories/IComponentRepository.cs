using Kickoffa.API.Domain.Models.Components.Base;

namespace Kickoffa.API.Domain.Repositories
{
	/// <summary>
	/// Interface para repositório de Componente
	/// </summary>
	public interface IComponentRepository : IBaseRepository<Component>
	{
		/// <summary>
		/// Busca componentes por seção
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de componentes ordenados</returns>
		Task<IEnumerable<Component>> GetBySectionIdAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca componentes por seção ordenados por Order
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de componentes ordenados</returns>
		Task<IEnumerable<Component>> GetBySectionIdOrderedAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Obtém a próxima ordem disponível para um novo componente
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Próximo número de ordem</returns>
		Task<int> GetNextOrderAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Reordena componentes de uma seção
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="componecomponentOrdersnteOrders">Dicionário com ID do componente e nova ordem</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		Task ReorderComponentsAsync(long sectionId, Dictionary<long, int> componentOrders, CancellationToken cancellationToken);

		/// <summary>
		/// Busca componentes obrigatórios por seção
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de componentes obrigatórios</returns>
		Task<IEnumerable<Component>> GetRequiredBySectionIdAsync(long sectionId, CancellationToken cancellationToken);
	}
}
