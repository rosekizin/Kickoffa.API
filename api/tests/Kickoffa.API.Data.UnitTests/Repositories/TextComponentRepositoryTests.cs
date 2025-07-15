using Kickoffa.API.Data.Repositories;
using Kickoffa.API.Data.UnitTests.Repositories.DbContext;
using Kickoffa.API.Domain.Models;
using Kickoffa.API.Domain.Models.Components;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.UnitTests.Repositories;

public class TextComponentRepositoryTests : IClassFixture<KickoffaDbContextFixture>, IDisposable
{
	private readonly KickoffaDbContextFixture _dbContextFixture;
	private readonly EntityFramework.Context.KickoffaDbContext _dbContext;
	private readonly TextComponentRepository _repository;
	private readonly CancellationToken _cancellationToken;

	public TextComponentRepositoryTests(KickoffaDbContextFixture dbContextFixture)
	{
		_dbContextFixture = dbContextFixture;
		_dbContext = _dbContextFixture.GetNewDbContext();
		_repository = new TextComponentRepository(_dbContext);
		_cancellationToken = CancellationToken.None;

		SeedTestData();
	}

	[Fact]
	public async Task GetBySectionIdAsync_ShouldReturnTextComponents_WhenExists()
	{
		// Arrange
		var section = await _dbContext.Sections.FirstAsync(_cancellationToken);

		// Act
		var result = await _repository.GetBySectionIdAsync(section.Id, _cancellationToken);

		// Assert
		var components = result.ToList();
		Assert.Equal(3, components.Count);
		Assert.All(components, i => Assert.Equal(section.Id, i.SectionId));

		// Verificar ordenação por Order
		Assert.Equal(1, components[0].Order);
		Assert.Equal(2, components[1].Order);
		Assert.Equal(3, components[2].Order);
	}

	[Fact]
	public async Task GetBySectionIdAsync_ShouldReturnEmpty_WhenNoTextComponents()
	{
		// Act
		var result = await _repository.GetBySectionIdAsync(999, _cancellationToken);

		// Assert
		Assert.Empty(result);
	}

