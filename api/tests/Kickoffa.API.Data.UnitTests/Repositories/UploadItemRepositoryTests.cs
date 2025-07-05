using Kickoffa.API.Data.Repositories;
using Kickoffa.API.Data.UnitTests.Repositories.DbContext;
using Kickoffa.API.Domain.Models;
using Kickoffa.API.Domain.Models.Items;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.UnitTests.Repositories;

public class UploadItemRepositoryTests : IClassFixture<KickoffaDbContextFixture>, IDisposable
{
	private readonly KickoffaDbContextFixture _dbContextFixture;
	private readonly EntityFramework.Context.KickoffaDbContext _dbContext;
	private readonly UploadItemRepository _repository;
	private readonly CancellationToken _cancellationToken;

	public UploadItemRepositoryTests(KickoffaDbContextFixture dbContextFixture)
	{
		_dbContextFixture = dbContextFixture;
		_dbContext = _dbContextFixture.GetNewDbContext();
		_repository = new UploadItemRepository(_dbContext);
		_cancellationToken = CancellationToken.None;

		SeedTestData();
	}

	[Fact]
	public async Task GetBySectionIdAsync_ShouldReturnUploadItems_WhenExists()
	{
		// Arrange
		var section = await _dbContext.Sections.FirstAsync(_cancellationToken);

		// Act
		var result = await _repository.GetBySectionIdAsync(section.Id, _cancellationToken);

		// Assert
		var items = result.ToList();
		Assert.Equal(3, items.Count);
		Assert.All(items, i => Assert.Equal(section.Id, i.SectionId));
		
		// Verificar ordenação por Order
		Assert.Equal(1, items[0].Order);
		Assert.Equal(2, items[1].Order);
		Assert.Equal(3, items[2].Order);
	}

	[Fact]
	public async Task GetBySectionIdAsync_ShouldReturnEmpty_WhenNoUploadItems()
	{
		// Act
		var result = await _repository.GetBySectionIdAsync(999, _cancellationToken);

		// Assert
		Assert.Empty(result);
	}

	[Theory]
	[InlineData(5, 1)] // Logo da Empresa
	[InlineData(10, 1)] // Documentos
	[InlineData(50, 1)] // Vídeo Promocional
	[InlineData(25, 0)] // Nenhum item com esse tamanho
	public async Task GetByMaxSizeAsync_ShouldReturnCorrectResults(int? maxSizeMB, int expectedCount)
	{
		// Act
		var result = await _repository.GetByMaxSizeAsync(maxSizeMB, _cancellationToken);

		// Assert
		var items = result.ToList();
		Assert.Equal(expectedCount, items.Count);
		Assert.All(items, i => Assert.Equal(maxSizeMB, i.MaxSizeMB));
	}

	[Fact]
	public async Task GetByPlaceholderAsync_ShouldReturnCorrectItems()
	{
		// Act
		var result = await _repository.GetByPlaceholderAsync("Selecione o arquivo do logo", _cancellationToken);

		// Assert
		var items = result.ToList();
		Assert.Single(items);
		Assert.Equal("Selecione o arquivo do logo", items[0].Placeholder);
		Assert.Equal("Logo da Empresa", items[0].Title);
	}

	[Fact]
	public async Task GetByPlaceholderAsync_ShouldReturnEmpty_WhenNoMatches()
	{
		// Act
		var result = await _repository.GetByPlaceholderAsync("Placeholder inexistente", _cancellationToken);

		// Assert
		Assert.Empty(result);
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
		Assert.Equal("Documentos", items[1].Title);
	}

	[Fact]
	public async Task GetByChecklistIdAsync_ShouldReturnItemsFromChecklist()
	{
		// Arrange
		var checklist = await _dbContext.Checklists.FirstAsync(_cancellationToken);

		// Act
		var result = await _repository.GetByChecklistIdAsync(checklist.Id, _cancellationToken);

		// Assert
		var items = result.ToList();
		Assert.Equal(3, items.Count);
		Assert.All(items, i => Assert.NotNull(i.Section));
		Assert.All(items, i => Assert.Equal(checklist.Id, i.Section.ChecklistId));
	}

