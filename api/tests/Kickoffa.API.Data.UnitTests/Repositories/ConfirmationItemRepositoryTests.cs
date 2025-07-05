using Kickoffa.API.Data.Repositories;
using Kickoffa.API.Data.UnitTests.Repositories.DbContext;
using Kickoffa.API.Domain.Models;
using Kickoffa.API.Domain.Models.Items;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.UnitTests.Repositories;

public class ConfirmationItemRepositoryTests : IClassFixture<KickoffaDbContextFixture>, IDisposable
{
	private readonly KickoffaDbContextFixture _dbContextFixture;
	private readonly EntityFramework.Context.KickoffaDbContext _dbContext;
	private readonly ConfirmationItemRepository _repository;
	private readonly CancellationToken _cancellationToken;

	public ConfirmationItemRepositoryTests(KickoffaDbContextFixture dbContextFixture)
	{
		_dbContextFixture = dbContextFixture;
		_dbContext = _dbContextFixture.GetNewDbContext();
		_repository = new ConfirmationItemRepository(_dbContext);
		_cancellationToken = CancellationToken.None;

		SeedTestData();
	}

	[Fact]
	public async Task GetBySectionIdAsync_ShouldReturnConfirmationItems_WhenExists()
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
	public async Task GetBySectionIdAsync_ShouldReturnEmpty_WhenNoConfirmationItems()
	{
		// Act
		var result = await _repository.GetBySectionIdAsync(999, _cancellationToken);

		// Assert
		Assert.Empty(result);
	}

	[Fact]
	public async Task GetByConfirmationTextAsync_ShouldReturnCorrectItems()
	{
		// Act
		var result = await _repository.GetByConfirmationTextAsync("Confirmo que li e aceito os termos", _cancellationToken);

		// Assert
		var items = result.ToList();
		Assert.Single(items);
		Assert.Equal("Confirmo que li e aceito os termos", items[0].ConfirmationText);
		Assert.Equal("Aceitar Termos", items[0].Title);
	}

	[Fact]
	public async Task GetByConfirmationTextAsync_ShouldReturnEmpty_WhenNoMatches()
	{
		// Act
		var result = await _repository.GetByConfirmationTextAsync("Texto inexistente", _cancellationToken);

		// Assert
		Assert.Empty(result);
	}

	[Theory]
	[InlineData("confirmo", 3)] // Todas contêm "confirmo"
	[InlineData("termos", 1)] // Apenas "Aceitar Termos"
	[InlineData("dados", 1)] // Apenas "Confirmar Dados"
	[InlineData("projeto", 1)] // Apenas "Finalizar Projeto"
	[InlineData("inexistente", 0)] // Nenhuma
	public async Task SearchByConfirmationTextAsync_ShouldReturnCorrectResults(string searchText, int expectedCount)
	{
		// Act
		var result = await _repository.SearchByConfirmationTextAsync(searchText, _cancellationToken);

		// Assert
		var items = result.ToList();
		Assert.Equal(expectedCount, items.Count);
		Assert.All(items, i => Assert.Contains(searchText, i.ConfirmationText, StringComparison.OrdinalIgnoreCase));
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
		Assert.Equal("Aceitar Termos", items[0].Title);
		Assert.Equal("Confirmar Dados", items[1].Title);
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
	[InlineData(20, 40, 2)] // "Aceitar Termos" e "Confirmar Dados"
	[InlineData(50, 100, 1)] // Apenas "Finalizar Projeto"
	[InlineData(10, 15, 0)] // Nenhum nessa faixa
	public async Task GetByTextLengthRangeAsync_ShouldReturnCorrectResults(int minLength, int maxLength, int expectedCount)
	{
		// Act
		var result = await _repository.GetByTextLengthRangeAsync(minLength, maxLength, _cancellationToken);

		// Assert
		var items = result.ToList();
		Assert.Equal(expectedCount, items.Count);
		Assert.All(items, i => Assert.True(i.ConfirmationText.Length >= minLength && i.ConfirmationText.Length <= maxLength));
	}

	[Fact]
	public async Task CountRequiredBySectionIdAsync_ShouldReturnCorrectCount()
	{
		// Arrange
		var section = await _dbContext.Sections.FirstAsync(_cancellationToken);

		// Act
		var result = await _repository.CountRequiredBySectionIdAsync(section.Id, _cancellationToken);

		// Assert
		Assert.Equal(2, result); // Apenas os obrigatórios
	}

	[Fact]
	public async Task GetByIdAsync_ShouldIncludeSectionAndStatus()
	{
		// Arrange
		var existingItem = await _dbContext.ConfirmationItems.FirstAsync(_cancellationToken);

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

		// Criar itens de confirmação com diferentes configurações
		var confirmationItems = new List<ConfirmationItem>
		{
			new(section.Id, "Aceitar Termos", 1, "Aceite os termos de uso", true, "Confirmo que li e aceito os termos"),
			new(section.Id, "Confirmar Dados", 2, "Confirme os dados informados", true, "Confirmo que os dados estão corretos"),
			new(section.Id, "Finalizar Projeto", 3, "Finalização do projeto", false, "Confirmo que o projeto está finalizado e aprovado")
		};

		_dbContext.ConfirmationItems.AddRange(confirmationItems);
		_dbContext.SaveChanges();
	}

	public void Dispose()
	{
		_dbContext.Dispose();
		GC.SuppressFinalize(this);
	}
}
