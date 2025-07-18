using Kickoffa.API.Contracts.Checklist;

namespace Kickoffa.API.Application.Interfaces.Checkilists
{
	/// <summary>
	/// Interface para serviços relacionados a checklists
	/// </summary>
	public interface IChecklistService
	{
		/// <summary>
		/// Busca todos os checklists do usuário autenticado (query filter no EF mapping)
		/// </summary>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de checklists</returns>
		Task<IEnumerable<ChecklistResponse>> GetAllAsync(CancellationToken cancellationToken);

		/// <summary>
		/// Busca um checklist por ID
		/// </summary>
		/// <param name="id">ID do checklist</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Checklist encontrado ou null</returns>
		Task<ChecklistResponse?> GetByIdAsync(long id, CancellationToken cancellationToken);

		/// <summary>
		/// Busca um checklist por slug
		/// </summary>
		/// <param name="slug">Slug do checklist</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Checklist encontrado ou null</returns>
		Task<ChecklistResponse?> GetBySlugAsync(string slug, CancellationToken cancellationToken);

		/// <summary>
		/// Busca um checklist por token de acesso (para visualização pública)
		/// </summary>
		/// <param name="accessToken">Token de acesso</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Checklist encontrado ou null</returns>
		Task<ChecklistResponse?> GetByAccessTokenAsync(string accessToken, CancellationToken cancellationToken);

		/// <summary>
		/// Atualiza um checklist existente
		/// </summary>
		/// <param name="id">ID do checklist</param>
		/// <param name="ownerId">ID do proprietário (para validação de autorização)</param>
		/// <param name="request">Dados atualizados do checklist</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Checklist atualizado ou null se não encontrado</returns>
		Task<ChecklistResponse?> UpdateAsync(long id, long ownerId, ChecklistRequest request, CancellationToken cancellationToken);

		/// <summary>
		/// Remove um checklist
		/// </summary>
		/// <param name="id">ID do checklist</param>
		/// <param name="ownerId">ID do proprietário (para validação de autorização)</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>True se removido com sucesso</returns>
		Task<bool> DeleteAsync(long id, long ownerId, CancellationToken cancellationToken);

		/// <summary>
		/// Publica um checklist
		/// </summary>
		/// <param name="id">ID do checklist</param>
		/// <param name="ownerId">ID do proprietário (para validação de autorização)</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>True se publicado com sucesso</returns>
		Task<bool> PublishAsync(long id, long ownerId, CancellationToken cancellationToken);

		/// <summary>
		/// Despublica um checklist
		/// </summary>
		/// <param name="id">ID do checklist</param>
		/// <param name="ownerId">ID do proprietário (para validação de autorização)</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>True se despublicado com sucesso</returns>
		Task<bool> UnpublishAsync(long id, long ownerId, CancellationToken cancellationToken);

		/// <summary>
		/// Regenera o token de acesso de um checklist
		/// </summary>
		/// <param name="id">ID do checklist</param>
		/// <param name="ownerId">ID do proprietário (para validação de autorização)</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Novo token de acesso ou null se não encontrado</returns>
		Task<string?> RegenerateAccessTokenAsync(long id, long ownerId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca checklists com paginação e filtros
		/// </summary>
		/// <param name="ownerId">ID do proprietário</param>
		/// <param name="searchRequest">Parâmetros de busca e paginação</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Response paginada com checklists</returns>
		Task<ChecklistPagedResponse> GetPagedAsync(long ownerId, ChecklistSearchRequest searchRequest, CancellationToken cancellationToken);
	}
}