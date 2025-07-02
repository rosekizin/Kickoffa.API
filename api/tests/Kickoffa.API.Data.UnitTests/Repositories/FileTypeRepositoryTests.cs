using Kickoffa.API.Data.EntityFramework;
using Kickoffa.API.Data.EntityFramework.Context;
using Kickoffa.API.Data.EntityFramework.Mapping;
using Kickoffa.API.Data.Repositories;
using Kickoffa.API.Domain.Models.Enums;
using Kickoffa.API.Domain.Models.Items;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Kickoffa.API.Data.UnitTests.Repositories;

public class FileTypeRepositoryTests : IDisposable
{
    private readonly KickoffaDbContext _context;
    private readonly FileTypeRepository _repository;
	private readonly IUserEntityFrameworkMapping _userEntityFrameworkMapping;
	private readonly ICustomerEntityFrameworkMapping _customerEntityFrameworkMapping;
	private readonly IFileTypeEntityFrameworkMapping _fileTypeEntityFrameworkMapping;

	public FileTypeRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<KickoffaDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

		_userEntityFrameworkMapping = Substitute.For<IUserEntityFrameworkMapping>();
		_customerEntityFrameworkMapping = Substitute.For<ICustomerEntityFrameworkMapping>();
		_fileTypeEntityFrameworkMapping = Substitute.For<IFileTypeEntityFrameworkMapping>();
		_context = new KickoffaDbContext(options, _customerEntityFrameworkMapping, _userEntityFrameworkMapping, _fileTypeEntityFrameworkMapping);
		_repository = new FileTypeRepository(_context);

