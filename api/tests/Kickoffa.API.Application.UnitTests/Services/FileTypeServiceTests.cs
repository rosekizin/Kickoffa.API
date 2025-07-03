using Kickoffa.API.Application.Services;
using Kickoffa.API.Domain.Models.Enums;
using Kickoffa.API.Domain.Models.Items;
using Kickoffa.API.Domain.Repositories;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Kickoffa.API.Application.UnitTests.Services;

public class FileTypeServiceTests
{
	private readonly IFileTypeRepository _fileTypeRepository;
	private readonly FileTypeService _fileTypeService;

	public FileTypeServiceTests()
	{
		_fileTypeRepository = Substitute.For<IFileTypeRepository>();
		_fileTypeService = new FileTypeService(_fileTypeRepository);
	}

	[Fact]
	public async Task GetActiveFileTypesAsync_ShouldReturnMappedResponse_WhenRepositoryReturnsData()
	{
		// Arrange
		var fileTypes = new List<FileType>
		{
			CreateFileType(1, "image/jpeg", ".jpg", "Imagem JPEG", "Formato de imagem comprimida", FileTypeCategory.Image, 10),
			CreateFileType(2, "application/pdf", ".pdf", "Documento PDF", "Documento portátil", FileTypeCategory.Document, 25)
		};

		_fileTypeRepository.GetActiveFileTypesAsync(Arg.Any<CancellationToken>())
			.Returns(fileTypes);

		// Act
		var result = await _fileTypeService.GetActiveFileTypesAsync(CancellationToken.None);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(2, result.TotalCount);
		Assert.Equal(2, result.FileTypes.Count());

		var firstFileType = result.FileTypes.First();
		Assert.Equal(1, firstFileType.Id);
		Assert.Equal("image/jpeg", firstFileType.MimeType);
		Assert.Equal(".jpg", firstFileType.Extension);
		Assert.Equal("Imagem JPEG", firstFileType.DisplayName);
		Assert.Equal("Image", firstFileType.Category);
		Assert.Equal(10, firstFileType.RecommendedMaxSizeMB);

		await _fileTypeRepository.Received(1).GetActiveFileTypesAsync(Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task GetActiveFileTypesAsync_ShouldReturnEmptyResponse_WhenRepositoryReturnsEmptyList()
	{
		// Arrange
		_fileTypeRepository.GetActiveFileTypesAsync(Arg.Any<CancellationToken>())
			.Returns(new List<FileType>());

		// Act
		var result = await _fileTypeService.GetActiveFileTypesAsync(CancellationToken.None);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(0, result.TotalCount);
		Assert.Empty(result.FileTypes);
	}

	[Fact]
	public async Task GetActiveFileTypesAsync_ShouldThrowException_WhenRepositoryThrowsException()
	{
		// Arrange
		_fileTypeRepository.GetActiveFileTypesAsync(Arg.Any<CancellationToken>())
			.ThrowsAsync(new Exception("Database error"));

		// Act & Assert
		await Assert.ThrowsAsync<Exception>(() =>
			_fileTypeService.GetActiveFileTypesAsync(CancellationToken.None));
	}

	[Theory]
	[InlineData("jpg")]
	[InlineData("pdf")]
	[InlineData("image")]
	public async Task SearchFileTypesAsync_ShouldReturnMappedResponse_WhenRepositoryReturnsData(string searchTerm)
	{
		// Arrange
		var fileTypes = new List<FileType>
		{
			CreateFileType(1, "image/jpeg", ".jpg", "Imagem JPEG", "Formato de imagem comprimida", FileTypeCategory.Image, 10)
		};

		_fileTypeRepository.SearchFileTypesAsync(searchTerm, Arg.Any<CancellationToken>())
			.Returns(fileTypes);

		// Act
		var result = await _fileTypeService.SearchFileTypesAsync(searchTerm, CancellationToken.None);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(1, result.TotalCount);
		Assert.Single(result.FileTypes);

		await _fileTypeRepository.Received(1).SearchFileTypesAsync(searchTerm, Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task SearchFileTypesAsync_ShouldReturnAllActiveFileTypes_WhenSearchTermIsNullOrEmpty()
	{
		// Arrange
		var fileTypes = new List<FileType>
		{
			CreateFileType(1, "image/jpeg", ".jpg", "Imagem JPEG", "Formato de imagem comprimida", FileTypeCategory.Image, 10),
			CreateFileType(2, "application/pdf", ".pdf", "Documento PDF", "Documento portátil", FileTypeCategory.Document, 25)
		};

		_fileTypeRepository.GetActiveFileTypesAsync(Arg.Any<CancellationToken>())
			.Returns(fileTypes);

		// Act
		var result = await _fileTypeService.SearchFileTypesAsync(null, CancellationToken.None);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(2, result.TotalCount);
		Assert.Equal(2, result.FileTypes.Count());

		await _fileTypeRepository.Received(1).GetActiveFileTypesAsync(Arg.Any<CancellationToken>());
		await _fileTypeRepository.DidNotReceive().SearchFileTypesAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task SearchFileTypesAsync_ShouldReturnAllActiveFileTypes_WhenSearchTermIsWhitespace()
	{
		// Arrange
		var fileTypes = new List<FileType>
		{
			CreateFileType(1, "image/jpeg", ".jpg", "Imagem JPEG", "Formato de imagem comprimida", FileTypeCategory.Image, 10)
		};

		_fileTypeRepository.GetActiveFileTypesAsync(Arg.Any<CancellationToken>())
			.Returns(fileTypes);

		// Act
		var result = await _fileTypeService.SearchFileTypesAsync("   ", CancellationToken.None);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(1, result.TotalCount);

		await _fileTypeRepository.Received(1).GetActiveFileTypesAsync(Arg.Any<CancellationToken>());
		await _fileTypeRepository.DidNotReceive().SearchFileTypesAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
	}

	[Theory]
	[InlineData(FileTypeCategory.Image)]
	[InlineData(FileTypeCategory.Document)]
	[InlineData(FileTypeCategory.Audio)]
	[InlineData(FileTypeCategory.Video)]
	[InlineData(FileTypeCategory.Archive)]
	public async Task GetFileTypesByCategoryAsync_ShouldReturnMappedResponse_WhenRepositoryReturnsData(FileTypeCategory category)
	{
		// Arrange
		var fileTypes = new List<FileType>
		{
			CreateFileType(1, "image/jpeg", ".jpg", "Imagem JPEG", "Formato de imagem comprimida", category, 10),
			CreateFileType(2, "image/png", ".png", "Imagem PNG", "Formato de imagem sem perda", category, 15)
		};

		_fileTypeRepository.GetFileTypesByCategoryAsync(category, Arg.Any<CancellationToken>())
			.Returns(fileTypes);

		// Act
		var result = await _fileTypeService.GetFileTypesByCategoryAsync(category, CancellationToken.None);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(2, result.TotalCount);
		Assert.Equal(2, result.FileTypes.Count());
		Assert.All(result.FileTypes, ft => Assert.Equal(category.ToString(), ft.Category));

		await _fileTypeRepository.Received(1).GetFileTypesByCategoryAsync(category, Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task GetFileTypesByCategoryAsync_ShouldReturnEmptyResponse_WhenRepositoryReturnsEmptyList()
	{
		// Arrange
		_fileTypeRepository.GetFileTypesByCategoryAsync(Arg.Any<FileTypeCategory>(), Arg.Any<CancellationToken>())
			.Returns(new List<FileType>());

		// Act
		var result = await _fileTypeService.GetFileTypesByCategoryAsync(FileTypeCategory.Image, CancellationToken.None);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(0, result.TotalCount);
		Assert.Empty(result.FileTypes);
	}

	[Fact]
	public async Task GetFileTypesByCategoryAsync_ShouldThrowException_WhenRepositoryThrowsException()
	{
		// Arrange
		_fileTypeRepository.GetFileTypesByCategoryAsync(Arg.Any<FileTypeCategory>(), Arg.Any<CancellationToken>())
			.ThrowsAsync(new Exception("Database error"));

		// Act & Assert
		await Assert.ThrowsAsync<Exception>(() =>
			_fileTypeService.GetFileTypesByCategoryAsync(FileTypeCategory.Image, CancellationToken.None));
	}

	private static FileType CreateFileType(long id, string mimeType, string extension, string displayName,
		string? description, FileTypeCategory category, int? recommendedMaxSizeMB)
	{
		var fileType = new FileType(mimeType, extension, displayName, category, description, recommendedMaxSizeMB, 1, true);

		// Simular ID usando reflection (já que BaseEntity pode ter ID protegido)
		var idProperty = typeof(FileType).BaseType?.GetProperty("Id");
		idProperty?.SetValue(fileType, id);

		return fileType;
	}
}