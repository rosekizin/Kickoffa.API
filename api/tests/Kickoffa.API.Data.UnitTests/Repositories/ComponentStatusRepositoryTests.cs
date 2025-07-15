using Kickoffa.API.Data.EntityFramework.Context;
using Kickoffa.API.Data.Repositories;
using Kickoffa.API.Data.UnitTests.Repositories.DbContext;
using Kickoffa.API.Domain.Models;
using Kickoffa.API.Domain.Models.Components;
using Kickoffa.API.Domain.Models.Components.Base;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.UnitTests.Repositories;

public class ComponentStatusRepositoryTests : IClassFixture<KickoffaDbContextFixture>, IDisposable
{
	private readonly KickoffaDbContext _dbContext;
	private readonly ComponentStatusRepository _repository;
	private readonly CancellationToken _cancellationToken;

	public ComponentStatusRepositoryTests(KickoffaDbContextFixture dbContextFixture)
	{
		var options = new DbContextOptionsBuilder<KickoffaDbContext>()
			.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
			.Options;

		_cancellationToken = new();
		_dbContext = dbContextFixture.GetNewDbContext();
		_repository = new ComponentStatusRepository(_dbContext);

		SeedTestData();
	}

	[Fact]
	public async Task GetByComponentIdAsync_ShouldReturnStatus_WhenExists()
	{
		// Arrange
		var component = await _dbContext.Components.FirstAsync(_cancellationToken);

		// Act
		var result = await _repository.GetByComponentIdAsync(component.Id, _cancellationToken);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(component.Id, result.ComponentId);
	}

	[Fact]
	public async Task GetByComponentIdAsync_ShouldReturnNull_WhenNotExists()
	{
		// Act
		var result = await _repository.GetByComponentIdAsync(999, _cancellationToken);

		// Assert
		Assert.Null(result);
	}

	[Fact]
	public async Task GetByComponentIdsAsync_ShouldReturnMatchingStatuses()
	{
		// Arrange
		var components = await _dbContext.Components.Take(2).ToListAsync(_cancellationToken);
		var componentIds = components.Select(i => i.Id).ToList();

		// Act
		var result = await _repository.GetByComponentIdsAsync(componentIds, _cancellationToken);

		// Assert
		var statuses = result.ToList();
		Assert.Equal(2, statuses.Count);
		Assert.All(statuses, s => Assert.Contains(s.ComponentId, componentIds));
	}

	[Fact]
	public async Task GetByComponentIdsAsync_ShouldReturnEmpty_WhenNoMatches()
	{
		// Arrange
		var nonExistentIds = new List<long> { 999, 998, 997 };

		// Act
		var result = await _repository.GetByComponentIdsAsync(nonExistentIds, _cancellationToken);

		// Assert
		Assert.Empty(result);
	}

	[Fact]
	public async Task GetCompletedBySectionIdAsync_ShouldReturnOnlyCompleted()
	{
		// Arrange
		var section = await _dbContext.Sections.FirstAsync(_cancellationToken);

		// Act
		var result = await _repository.GetCompletedBySectionIdAsync(section.Id, _cancellationToken);

		// Assert
		var statuses = result.ToList();
		Assert.Equal(2, statuses.Count); // Apenas os completados
		Assert.All(statuses, s => Assert.True(s.IsCompleted));
		Assert.All(statuses, s => Assert.Equal(section.Id, s.Component.SectionId));
	}

	[Fact]
	public async Task GetCompletedByChecklistIdAsync_ShouldReturnOnlyCompleted()
	{
		// Arrange
		var checklist = await _dbContext.Checklists.FirstAsync(_cancellationToken);

		// Act
		var result = await _repository.GetCompletedByChecklistIdAsync(checklist.Id, _cancellationToken);

		// Assert
		var statuses = result.ToList();
		Assert.Equal(2, statuses.Count); // Apenas os completados
		Assert.All(statuses, s => Assert.True(s.IsCompleted));
		Assert.All(statuses, s => Assert.Equal(checklist.Id, s.Component.Section.ChecklistId));
	}

	[Fact]
	public async Task CountCompletedByChecklistIdAsync_ShouldReturnCorrectCount()
	{
		// Arrange
		var checklist = await _dbContext.Checklists.FirstAsync(_cancellationToken);

		// Act
		var result = await _repository.CountCompletedByChecklistIdAsync(checklist.Id, _cancellationToken);

		// Assert
		Assert.Equal(2, result); // 2 itens completados
	}

