using Kickoffa.API.Application.Interfaces.Checkilists;
using Kickoffa.API.Contracts.Checklist;
using Kickoffa.API.Domain.Repositories;
using Kickoffa.API.Domain.Services;

namespace Kickoffa.API.Application.Services.Checklists
{
	/// <summary>
	/// Serviço para operações relacionadas a Checklist
	/// </summary>
	public sealed class ChecklistService : IChecklistService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly ICurrentUserService _currentUserService;
		private readonly IChecklistRepository _checklistRepository;
		private readonly IUpdateChecklistService _updateChecklistService;
		private readonly IMapChecklistToResponse _mapChecklistToResponse;

		/// <summary>
		/// Inicializa uma nova instância do ChecklistService
		/// </summary>
		/// <param name="checklistRepository">Repositório de checklists</param>
		/// <param name="unitOfWork">Unit of Work para transações</param>
		/// <param name="updateChecklistService">Serviço de atualização de checklists</param>
		/// <param name="mapChecklistToResponse">Serviço de mapeamento</param>
		public ChecklistService(
			IUnitOfWork unitOfWork,
			ICurrentUserService currentUserService,
			IChecklistRepository checklistRepository,
			IUpdateChecklistService updateChecklistService,
			IMapChecklistToResponse mapChecklistToResponse)
		{
			_unitOfWork = unitOfWork;
			_currentUserService = currentUserService;
			_checklistRepository = checklistRepository;
			_updateChecklistService = updateChecklistService;
			_mapChecklistToResponse = mapChecklistToResponse;
		}

		/// <inheritdoc />
		public async Task<IEnumerable<ChecklistResponse>> GetAllAsync(CancellationToken cancellationToken)
		{
			var checklists = await _checklistRepository.GetAllAsync(cancellationToken);
			return checklists.Select(_mapChecklistToResponse.MapToResponse);
		}

		/// <inheritdoc />
		public async Task<ChecklistResponse?> GetByIdAsync(long id, CancellationToken cancellationToken)
		{
			var checklist = await _checklistRepository.GetByIdWithCompleteHierarchyAsync(id, cancellationToken);
			return checklist != null ? _mapChecklistToResponse.MapToResponse(checklist) : null;
		}

		/// <inheritdoc />
		public async Task<ChecklistResponse?> GetBySlugAsync(string slug, CancellationToken cancellationToken)
		{
			var checklist = await _checklistRepository.GetBySlugAsync(slug, cancellationToken);
			return checklist != null ? _mapChecklistToResponse.MapToResponse(checklist) : null;
		}

		/// <inheritdoc />
		public async Task<ChecklistResponse?> GetByAccessTokenAsync(string accessToken, CancellationToken cancellationToken)
		{
			var checklist = await _checklistRepository.GetByAccessTokenAsync(accessToken, cancellationToken);
			return checklist != null ? _mapChecklistToResponse.MapToResponse(checklist) : null;
		}

		/// <inheritdoc />
		public async Task<ChecklistResponse?> UpdateAsync(long id, ChecklistRequest request, CancellationToken cancellationToken)
		{
			return await _updateChecklistService.UpdateAsync(id, request, cancellationToken);
		}

		/// <inheritdoc />
		public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken)
		{
			var checklist = await _checklistRepository.GetByIdAsync(id, cancellationToken);
			if (checklist == null || checklist.OwnerId != _currentUserService.UserId)
				return false;

			_checklistRepository.Remove(checklist);
			await _unitOfWork.SaveChangesAsync(cancellationToken);
			return true;
		}

		/// <inheritdoc />
		public async Task<bool> PublishAsync(long id, CancellationToken cancellationToken)
		{
			var checklist = await _checklistRepository.GetByIdAsync(id, cancellationToken);
			if (checklist == null || checklist.OwnerId != _currentUserService.UserId)
				return false;

			checklist.Publish();
			await _unitOfWork.SaveChangesAsync(cancellationToken);
			return true;
		}

		/// <inheritdoc />
		public async Task<bool> UnpublishAsync(long id, CancellationToken cancellationToken)
		{
			var checklist = await _checklistRepository.GetByIdAsync(id, cancellationToken);
			if (checklist == null || checklist.OwnerId != _currentUserService.UserId)
				return false;

			checklist.Unpublish();
			await _unitOfWork.SaveChangesAsync(cancellationToken);
			return true;
		}

		/// <inheritdoc />
		public async Task<string?> RegenerateAccessTokenAsync(long id, CancellationToken cancellationToken)
		{
			var checklist = await _checklistRepository.GetByIdAsync(id, cancellationToken);
			if (checklist == null || checklist.OwnerId != _currentUserService.UserId)
				return null;

			checklist.RegenerateAccessToken();
			await _unitOfWork.SaveChangesAsync(cancellationToken);
			return checklist.AccessToken;
		}

		/// <inheritdoc />
		public async Task<ChecklistPagedResponse> GetPagedAsync(ChecklistSearchRequest searchRequest, CancellationToken cancellationToken)
		{
			// Validar parâmetros
			var page = Math.Max(1, searchRequest.Page);
			var pageSize = Math.Min(100, Math.Max(1, searchRequest.PageSize)); // Máximo 100 itens por página

			// Buscar dados paginados
			var (checklists, totalCount) = await _checklistRepository.GetPagedAsync(
				searchRequest.Search,
				page,
				pageSize,
				searchRequest.Filters.Statuses.Select(x => (Domain.Models.Enums.ChecklistStatus)x),
				cancellationToken);

			// Mapear para response
			var checklistResponses = checklists.Select(_mapChecklistToResponse.MapToResponse);

			// Calcular informações de paginação
			var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

			return new ChecklistPagedResponse
			{
				Data = checklistResponses,
				TotalCount = totalCount,
				Page = page,
				PageSize = pageSize,
				TotalPages = totalPages,
				HasPreviousPage = page > 1,
				HasNextPage = page < totalPages
			};
		}
	}
}