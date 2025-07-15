using Kickoffa.API.Data.EntityFramework.Context;
using Kickoffa.API.Data.Repositories;
using Kickoffa.API.Data.UnitTests.Repositories.DbContext;
using Kickoffa.API.Domain.Models;
using Kickoffa.API.TestUtils.InternalMember;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.UnitTests.Repositories;

public class BriefingMediaRepositoryTests : IClassFixture<KickoffaDbContextFixture>
{
	private readonly BriefingMediaRepository _repository;
	private readonly CancellationToken _cancellationToken;
	private readonly KickoffaDbContextFixture _contextFixture;

	public BriefingMediaRepositoryTests(KickoffaDbContextFixture context)
	{
		_cancellationToken = new();
		var options = new DbContextOptionsBuilder<KickoffaDbContext>()
			.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
			.Options;

		_contextFixture = context;

		_repository = new BriefingMediaRepository(_contextFixture.KickoffaDbContext);

		SeedTestData();
	}

	[Fact]
	public async Task GetBySectionIdAsync_ShouldReturnMedias_WhenExists()
	{
		// Arrange
		var section = await _contextFixture.KickoffaDbContext.Sections.OfType<BriefingSection>().FirstAsync(_cancellationToken);

		// Act
		var result = await _repository.GetBySectionIdAsync(section.Id, _cancellationToken);

		// Assert
		var medias = result.ToList();
		Assert.Equal(3, medias.Count);
		Assert.All(medias, m => Assert.Equal(section.Id, m.SectionId));

		// Verificar ordenação por data de criação
		for (int i = 0; i < medias.Count - 1; i++)
		{
			Assert.True(medias[i].CreatedDateUtc <= medias[i + 1].CreatedDateUtc);
		}
	}

	[Fact]
	public async Task GetBySectionIdAsync_ShouldReturnEmpty_WhenNoMedias()
	{
		// Act
		var result = await _repository.GetBySectionIdAsync(999, _cancellationToken);

		// Assert
		Assert.Empty(result);
	}

	[Fact]
	public async Task GetByFileNameAsync_ShouldReturnMedia_WhenExists()
	{
		// Act
		var result = await _repository.GetByFileNameAsync("logo-empresa.png", _cancellationToken);

		// Assert
		Assert.NotNull(result);
		Assert.Equal("logo-empresa.png", result.FileName);
	}

	[Fact]
	public async Task GetByFileNameAsync_ShouldReturnNull_WhenNotExists()
	{
		// Act
		var result = await _repository.GetByFileNameAsync("arquivo-inexistente.jpg", _cancellationToken);

		// Assert
		Assert.Null(result);
	}

	[Fact]
	public async Task GetByIdAsync_ShouldIncludeSection()
	{
		// Arrange
		var existingMedia = await _contextFixture.KickoffaDbContext.BriefingMedias.FirstAsync(_cancellationToken);

		// Act
		var result = await _repository.GetByIdAsync(existingMedia.Id, _cancellationToken);

		// Assert
		Assert.NotNull(result);
		Assert.NotNull(result.Section);
		Assert.Equal(existingMedia.SectionId, result.Section.Id);
	}

	[Fact]
	public async Task GetAllAsync_ShouldIncludeSectionAndBeOrdered()
	{
		// Act
		var result = await _repository.GetAllAsync(_cancellationToken);

		// Assert
		var medias = result.ToList();
		Assert.NotEmpty(medias);
		Assert.All(medias, m => Assert.NotNull(m.Section));

		// Verificar ordenação por data de criação (mais recente primeiro)
		for (int i = 0; i < medias.Count - 1; i++)
		{
			Assert.True(medias[i].CreatedDateUtc >= medias[i + 1].CreatedDateUtc);
		}
	}

	private void SeedTestData()
	{
		// Criar estrutura básica
		var checklist = new Checklist(1, 1, "Test Checklist", "test-checklist", "Descrição", null);
		_contextFixture.KickoffaDbContext.Checklists.Add(checklist);
		_contextFixture.KickoffaDbContext.SaveChanges();

		var briefingSection = new BriefingSection(checklist.Id, "Briefing Section", 1, "Conteúdo JSON", "<p>Conteúdo HTML</p>");
		_contextFixture.KickoffaDbContext.Sections.Add(briefingSection);
		_contextFixture.KickoffaDbContext.SaveChanges();

		// Criar mídias com diferentes tipos e datas
		var baseDate = DateTime.UtcNow.AddDays(-10);
		var medias = new List<BriefingMedia>
		{
			new(briefingSection.Id, "logo-empresa.png", "/uploads/logo-empresa.png", "www.aws.com", "image/png", 2048)
			{
				// Simular data de criação mais antiga
			},
			new(briefingSection.Id, "foto-produto.jpg", "/uploads/foto-produto.jpg", "www.aws.com", "image/jpeg", 4096)
			{
				// Simular data de criação intermediária
			},
			new(briefingSection.Id, "banner-site.png", "/uploads/banner-site.png", "www.aws.com", "image/png", 3072)
			{
				// Simular data de criação mais recente
			}
		};

		// Definir datas manualmente para garantir ordenação nos testes
		medias[0].SetPrivatePropertyBackingField("CreatedDateUtc", baseDate);
		medias[1].SetPrivatePropertyBackingField("CreatedDateUtc", baseDate.AddDays(1));
		medias[2].SetPrivatePropertyBackingField("CreatedDateUtc", baseDate.AddDays(2));

		_contextFixture.KickoffaDbContext.BriefingMedias.AddRange(medias);
		_contextFixture.KickoffaDbContext.SaveChanges();
	}
}