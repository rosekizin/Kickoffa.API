using Kickoffa.API.Data.EntityFramework.Context;
using Kickoffa.API.Data.Repositories;
using Kickoffa.API.Data.UnitTests.Repositories.DbContext;
using Kickoffa.API.Domain.Models;
using Kickoffa.API.Domain.Models.Items;
using Kickoffa.API.Domain.Models.Items.Base;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.UnitTests.Repositories;

public class ItemStatusRepositoryTests : IClassFixture<KickoffaDbContextFixture>
{
	private readonly ItemStatusRepository _repository;
	private readonly CancellationToken _cancellationToken;
	private readonly KickoffaDbContextFixture _contextFixture;

	public ItemStatusRepositoryTests(KickoffaDbContextFixture contextFixture)
	{
		var options = new DbContextOptionsBuilder<KickoffaDbContext>()
			.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
			.Options;

		_cancellationToken = new();
		_contextFixture = contextFixture;

		_repository = new ItemStatusRepository(_contextFixture.KickoffaDbContext);

		SeedTestData();
	}

	[Fact]
	public async Task GetByItemIdAsync_ShouldReturnStatus_WhenExists()
	{
		// Arrange
		var item = await _contextFixture.KickoffaDbContext.Items.FirstAsync(_cancellationToken);

		// Act
		var result = await _repository.GetByItemIdAsync(item.Id, _cancellationToken);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(item.Id, result.ItemId);
	}

	[Fact]
	public async Task GetByItemIdAsync_ShouldReturnNull_WhenNotExists()
	{
		// Act
		var result = await _repository.GetByItemIdAsync(999, _cancellationToken);

		// Assert
		Assert.Null(result);
	}

	[Fact]
	public async Task GetByItemIdsAsync_ShouldReturnMatchingStatuses()
	{
		// Arrange
		var items = await _contextFixture.KickoffaDbContext.Items.Take(2).ToListAsync(_cancellationToken);
		var itemIds = items.Select(i => i.Id).ToList();

		// Act
		var result = await _repository.GetByItemIdsAsync(itemIds, _cancellationToken);

		// Assert
		var statuses = result.ToList();
		Assert.Equal(2, statuses.Count);
		Assert.All(statuses, s => Assert.Contains(s.ItemId, itemIds));
	}

	[Fact]
	public async Task GetByItemIdsAsync_ShouldReturnEmpty_WhenNoMatches()
	{
		// Arrange
		var nonExistentIds = new List<long> { 999, 998, 997 };

		// Act
		var result = await _repository.GetByItemIdsAsync(nonExistentIds, _cancellationToken);

		// Assert
		Assert.Empty(result);
	}

	[Fact]
	public async Task GetCompletedBySectionIdAsync_ShouldReturnOnlyCompleted()
	{
		// Arrange
		var section = await _contextFixture.KickoffaDbContext.Sections.FirstAsync(_cancellationToken);

		// Act
		var result = await _repository.GetCompletedBySectionIdAsync(section.Id, _cancellationToken);

		// Assert
		var statuses = result.ToList();
		Assert.Equal(2, statuses.Count); // Apenas os completados
		Assert.All(statuses, s => Assert.True(s.IsCompleted));
		Assert.All(statuses, s => Assert.Equal(section.Id, s.Item.SectionId));
	}

	[Fact]
	public async Task GetCompletedByChecklistIdAsync_ShouldReturnOnlyCompleted()
	{
		// Arrange
		var checklist = await _contextFixture.KickoffaDbContext.Checklists.FirstAsync(_cancellationToken);

		// Act
		var result = await _repository.GetCompletedByChecklistIdAsync(checklist.Id, _cancellationToken);

		// Assert
		var statuses = result.ToList();
		Assert.Equal(2, statuses.Count); // Apenas os completados
		Assert.All(statuses, s => Assert.True(s.IsCompleted));
		Assert.All(statuses, s => Assert.Equal(checklist.Id, s.Item.Section.ChecklistId));
	}

	[Fact]
	public async Task CountCompletedByChecklistIdAsync_ShouldReturnCorrectCount()
	{
		// Arrange
		var checklist = await _contextFixture.KickoffaDbContext.Checklists.FirstAsync(_cancellationToken);

		// Act
		var result = await _repository.CountCompletedByChecklistIdAsync(checklist.Id, _cancellationToken);

		// Assert
		Assert.Equal(2, result); // 2 itens completados
	}

