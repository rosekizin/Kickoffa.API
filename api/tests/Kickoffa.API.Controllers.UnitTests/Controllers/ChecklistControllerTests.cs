using Kickoffa.API.Application.Interfaces.Checkilists;
using Kickoffa.API.Contracts.Checklist;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Security.Claims;

namespace Kickoffa.API.Controllers.UnitTests.Controllers
{
	public class ChecklistControllerTests
	{
		private readonly IChecklistService _checklistService;
		private readonly ILogger<ChecklistController> _logger;
		private readonly ChecklistController _checklistController;
		private readonly ICreateChecklistService _createChecklistService;

		public ChecklistControllerTests()
		{
			_checklistService = Substitute.For<IChecklistService>();
			_logger = Substitute.For<ILogger<ChecklistController>>();
			_createChecklistService = Substitute.For<ICreateChecklistService>();
			_checklistController = new ChecklistController(_checklistService, _logger, _createChecklistService);

			// Mock HttpContext com usuário autenticado
			SetupAuthenticatedUser(1L);
		}

		[Fact]
		public async Task GetAllAsync_ShouldReturnOk_WhenUserIsAuthenticated()
		{
			// Arrange
			var userId = 1L;
			var expectedChecklists = new List<ChecklistResponse>
			{
				CreateChecklistResponse(1, userId, "Checklist 1"),
				CreateChecklistResponse(2, userId, "Checklist 2")
			};

			_checklistService.GetByOwnerIdAsync(userId, Arg.Any<CancellationToken>())
				.Returns(expectedChecklists);

			// Act
			var result = await _checklistController.GetAllAsync(CancellationToken.None);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			var value = Assert.IsAssignableFrom<IEnumerable<ChecklistResponse>>(okResult.Value);
			Assert.Equal(2, value.Count());

			await _checklistService.Received(1).GetByOwnerIdAsync(userId, Arg.Any<CancellationToken>());
		}

		[Fact]
		public async Task GetAllAsync_ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
		{
			// Arrange
			SetupUnauthenticatedUser();

			// Act
			var result = await _checklistController.GetAllAsync(CancellationToken.None);

			// Assert
			var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
			Assert.Equal("Usuário não autenticado", unauthorizedResult.Value);

			await _checklistService.DidNotReceive().GetByOwnerIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>());
		}

		[Fact]
		public async Task GetByIdAsync_ShouldReturnOk_WhenChecklistExistsAndBelongsToUser()
		{
			// Arrange
			var checklistId = 1L;
			var userId = 1L;
			var expectedChecklist = CreateChecklistResponse(checklistId, userId, "Test Checklist");

			_checklistService.GetByIdAsync(checklistId, Arg.Any<CancellationToken>())
				.Returns(expectedChecklist);

			// Act
			var result = await _checklistController.GetByIdAsync(checklistId, CancellationToken.None);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			var value = Assert.IsType<ChecklistResponse>(okResult.Value);
			Assert.Equal(checklistId, value.Id);

			await _checklistService.Received(1).GetByIdAsync(checklistId, Arg.Any<CancellationToken>());
		}

		[Fact]
		public async Task GetByIdAsync_ShouldReturnNotFound_WhenChecklistDoesNotExist()
		{
			// Arrange
			var checklistId = 1L;

			_checklistService.GetByIdAsync(checklistId, Arg.Any<CancellationToken>())
				.Returns((ChecklistResponse?)null);

			// Act
			var result = await _checklistController.GetByIdAsync(checklistId, CancellationToken.None);

			// Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
			Assert.Equal("Checklist não encontrado", notFoundResult.Value);

			await _checklistService.Received(1).GetByIdAsync(checklistId, Arg.Any<CancellationToken>());
		}

		[Fact]
		public async Task GetByIdAsync_ShouldReturnForbid_WhenChecklistDoesNotBelongToUser()
		{
			// Arrange
			var checklistId = 1L;
			var differentUserId = 2L;
			var checklist = CreateChecklistResponse(checklistId, differentUserId, "Test Checklist");

			_checklistService.GetByIdAsync(checklistId, Arg.Any<CancellationToken>())
				.Returns(checklist);

			// Act
			var result = await _checklistController.GetByIdAsync(checklistId, CancellationToken.None);

			// Assert
			var forbidResult = Assert.IsType<ForbidResult>(result.Result);

			await _checklistService.Received(1).GetByIdAsync(checklistId, Arg.Any<CancellationToken>());
		}

		[Fact]
		public async Task CreateAsync_ShouldReturnCreated_WhenValidRequest()
		{
			// Arrange
			var userId = 1L;
			var request = CreateValidChecklistRequest();
			var expectedResponse = CreateChecklistResponse(1, userId, request.Title);

			_createChecklistService.CreateAsync(userId, request, Arg.Any<CancellationToken>())
				.Returns(expectedResponse);

			// Act
			var result = await _checklistController.CreateAsync(request, CancellationToken.None);

			// Assert
			var createdResult = Assert.IsType<CreatedResult>(result.Result);
			var value = Assert.IsType<ChecklistResponse>(createdResult.Value);
			Assert.Equal(expectedResponse.Id, value.Id);
			Assert.Equal(expectedResponse.Title, value.Title);

			await _createChecklistService.Received(1).CreateAsync(userId, request, Arg.Any<CancellationToken>());
		}

