using Bogus;
using Kickoffa.API.Application.Interfaces.Checkilists;
using Kickoffa.API.Application.Services.Checklists;
using Kickoffa.API.Contracts.Checklist;
using Kickoffa.API.Domain.Interfaces.Models;
using Kickoffa.API.Domain.Models;
using Kickoffa.API.Domain.Repositories;
using Kickoffa.API.Domain.Services;
using NSubstitute;

namespace Kickoffa.API.Application.UnitTests.Services.Checklists
{
	public class ChecklistServiceTests
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly ChecklistService _checklistService;
		private readonly CancellationToken _cancellationToken;
		private readonly ICurrentUserService _currentUserService;
		private readonly IChecklistRepository _checklistRepository;
		private readonly IUpdateChecklistService _updateChecklistService;
		private readonly IMapChecklistToResponse _mapChecklistToResponse;

		public ChecklistServiceTests()
		{
			_cancellationToken = new();
			_unitOfWork = Substitute.For<IUnitOfWork>();
			_currentUserService = Substitute.For<ICurrentUserService>();
			_checklistRepository = Substitute.For<IChecklistRepository>();
			_updateChecklistService = Substitute.For<IUpdateChecklistService>();
			_mapChecklistToResponse = Substitute.For<IMapChecklistToResponse>();
			_checklistService = new ChecklistService(_unitOfWork, _currentUserService, _checklistRepository, _updateChecklistService, _mapChecklistToResponse);
		}

		[Fact]
		public async Task GetAllAsync_ShouldReturnMappedResponse_WhenRepositoryReturnsData()
		{
			// Arrange
			var checklist1 = Substitute.For<IChecklist>();
			var checklist2 = Substitute.For<IChecklist>();

			var checklists = new List<IChecklist> { checklist1, checklist2 };

			_checklistRepository
				.GetAllAsync(Arg.Any<CancellationToken>())
				.Returns(checklists);

			var checklistResponse = new Faker<ChecklistResponse>().Generate();
			_mapChecklistToResponse
				.MapToResponse(Arg.Any<IChecklist>())
				.Returns(checklistResponse);

			// Act
			var result = await _checklistService.GetAllAsync(_cancellationToken);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(2, result.Count());

			await _checklistRepository.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
		}

		[Fact]
		public async Task GetByIdAsync_ShouldReturnMappedResponse_WhenChecklistExists()
		{
			// Arrange
			var checklistId = 1L;
			var checklist = Substitute.For<IChecklist>();

			_checklistRepository
				.GetByIdWithCompleteHierarchyAsync(checklistId, Arg.Any<CancellationToken>())
				.Returns(checklist);

			var checklistResponse = new Faker<ChecklistResponse>().Generate();
			_mapChecklistToResponse
				.MapToResponse(checklist)
				.Returns(checklistResponse);

			// Act
			var result = await _checklistService.GetByIdAsync(checklistId, _cancellationToken);

			// Assert
			Assert.NotNull(result);
			Assert.Same(checklistResponse, result);

			await _checklistRepository
				.Received(1)
				.GetByIdWithCompleteHierarchyAsync(checklistId, Arg.Any<CancellationToken>());
		}

		[Fact]
		public async Task GetByIdAsync_ShouldReturnNull_WhenChecklistDoesNotExist()
		{
			// Arrange
			var checklistId = 1L;

			_checklistRepository.GetByIdWithCompleteHierarchyAsync(checklistId, Arg.Any<CancellationToken>())
				.Returns((Checklist?)null);

			// Act
			var result = await _checklistService.GetByIdAsync(checklistId, _cancellationToken);

			// Assert
			Assert.Null(result);

			await _checklistRepository
				.Received(1)
				.GetByIdWithCompleteHierarchyAsync(checklistId, Arg.Any<CancellationToken>());
		}

		[Fact]
		public async Task DeleteAsync_ShouldReturnTrue_WhenChecklistExistsAndBelongsToUser()
		{
			// Arrange
			var checklistId = 1L;
			var ownerId = 1L;
			var checklist = CreateChecklist(checklistId, ownerId, "Test Checklist", "test-checklist");

			_checklistRepository.GetByIdAsync(checklistId, Arg.Any<CancellationToken>())
				.Returns(checklist);

			_currentUserService.UserId.Returns(ownerId);

			// Act
			var result = await _checklistService.DeleteAsync(checklistId, _cancellationToken);

			// Assert
			Assert.True(result);

			await _checklistRepository.Received(1).GetByIdAsync(checklistId, Arg.Any<CancellationToken>());
			_checklistRepository.Received(1).Remove(checklist);
			await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
			_currentUserService.Received(1).UserId.Returns(ownerId);
		}

