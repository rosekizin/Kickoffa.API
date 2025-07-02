using Kickoffa.API.Application.Interfaces;
using Kickoffa.API.Contracts.FileType;
using Kickoffa.API.Domain.Models.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Kickoffa.API.Controllers.UnitTests.Controllers;

public class FileTypeControllerTests
{
	private readonly IFileTypeService _fileTypeService;
	private readonly FileTypeController _fileTypeController;

	public FileTypeControllerTests()
	{
		_fileTypeService = Substitute.For<IFileTypeService>();
		_fileTypeController = new FileTypeController(_fileTypeService)
		{
			// Mock HttpContext para testes
			ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext()
			}
		};
	}

	[Fact]
	public async Task GetFileTypes_ShouldReturnOk_WhenServiceReturnsData()
	{
		// Arrange
		var expectedResponse = new FileTypesSearchResponse(
			[
				new(1, "image/jpeg", ".jpg", "Imagem JPEG", "Formato de imagem comprimida", "Image", 10),
				new(2, "application/pdf", ".pdf", "Documento PDF", "Documento portátil", "Document", 25)
			],
			2
		);

		_fileTypeService.GetActiveFileTypesAsync(Arg.Any<CancellationToken>())
			.Returns(expectedResponse);

		// Act
		var result = await _fileTypeController.GetFileTypes(CancellationToken.None);

		// Assert
		var okResult = Assert.IsType<OkObjectResult>(result.Result);
		var value = Assert.IsType<FileTypesSearchResponse>(okResult.Value);
		Assert.Equal(2, value!.TotalCount);
		Assert.Equal(2, value!.FileTypes.Count());

		await _fileTypeService.Received(1).GetActiveFileTypesAsync(Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task GetFileType_ShouldReturnOk_WhenServiceReturnsEmptyList()
	{
		// Arrange
		var expectedResponse = new FileTypesSearchResponse(new List<FileTypeResponse>(), 0);

		_fileTypeService.GetActiveFileTypesAsync(Arg.Any<CancellationToken>())
			.Returns(expectedResponse);

		// Act
		var result = await _fileTypeController.GetFileTypes(CancellationToken.None);

		// Assert
		var okResult = Assert.IsType<OkObjectResult>(result.Result);
		var value = Assert.IsType<FileTypesSearchResponse>(okResult.Value);
		Assert.Equal(0, value?.TotalCount);
		Assert.Empty(value?.FileTypes ?? []);
	}

	[Fact]
	public async Task GetFileType_ShouldReturnException_WhenServiceThrowsException()
	{
		// Arrange
		_fileTypeService.GetActiveFileTypesAsync(Arg.Any<CancellationToken>())
			.ThrowsAsync(new Exception("Database error"));

		// Act
		var result = await Assert.ThrowsAsync<Exception>(() => _fileTypeController.GetFileTypes(CancellationToken.None));

		// Assert
		Assert.Equal("Database error", result.Message);
	}

	[Theory]
	[InlineData("jpg")]
	[InlineData("pdf")]
	[InlineData("image")]
	[InlineData("")]
	[InlineData(null)]
	public async Task SearchFileTypes_ShouldReturnOk_WithValidSearchTerm(string? searchTerm)
	{
		// Arrange
		var expectedResponse = new FileTypesSearchResponse(
			[
				new(1, "image/jpeg", ".jpg", "Imagem JPEG", "Formato de imagem comprimida", "Image", 10)
			],
			1
		);

		_fileTypeService.SearchFileTypesAsync(searchTerm, Arg.Any<CancellationToken>())
			.Returns(expectedResponse);

		// Act
		var result = await _fileTypeController.SearchFileTypes(searchTerm, CancellationToken.None);

		// Assert
		var okResult = Assert.IsType<OkObjectResult>(result.Result);
		var value = Assert.IsType<FileTypesSearchResponse>(okResult.Value);
		Assert.Equal(1, value?.TotalCount);

		await _fileTypeService.Received(1).SearchFileTypesAsync(searchTerm, Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task SearchFileTypes_ShouldReturnException_WhenServiceThrowsException()
	{
		// Arrange
		_fileTypeService.SearchFileTypesAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
			.ThrowsAsync(new Exception("Search error"));

		// Act
		var result = await Assert.ThrowsAsync<Exception>(() => _fileTypeController.SearchFileTypes("test", CancellationToken.None));

		// Assert
		Assert.Equal("Search error", result.Message);
	}

	[Theory]
	[InlineData(FileTypeCategory.Image)]
	[InlineData(FileTypeCategory.Document)]
	[InlineData(FileTypeCategory.Audio)]
	[InlineData(FileTypeCategory.Video)]
	public async Task GetFileTypesByCategory_ShouldReturnOk_WithValidCategory(FileTypeCategory category)
	{
		// Arrange
		var expectedResponse = new FileTypesSearchResponse(
			[
				new(1, "image/jpeg", ".jpg", "Imagem JPEG", "Formato de imagem comprimida", category.ToString(), 10)
			],
			1
		);

		_fileTypeService.GetFileTypesByCategoryAsync(category, Arg.Any<CancellationToken>())
			.Returns(expectedResponse);

		// Act
		var result = await _fileTypeController.GetFileTypesByCategory(category, CancellationToken.None);

		// Assert
		var okResult = Assert.IsType<OkObjectResult>(result.Result);
		var value = Assert.IsType<FileTypesSearchResponse>(okResult.Value);
		Assert.Equal(1, value?.TotalCount);

		await _fileTypeService.Received(1).GetFileTypesByCategoryAsync(category, Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task GetFileTypesByCategory_ShouldReturnException_WhenServiceThrowsException()
	{
		// Arrange
		_fileTypeService.GetFileTypesByCategoryAsync(Arg.Any<FileTypeCategory>(), Arg.Any<CancellationToken>())
			.ThrowsAsync(new Exception("Category error"));

		// Act
		var result = await Assert.ThrowsAsync<Exception>(() => _fileTypeController.GetFileTypesByCategory(FileTypeCategory.Image, CancellationToken.None));

		// Assert
		Assert.Equal("Category error", result.Message);
	}
}