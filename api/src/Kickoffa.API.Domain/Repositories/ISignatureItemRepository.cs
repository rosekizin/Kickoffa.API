using Kickoffa.API.Domain.Models.Items;

namespace Kickoffa.API.Domain.Repositories
{
	/// <summary>
	/// Interface para repositório de SignatureItem
	/// </summary>
	public interface ISignatureItemRepository : IBaseRepository<SignatureItem>
	{
		/// <summary>
		/// Busca itens de assinatura por seção
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de itens de assinatura da seção</returns>
		Task<IEnumerable<SignatureItem>> GetBySectionIdAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca itens de assinatura ordenados por seção
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de itens de assinatura ordenados</returns>
		Task<IEnumerable<SignatureItem>> GetBySectionIdOrderedAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca itens de assinatura obrigatórios por seção
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de itens de assinatura obrigatórios</returns>
		Task<IEnumerable<SignatureItem>> GetRequiredBySectionIdAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca itens de assinatura por checklist
		/// </summary>
		/// <param name="checklistId">ID do checklist</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de itens de assinatura do checklist</returns>
		Task<IEnumerable<SignatureItem>> GetByChecklistIdAsync(long checklistId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca itens de assinatura por múltiplas seções
		/// </summary>
		/// <param name="sectionIds">IDs das seções</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de itens de assinatura das seções</returns>
		Task<IEnumerable<SignatureItem>> GetBySectionIdsAsync(IEnumerable<long> sectionIds, CancellationToken cancellationToken);

		/// <summary>
		/// Conta itens de assinatura por seção
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Número de itens de assinatura na seção</returns>
		Task<int> CountBySectionIdAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Conta itens de assinatura obrigatórios por seção
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Número de itens de assinatura obrigatórios na seção</returns>
		Task<int> CountRequiredBySectionIdAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca itens de assinatura por status de obrigatoriedade
		/// </summary>
		/// <param name="isRequired">Se é obrigatório ou não</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de itens de assinatura com o status especificado</returns>
		Task<IEnumerable<SignatureItem>> GetByRequiredStatusAsync(bool isRequired, CancellationToken cancellationToken);

		/// <summary>
		/// Busca itens de assinatura completados por seção
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de itens de assinatura completados</returns>
		Task<IEnumerable<SignatureItem>> GetCompletedBySectionIdAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Conta itens de assinatura completados por seção
		/// </summary>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Número de itens de assinatura completados na seção</returns>
		Task<int> CountCompletedBySectionIdAsync(long sectionId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca itens de assinatura pendentes por checklist
		/// </summary>
		/// <param name="checklistId">ID do checklist</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de itens de assinatura pendentes</returns>
		Task<IEnumerable<SignatureItem>> GetPendingByChecklistIdAsync(long checklistId, CancellationToken cancellationToken);
	}
}
