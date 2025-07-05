using Kickoffa.API.Data.EntityFramework.Context;
using Kickoffa.API.Data.Repositories;
using Kickoffa.API.Data.UnitTests.Repositories.DbContext;
using Kickoffa.API.Domain.Models;
using Kickoffa.API.Domain.Models.Items;
using Kickoffa.API.Domain.Models.Items.Base;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.UnitTests.Repositories;

public class ItemRepositoryTests : IClassFixture<KickoffaDbContextFixture>
{
	private readonly ItemRepository _repository;
	private readonly KickoffaDbContext _dbContext;
	private readonly CancellationToken _cancellationToken;

	public ItemRepositoryTests(KickoffaDbContextFixture contextFixture)
	{
		_cancellationToken = new();
		_dbContext = contextFixture.GetNewDbContext();
		_repository = new ItemRepository(_dbContext);
		SeedTestData();
	}

	[Fact]
	public async Task GetBySectionIdAsync_ShouldReturnItems_WhenExists()
	{
		// Arrange
		var section = await _dbContext.Sections.FirstAsync(_cancellationToken);

		// Act
		var result = await _repository.GetBySectionIdAsync(section.Id, _cancellationToken);

		// Assert
		var items = result.ToList();
		Assert.Equal(3, items.Count);
		Assert.All(items, i => Assert.Equal(section.Id, i.SectionId));
	}

	[Fact]
	public async Task GetBySectionIdAsync_ShouldReturnEmpty_WhenNoItems()
	{
		// Act
		var result = await _repository.GetBySectionIdAsync(999, _cancellationToken);

		// Assert
		Assert.Empty(result);
	}

	[Fact]
	public async Task GetBySectionIdOrderedAsync_ShouldReturnOrderedItems()
	{
		// Arrange
		var section = await _dbContext.Sections.FirstAsync(_cancellationToken);

		// Act
		var result = await _repository.GetBySectionIdOrderedAsync(section.Id, _cancellationToken);

		// Assert
		var items = result.ToList();
		Assert.Equal(3, items.Count);

		// Verificar ordenação
		Assert.Equal(1, items[0].Order);
		Assert.Equal(2, items[1].Order);
		Assert.Equal(3, items[2].Order);

		// Verificar títulos na ordem correta
		Assert.Equal("Logo da Empresa", items[0].Title);
		Assert.Equal("Cores Principais", items[1].Title);
		Assert.Equal("Confirmação Final", items[2].Title);
	}

	[Fact]
	public async Task GetNextOrderAsync_ShouldReturnCorrectNextOrder()
	{
		// Arrange
		var section = await _dbContext.Sections.FirstAsync(_cancellationToken);

		// Act
		var result = await _repository.GetNextOrderAsync(section.Id, _cancellationToken);

		// Assert
		Assert.Equal(4, result); // Próxima ordem após 3 itens existentes
	}

	[Fact]
	public async Task GetNextOrderAsync_ShouldReturnOne_WhenNoItems()
	{
		// Act
		var result = await _repository.GetNextOrderAsync(999, _cancellationToken);

		// Assert
		Assert.Equal(1, result);
	}

	[Fact]
	public async Task ReorderItemsAsync_ShouldUpdateItemOrders()
	{
		// Arrange
		var section = await _dbContext.Sections.FirstAsync(_cancellationToken);
		var items = await _dbContext.Items.Where(i => i.SectionId == section.Id).ToListAsync(_cancellationToken);
		var itemOrders = new Dictionary<long, int>
		{
			{ items[0].Id, 3 }, // Primeiro item vai para posição 3
			{ items[1].Id, 1 }, // Segundo item vai para posição 1
			{ items[2].Id, 2 }  // Terceiro item vai para posição 2
		};

		// Act
		await _repository.ReorderItemsAsync(section.Id, itemOrders, _cancellationToken);

		// Assert
		var reorderedItems = await _dbContext.Items
			.Where(i => i.SectionId == section.Id)
			.OrderBy(i => i.Order)
			.ToListAsync(_cancellationToken);

		Assert.Equal("Cores Principais", reorderedItems[0].Title); // Order = 1
		Assert.Equal("Confirmação Final", reorderedItems[1].Title); // Order = 2
		Assert.Equal("Logo da Empresa", reorderedItems[2].Title);   // Order = 3
	}

	[Fact]
	public async Task GetRequiredBySectionIdAsync_ShouldReturnOnlyRequiredItems()
	{
		// Arrange
		var section = await _dbContext.Sections.FirstAsync(_cancellationToken);

		// Act
		var result = await _repository.GetRequiredBySectionIdAsync(section.Id, _cancellationToken);

		// Assert
		var items = result.ToList();
		Assert.Equal(2, items.Count); // Apenas os obrigatórios
		Assert.All(items, i => Assert.True(i.IsRequired));

		// Verificar ordenação
		Assert.Equal("Logo da Empresa", items[0].Title);
		Assert.Equal("Cores Principais", items[1].Title);
	}

