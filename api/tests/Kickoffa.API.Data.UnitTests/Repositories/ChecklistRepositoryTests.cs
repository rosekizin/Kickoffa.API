using Kickoffa.API.Data.EntityFramework.Context;
using Kickoffa.API.Data.Repositories;
using Kickoffa.API.Data.UnitTests.Repositories.DbContext;
using Kickoffa.API.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.UnitTests.Repositories;

public class ChecklistRepositoryTests : IClassFixture<KickoffaDbContextFixture>
{
	private readonly KickoffaDbContext _dbContext;
	private readonly ChecklistRepository _repository;
	private readonly CancellationToken _cancellationToken;

	public ChecklistRepositoryTests(KickoffaDbContextFixture contextFixture)
	{
		_cancellationToken = new();
		var options = new DbContextOptionsBuilder<KickoffaDbContext>()
			.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
			.Options;

		_dbContext = contextFixture.GetNewDbContext();
		_repository = new ChecklistRepository(_dbContext);

		SeedTestData();
	}

	[Fact]
	public async Task GetBySlugAsync_ShouldReturnChecklist_WhenExists()
	{
		// Act
		var result = await _repository.GetBySlugAsync("onboarding-website", _cancellationToken);

		// Assert
		Assert.NotNull(result);
		Assert.Equal("onboarding-website", result.Slug);
		Assert.Equal("Onboarding Website", result.Title);
		Assert.NotEmpty(result.Sections);
	}

	[Fact]
	public async Task GetBySlugAsync_ShouldReturnNull_WhenNotExists()
	{
		// Act
		var result = await _repository.GetBySlugAsync("non-existent-slug", _cancellationToken);

		// Assert
		Assert.Null(result);
	}

	[Fact]
	public async Task GetByAccessTokenAsync_ShouldReturnChecklist_WhenExists()
	{
		// Arrange
		var existingChecklist = await _dbContext.Checklists.FirstAsync(_cancellationToken);

		// Act
		var result = await _repository.GetByAccessTokenAsync(existingChecklist.AccessToken!, _cancellationToken);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(existingChecklist.AccessToken, result.AccessToken);
		Assert.NotEmpty(result.Sections);
	}

	[Fact]
	public async Task GetByAccessTokenAsync_ShouldReturnNull_WhenNotExists()
	{
		// Act
		var result = await _repository.GetByAccessTokenAsync("invalid-token", _cancellationToken);

		// Assert
		Assert.Null(result);
	}

	[Fact]
	public async Task GetByOwnerIdAsync_ShouldReturnChecklists_WhenExists()
	{
		// Act
		var result = await _repository.GetByOwnerIdAsync(1, _cancellationToken);

		// Assert
		var checklists = result.ToList();
		Assert.Equal(2, checklists.Count);
		Assert.All(checklists, c => Assert.Equal(1, c.OwnerId));
		Assert.All(checklists, c => Assert.NotEmpty(c.Sections));

		// Verificar ordenação por data de criação (mais recente primeiro)
		Assert.True(checklists[0].CreatedDateUtc >= checklists[1].CreatedDateUtc);
	}

	[Fact]
	public async Task GetByOwnerIdAsync_ShouldReturnEmpty_WhenNoChecklists()
	{
		// Act
		var result = await _repository.GetByOwnerIdAsync(999, _cancellationToken);

		// Assert
		Assert.Empty(result);
	}

	[Fact]
	public async Task GetPublishedByOwnerIdAsync_ShouldReturnOnlyPublished()
	{
		// Act
		var result = await _repository.GetPublishedByOwnerIdAsync(1, _cancellationToken);

		// Assert
		var checklists = result.ToList();
		Assert.Single(checklists);
		Assert.All(checklists, c => Assert.True(c.IsPublished));
		Assert.All(checklists, c => Assert.Equal(1, c.OwnerId));
	}

	[Theory]
	[InlineData("existing-slug", null, true)]
	[InlineData("existing-slug", 1L, false)] // Excluindo o próprio
	[InlineData("non-existent-slug", null, false)]
	//[InlineData("non-existent-slug", 1L, false)]
	public async Task ExistsBySlugAsync_ShouldReturnCorrectResult(string slug, long? excludeId, bool expectedExists)
	{
		// Act
		var result = await _repository.ExistsBySlugAsync(slug, excludeId, _cancellationToken);

		// Assert
		Assert.Equal(expectedExists, result);
	}

	[Fact]
	public async Task GetByIdAsync_ShouldIncludeSections()
	{
		// Arrange
		var existingChecklist = await _dbContext.Checklists.FirstAsync(_cancellationToken);

		// Act
		var result = await _repository.GetByIdAsync(existingChecklist.Id, _cancellationToken);

		// Assert
		Assert.NotNull(result);
		Assert.NotEmpty(result.Sections);
	}

	[Fact]
	public async Task GetAllAsync_ShouldIncludeSections()
	{
		// Act
		var result = await _repository.GetAllAsync(_cancellationToken);

		// Assert
		var checklists = result.ToList();
		Assert.NotEmpty(checklists);
		Assert.All(checklists, c => Assert.NotEmpty(c.Sections));

		// Verificar ordenação por data de criação (mais recente primeiro)
		for (int i = 0; i < checklists.Count - 1; i++)
		{
			Assert.True(checklists[i].CreatedDateUtc >= checklists[i + 1].CreatedDateUtc);
		}
	}

	private void SeedTestData()
	{
		var checklists = new List<Checklist>
		{
			new(1, "Onboarding Website", "onboarding-website", "Checklist para onboarding de website", DateTime.UtcNow.AddDays(30)),
			new(1, "App Mobile", "app-mobile", "Checklist para desenvolvimento de app", DateTime.UtcNow.AddDays(15)),
			new(2, "Branding Project", "branding-project", "Checklist para projeto de branding", DateTime.UtcNow.AddDays(45))
		};

		// Publicar apenas o primeiro
		checklists[0].Publish();

		// Adicionar slug existente para teste de duplicação
		var duplicateSlugChecklist = new Checklist(3, "Existing Slug Test", "existing-slug", "Teste de slug duplicado", null);
		checklists.Add(duplicateSlugChecklist);

		_dbContext.Checklists.AddRange(checklists);

		// Adicionar seções para teste de Include
		var sections = new List<BriefingSection>
		{
			new(checklists[0].Id, "Briefing Inicial", 1, "Conteúdo do briefing", "<p>HTML do briefing</p>"),
			new(checklists[1].Id, "Requisitos", 1, "Conteúdo dos requisitos", "<p>HTML dos requisitos</p>"),
			new(checklists[2].Id, "Section 3", 1, "Title 2", "<p>HTML dos requisitos</p>"),
			new(checklists[3].Id, "Section 4", 1, "Title 3", "<p>HTML dos requisitos</p>"),
		};

		_dbContext.Sections.AddRange(sections);
		_dbContext.SaveChanges();
	}
}