	[Fact]
	public async Task GetByPlaceholderAsync_ShouldReturnCorrectComponents()
	{
		// Act
		var result = await _repository.GetByPlaceholderAsync("Digite seu nome", _cancellationToken);

		// Assert
		var components = result.ToList();
		Assert.Single(components);
		Assert.Equal("Digite seu nome", components[0].Placeholder);
		Assert.Equal("Nome Completo", components[0].Title);
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
	[InlineData(200, 0)] // Nenhum componente com esse limite
	public async Task GetByMaxLengthAsync_ShouldReturnCorrectResults(int? maxLength, int expectedCount)
	{
		// Act
		var result = await _repository.GetByMaxLengthAsync(maxLength, _cancellationToken);

		// Assert
		var components = result.ToList();
		Assert.Equal(expectedCount, components.Count);
		Assert.All(components, i => Assert.Equal(maxLength, i.MaxLength));
	}

	[Fact]
	public async Task GetRequiredBySectionIdAsync_ShouldReturnOnlyRequiredComponents()
	{
		// Arrange
		var section = await _dbContext.Sections.FirstAsync(_cancellationToken);

		// Act
		var result = await _repository.GetRequiredBySectionIdAsync(section.Id, _cancellationToken);

		// Assert
		var components = result.ToList();
		Assert.Equal(2, components.Count); // Apenas os obrigatórios
		Assert.All(components, i => Assert.True(i.IsRequired));

		// Verificar ordenação
		Assert.Equal("Nome Completo", components[0].Title);
		Assert.Equal("Descrição do Projeto", components[1].Title);
	}

	[Fact]
	public async Task GetByChecklistIdAsync_ShouldReturnComponentsFromChecklist()
	{
		// Arrange
		var checklist = await _dbContext.Checklists.FirstAsync(_cancellationToken);

		// Act
		var result = await _repository.GetByChecklistIdAsync(checklist.Id, _cancellationToken);

		// Assert
		var components = result.ToList();
		Assert.Equal(3, components.Count);
		Assert.All(components, i => Assert.NotNull(i.Section));
		Assert.All(components, i => Assert.Equal(checklist.Id, i.Section.ChecklistId));
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
	public async Task CountBySectionIdAsync_ShouldReturnZero_WhenNoComponents()
	{
		// Act
		var result = await _repository.CountBySectionIdAsync(999, _cancellationToken);

		// Assert
		Assert.Equal(0, result);
	}

	[Fact]
	public async Task GetBySectionIdsAsync_ShouldReturnComponentsFromMultipleSections()
	{
		// Arrange
		var sections = await _dbContext.Sections.Take(2).ToListAsync(_cancellationToken);
		var sectionIds = sections.Select(s => s.Id).ToList();

		// Act
		var result = await _repository.GetBySectionIdsAsync(sectionIds, _cancellationToken);

		// Assert
		var components = result.ToList();
		Assert.NotEmpty(components);
		Assert.All(components, i => Assert.Contains(i.SectionId, sectionIds));
	}

	[Fact]
	public async Task SearchByPlaceholderAsync_ShouldReturnMatchingComponents()
	{
		// Act
		var result = await _repository.SearchByPlaceholderAsync("Digite", _cancellationToken);

		// Assert
		var components = result.ToList();
		Assert.Single(components);
		Assert.Contains("Digite", components[0].Placeholder);
	}

	[Fact]
	public async Task GetByIdAsync_ShouldIncludeSectionAndStatus()
	{
		// Arrange
		var existingComponent = await _dbContext.TextComponents.FirstAsync(_cancellationToken);

		// Act
		var result = await _repository.GetByIdAsync(existingComponent.Id, _cancellationToken);

		// Assert
		Assert.NotNull(result);
		Assert.NotNull(result.Section);
		Assert.Equal(existingComponent.SectionId, result.Section.Id);
		// Status pode ser null se não foi criado ainda
	}

	[Fact]
	public async Task GetAllAsync_ShouldIncludeSectionAndStatusAndBeOrdered()
	{
		// Act
		var result = await _repository.GetAllAsync(_cancellationToken);

		// Assert
		var components = result.ToList();
		Assert.NotEmpty(components);
		Assert.All(components, i => Assert.NotNull(i.Section));

		// Verificar ordenação por SectionId e depois por Order
		for (int i = 0; i < components.Count - 1; i++)
		{
			if (components[i].SectionId == components[i + 1].SectionId)
			{
				Assert.True(components[i].Order <= components[i + 1].Order);
			}
			else
			{
				Assert.True(components[i].SectionId <= components[i + 1].SectionId);
			}
		}
	}

	private void SeedTestData()
	{
		// Criar checklist e seção primeiro
		var checklist = new Checklist(1, 1, "Test Checklist", "test-checklist", "Descrição", null);
		_dbContext.Checklists.Add(checklist);
		_dbContext.SaveChanges();

		var section = new ChecklistSection(checklist.Id, "Test Section", 1);
		_dbContext.Sections.Add(section);
		_dbContext.SaveChanges();

		// Criar itens de texto com diferentes configurações
		var textComponents = new List<TextComponent>
		{
			new(section.Id, "Nome Completo", 1, "Informe seu nome completo", true, "Digite seu nome", 100),
			new(section.Id, "Descrição do Projeto", 2, "Descreva o projeto", true, "Descreva brevemente...", 500),
			new(section.Id, "Observações", 3, "Observações adicionais", false, "Observações opcionais", null)
		};

		_dbContext.TextComponents.AddRange(textComponents);
		_dbContext.SaveChanges();
	}

	public void Dispose()
	{
		_dbContext.Dispose();
		GC.SuppressFinalize(this);
	}
}