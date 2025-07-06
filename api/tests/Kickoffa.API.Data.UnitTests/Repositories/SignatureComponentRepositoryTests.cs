using Kickoffa.API.Data.Repositories;
using Kickoffa.API.Data.UnitTests.Repositories.DbContext;
using Kickoffa.API.Domain.Models;
using Kickoffa.API.Domain.Models.Components;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.UnitTests.Repositories;

public class SignatureComponentRepositoryTests : IClassFixture<KickoffaDbContextFixture>, IDisposable
{
	private readonly KickoffaDbContextFixture _dbContextFixture;
	private readonly EntityFramework.Context.KickoffaDbContext _dbContext;
	private readonly SignatureComponentRepository _repository;
	private readonly CancellationToken _cancellationToken;

	public SignatureComponentRepositoryTests(KickoffaDbContextFixture dbContextFixture)
	{
		_dbContextFixture = dbContextFixture;
		_dbContext = _dbContextFixture.GetNewDbContext();
		_repository = new SignatureComponentRepository(_dbContext);
		_cancellationToken = CancellationToken.None;

		SeedTestData();
	}

	[Fact]
	public async Task GetBySectionIdAsync_ShouldReturnSignatureComponents_WhenExists()
	{
		// Arrange
		var section = await _dbContext.Sections.FirstAsync(_cancellationToken);

		// Act
		var result = await _repository.GetBySectionIdAsync(section.Id, _cancellationToken);

		// Assert
		var components = result.ToList();
		Assert.Equal(3, components.Count);
		Assert.All(components, i => Assert.Equal(section.Id, i.SectionId));
	}

	[Fact]
	public async Task GetBySectionIdOrderedAsync_ShouldReturnOrderedComponents()
	{
		// Arrange
		var section = await _dbContext.Sections.FirstAsync(_cancellationToken);

		// Act
		var result = await _repository.GetBySectionIdOrderedAsync(section.Id, _cancellationToken);

		// Assert
		var components = result.ToList();
		Assert.Equal(3, components.Count);

		// Verificar ordenação por Order
		Assert.Equal(1, components[0].Order);
		Assert.Equal(2, components[1].Order);
		Assert.Equal(3, components[2].Order);

		// Verificar títulos na ordem correta
		Assert.Equal("Assinatura do Cliente", components[0].Title);
		Assert.Equal("Assinatura do Freelancer", components[1].Title);
		Assert.Equal("Assinatura da Testemunha", components[2].Title);
	}

	[Fact]
	public async Task GetBySectionIdAsync_ShouldReturnEmpty_WhenNoSignatureComponents()
	{
		// Act
		var result = await _repository.GetBySectionIdAsync(999, _cancellationToken);

		// Assert
		Assert.Empty(result);
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
		Assert.Equal("Assinatura do Cliente", components[0].Title);
		Assert.Equal("Assinatura do Freelancer", components[1].Title);
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
	public async Task CountBySectionIdAsync_ShouldReturnZero_WhenNoComponents()
	{
		// Act
		var result = await _repository.CountBySectionIdAsync(999, _cancellationToken);

		// Assert
		Assert.Equal(0, result);
	}

	[Theory]
	[InlineData(true, 2)] // Obrigatórios
	[InlineData(false, 1)] // Não obrigatórios
	public async Task GetByRequiredStatusAsync_ShouldReturnCorrectComponents(bool isRequired, int expectedCount)
	{
		// Act
		var result = await _repository.GetByRequiredStatusAsync(isRequired, _cancellationToken);

		// Assert
		var components = result.ToList();
		Assert.Equal(expectedCount, components.Count);
		Assert.All(components, i => Assert.Equal(isRequired, i.IsRequired));
	}

	[Fact]
	public async Task GetPendingByChecklistIdAsync_ShouldReturnPendingComponents()
	{
		// Arrange
		var checklist = await _dbContext.Checklists.FirstAsync(_cancellationToken);

		// Act
		var result = await _repository.GetPendingByChecklistIdAsync(checklist.Id, _cancellationToken);

		// Assert
		var components = result.ToList();
		Assert.Equal(3, components.Count); // Todos são pendentes (sem status criado)
		Assert.All(components, i => Assert.NotNull(i.Section));
		Assert.All(components, i => Assert.Equal(checklist.Id, i.Section.ChecklistId));
	}

	[Fact]
	public async Task GetByIdAsync_ShouldIncludeSectionAndStatus()
	{
		// Arrange
		var existingComponent = await _dbContext.SignatureComponents.FirstAsync(_cancellationToken);

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
		var checklist = new Checklist(1, "Test Checklist", "test-checklist", "Descrição", null);
		_dbContext.Checklists.Add(checklist);
		_dbContext.SaveChanges();

		var section = new ChecklistSection(checklist.Id, "Test Section", 1);
		_dbContext.Sections.Add(section);
		_dbContext.SaveChanges();

		// Criar itens de assinatura com diferentes configurações
		var signatureComponents = new List<SignatureComponent>
		{
			new(section.Id, "Assinatura do Cliente", 1, "Assinatura digital do cliente", true),
			new(section.Id, "Assinatura do Freelancer", 2, "Assinatura digital do freelancer", true),
			new(section.Id, "Assinatura da Testemunha", 3, "Assinatura opcional da testemunha", false)
		};

		_dbContext.SignatureComponents.AddRange(signatureComponents);
		_dbContext.SaveChanges();
	}

	public void Dispose()
	{
		_dbContext.Dispose();
		GC.SuppressFinalize(this);
	}
}