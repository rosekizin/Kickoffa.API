using Kickoffa.API.Data.Repositories;
using Kickoffa.API.Data.UnitTests.Repositories.DbContext;
using Kickoffa.API.Domain.Models;
using Kickoffa.API.Domain.Models.Items;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.UnitTests.Repositories;

public class TextItemRepositoryTests : IClassFixture<KickoffaDbContextFixture>, IDisposable
{
	private readonly KickoffaDbContextFixture _dbContextFixture;
	private readonly EntityFramework.Context.KickoffaDbContext _dbContext;
	private readonly TextItemRepository _repository;
	private readonly CancellationToken _cancellationToken;

	public TextItemRepositoryTests(KickoffaDbContextFixture dbContextFixture)
	{
		_dbContextFixture = dbContextFixture;
		_dbContext = _dbContextFixture.GetNewDbContext();
		_repository = new TextItemRepository(_dbContext);
		_cancellationToken = CancellationToken.None;

		SeedTestData();
	}

	[Fact]
	public async Task GetBySectionIdAsync_ShouldReturnTextItems_WhenExists()
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
	public async Task GetBySectionIdAsync_ShouldReturnEmpty_WhenNoTextItems()
	{
		// Act
		var result = await _repository.GetBySectionIdAsync(999, _cancellationToken);

		// Assert
		Assert.Empty(result);
	}

	[Fact]
	public async Task GetByPlaceholderAsync_ShouldReturnCorrectItems()
	{
		// Act
		var result = await _repository.GetByPlaceholderAsync("Digite seu nome", _cancellationToken);

		// Assert
		var items = result.ToList();
		Assert.Single(items);
		Assert.Equal("Digite seu nome", items[0].Placeholder);
		Assert.Equal("Nome Completo", items[0].Title);
	}

	[Fact]
	public async Task GetByPlaceholderAsync_ShouldReturnEmpty_WhenNoMatches()
	{
		// Act
		var result = await _repository.GetByPlaceholderAsync("Placeholder inexistente", _cancellationToken);

		// Assert
		Assert.Empty(result);
	}

	[Theory]
	[InlineData(100, 1)] // Nome Completo
	[InlineData(500, 1)] // Descrição do Projeto
	[InlineData(null, 1)] // Observações (sem limite)
	[InlineData(200, 0)] // Nenhum item com esse limite
	public async Task GetByMaxLengthAsync_ShouldReturnCorrectResults(int? maxLength, int expectedCount)
	{
		// Act
		var result = await _repository.GetByMaxLengthAsync(maxLength, _cancellationToken);

		// Assert
		var items = result.ToList();
		Assert.Equal(expectedCount, items.Count);
		Assert.All(items, i => Assert.Equal(maxLength, i.MaxLength));
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
		Assert.Equal("Nome Completo", items[0].Title);
		Assert.Equal("Descrição do Projeto", items[1].Title);
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

	[Fact]
	public async Task SearchByPlaceholderAsync_ShouldReturnMatchingItems()
	{
		// Act
		var result = await _repository.SearchByPlaceholderAsync("Digite", _cancellationToken);

		// Assert
		var items = result.ToList();
		Assert.Single(items);
		Assert.Contains("Digite", items[0].Placeholder);
	}

	[Fact]
	public async Task GetByIdAsync_ShouldIncludeSectionAndStatus()
	{
		// Arrange
		var existingItem = await _dbContext.TextItems.FirstAsync(_cancellationToken);

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

	private void SeedTestData()
	{
		// Criar checklist e seção primeiro
		var checklist = new Checklist(1, "Test Checklist", "test-checklist", "Descrição", null);
		_dbContext.Checklists.Add(checklist);
		_dbContext.SaveChanges();

		var section = new ChecklistSection(checklist.Id, "Test Section", 1);
		_dbContext.Sections.Add(section);
		_dbContext.SaveChanges();

		// Criar itens de texto com diferentes configurações
		var textItems = new List<TextItem>
		{
			new(section.Id, "Nome Completo", 1, "Informe seu nome completo", true, "Digite seu nome", 100),
			new(section.Id, "Descrição do Projeto", 2, "Descreva o projeto", true, "Descreva brevemente...", 500),
			new(section.Id, "Observações", 3, "Observações adicionais", false, "Observações opcionais", null)
		};

		_dbContext.TextItems.AddRange(textItems);
		_dbContext.SaveChanges();
	}

	public void Dispose()
	{
		_dbContext.Dispose();
		GC.SuppressFinalize(this);
	}
}