		[Fact]
		public async Task CreateAsync_ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
		{
			// Arrange
			SetupUnauthenticatedUser();
			var request = CreateValidChecklistRequest();

			// Act
			var result = await _checklistController.CreateAsync(request, CancellationToken.None);

			// Assert
			var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
			Assert.Equal("Usuário não autenticado", unauthorizedResult.Value);

			await _createChecklistService.DidNotReceive().CreateAsync(Arg.Any<long>(), Arg.Any<ChecklistRequest>(), Arg.Any<CancellationToken>());
		}

		[Fact]
		public async Task CreateAsync_ShouldReturnInternalServerError_WhenServiceThrowsException()
		{
			// Arrange
			var userId = 1L;
			var request = CreateValidChecklistRequest();

			_createChecklistService.CreateAsync(userId, request, Arg.Any<CancellationToken>())
				.ThrowsAsync(new Exception("Database error"));

			// Act
			var result = await _checklistController.CreateAsync(request, CancellationToken.None);

			// Assert
			var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
			Assert.Equal(500, statusCodeResult.StatusCode);
			Assert.Equal("Erro interno do servidor", statusCodeResult.Value);
		}

		[Fact]
		public async Task DeleteAsync_ShouldReturnNoContent_WhenChecklistDeletedSuccessfully()
		{
			// Arrange
			var checklistId = 1L;
			var userId = 1L;

			_checklistService.DeleteAsync(checklistId, userId, Arg.Any<CancellationToken>())
				.Returns(true);

			// Act
			var result = await _checklistController.DeleteAsync(checklistId, CancellationToken.None);

			// Assert
			Assert.IsType<NoContentResult>(result);

			await _checklistService.Received(1).DeleteAsync(checklistId, userId, Arg.Any<CancellationToken>());
		}

		[Fact]
		public async Task DeleteAsync_ShouldReturnNotFound_WhenChecklistNotFoundOrAccessDenied()
		{
			// Arrange
			var checklistId = 1L;
			var userId = 1L;

			_checklistService.DeleteAsync(checklistId, userId, Arg.Any<CancellationToken>())
				.Returns(false);

			// Act
			var result = await _checklistController.DeleteAsync(checklistId, CancellationToken.None);

			// Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal("Checklist não encontrado ou acesso negado", notFoundResult.Value);

			await _checklistService.Received(1).DeleteAsync(checklistId, userId, Arg.Any<CancellationToken>());
		}

		[Fact]
		public async Task PublishAsync_ShouldReturnOk_WhenChecklistPublishedSuccessfully()
		{
			// Arrange
			var checklistId = 1L;
			var userId = 1L;

			_checklistService.PublishAsync(checklistId, userId, Arg.Any<CancellationToken>())
				.Returns(true);

			// Act
			var result = await _checklistController.PublishAsync(checklistId, CancellationToken.None);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var value = okResult.Value;
			Assert.NotNull(value);

			await _checklistService.Received(1).PublishAsync(checklistId, userId, Arg.Any<CancellationToken>());
		}

		[Fact]
		public async Task RegenerateAccessTokenAsync_ShouldReturnOk_WhenTokenRegeneratedSuccessfully()
		{
			// Arrange
			var checklistId = 1L;
			var userId = 1L;
			var newToken = "new-access-token";

			_checklistService.RegenerateAccessTokenAsync(checklistId, userId, Arg.Any<CancellationToken>())
				.Returns(newToken);

			// Act
			var result = await _checklistController.RegenerateAccessTokenAsync(checklistId, CancellationToken.None);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var value = okResult.Value;
			Assert.NotNull(value);

			await _checklistService.Received(1).RegenerateAccessTokenAsync(checklistId, userId, Arg.Any<CancellationToken>());
		}

		#region Helper Methods

		private void SetupAuthenticatedUser(long userId)
		{
			var claims = new List<Claim>
		{
			new(ClaimTypes.NameIdentifier, userId.ToString())
		};

			var identity = new ClaimsIdentity(claims, "Test");
			var principal = new ClaimsPrincipal(identity);

			_checklistController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = principal
				}
			};
		}

		private void SetupUnauthenticatedUser()
		{
			_checklistController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext()
			};
		}

		private static ChecklistResponse CreateChecklistResponse(long id, long ownerId, string title)
		{
			return new ChecklistResponse
			{
				Id = id,
				OwnerId = ownerId,
				Title = title,
				Slug = title.ToLowerInvariant().Replace(" ", "-"),
				Description = "Test description",
				Deadline = DateTime.UtcNow.AddDays(7),
				AccessToken = "test-token",
				IsPublished = false,
				Sections = new List<SectionResponse>(),
				CreatedDateUtc = DateTime.UtcNow,
				LastUpdatedDateUtc = DateTime.UtcNow
			};
		}

		private static ChecklistRequest CreateValidChecklistRequest()
		{
			return new ChecklistRequest
			{
				Id = 1,
				Title = "Test Checklist",
				Description = "Test description",
				Deadline = DateTime.UtcNow.AddDays(7),
				Sections =
				[
					new SectionRequest
					{
						Id = 1,
						Title = "Test Section",
						Type = SectionTypeRequest.Briefing,
						Order = 1,
						ContentHtml = "<p>Test content</p>"
					}
				]
			};
		}

		#endregion
	}
}