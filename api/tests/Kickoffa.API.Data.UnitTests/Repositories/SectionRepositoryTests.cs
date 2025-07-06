using Kickoffa.API.Data.EntityFramework.Context;
using Kickoffa.API.Data.Repositories;
using Kickoffa.API.Data.UnitTests.Repositories.DbContext;
using Kickoffa.API.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.UnitTests.Repositories;

public class SectionRepositoryTests : IClassFixture<KickoffaDbContextFixture>, IDisposable
{
	private readonly KickoffaDbContext _dbContext;
	private readonly SectionRepository _repository;
	private readonly CancellationToken _cancellationToken;

	public SectionRepositoryTests(KickoffaDbContextFixture dbContextFixture)
	{
		var options = new DbContextOptionsBuilder<KickoffaDbContext>()
			.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
			.Options;

		_cancellationToken = new();
		_dbContext = dbContextFixture.GetNewDbContext();
		_repository = new SectionRepository(_dbContext);

		SeedTestData();
	}

	[Fact]
	public async Task GetByChecklistIdAsync_ShouldReturnSections_WhenExists()
	{
		// Act
		var result = await _repository.GetByChecklistIdAsync(1, _cancellationToken);

		// Assert
		var sections = result.ToList();
		Assert.Equal(3, sections.Count);
		Assert.All(sections, s => Assert.Equal(1, s.ChecklistId));
	}

	[Fact]
	public async Task GetByChecklistIdAsync_ShouldReturnEmpty_WhenNoSections()
	{
		// Act
		var result = await _repository.GetByChecklistIdAsync(999, _cancellationToken);

		// Assert
		Assert.Empty(result);
	}

	[Fact]
	public async Task GetByChecklistIdOrderedAsync_ShouldReturnOrderedSections()
	{
		// Act
		var result = await _repository.GetByChecklistIdOrderedAsync(1, _cancellationToken);

		// Assert
		var sections = result.ToList();
		Assert.Equal(3, sections.Count);

		// Verificar ordenação
		Assert.Equal(1, sections[0].Order);
		Assert.Equal(2, sections[1].Order);
		Assert.Equal(3, sections[2].Order);

		// Verificar títulos na ordem correta
		Assert.Equal("Briefing Inicial", sections[0].Title);
		Assert.Equal("Requisitos", sections[1].Title);
		Assert.Equal("Entrega", sections[2].Title);
	}

	[Fact]
	public async Task GetNextOrderAsync_ShouldReturnCorrectNextOrder()
	{
		// Act
		var result = await _repository.GetNextOrderAsync(1, _cancellationToken);

		// Assert
		Assert.Equal(4, result); // Próxima ordem após 3 seções existentes
	}

	[Fact]
	public async Task GetNextOrderAsync_ShouldReturnOne_WhenNoSections()
	{
		// Act
		var result = await _repository.GetNextOrderAsync(999, _cancellationToken);

		// Assert
		Assert.Equal(1, result);
	}

	[Fact]
	public async Task ReorderSectionsAsync_ShouldUpdateSectionOrders()
	{
		// Arrange
		var sections = await _dbContext.Sections.Where(s => s.ChecklistId == 1).OrderBy(s => s.Order).ToListAsync(_cancellationToken);
		var sectionOrders = new Dictionary<long, int>
		{
			{ sections[0].Id, 3 }, // Primeira seção vai para posição 3
			{ sections[1].Id, 1 }, // Segunda seção vai para posição 1
			{ sections[2].Id, 2 }  // Terceira seção vai para posição 2
		};

		// Act
		await _repository.ReorderSectionsAsync(1, sectionOrders, _cancellationToken);

		// Assert
		var reorderedSections = await _dbContext.Sections
			.Where(s => s.ChecklistId == 1)
			.OrderBy(s => s.Order)
			.ToListAsync(_cancellationToken);

		Assert.Equal("Requisitos", reorderedSections[0].Title); // Order = 1
		Assert.Equal("Entrega", reorderedSections[1].Title);    // Order = 2
		Assert.Equal("Briefing Inicial", reorderedSections[2].Title); // Order = 3
	}

	[Fact]
	public async Task ReorderSectionsAsync_ShouldIgnoreNonExistentSections()
	{
		// Arrange
		var sectionOrders = new Dictionary<long, int>
		{
			{ 999, 1 }, // Seção inexistente
			{ 998, 2 }  // Seção inexistente
		};

		// Act & Assert - Não deve lançar exceção
		await _repository.ReorderSectionsAsync(1, sectionOrders, _cancellationToken);

		// Verificar que as seções existentes não foram alteradas
		var sections = await _dbContext.Sections
			.Where(s => s.ChecklistId == 1)
			.OrderBy(s => s.Order)
			.ToListAsync(_cancellationToken);

		Assert.Equal(1, sections[0].Order);
		Assert.Equal(2, sections[1].Order);
		Assert.Equal(3, sections[2].Order);
	}

	[Fact]
	public async Task GetByIdAsync_ShouldIncludeChecklist()
	{
		// Arrange
		var existingSection = await _dbContext.Sections.FirstAsync(_cancellationToken);

		// Act
		var result = await _repository.GetByIdAsync(existingSection.Id, _cancellationToken);

		// Assert
		Assert.NotNull(result);
		Assert.NotNull(result.Checklist);
		Assert.Equal(existingSection.ChecklistId, result.Checklist.Id);
	}

	[Fact]
	public async Task GetAllAsync_ShouldIncludeChecklistAndBeOrdered()
	{
		// Act
		var result = await _repository.GetAllAsync(_cancellationToken);

		// Assert
		var sections = result.ToList();
		Assert.NotEmpty(sections);
		Assert.All(sections, s => Assert.NotNull(s.Checklist));

		// Verificar ordenação por ChecklistId e depois por Order
		for (int i = 0; i < sections.Count - 1; i++)
		{
			if (sections[i].ChecklistId == sections[i + 1].ChecklistId)
			{
				Assert.True(sections[i].Order <= sections[i + 1].Order);
			}
			else
			{
				Assert.True(sections[i].ChecklistId <= sections[i + 1].ChecklistId);
			}
		}
	}

	private void SeedTestData()
	{
		// Criar checklists primeiro
		var checklists = new List<Checklist>
		{
			new(1, "Checklist 1", "checklist-1", "Descrição 1", null),
			new(2, "Checklist 2", "checklist-2", "Descrição 2", null)
		};

		_dbContext.Checklists.AddRange(checklists);
		_dbContext.SaveChanges();

		// Criar seções
		var sections = new List<Section>
		{
			// Seções do checklist 1 (fora de ordem para testar ordenação)
			new BriefingSection(1, "Briefing Inicial", 1, "Conteúdo inicial", "<p>HTML inicial</p>"),
			new ChecklistSection(1, "Requisitos", 2),
			new BriefingSection(1, "Entrega", 3, "Conteúdo entrega", "<p>HTML entrega</p>"),
			
			// Seções do checklist 2
			new ChecklistSection(2, "Tarefas", 1),
			new BriefingSection(2, "Documentação", 2, "Conteúdo doc", "<p>HTML doc</p>")
		};

		_dbContext.Sections.AddRange(sections);
		_dbContext.SaveChanges();
	}

	public void Dispose()
	{
		_dbContext.Dispose();
		GC.SuppressFinalize(this);
	}
}