	[Fact]
	public async Task GetRequiredBySectionIdAsync_ShouldReturnEmpty_WhenNoRequiredItems()
	{
		// Arrange - Criar seção sem itens obrigatórios
		var checklist = new Checklist(1, "Test", "test", null, null);
		_dbContext.Checklists.Add(checklist);
		await _dbContext.SaveChangesAsync(_cancellationToken);

		var section = new ChecklistSection(checklist.Id, "Test Section", 1);
		_dbContext.Sections.Add(section);
		await _dbContext.SaveChangesAsync(_cancellationToken);

		var item = new TextItem(section.Id, "Optional Item", 1, "Descrição", false, "Placeholder", 100);
		_dbContext.Items.Add(item);
		await _dbContext.SaveChangesAsync(_cancellationToken);

		// Act
		var result = await _repository.GetRequiredBySectionIdAsync(section.Id, _cancellationToken);

		// Assert
		Assert.Empty(result);
	}

	[Fact]
	public async Task GetByIdAsync_ShouldIncludeSectionAndStatus()
	{
		// Arrange
		var existingItem = await _dbContext.Items.FirstAsync(_cancellationToken);

		// Act
		var result = await _repository.GetByIdAsync(existingItem.Id, _cancellationToken);

		// Assert
		Assert.NotNull(result);
		Assert.NotNull(result.Section);
		Assert.Equal(existingItem.SectionId, result.Section.Id);
		// Status pode ser null se não foi criado ainda
	}

	[Fact]
	public async Task GetAllAsync_ShouldIncludeSectionAndStatusAndBeOrdered()
	{
		// Act
		var result = await _repository.GetAllAsync(_cancellationToken);

		// Assert
		var items = result.ToList();
		Assert.NotEmpty(items);
		Assert.All(items, i => Assert.NotNull(i.Section));

		// Verificar ordenação por SectionId e depois por Order
		for (int i = 0; i < items.Count - 1; i++)
		{
			if (items[i].SectionId == items[i + 1].SectionId)
			{
				Assert.True(items[i].Order <= items[i + 1].Order);
			}
			else
			{
				Assert.True(items[i].SectionId <= items[i + 1].SectionId);
			}
		}
	}

	[Fact]
	public async Task GetBySectionIdAsync_Generic_ShouldReturnSpecificType()
	{
		// Arrange
		var section = await _dbContext.Sections.FirstAsync(_cancellationToken);

		// Act - Buscar apenas TextItems
		var textItems = await _repository.GetBySectionIdAsync<TextItem>(section.Id, _cancellationToken);
		var uploadItems = await _repository.GetBySectionIdAsync<UploadItem>(section.Id, _cancellationToken);

		// Assert
		var textItemsList = textItems.ToList();
		var uploadItemsList = uploadItems.ToList();

		Assert.Single(textItemsList); // Apenas 1 TextItem
		Assert.Single(uploadItemsList); // Apenas 1 UploadItem

		Assert.All(textItemsList, i => Assert.IsType<TextItem>(i));
		Assert.All(uploadItemsList, i => Assert.IsType<UploadItem>(i));
	}

	[Theory]
	[InlineData(1)]
	public async Task CountBySectionIdAsync_Generic_ShouldReturnCorrectCount(int expectedCount)
	{
		// Arrange
		var section = await _dbContext.Sections.FirstAsync(_cancellationToken);

		// Act & Assert
		var textCount = await _repository.CountBySectionIdAsync<TextItem>(section.Id, _cancellationToken);
		var uploadCount = await _repository.CountBySectionIdAsync<UploadItem>(section.Id, _cancellationToken);
		var confirmationCount = await _repository.CountBySectionIdAsync<ConfirmationItem>(section.Id, _cancellationToken);

		Assert.Equal(expectedCount, textCount);
		Assert.Equal(expectedCount, uploadCount);
		Assert.Equal(expectedCount, confirmationCount);
	}

	private void SeedTestData()
	{
		// Criar checklist e seção primeiro
		var checklist = new Checklist(1, "Test Checklist", "test-checklist", "Descrição", null);
		_dbContext.Checklists.Add(checklist);
		_dbContext.SaveChanges();

		var section = new ChecklistSection(checklist.Id, "Test Section", 1);
		_dbContext.Sections.Add(section);
		_dbContext.SaveChanges();

		// Criar itens de diferentes tipos
		var items = new List<Item>
		{
			new UploadItem(section.Id, "Logo da Empresa",  1, "Upload do logo", true, "Selecione o arquivo do logo", 5),
			new TextItem(section.Id, "Cores Principais", 2, "Informe as cores", true, "Ex: #FF0000, #00FF00", 200),
			new ConfirmationItem(section.Id, "Confirmação Final", 3, "Confirme os dados", false, "Confirmo que todas as informações estão corretas")
		};

		_dbContext.Items.AddRange(items);
		_dbContext.SaveChanges();
	}
}