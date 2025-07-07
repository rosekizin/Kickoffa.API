using Kickoffa.API.Domain.Models.Components;

namespace Kickoffa.API.Domain.Repositories
{
	/// <summary>
	/// Interface para repositório de SignatureComponent
	/// </summary>
	public interface ISignatureComponentRepository : IBaseRepository<SignatureComponent>
	{
		/// <summary>
		/// Busca componentes de assinatura por seção
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de componentes de assinatura da seção</returns>
		Task<IEnumerable<SignatureComponent>> GetBySectionIdAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca componentes de assinatura ordenados por seção
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de componentes de assinatura ordenados</returns>
		Task<IEnumerable<SignatureComponent>> GetBySectionIdOrderedAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca componentes de assinatura obrigatórios por seção
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de componentes de assinatura obrigatórios</returns>
		Task<IEnumerable<SignatureComponent>> GetRequiredBySectionIdAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca componentes de assinatura por checklist
		/// </summary>
		/// <param name="checklistId">ID do checklist</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de componentes de assinatura do checklist</returns>
		Task<IEnumerable<SignatureComponent>> GetByChecklistIdAsync(long checklistId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca componentes de assinatura por múltiplas seções
		/// </summary>
		/// <param name="sectionIds">IDs das seções</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de componentes de assinatura das seções</returns>
		Task<IEnumerable<SignatureComponent>> GetBySectionIdsAsync(IEnumerable<long> sectionIds, CancellationToken cancellationToken);

		/// <summary>
		/// Conta componentes de assinatura por seção
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Número de componentes de assinatura na seção</returns>
		Task<int> CountBySectionIdAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Conta componentes de assinatura obrigatórios por seção
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Número de componentes de assinatura obrigatórios na seção</returns>
		Task<int> CountRequiredBySectionIdAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca componentes de assinatura por status de obrigatoriedade
		/// </summary>
		/// <param name="isRequired">Se é obrigatório ou não</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de componentes de assinatura com o status especificado</returns>
		Task<IEnumerable<SignatureComponent>> GetByRequiredStatusAsync(bool isRequired, CancellationToken cancellationToken);

		/// <summary>
		/// Busca componentes de assinatura completados por seção
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de componentes de assinatura completados</returns>
		Task<IEnumerable<SignatureComponent>> GetCompletedBySectionIdAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Conta componentes de assinatura completados por seção
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Número de componentes de assinatura completados na seção</returns>
		Task<int> CountCompletedBySectionIdAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca componentes de assinatura pendentes por checklist
		/// </summary>
		/// <param name="checklistId">ID do checklist</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de componentes de assinatura pendentes</returns>
		Task<IEnumerable<SignatureComponent>> GetPendingByChecklistIdAsync(long checklistId, CancellationToken cancellationToken);
	}
}
