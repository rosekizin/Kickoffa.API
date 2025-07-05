using Kickoffa.API.Data.Repositories;
using Kickoffa.API.Data.UnitTests.Repositories.DbContext;
using Kickoffa.API.Domain.Models;
using Kickoffa.API.Domain.Models.Items;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.UnitTests.Repositories;

public class SignatureItemRepositoryTests : IClassFixture<KickoffaDbContextFixture>, IDisposable
{
	private readonly KickoffaDbContextFixture _dbContextFixture;
	private readonly EntityFramework.Context.KickoffaDbContext _dbContext;
	private readonly SignatureItemRepository _repository;
	private readonly CancellationToken _cancellationToken;

	public SignatureItemRepositoryTests(KickoffaDbContextFixture dbContextFixture)
	{
		_dbContextFixture = dbContextFixture;
		_dbContext = _dbContextFixture.GetNewDbContext();
		_repository = new SignatureItemRepository(_dbContext);
		_cancellationToken = CancellationToken.None;

		SeedTestData();
	}

	[Fact]
	public async Task GetBySectionIdAsync_ShouldReturnSignatureItems_WhenExists()
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
	public async Task GetBySectionIdOrderedAsync_ShouldReturnOrderedItems()
	{
		// Arrange
		var section = await _dbContext.Sections.FirstAsync(_cancellationToken);

		// Act
		var result = await _repository.GetBySectionIdOrderedAsync(section.Id, _cancellationToken);

		// Assert
		var items = result.ToList();
		Assert.Equal(3, items.Count);
		
		// Verificar ordenação por Order
		Assert.Equal(1, items[0].Order);
		Assert.Equal(2, items[1].Order);
		Assert.Equal(3, items[2].Order);
		
		// Verificar títulos na ordem correta
		Assert.Equal("Assinatura do Cliente", items[0].Title);
		Assert.Equal("Assinatura do Freelancer", items[1].Title);
		Assert.Equal("Assinatura da Testemunha", items[2].Title);
	}

	[Fact]
	public async Task GetBySectionIdAsync_ShouldReturnEmpty_WhenNoSignatureItems()
	{
		// Act
		var result = await _repository.GetBySectionIdAsync(999, _cancellationToken);

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
		Assert.Equal("Assinatura do Cliente", items[0].Title);
		Assert.Equal("Assinatura do Freelancer", items[1].Title);
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
	public async Task GetBySectionIdsAsync_ShouldReturnEmpty_WhenNoMatches()
	{
		// Arrange
		var nonExistentIds = new List<long> { 999, 998 };

		// Act
		var result = await _repository.GetBySectionIdsAsync(nonExistentIds, _cancellationToken);

		// Assert
		Assert.Empty(result);
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
	public async Task CountBySectionIdAsync_ShouldReturnZero_WhenNoItems()
	{
		// Act
		var result = await _repository.CountBySectionIdAsync(999, _cancellationToken);

		// Assert
		Assert.Equal(0, result);
	}

	[Theory]
	[InlineData(true, 2)] // Obrigatórios
	[InlineData(false, 1)] // Não obrigatórios
	public async Task GetByRequiredStatusAsync_ShouldReturnCorrectItems(bool isRequired, int expectedCount)
	{
		// Act
		var result = await _repository.GetByRequiredStatusAsync(isRequired, _cancellationToken);

		// Assert
		var items = result.ToList();
		Assert.Equal(expectedCount, items.Count);
		Assert.All(items, i => Assert.Equal(isRequired, i.IsRequired));
	}

	[Fact]
	public async Task GetPendingByChecklistIdAsync_ShouldReturnPendingItems()
	{
		// Arrange
		var checklist = await _dbContext.Checklists.FirstAsync(_cancellationToken);

		// Act
		var result = await _repository.GetPendingByChecklistIdAsync(checklist.Id, _cancellationToken);

		// Assert
		var items = result.ToList();
		Assert.Equal(3, items.Count); // Todos são pendentes (sem status criado)
		Assert.All(items, i => Assert.NotNull(i.Section));
		Assert.All(items, i => Assert.Equal(checklist.Id, i.Section.ChecklistId));
	}

	[Fact]
	public async Task GetByIdAsync_ShouldIncludeSectionAndStatus()
	{
		// Arrange
		var existingItem = await _dbContext.SignatureItems.FirstAsync(_cancellationToken);

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

		// Criar itens de assinatura com diferentes configurações
		var signatureItems = new List<SignatureItem>
		{
			new(section.Id, "Assinatura do Cliente", 1, "Assinatura digital do cliente", true),
			new(section.Id, "Assinatura do Freelancer", 2, "Assinatura digital do freelancer", true),
			new(section.Id, "Assinatura da Testemunha", 3, "Assinatura opcional da testemunha", false)
		};

		_dbContext.SignatureItems.AddRange(signatureItems);
		_dbContext.SaveChanges();
	}

	public void Dispose()
	{
		_dbContext.Dispose();
		GC.SuppressFinalize(this);
	}
}
