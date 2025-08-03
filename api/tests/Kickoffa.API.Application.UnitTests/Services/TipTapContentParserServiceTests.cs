using Kickoffa.API.Application.Services;
using Kickoffa.API.Domain.Interfaces.Models;
using Kickoffa.API.Domain.Models;
using NSubstitute;
using Xunit;

namespace Kickoffa.API.Application.UnitTests.Services
{
	/// <summary>
	/// Testes unitários para TipTapContentParserService
	/// </summary>
	public class TipTapContentParserServiceTests
	{
		private readonly TipTapContentParserService _service;

		public TipTapContentParserServiceTests()
		{
			_service = new TipTapContentParserService();
		}

		[Fact]
		public void ExtractImageUrlsFromContentJson_WithValidJson_ShouldReturnImageUrls()
		{
			// Arrange
			var contentJson = @"{
				""type"": ""doc"",
				""content"": [
					{
						""type"": ""paragraph"",
						""content"": [
							{
								""type"": ""text"",
								""text"": ""Aqui está uma imagem:""
							}
						]
					},
					{
						""type"": ""image"",
						""attrs"": {
							""src"": ""https://example.com/image1.jpg"",
							""alt"": ""Imagem 1""
						}
					},
					{
						""type"": ""image"",
						""attrs"": {
							""src"": ""https://example.com/image2.png""
						}
					}
				]
			}";

			// Act
			var result = _service.ExtractImageUrlsFromContentJson(contentJson);

			// Assert
			Assert.Equal(2, result.Count);
			Assert.Contains("https://example.com/image1.jpg", result);
			Assert.Contains("https://example.com/image2.png", result);
		}

		[Fact]
		public void ExtractImageUrlsFromContentJson_WithEmptyJson_ShouldReturnEmptyList()
		{
			// Act
			var result = _service.ExtractImageUrlsFromContentJson("");

			// Assert
			Assert.Empty(result);
		}

		[Fact]
		public void ExtractImageUrlsFromContentJson_WithInvalidJson_ShouldReturnEmptyList()
		{
			// Act
			var result = _service.ExtractImageUrlsFromContentJson("invalid json");

			// Assert
			Assert.Empty(result);
		}

		[Fact]
		public void ExtractImageUrlsFromContentJson_WithNullJson_ShouldReturnEmptyList()
		{
			// Act
			var result = _service.ExtractImageUrlsFromContentJson(null!);

			// Assert
			Assert.Empty(result);
		}

		[Theory]
		[InlineData("https://example.com/image.jpg", "image.jpg")]
		[InlineData("https://example.com/folder/photo.png", "photo.png")]
		[InlineData("https://example.com/", "image.jpg")]
		[InlineData("invalid-url", "image.jpg")]
		public void ExtractFileNameFromUrl_WithVariousUrls_ShouldReturnCorrectFileName(string url, string expectedFileName)
		{
			// Act
			var result = _service.ExtractFileNameFromUrl(url);

			// Assert
			Assert.Equal(expectedFileName, result);
		}

		[Fact]
		public void CompareImageUrls_WithDifferentLists_ShouldReturnCorrectDifferences()
		{
			// Arrange
			var currentUrls = new[] { "url1", "url2", "url3" };
			var newUrls = new[] { "url2", "url3", "url4", "url5" };

			// Act
			var (toAdd, toRemove) = _service.CompareImageUrls(currentUrls, newUrls);

			// Assert
			Assert.Equal(2, toAdd.Count);
			Assert.Contains("url4", toAdd);
			Assert.Contains("url5", toAdd);

			Assert.Single(toRemove);
			Assert.Contains("url1", toRemove);
		}

		[Fact]
		public void CompareImageUrls_WithIdenticalLists_ShouldReturnEmptyDifferences()
		{
			// Arrange
			var urls = new[] { "url1", "url2", "url3" };

			// Act
			var (toAdd, toRemove) = _service.CompareImageUrls(urls, urls);

			// Assert
			Assert.Empty(toAdd);
			Assert.Empty(toRemove);
		}

		[Fact]
		public void ExtractImageUrlsFromContentJson_WithNestedStructure_ShouldFindAllImages()
		{
			// Arrange
			var contentJson = @"{
				""type"": ""doc"",
				""content"": [
					{
						""type"": ""blockquote"",
						""content"": [
							{
								""type"": ""paragraph"",
								""content"": [
									{
										""type"": ""image"",
										""attrs"": {
											""src"": ""https://example.com/nested-image.jpg""
										}
									}
								]
							}
						]
					},
					{
						""type"": ""table"",
						""content"": [
							{
								""type"": ""tableRow"",
								""content"": [
									{
										""type"": ""tableCell"",
										""content"": [
											{
												""type"": ""image"",
												""attrs"": {
													""src"": ""https://example.com/table-image.png""
												}
											}
										]
									}
								]
							}
						]
					}
				]
			}";

			// Act
			var result = _service.ExtractImageUrlsFromContentJson(contentJson);

			// Assert
			Assert.Equal(2, result.Count);
			Assert.Contains("https://example.com/nested-image.jpg", result);
			Assert.Contains("https://example.com/table-image.png", result);
		}
	}
}