        SeedTestData();
    }

    [Fact]
    public async Task GetActiveFileTypesAsync_ShouldReturnOnlyActiveFileTypes()
    {
        // Act
        var result = await _repository.GetActiveFileTypesAsync(CancellationToken.None);

        // Assert
        var fileTypes = result.ToList();
        Assert.Equal(3, fileTypes.Count); // Apenas os ativos
        Assert.All(fileTypes, ft => Assert.True(ft.IsActive));
        
        // Verificar se estão ordenados por DisplayOrder
        Assert.Equal("Imagem JPEG", fileTypes[0].DisplayName);
        Assert.Equal("Imagem PNG", fileTypes[1].DisplayName);
        Assert.Equal("Documento PDF", fileTypes[2].DisplayName);
    }

    [Fact]
    public async Task GetActiveFileTypesAsync_ShouldReturnEmptyList_WhenNoActiveFileTypes()
    {
        // Arrange - Desativar todos os tipos
        var allFileTypes = await _context.FileTypes.ToListAsync();
        foreach (var ft in allFileTypes)
        {
            ft.IsActive = false;
        }
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetActiveFileTypesAsync(CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }

    [Theory]
    [InlineData("jpg", 1)] // Deve encontrar JPEG
    [InlineData("JPEG", 1)] // Case insensitive
    [InlineData("pdf", 1)] // Deve encontrar PDF
    [InlineData("imagem", 2)] // Deve encontrar JPEG e PNG
    [InlineData("formato", 3)] // Deve encontrar todos (na descrição)
    [InlineData("inexistente", 0)] // Não deve encontrar nada
    public async Task SearchFileTypesAsync_ShouldReturnCorrectResults(string searchTerm, int expectedCount)
    {
        // Act
        var result = await _repository.SearchFileTypesAsync(searchTerm, CancellationToken.None);

        // Assert
        var fileTypes = result.ToList();
        Assert.Equal(expectedCount, fileTypes.Count);
        Assert.All(fileTypes, ft => Assert.True(ft.IsActive)); // Apenas ativos
    }

    [Fact]
    public async Task SearchFileTypesAsync_ShouldSearchInDisplayName()
    {
        // Act
        var result = await _repository.SearchFileTypesAsync("JPEG", CancellationToken.None);

        // Assert
        var fileTypes = result.ToList();
        Assert.Single(fileTypes);
        Assert.Equal("Imagem JPEG", fileTypes[0].DisplayName);
    }

    [Fact]
    public async Task SearchFileTypesAsync_ShouldSearchInExtension()
    {
        // Act
        var result = await _repository.SearchFileTypesAsync(".pdf", CancellationToken.None);

        // Assert
        var fileTypes = result.ToList();
        Assert.Single(fileTypes);
        Assert.Equal(".pdf", fileTypes[0].Extension);
    }

    [Fact]
    public async Task SearchFileTypesAsync_ShouldSearchInDescription()
    {
        // Act
        var result = await _repository.SearchFileTypesAsync("comprimida", CancellationToken.None);

        // Assert
        var fileTypes = result.ToList();
        Assert.Single(fileTypes);
        Assert.Equal("Imagem JPEG", fileTypes[0].DisplayName);
    }

    [Theory]
    [InlineData(FileTypeCategory.Image, 2)] // JPEG e PNG
    [InlineData(FileTypeCategory.Document, 1)] // PDF
    [InlineData(FileTypeCategory.Audio, 0)] // Nenhum
    [InlineData(FileTypeCategory.Video, 0)] // Nenhum
    public async Task GetFileTypesByCategoryAsync_ShouldReturnCorrectResults(FileTypeCategory category, int expectedCount)
    {
        // Act
        var result = await _repository.GetFileTypesByCategoryAsync(category, CancellationToken.None);

        // Assert
        var fileTypes = result.ToList();
        Assert.Equal(expectedCount, fileTypes.Count);
        Assert.All(fileTypes, ft => Assert.Equal(category, ft.Category));
        Assert.All(fileTypes, ft => Assert.True(ft.IsActive)); // Apenas ativos
    }

    [Fact]
    public async Task GetFileTypesByCategoryAsync_ShouldReturnOrderedByDisplayOrder()
    {
        // Act
        var result = await _repository.GetFileTypesByCategoryAsync(FileTypeCategory.Image, CancellationToken.None);

        // Assert
        var fileTypes = result.ToList();
        Assert.Equal(2, fileTypes.Count);
        Assert.Equal("Imagem JPEG", fileTypes[0].DisplayName); // DisplayOrder = 1
        Assert.Equal("Imagem PNG", fileTypes[1].DisplayName); // DisplayOrder = 2
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnFileType_WhenExists()
    {
        // Arrange
        var existingFileType = await _context.FileTypes.FirstAsync();

        // Act
        var result = await _repository.GetByIdAsync(existingFileType.Id, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(existingFileType.Id, result.Id);
        Assert.Equal(existingFileType.DisplayName, result.DisplayName);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        // Act
        var result = await _repository.GetByIdAsync(999, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnInactiveFileType()
    {
        // Arrange - Pegar o tipo inativo
        var inactiveFileType = await _context.FileTypes.FirstAsync(ft => !ft.IsActive);

        // Act
        var result = await _repository.GetByIdAsync(inactiveFileType.Id, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsActive);
    }

    [Fact]
    public async Task GetByIdsAsync_ShouldReturnMatchingFileTypes()
    {
        // Arrange
        var allFileTypes = await _context.FileTypes.Take(2).ToListAsync();
        var ids = allFileTypes.Select(ft => ft.Id).ToList();

        // Act
        var result = await _repository.GetByIdsAsync(ids, CancellationToken.None);

        // Assert
        var fileTypes = result.ToList();
        Assert.Equal(2, fileTypes.Count);
        Assert.All(fileTypes, ft => Assert.Contains(ft.Id, ids));
    }

    [Fact]
    public async Task GetByIdsAsync_ShouldReturnEmpty_WhenNoMatches()
    {
        // Arrange
        var nonExistentIds = new List<long> { 999, 998, 997 };

        // Act
        var result = await _repository.GetByIdsAsync(nonExistentIds, CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdsAsync_ShouldReturnBothActiveAndInactive()
    {
        // Arrange
        var activeFileType = await _context.FileTypes.FirstAsync(ft => ft.IsActive);
        var inactiveFileType = await _context.FileTypes.FirstAsync(ft => !ft.IsActive);
        var ids = new List<long> { activeFileType.Id, inactiveFileType.Id };

        // Act
        var result = await _repository.GetByIdsAsync(ids, CancellationToken.None);

        // Assert
        var fileTypes = result.ToList();
        Assert.Equal(2, fileTypes.Count);
        Assert.Contains(fileTypes, ft => ft.IsActive);
        Assert.Contains(fileTypes, ft => !ft.IsActive);
    }

    private void SeedTestData()
    {
        var fileTypes = new List<FileType>
        {
            new()
            {
                MimeType = "image/jpeg",
                Extension = ".jpg",
                DisplayName = "Imagem JPEG",
                Description = "Formato de imagem comprimida",
                Category = FileTypeCategory.Image,
                IsActive = true,
                RecommendedMaxSizeMB = 10,
                DisplayOrder = 1
            },
            new()
            {
                MimeType = "image/png",
                Extension = ".png",
                DisplayName = "Imagem PNG",
                Description = "Formato de imagem sem perda",
                Category = FileTypeCategory.Image,
                IsActive = true,
                RecommendedMaxSizeMB = 15,
                DisplayOrder = 2
            },
            new()
            {
                MimeType = "application/pdf",
                Extension = ".pdf",
                DisplayName = "Documento PDF",
                Description = "Formato de documento portátil",
                Category = FileTypeCategory.Document,
                IsActive = true,
                RecommendedMaxSizeMB = 25,
                DisplayOrder = 3
            },
            new()
            {
                MimeType = "image/gif",
                Extension = ".gif",
                DisplayName = "Imagem GIF",
                Description = "Formato de imagem animada",
                Category = FileTypeCategory.Image,
                IsActive = false, // Inativo para testes
                RecommendedMaxSizeMB = 5,
                DisplayOrder = 4
            }
        };

        _context.FileTypes.AddRange(fileTypes);
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