	[Fact]
	public async Task CountCompletedByChecklistIdAsync_ShouldReturnZero_WhenNoCompleted()
	{
		// Arrange - Criar checklist sem itens completados
		var newChecklist = new Checklist(2, "New Checklist", "new-checklist", null, null);
		_contextFixture.KickoffaDbContext.Checklists.Add(newChecklist);
		await _contextFixture.KickoffaDbContext.SaveChangesAsync(_cancellationToken);

		var newSection = new ChecklistSection(newChecklist.Id, "New Section", 1);
		_contextFixture.KickoffaDbContext.Sections.Add(newSection);
		await _contextFixture.KickoffaDbContext.SaveChangesAsync(_cancellationToken);

		var newItem = new TextItem(newSection.Id, "New Item", 1, null, false, null, null);
		_contextFixture.KickoffaDbContext.Items.Add(newItem);
		await _contextFixture.KickoffaDbContext.SaveChangesAsync(_cancellationToken);

		var newStatus = new ItemStatus(newItem.Id, false, null, null);
		_contextFixture.KickoffaDbContext.ItemStatuses.Add(newStatus);
		await _contextFixture.KickoffaDbContext.SaveChangesAsync(_cancellationToken);

		// Act
		var result = await _repository.CountCompletedByChecklistIdAsync(newChecklist.Id, _cancellationToken);

		// Assert
		Assert.Equal(0, result);
	}

	[Fact]
	public async Task CountTotalByChecklistIdAsync_ShouldReturnCorrectCount()
	{
		// Arrange
		var checklist = await _contextFixture.KickoffaDbContext.Checklists.FirstAsync(_cancellationToken);

		// Act
		var result = await _repository.CountTotalByChecklistIdAsync(checklist.Id, _cancellationToken);

		// Assert
		Assert.Equal(3, result); // 3 itens totais
	}

	[Fact]
	public async Task CountTotalByChecklistIdAsync_ShouldReturnZero_WhenNoItems()
	{
		// Act
		var result = await _repository.CountTotalByChecklistIdAsync(999, _cancellationToken);

		// Assert
		Assert.Equal(0, result);
	}

	[Fact]
	public async Task GetByIdAsync_ShouldIncludeItemAndUploadedFiles()
	{
		// Arrange
		var existingStatus = await _contextFixture.KickoffaDbContext.ItemStatuses.FirstAsync(_cancellationToken);

		// Act
		var result = await _repository.GetByIdAsync(existingStatus.Id, _cancellationToken);

		// Assert
		Assert.NotNull(result);
		Assert.NotNull(result.Item);
		Assert.Equal(existingStatus.ItemId, result.Item.Id);
	}

	[Fact]
	public async Task GetAllAsync_ShouldIncludeItemAndUploadedFilesAndBeOrdered()
	{
		// Act
		var result = await _repository.GetAllAsync(_cancellationToken);

		// Assert
		var statuses = result.ToList();
		Assert.NotEmpty(statuses);
		Assert.All(statuses, s => Assert.NotNull(s.Item));

		// Verificar ordenação por data de criação (mais recente primeiro)
		for (int i = 0; i < statuses.Count - 1; i++)
		{
			Assert.True(statuses[i].CreatedDateUtc >= statuses[i + 1].CreatedDateUtc);
		}
	}

	private void SeedTestData()
	{
		// Criar estrutura básica
		var checklist = new Checklist(1, "Test Checklist", "test-checklist", "Descrição", null);
		_contextFixture.KickoffaDbContext.Checklists.Add(checklist);
		_contextFixture.KickoffaDbContext.SaveChanges();

		var section = new ChecklistSection(checklist.Id, "Test Section", 1);
		_contextFixture.KickoffaDbContext.Sections.Add(section);
		_contextFixture.KickoffaDbContext.SaveChanges();

		// Criar itens
		var items = new List<Item>
		{
			new TextItem(section.Id, "Item 1", 1, "Descrição 1", true, "Placeholder 1", 100),
			new UploadItem(section.Id, "Item 2", 2, "Descrição 2", true, "Placeholder 2", 5),
			new ConfirmationItem(section.Id, "Item 3", 3, "Descrição 3", false, "Texto de confirmação")
		};

		_contextFixture.KickoffaDbContext.Items.AddRange(items);
		_contextFixture.KickoffaDbContext.SaveChanges();

		// Buscar os itens salvos para obter os IDs
		var savedItems = _contextFixture.KickoffaDbContext.Items.OrderBy(i => i.Order).ToList();

		// Criar status (alguns completados, outros não)
		var statuses = new List<ItemStatus>
		{
			new(savedItems[0].Id, true, DateTime.UtcNow, "Resposta do texto"), // Completado
			new(savedItems[1].Id, true, DateTime.UtcNow, null), // Completado
			new(savedItems[2].Id, false, null, null) // Não completado
		};

		_contextFixture.KickoffaDbContext.ItemStatuses.AddRange(statuses);
		_contextFixture.KickoffaDbContext.SaveChanges();
	}
}