		[Fact]
		public async Task DeleteAsync_ShouldReturnFalse_WhenChecklistDoesNotExist()
		{
			// Arrange
			var checklistId = 1L;
			var ownerId = 1L;

			_checklistRepository.GetByIdAsync(checklistId, Arg.Any<CancellationToken>())
				.Returns((Checklist?)null);

			_currentUserService.UserId.Returns(ownerId);

			// Act
			var result = await _checklistService.DeleteAsync(checklistId, _cancellationToken);

			// Assert
			Assert.False(result);

			await _checklistRepository.Received(1).GetByIdAsync(checklistId, Arg.Any<CancellationToken>());
			_checklistRepository.DidNotReceive().Remove(Arg.Any<Checklist>());
			await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
			_currentUserService.Received(1).UserId.Returns(ownerId);
		}

		[Fact]
		public async Task DeleteAsync_ShouldReturnFalse_WhenChecklistDoesNotBelongToUser()
		{
			// Arrange
			var checklistId = 1L;
			var ownerId = 1L;
			var differentOwnerId = 2L;
			var checklist = CreateChecklist(checklistId, differentOwnerId, "Test Checklist", "test-checklist");

			_checklistRepository.GetByIdAsync(checklistId, Arg.Any<CancellationToken>())
				.Returns(checklist);

			_currentUserService.UserId.Returns(ownerId);

			// Act
			var result = await _checklistService.DeleteAsync(checklistId, _cancellationToken);

			// Assert
			Assert.False(result);

			await _checklistRepository.Received(1).GetByIdAsync(checklistId, Arg.Any<CancellationToken>());
			_checklistRepository.DidNotReceive().Remove(Arg.Any<Checklist>());
			await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
			_currentUserService.Received(1).UserId.Returns(ownerId);
		}

		[Fact]
		public async Task PublishAsync_ShouldReturnTrue_WhenChecklistExistsAndBelongsToUser()
		{
			// Arrange
			var checklistId = 1L;
			var ownerId = 1L;
			var checklist = CreateChecklist(checklistId, ownerId, "Test Checklist", "test-checklist");

			_checklistRepository.GetByIdAsync(checklistId, Arg.Any<CancellationToken>())
				.Returns(checklist);

			_currentUserService.UserId.Returns(ownerId);

			// Act
			var result = await _checklistService.PublishAsync(checklistId, _cancellationToken);

			// Assert
			Assert.True(result);

			await _checklistRepository.Received(1).GetByIdAsync(checklistId, Arg.Any<CancellationToken>());
			await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
			_currentUserService.Received(1).UserId.Returns(ownerId);
		}

		[Fact]
		public async Task RegenerateAccessTokenAsync_ShouldReturnNewToken_WhenChecklistExistsAndBelongsToUser()
		{
			// Arrange
			var checklistId = 1L;
			var ownerId = 1L;
			var checklist = CreateChecklist(checklistId, ownerId, "Test Checklist", "test-checklist");

			_checklistRepository.GetByIdAsync(checklistId, Arg.Any<CancellationToken>())
				.Returns(checklist);

			_currentUserService.UserId.Returns(ownerId);

			// Act
			var result = await _checklistService.RegenerateAccessTokenAsync(checklistId, _cancellationToken);

			// Assert
			Assert.NotNull(result);
			Assert.NotEmpty(result);

			await _checklistRepository.Received(1).GetByIdAsync(checklistId, Arg.Any<CancellationToken>());
			await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
			_currentUserService.Received(1).UserId.Returns(ownerId);
		}

		#region Helper Methods

		private static Checklist CreateChecklist(long id, long ownerId, string title, string slug)
		{
			var checklist = new Checklist(ownerId, 1, title, slug, "Test description", DateTime.UtcNow.AddDays(7));

			// Usar reflexão para definir o ID (propriedade privada)
			var idProperty = typeof(Checklist).BaseType?.GetProperty("Id");
			idProperty?.SetValue(checklist, id);

			return checklist;
		}

		#endregion
	}
}