using Bogus;
using Kickoffa.API.Application.Interfaces.Checkilists;
using Kickoffa.API.Application.Interfaces.Factories;
using Kickoffa.API.Application.Services.Checklists;
using Kickoffa.API.Contracts.Checklist;
using Kickoffa.API.Contracts.Checklist.Components.Request;
using Kickoffa.API.Contracts.Checklist.Sections;
using Kickoffa.API.Domain.Interfaces.Models;
using Kickoffa.API.Domain.Models;
using Kickoffa.API.Domain.Repositories;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Kickoffa.API.Application.UnitTests.Services
{
    /// <summary>
    /// Testes unitários para UpdateChecklistService
    /// </summary>
    public class UpdateChecklistServiceTests
    {
        // Constantes para reutilização nos testes
        private const long CHECKLIST_ID = 1L;
        private const long CUSTOMER_ID = 100L;
        private const long OWNER_ID = 200L;
        private const string CHECKLIST_TITLE = "Test Checklist";
        private const string CHECKLIST_SLUG = "test-checklist";
        private const string CHECKLIST_DESCRIPTION = "Test Description";

        private readonly IUnitOfWork _unitOfWork;
        private readonly IChecklistRepository _checklistRepository;
        private readonly ISectionRepository _sectionRepository;
        private readonly IComponentRepository _componentRepository;
        private readonly ISectionFactory _sectionFactory;
        private readonly IComponentFactory _componentFactory;
        private readonly IMapChecklistToResponse _mapChecklistToResponse;
        private readonly IBriefingMediaRepository _briefingMediaRepository;
        private readonly IUpdateBriefingSectionService _updateBriefingSectionService;
        private readonly IUpdateChecklistSectionService _updateChecklistSectionService;
        private readonly UpdateChecklistService _service;

        private static readonly DateTime _lastUpdatedDateUtc = DateTime.UtcNow;
        private static readonly DateTime _createdDateUtc = DateTime.UtcNow.AddDays(-1);

        public UpdateChecklistServiceTests()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _checklistRepository = Substitute.For<IChecklistRepository>();
            _sectionRepository = Substitute.For<ISectionRepository>();
            _componentRepository = Substitute.For<IComponentRepository>();
            _sectionFactory = Substitute.For<ISectionFactory>();
            _componentFactory = Substitute.For<IComponentFactory>();
            _mapChecklistToResponse = Substitute.For<IMapChecklistToResponse>();
            _briefingMediaRepository = Substitute.For<IBriefingMediaRepository>();
            _updateBriefingSectionService = Substitute.For<IUpdateBriefingSectionService>();
            _updateChecklistSectionService = Substitute.For<IUpdateChecklistSectionService>();

            _service = new UpdateChecklistService(
                _unitOfWork,
                _checklistRepository,
                _sectionRepository,
                _componentRepository,
                _sectionFactory,
                _componentFactory,
                _mapChecklistToResponse,
                _briefingMediaRepository,
                _updateBriefingSectionService,
                _updateChecklistSectionService
            );
        }

        [Fact]
        public async Task UpdateAsync_WithValidRequest_ShouldUpdateChecklistAndReturnSuccess()
        {
            // Arrange
            var updatedTitle = "Updated Checklist";
            var updatedDescription = "Updated Description";
            var request = new ChecklistRequest
            {
                Id = CHECKLIST_ID,
                CustomerId = CUSTOMER_ID,
                Title = updatedTitle,
                Description = updatedDescription,
                Sections =
                [
                    new BriefingSectionRequest
                    {
                        Id = 1,
                        Title = "Updated Briefing Section",
                        Order = 1,
                        Type = SectionTypeRequest.Briefing,
                        ContentJson = @"{""type"": ""doc"", ""content"": []}",
                        ContentHtml = "<p>Updated content</p>"
                    }
                ]
            };

            var existingChecklist = Substitute.For<IChecklist>();

            var existingSection = new Faker<BriefingSection>()
                .CustomInstantiator(c => new BriefingSection(CHECKLIST_ID, "existing title", 1, "contentJson", "contentHtml"))
                .RuleFor(x => x.Id, 1L)
                .Generate();

            var sections = new List<ISection> { existingSection };
            existingChecklist.Sections.Returns(sections);

            var checklistResponse = new ChecklistResponse
            {
                Id = CHECKLIST_ID,
                OwnerId = OWNER_ID,
                CustomerId = CUSTOMER_ID,
                Title = CHECKLIST_TITLE,
                Slug = CHECKLIST_SLUG,
                Status = ChecklistStatus.Draft,
                Sections = [],
                CreatedDateUtc = _createdDateUtc,
                LastUpdatedDateUtc = _lastUpdatedDateUtc
            };

            _checklistRepository.GetByIdWithCompleteHierarchyAsync(CHECKLIST_ID, Arg.Any<CancellationToken>())
                .Returns(existingChecklist);
            _mapChecklistToResponse.MapToResponse(existingChecklist)
                .Returns(checklistResponse);

            // Act
            var result = await _service.UpdateAsync(CHECKLIST_ID, request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(checklistResponse, result);

            existingChecklist.Received(1).UpdateTitle(updatedTitle);
            existingChecklist.Received(1).UpdateDescription(updatedDescription);
            existingChecklist.Received(1).UpdateCustomer(CUSTOMER_ID);

            await _updateBriefingSectionService.Received(1).UpdateAsync(
                Arg.Any<BriefingSection>(),
                Arg.Any<BriefingSectionRequest>(),
                Arg.Any<CancellationToken>());

            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task UpdateAsync_WithNonExistentChecklist_ShouldReturnNull()
        {
            // Arrange
            var nonExistentChecklistId = 999L;
            var request = new ChecklistRequest
            {
                Id = nonExistentChecklistId,
                CustomerId = CUSTOMER_ID,
                Title = "Updated Checklist",
                Description = "Updated Description",
                Sections = []
            };

            _checklistRepository.GetByIdWithCompleteHierarchyAsync(nonExistentChecklistId, Arg.Any<CancellationToken>())
                .Returns((IChecklist?)null);

            // Act
            var result = await _service.UpdateAsync(nonExistentChecklistId, request, CancellationToken.None);

            // Assert
            Assert.Null(result);
            await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task UpdateAsync_WithNewSection_ShouldCreateAndAddSection()
        {
            // Arrange
            var request = new ChecklistRequest
            {
                Id = CHECKLIST_ID,
                CustomerId = CUSTOMER_ID,
                Title = "Updated Checklist",
                Description = "Updated Description",
                Sections =
                [
                    new BriefingSectionRequest
                    {
                        Id = 0, // Nova seção
						Title = "New Briefing Section",
                        Order = 1,
                        Type = SectionTypeRequest.Briefing,
                        ContentJson = @"{""type"": ""doc"", ""content"": []}",
                        ContentHtml = "<p>New content</p>"
                    }
                ]
            };

            var existingChecklist = Substitute.For<IChecklist>();
            existingChecklist.Sections.Returns([]);
            existingChecklist.Id.Returns(CHECKLIST_ID);

            var newSection = Substitute.For<IBriefingSection>();
            var checklistResponse = new ChecklistResponse
            {
                Id = CHECKLIST_ID,
                OwnerId = OWNER_ID,
                CustomerId = CUSTOMER_ID,
                Title = CHECKLIST_TITLE,
                Slug = CHECKLIST_SLUG,
                Status = ChecklistStatus.Draft,
                Sections = new List<SectionResponse>(),
                CreatedDateUtc = _createdDateUtc,
                LastUpdatedDateUtc = _lastUpdatedDateUtc
            };

            _checklistRepository.GetByIdWithCompleteHierarchyAsync(CHECKLIST_ID, Arg.Any<CancellationToken>())
                .Returns(existingChecklist);
            _sectionFactory.CreateBriefingSection(CHECKLIST_ID, Arg.Any<SectionRequest>())
                .Returns(newSection);
            _mapChecklistToResponse.MapToResponse(existingChecklist)
                .Returns(checklistResponse);

            // Act
            var result = await _service.UpdateAsync(CHECKLIST_ID, request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            _sectionFactory.Received(1).CreateBriefingSection(CHECKLIST_ID, Arg.Any<SectionRequest>());
            await _sectionRepository.Received(1).AddAsync(newSection, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task UpdateAsync_WithRemovedSection_ShouldRemoveSection()
        {
            // Arrange
            var request = new ChecklistRequest
            {
                Id = CHECKLIST_ID,
                CustomerId = CUSTOMER_ID,
                Title = "Updated Checklist",
                Description = "Updated Description",
                Sections =
                [
                    new ChecklistSectionRequest
                    {
                        Id = 10,
                        Title = "Updated Checklist Section",
                        Order = 1,
                        Type = SectionTypeRequest.Checklist,
                        Components = []
                    }
                ]
            };

            var existingChecklist = Substitute.For<IChecklist>();


            var existingSection = new Faker<BriefingSection>()
                .CustomInstantiator(c => new BriefingSection(CHECKLIST_ID, "existing title", 1, "contentJson", "contentHtml"))
                .RuleFor(x => x.Id, 1L)
                .Generate();

            var sections = new List<ISection> { existingSection };
            existingChecklist.Sections.Returns(sections);

            var checklistResponse = new ChecklistResponse
            {
                Id = CHECKLIST_ID,
                OwnerId = OWNER_ID,
                CustomerId = CUSTOMER_ID,
                Title = CHECKLIST_TITLE,
                Slug = CHECKLIST_SLUG,
                Status = ChecklistStatus.Draft,
                Sections = [],
                CreatedDateUtc = _createdDateUtc,
                LastUpdatedDateUtc = _lastUpdatedDateUtc
            };

            _checklistRepository.GetByIdWithCompleteHierarchyAsync(CHECKLIST_ID, Arg.Any<CancellationToken>())
                .Returns(existingChecklist);
            _mapChecklistToResponse.MapToResponse(existingChecklist).Returns(checklistResponse);

            // Act
            var result = await _service.UpdateAsync(CHECKLIST_ID, request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            _sectionRepository.Received(1).Remove(existingSection);
        }

        [Fact]
        public async Task UpdateAsync_WithChecklistSection_ShouldUpdateComponents()
        {
            // Arrange
            var request = new ChecklistRequest
            {
                Id = CHECKLIST_ID,
                CustomerId = CUSTOMER_ID,
                Title = "Updated Checklist",
                Description = "Updated Description",
                Sections =
                [
                    new ChecklistSectionRequest
                    {
                        Id = 1,
                        Title = "Updated Checklist Section",
                        Order = 1,
                        Type = SectionTypeRequest.Checklist,
                        Components = []
                    }
                ]
            };

            var existingChecklist = Substitute.For<IChecklist>();
            var existingSection = new Faker<ChecklistSection>()
                .CustomInstantiator(c => new ChecklistSection(CHECKLIST_ID, "title", 1))
                .RuleFor(x => x.Id, 1L)
                .Generate();

            var sections = new List<ISection> { existingSection };
            existingChecklist.Sections.Returns(sections);

            var checklistResponse = new ChecklistResponse
            {
                Id = CHECKLIST_ID,
                OwnerId = OWNER_ID,
                CustomerId = CUSTOMER_ID,
                Title = CHECKLIST_TITLE,
                Slug = CHECKLIST_SLUG,
                Status = ChecklistStatus.Draft,
                Sections = [],
                CreatedDateUtc = _createdDateUtc,
                LastUpdatedDateUtc = _lastUpdatedDateUtc
            };

            _checklistRepository.GetByIdWithCompleteHierarchyAsync(CHECKLIST_ID, Arg.Any<CancellationToken>())
                .Returns(existingChecklist);
            _mapChecklistToResponse.MapToResponse(existingChecklist).Returns(checklistResponse);

            // Act
            var result = await _service.UpdateAsync(CHECKLIST_ID, request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            await _updateChecklistSectionService.Received(1).UpdateAsync(
                Arg.Any<IChecklistSection>(),
                Arg.Any<IEnumerable<ComponentRequest>>(),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task UpdateAsync_WhenSaveChangesFails_ShouldThrowException()
        {
            // Arrange
            var request = new ChecklistRequest
            {
                Id = CHECKLIST_ID,
                CustomerId = CUSTOMER_ID,
                Title = "Updated Checklist",
                Description = "Updated Description",
                Sections = []
            };

            var existingChecklist = Substitute.For<IChecklist>();
            existingChecklist.Sections.Returns([]);

            _checklistRepository.GetByIdWithCompleteHierarchyAsync(CHECKLIST_ID, Arg.Any<CancellationToken>()).Returns(existingChecklist);
            _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.UpdateAsync(CHECKLIST_ID, request, CancellationToken.None));
        }
    }
}