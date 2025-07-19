using Kickoffa.API.Domain.Interfaces.Models;
using Kickoffa.API.Domain.Models;
using Kickoffa.API.Domain.Models.Enums;

namespace Kickoffa.API.Domain.Repositories
{
	/// <summary>
	/// Interface para repositório de Checklist
	/// </summary>
	public interface IChecklistRepository : IBaseRepository<IChecklist, Checklist>
	{
		/// <summary>
		/// Busca checklist por slug
		/// </summary>
		/// <param name="slug">Slug do checklist</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Checklist encontrado ou null</returns>
		Task<IChecklist?> GetBySlugAsync(string slug, CancellationToken cancellationToken);

		/// <summary>
		/// Busca checklist por token de acesso
		/// </summary>
		/// <param name="accessToken">Token de acesso</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Checklist encontrado ou null</returns>
		Task<IChecklist?> GetByAccessTokenAsync(string accessToken, CancellationToken cancellationToken);

		/// <summary>
		/// Busca todos checklists
		/// </summary>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de checklists do proprietário</returns>
		new Task<IEnumerable<IChecklist>> GetAllAsync(CancellationToken cancellationToken);

		/// <summary>
		/// Busca checklists publicados por proprietário
		/// </summary>
		/// <param name="ownerId">ID do proprietário</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de checklists publicados do proprietário</returns>
		Task<IEnumerable<IChecklist>> GetPublishedByOwnerIdAsync(long ownerId, CancellationToken cancellationToken);

		/// <summary>
		/// Verifica se existe checklist com o slug especificado
		/// </summary>
		/// <param name="slug">Slug a verificar</param>
		/// <param name="excludeId">ID do checklist a excluir da verificação (para updates)</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>True se existe, false caso contrário</returns>
		Task<bool> ExistsBySlugAsync(string slug, long? excludeId, CancellationToken cancellationToken);

		/// <summary>
		/// Busca checklist por ID incluindo toda a hierarquia: Sections -> Components -> Status
		/// </summary>
		/// <param name="id">ID do checklist</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Checklist com hierarquia completa ou null</returns>
		Task<IChecklist?> GetByIdWithFullHierarchyAsync(long id, CancellationToken cancellationToken);

		/// <summary>
		/// Busca checklist por ID incluindo toda a hierarquia com componentes específicos
		/// Inclui todos os tipos de componentes e seus relacionamentos (FileTypes, ComponentFiles, etc.)
		/// </summary>
		/// <param name="id">ID do checklist</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Checklist com hierarquia completa ou null</returns>
		Task<IChecklist?> GetByIdWithCompleteHierarchyAsync(long id, CancellationToken cancellationToken);

		/// <summary>
		/// Busca checklist por slug incluindo toda a hierarquia
		/// </summary>
		/// <param name="slug">Slug do checklist</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Checklist com hierarquia completa ou null</returns>
		Task<IChecklist?> GetBySlugWithFullHierarchyAsync(string slug, CancellationToken cancellationToken);

		/// <summary>
		/// Busca checklist por token de acesso incluindo toda a hierarquia
		/// </summary>
		/// <param name="accessToken">Token de acesso</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Checklist com hierarquia completa ou null</returns>
		Task<IChecklist?> GetByAccessTokenWithFullHierarchyAsync(string accessToken, CancellationToken cancellationToken);

		/// <summary>
		/// Busca checklists com paginação e filtros
		/// </summary>
		/// <param name="search">Termo de busca (título ou cliente)</param>
		/// <param name="page">Número da página (baseado em 1)</param>
		/// <param name="pageSize">Tamanho da página</param>
		/// <param name="statusFilter">Filtro de status para busca</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Tupla com lista de checklists e total de registros</returns>
		Task<(IEnumerable<IChecklist> Checklists, int TotalCount)> GetPagedAsync(
			string? search,
			int page,
			int pageSize,
			IEnumerable<ChecklistStatus> statusFilter,
			CancellationToken cancellationToken);

		/*
		/// <summary>
		/// OTIMIZADO: Busca checklist com carregamento manual em etapas para máxima performance
		/// </summary>
		/// <param name="id">ID do checklist</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Checklist com hierarquia completa ou null</returns>
		Task<IChecklist?> GetByIdWithOptimizedHierarchyAsync(long id, CancellationToken cancellationToken);

		/// <summary>
		/// OTIMIZADO: Busca com projeção para leitura eficiente (não modificação)
		/// </summary>
		/// <param name="id">ID do checklist</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Checklist com hierarquia completa ou null</returns>
		Task<IChecklist?> GetByIdWithProjectionAsync(long id, CancellationToken cancellationToken);

		/// <summary>
		/// OTIMIZADO: Busca com filtro de segurança por owner
		/// </summary>
		/// <param name="id">ID do checklist</param>
		/// <param name="ownerId">ID do proprietário</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Checklist com hierarquia completa ou null</returns>
		Task<IChecklist?> GetByIdWithFullHierarchyForOwnerAsync(long id, long ownerId, CancellationToken cancellationToken);
		*/
	}
}