	[Fact]
	public async Task CountCompletedByChecklistIdAsync_ShouldReturnZero_WhenNoCompleted()
	{
		// Arrange - Criar checklist sem itens completados
		var newChecklist = new Checklist(2, 1, "New Checklist", "new-checklist", null, null);
		_dbContext.Checklists.Add(newChecklist);
		await _dbContext.SaveChangesAsync(_cancellationToken);

		var newSection = new ChecklistSection(newChecklist.Id, "New Section", 1);
		_dbContext.Sections.Add(newSection);
		await _dbContext.SaveChangesAsync(_cancellationToken);

		var newComponent = new TextComponent(newSection.Id, "New Component", 1, null, false, null, null);
		_dbContext.Components.Add(newComponent);
		await _dbContext.SaveChangesAsync(_cancellationToken);

		var newStatus = new ComponentStatus(newComponent.Id, false, null, null);
		_dbContext.ComponentStatuses.Add(newStatus);
		await _dbContext.SaveChangesAsync(_cancellationToken);

		// Act
		var result = await _repository.CountCompletedByChecklistIdAsync(newChecklist.Id, _cancellationToken);

		// Assert
		Assert.Equal(0, result);
	}

	[Fact]
	public async Task CountTotalByChecklistIdAsync_ShouldReturnCorrectCount()
	{
		// Arrange
		var checklist = await _dbContext.Checklists.FirstAsync(_cancellationToken);

		// Act
		var result = await _repository.CountTotalByChecklistIdAsync(checklist.Id, _cancellationToken);

		// Assert
		Assert.Equal(3, result); // 3 itens totais
	}

	[Fact]
	public async Task CountTotalByChecklistIdAsync_ShouldReturnZero_WhenNoComponents()
	{
		// Act
		var result = await _repository.CountTotalByChecklistIdAsync(999, _cancellationToken);

		// Assert
		Assert.Equal(0, result);
	}

	[Fact]
	public async Task GetByIdAsync_ShouldIncludeComponentAndUploadedFiles()
	{
		// Arrange
		var existingStatus = await _dbContext.ComponentStatuses.FirstAsync(_cancellationToken);

		// Act
		var result = await _repository.GetByIdAsync(existingStatus.Id, _cancellationToken);

		// Assert
		Assert.NotNull(result);
		Assert.NotNull(result.Component);
		Assert.Equal(existingStatus.ComponentId, result.Component.Id);
	}

	[Fact]
	public async Task GetAllAsync_ShouldIncludeComponentAndUploadedFilesAndBeOrdered()
	{
		// Act
		var result = await _repository.GetAllAsync(_cancellationToken);

		// Assert
		var statuses = result.ToList();
		Assert.NotEmpty(statuses);
		Assert.All(statuses, s => Assert.NotNull(s.Component));

		// Verificar ordenação por data de criação (mais recente primeiro)
		for (int i = 0; i < statuses.Count - 1; i++)
		{
			Assert.True(statuses[i].CreatedDateUtc >= statuses[i + 1].CreatedDateUtc);
		}
	}

	private void SeedTestData()
	{
		// Criar estrutura básica
		var checklist = new Checklist(1, 1, "Test Checklist", "test-checklist", "Descrição", null);
		_dbContext.Checklists.Add(checklist);
		_dbContext.SaveChanges();

		var section = new ChecklistSection(checklist.Id, "Test Section", 1);
		_dbContext.Sections.Add(section);
		_dbContext.SaveChanges();

		// Criar itens
		var components = new List<Component>
		{
			new TextComponent(section.Id, "Component 1", 1, "Descrição 1", true, "Placeholder 1", 100),
			new UploadComponent(section.Id, "Component 2", 2, "Descrição 2", true, "Placeholder 2"),
			new ConfirmationComponent(section.Id, "Component 3", 3, "Descrição 3", false, "Texto de confirmação")
		};

		_dbContext.Components.AddRange(components);
		_dbContext.SaveChanges();

		// Buscar os itens salvos para obter os IDs
		var savedComponents = _dbContext.Components.OrderBy(i => i.Order).ToList();

		// Criar status (alguns completados, outros não)
		var statuses = new List<ComponentStatus>
		{
			new(savedComponents[0].Id, true, DateTime.UtcNow, "Resposta do texto"), // Completado
			new(savedComponents[1].Id, true, DateTime.UtcNow, null), // Completado
			new(savedComponents[2].Id, false, null, null) // Não completado
		};

		_dbContext.ComponentStatuses.AddRange(statuses);
		_dbContext.SaveChanges();
	}

	public void Dispose()
	{
		_dbContext.Dispose();
		GC.SuppressFinalize(this);
	}
}