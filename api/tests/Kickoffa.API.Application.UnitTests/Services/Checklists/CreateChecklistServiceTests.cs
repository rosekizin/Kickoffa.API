using Bogus;
using Kickoffa.API.Application.Interfaces.Checkilists;
using Kickoffa.API.Application.Interfaces.Factories;
using Kickoffa.API.Application.Services.Checklists;
using Kickoffa.API.Contracts.Checklist;
using Kickoffa.API.Contracts.Checklist.Sections;
using Kickoffa.API.Domain.Models;
using Kickoffa.API.Domain.Repositories;
using NSubstitute;

namespace Kickoffa.API.Application.UnitTests.Services.Checklists
{
	public class CreateChecklistServiceTests
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly ISectionFactory _sectionFactory;
		private readonly IChecklistFactory _checklistFactory;
		private readonly CancellationToken _cancellationToken;
		private readonly IChecklistRepository _checklistRepository;
		private readonly CreateChecklistService _createChecklistService;
		private readonly IMapChecklistToResponse _mapChecklistToResponse;

		public CreateChecklistServiceTests()
		{
			_cancellationToken = new();
			_unitOfWork = Substitute.For<IUnitOfWork>();
			_sectionFactory = Substitute.For<ISectionFactory>();
			_checklistFactory = Substitute.For<IChecklistFactory>();
			_checklistRepository = Substitute.For<IChecklistRepository>();
			_mapChecklistToResponse = Substitute.For<IMapChecklistToResponse>();
			_createChecklistService = new CreateChecklistService(_unitOfWork, _sectionFactory, _checklistFactory, _checklistRepository, _mapChecklistToResponse);
		}

		[Fact]
		public async Task CreateAsync_ShouldCreateChecklistAndReturnResponse_WhenValidRequest()
		{
			// Arrange
			var ownerId = 1L;
			var request = CreateValidChecklistRequest();

			_checklistRepository
				.ExistsBySlugAsync(Arg.Any<string>(), null, _cancellationToken)
				.Returns(false);

			var expectedSlug = "test-acao-checklist";
			var newChecklist = new Checklist(ownerId, request.Title, expectedSlug, request.Description, request.Deadline);
			_checklistFactory
				.CreateChecklist(ownerId, request.Title, request.Description, request.Deadline, expectedSlug)
				.Returns(newChecklist);

			foreach (var sectionRequest in request.Sections)
			{
				_sectionFactory
					.CreateSection(sectionRequest, _cancellationToken)
					.Returns(Substitute.For<Section>());
			}

			_checklistRepository.AddAsync(newChecklist, _cancellationToken).Returns(Task.CompletedTask);
			_checklistRepository.AddAsync(newChecklist, _cancellationToken).Returns(Task.CompletedTask);

			var checklistResponse = new Faker<ChecklistResponse>().Generate();
			_mapChecklistToResponse.MapToResponse(newChecklist).Returns(checklistResponse);

			// Act
			var result = await _createChecklistService.CreateAsync(ownerId, request, _cancellationToken);

			// Assert
			Assert.NotNull(result);
			await _sectionFactory.Received(request.Sections.Count).CreateSection(Arg.Is<SectionRequest>(x => request.Sections.Contains(x)), _cancellationToken);
			await _checklistRepository.Received(1).AddAsync(Arg.Any<Checklist>(), _cancellationToken);
			await _unitOfWork.Received(1).SaveChangesAsync(_cancellationToken);
		}

		[Fact]
		public async Task CreateAsync_ShouldGenerateUniqueSlug_WhenSlugAlreadyExists()
		{
			// Arrange
			var ownerId = 1L;
			var request = CreateValidChecklistRequest();

			// Simular que o primeiro slug já existe, mas o segundo não
			_checklistRepository.ExistsBySlugAsync("test-acao-checklist", null, _cancellationToken)
				.Returns(true);
			_checklistRepository.ExistsBySlugAsync("test-acao-checklist-1", null, _cancellationToken)
				.Returns(false);

			var expectedSlug = "test-acao-checklist-1";
			var newChecklist = new Checklist(ownerId, request.Title, expectedSlug, request.Description, request.Deadline);
			_checklistFactory
				.CreateChecklist(ownerId, request.Title, request.Description, request.Deadline, expectedSlug)
				.Returns(newChecklist);

			foreach (var sectionRequest in request.Sections)
			{
				_sectionFactory
					.CreateSection(sectionRequest, _cancellationToken)
					.Returns(Substitute.For<Section>());
			}

			_checklistRepository.AddAsync(newChecklist, _cancellationToken).Returns(Task.CompletedTask);
			_checklistRepository.AddAsync(newChecklist, _cancellationToken).Returns(Task.CompletedTask);

			var checklistResponse = new Faker<ChecklistResponse>()
				.RuleFor(x => x.Slug, expectedSlug)
				.Generate();
			_mapChecklistToResponse.MapToResponse(newChecklist).Returns(checklistResponse);

			// Act
			var result = await _createChecklistService.CreateAsync(ownerId, request, CancellationToken.None);

			// Assert
			Assert.NotNull(result);
			Assert.Equal("test-acao-checklist-1", result.Slug);

			await _sectionFactory
				.Received(request.Sections.Count)
				.CreateSection(Arg.Is<SectionRequest>(x => request.Sections.Contains(x)), _cancellationToken);
			await _checklistRepository.Received(1).ExistsBySlugAsync("test-acao-checklist", null, _cancellationToken);
			await _checklistRepository.Received(1).ExistsBySlugAsync("test-acao-checklist-1", null, _cancellationToken);
		}

		private static ChecklistRequest CreateValidChecklistRequest()
		{
			return new ChecklistRequest
			{
				Id = 1,
				Title = "Test Ação - Checklist $",
				Description = "Test description",
				Deadline = DateTime.UtcNow.AddDays(7),
				Sections =
				[
					new BriefingSectionRequest
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
	}
}