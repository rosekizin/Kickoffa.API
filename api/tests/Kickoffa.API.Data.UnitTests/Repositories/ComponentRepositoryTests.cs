using Kickoffa.API.Data.EntityFramework.Context;
using Kickoffa.API.Data.Repositories;
using Kickoffa.API.Data.UnitTests.Repositories.DbContext;
using Kickoffa.API.Domain.Models;
using Kickoffa.API.Domain.Models.Components;
using Kickoffa.API.Domain.Models.Components.Base;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.UnitTests.Repositories;

public class ComponentRepositoryTests : IClassFixture<KickoffaDbContextFixture>, IDisposable
{
	private readonly ComponentRepository _repository;
	private readonly KickoffaDbContext _dbContext;
	private readonly CancellationToken _cancellationToken;

	public ComponentRepositoryTests(KickoffaDbContextFixture contextFixture)
	{
		_cancellationToken = new();
		_dbContext = contextFixture.GetNewDbContext();
		_repository = new ComponentRepository(_dbContext);
		SeedTestData();
	}

	[Fact]
	public async Task GetBySectionIdAsync_ShouldReturnComponents_WhenExists()
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
	public async Task GetBySectionIdAsync_ShouldReturnEmpty_WhenNoComponents()
	{
		// Act
		var result = await _repository.GetBySectionIdAsync(999, _cancellationToken);

		// Assert
		Assert.Empty(result);
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

		// Verificar ordenação
		Assert.Equal(1, components[0].Order);
		Assert.Equal(2, components[1].Order);
		Assert.Equal(3, components[2].Order);

		// Verificar títulos na ordem correta
		Assert.Equal("Logo da Empresa", components[0].Title);
		Assert.Equal("Cores Principais", components[1].Title);
		Assert.Equal("Confirmação Final", components[2].Title);
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
	public async Task GetNextOrderAsync_ShouldReturnOne_WhenNoComponents()
	{
		// Act
		var result = await _repository.GetNextOrderAsync(999, _cancellationToken);

		// Assert
		Assert.Equal(1, result);
	}

	[Fact]
	public async Task ReorderComponentsAsync_ShouldUpdateComponentOrders()
	{
		// Arrange
		var section = await _dbContext.Sections.FirstAsync(_cancellationToken);
		var components = await _dbContext
			.Components.Where(i => i.SectionId == section.Id)
			.OrderBy(i => i.Order).ToListAsync(_cancellationToken);
		var componentOrders = new Dictionary<long, int>
		{
			{ components[0].Id, 3 }, // Primeiro component vai para posição 3
			{ components[1].Id, 1 }, // Segundo component vai para posição 1
			{ components[2].Id, 2 }  // Terceiro component vai para posição 2
		};

		// Act
		await _repository.ReorderComponentsAsync(section.Id, componentOrders, _cancellationToken);

		// Assert
		var reorderedComponents = await _dbContext.Components
			.Where(i => i.SectionId == section.Id)
			.OrderBy(i => i.Order)
			.ToListAsync(_cancellationToken);

		Assert.Equal("Cores Principais", reorderedComponents[0].Title); // Order = 1
		Assert.Equal("Confirmação Final", reorderedComponents[1].Title); // Order = 2
		Assert.Equal("Logo da Empresa", reorderedComponents[2].Title);   // Order = 3
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
		Assert.Equal("Logo da Empresa", components[0].Title);
		Assert.Equal("Cores Principais", components[1].Title);
	}

	[Fact]
	public async Task GetRequiredBySectionIdAsync_ShouldReturnEmpty_WhenNoRequiredComponents()
	{
		// Arrange - Criar seção sem itens obrigatórios
		var checklist = new Checklist(1, 1, "Test", "test", null, null);
		_dbContext.Checklists.Add(checklist);
		await _dbContext.SaveChangesAsync(_cancellationToken);

		var section = new ChecklistSection(checklist.Id, "Test Section", 1);
		_dbContext.Sections.Add(section);
		await _dbContext.SaveChangesAsync(_cancellationToken);

		var component = new TextComponent(section.Id, "Optional Component", 1, "Descrição", false, "Placeholder", 100);
		_dbContext.Components.Add(component);
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
		var existingComponent = await _dbContext.Components.FirstAsync(_cancellationToken);

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

	[Fact]
	public async Task GetBySectionIdAsync_Generic_ShouldReturnSpecificType()
	{
		// Arrange
		var section = await _dbContext.Sections.FirstAsync(_cancellationToken);

		// Act - Buscar apenas TextComponents
		var textComponents = await _repository.GetBySectionIdAsync<TextComponent>(section.Id, _cancellationToken);
		var uploadComponents = await _repository.GetBySectionIdAsync<UploadComponent>(section.Id, _cancellationToken);

		// Assert
		var textComponentsList = textComponents.ToList();
		var uploadComponentsList = uploadComponents.ToList();

		Assert.Single(textComponentsList); // Apenas 1 TextComponent
		Assert.Single(uploadComponentsList); // Apenas 1 UploadComponent

		Assert.All(textComponentsList, i => Assert.IsType<TextComponent>(i));
		Assert.All(uploadComponentsList, i => Assert.IsType<UploadComponent>(i));
	}

	[Theory]
	[InlineData(1)]
	public async Task CountBySectionIdAsync_Generic_ShouldReturnCorrectCount(int expectedCount)
	{
		// Arrange
		var section = await _dbContext.Sections.FirstAsync(_cancellationToken);

		// Act & Assert
		var textCount = await _repository.CountBySectionIdAsync<TextComponent>(section.Id, _cancellationToken);
		var uploadCount = await _repository.CountBySectionIdAsync<UploadComponent>(section.Id, _cancellationToken);
		var confirmationCount = await _repository.CountBySectionIdAsync<ConfirmationComponent>(section.Id, _cancellationToken);

		Assert.Equal(expectedCount, textCount);
		Assert.Equal(expectedCount, uploadCount);
		Assert.Equal(expectedCount, confirmationCount);
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

		// Criar itens de diferentes tipos
		var components = new List<Component>
		{
			new UploadComponent(section.Id, "Logo da Empresa",  1, "Upload do logo", true, "Selecione o arquivo do logo"),
			new TextComponent(section.Id, "Cores Principais", 2, "Informe as cores", true, "Ex: #FF0000, #00FF00", 200),
			new ConfirmationComponent(section.Id, "Confirmação Final", 3, "Confirme os dados", false, "Confirmo que todas as informações estão corretas")
		};

		_dbContext.Components.AddRange(components);
		_dbContext.SaveChanges();
	}

	public void Dispose()
	{
		_dbContext.Dispose();
		GC.SuppressFinalize(this);
	}
}