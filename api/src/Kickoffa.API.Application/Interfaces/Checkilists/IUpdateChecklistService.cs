using Kickoffa.API.Contracts.Checklist;

namespace Kickoffa.API.Application.Interfaces.Checkilists
{
	/// <summary>
	/// Interface para serviço de atualização de checklists
	/// </summary>
	public interface IUpdateChecklistService
	{
		/// <summary>
		/// Atualiza um checklist existente com todas as suas seções e componentes
		/// </summary>
		/// <param name="id">ID do checklist</param>
		/// <param name="ownerId">ID do proprietário (para validação de autorização)</param>
		/// <param name="request">Dados atualizados do checklist</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Checklist atualizado ou null se não encontrado</returns>
		Task<ChecklistResponse?> UpdateAsync(long id, long ownerId, ChecklistRequest request, CancellationToken cancellationToken);
	}
}