	[Fact]
	public async Task CountBySectionIdAsync_ShouldReturnCorrectCount()
	{
		// Arrange
		var section = await _dbContext.Sections.FirstAsync(_cancellationToken);

		// Act
		var result = await _repository.CountBySectionIdAsync(section.Id, _cancellationToken);

		// Assert
		Assert.Equal(3, result);
	}

	[Fact]
	public async Task CountBySectionIdAsync_ShouldReturnZero_WhenNoItems()
	{
		// Act
		var result = await _repository.CountBySectionIdAsync(999, _cancellationToken);

		// Assert
		Assert.Equal(0, result);
	}

	[Fact]
	public async Task GetBySectionIdsAsync_ShouldReturnItemsFromMultipleSections()
	{
		// Arrange
		var sections = await _dbContext.Sections.Take(2).ToListAsync(_cancellationToken);
		var sectionIds = sections.Select(s => s.Id).ToList();

		// Act
		var result = await _repository.GetBySectionIdsAsync(sectionIds, _cancellationToken);

		// Assert
		var items = result.ToList();
		Assert.NotEmpty(items);
		Assert.All(items, i => Assert.Contains(i.SectionId, sectionIds));
	}

	[Theory]
	[InlineData(1, 10, 2)] // Logo (5MB) e Documentos (10MB)
	[InlineData(6, 50, 1)] // Apenas Vídeo (50MB)
	[InlineData(1, 4, 0)] // Nenhum nessa faixa
	public async Task GetBySizeRangeAsync_ShouldReturnCorrectResults(int? minSizeMB, int? maxSizeMB, int expectedCount)
	{
		// Act
		var result = await _repository.GetBySizeRangeAsync(minSizeMB, maxSizeMB, _cancellationToken);

		// Assert
		var items = result.ToList();
		Assert.Equal(expectedCount, items.Count);
		
		if (minSizeMB.HasValue)
			Assert.All(items, i => Assert.True(i.MaxSizeMB >= minSizeMB.Value));
		
		if (maxSizeMB.HasValue)
			Assert.All(items, i => Assert.True(i.MaxSizeMB <= maxSizeMB.Value));
	}

	[Fact]
	public async Task GetByIdAsync_ShouldIncludeSectionAndRelationships()
	{
		// Arrange
		var existingItem = await _dbContext.UploadItems.FirstAsync(_cancellationToken);

		// Act
		var result = await _repository.GetByIdAsync(existingItem.Id, _cancellationToken);

		// Assert
		Assert.NotNull(result);
		Assert.NotNull(result.Section);
		Assert.Equal(existingItem.SectionId, result.Section.Id);
		Assert.NotNull(result.AllowedFileTypes);
		Assert.NotNull(result.ItemFiles);
	}

	[Fact]
	public async Task GetAllAsync_ShouldIncludeRelationshipsAndBeOrdered()
	{
		// Act
		var result = await _repository.GetAllAsync(_cancellationToken);

		// Assert
		var items = result.ToList();
		Assert.NotEmpty(items);
		Assert.All(items, i => Assert.NotNull(i.Section));
		Assert.All(items, i => Assert.NotNull(i.AllowedFileTypes));
		Assert.All(items, i => Assert.NotNull(i.ItemFiles));
		
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

	private void SeedTestData()
	{
		// Criar checklist e seção primeiro
		var checklist = new Checklist(1, "Test Checklist", "test-checklist", "Descrição", null);
		_dbContext.Checklists.Add(checklist);
		_dbContext.SaveChanges();

		var section = new ChecklistSection(checklist.Id, "Test Section", 1);
		_dbContext.Sections.Add(section);
		_dbContext.SaveChanges();

		// Criar itens de upload com diferentes configurações
		var uploadItems = new List<UploadItem>
		{
			new(section.Id, "Logo da Empresa", 1, "Upload do logo da empresa", true, "Selecione o arquivo do logo", 5),
			new(section.Id, "Documentos", 2, "Upload de documentos", true, "Selecione os documentos", 10),
			new(section.Id, "Vídeo Promocional", 3, "Upload do vídeo promocional", false, "Selecione o vídeo", 50)
		};

		_dbContext.UploadItems.AddRange(uploadItems);
		_dbContext.SaveChanges();
	}

	public void Dispose()
	{
		_dbContext.Dispose();
		GC.SuppressFinalize(this);
	}
}