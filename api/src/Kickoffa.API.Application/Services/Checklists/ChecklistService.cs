using Kickoffa.API.Application.Interfaces.Checkilists;
using Kickoffa.API.Contracts.Checklist;
using Kickoffa.API.Domain.Repositories;

namespace Kickoffa.API.Application.Services.Checklists
{
	/// <summary>
	/// Serviço para operações relacionadas a Checklist
	/// </summary>
	public sealed class ChecklistService : IChecklistService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IChecklistRepository _checklistRepository;
		private readonly IMapChecklistToResponse _mapChecklistToResponse;

		/// <summary>
		/// Inicializa uma nova instância do ChecklistService
		/// </summary>
		/// <param name="checklistRepository">Repositório de checklists</param>
		/// <param name="unitOfWork">Unit of Work para transações</param>
		public ChecklistService(
			IUnitOfWork unitOfWork,
			IChecklistRepository checklistRepository,
			IMapChecklistToResponse mapChecklistToResponse)
		{
			_unitOfWork = unitOfWork;
			_checklistRepository = checklistRepository;
			_mapChecklistToResponse = mapChecklistToResponse;
		}

		/// <inheritdoc />
		public async Task<IEnumerable<ChecklistResponse>> GetByOwnerIdAsync(long ownerId, CancellationToken cancellationToken)
		{
			var checklists = await _checklistRepository.GetByOwnerIdAsync(ownerId, cancellationToken);
			return checklists.Select(_mapChecklistToResponse.MapToResponse);
		}

		/// <inheritdoc />
		public async Task<ChecklistResponse?> GetByIdAsync(long id, CancellationToken cancellationToken)
		{
			var checklist = await _checklistRepository.GetByIdAsync(id, cancellationToken);
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
		public async Task<ChecklistResponse?> UpdateAsync(long id, long ownerId, ChecklistRequest request, CancellationToken cancellationToken)
		{
			var checklist = await _checklistRepository.GetByIdAsync(id, cancellationToken);
			if (checklist == null || checklist.OwnerId != ownerId)
				return null;

			// Atualizar propriedades básicas
			checklist.UpdateTitle(request.Title);
			checklist.UpdateDescription(request.Description);
			checklist.UpdateDueDate(request.Deadline);

			// TODO: Implementar atualização de seções (mais complexo, requer comparação)
			// Por enquanto, vamos apenas atualizar as propriedades básicas

			await _unitOfWork.SaveChangesAsync(cancellationToken);
			return _mapChecklistToResponse.MapToResponse(checklist);
		}

		/// <inheritdoc />
		public async Task<bool> DeleteAsync(long id, long ownerId, CancellationToken cancellationToken)
		{
			var checklist = await _checklistRepository.GetByIdAsync(id, cancellationToken);
			if (checklist == null || checklist.OwnerId != ownerId)
				return false;

			_checklistRepository.Remove(checklist);
			await _unitOfWork.SaveChangesAsync(cancellationToken);
			return true;
		}

		/// <inheritdoc />
		public async Task<bool> PublishAsync(long id, long ownerId, CancellationToken cancellationToken)
		{
			var checklist = await _checklistRepository.GetByIdAsync(id, cancellationToken);
			if (checklist == null || checklist.OwnerId != ownerId)
				return false;

			checklist.Publish();
			await _unitOfWork.SaveChangesAsync(cancellationToken);
			return true;
		}

		/// <inheritdoc />
		public async Task<bool> UnpublishAsync(long id, long ownerId, CancellationToken cancellationToken)
		{
			var checklist = await _checklistRepository.GetByIdAsync(id, cancellationToken);
			if (checklist == null || checklist.OwnerId != ownerId)
				return false;

			checklist.Unpublish();
			await _unitOfWork.SaveChangesAsync(cancellationToken);
			return true;
		}

		/// <inheritdoc />
		public async Task<string?> RegenerateAccessTokenAsync(long id, long ownerId, CancellationToken cancellationToken)
		{
			var checklist = await _checklistRepository.GetByIdAsync(id, cancellationToken);
			if (checklist == null || checklist.OwnerId != ownerId)
				return null;

			checklist.RegenerateAccessToken();
			await _unitOfWork.SaveChangesAsync(cancellationToken);
			return checklist.AccessToken;
		}
	}
}