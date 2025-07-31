using Kickoffa.API.Application.Factories;
using Kickoffa.API.Application.Interfaces.Factories;
using Kickoffa.API.Contracts.Checklist.Sections;
using Kickoffa.API.Domain.Interfaces.Models;
using NSubstitute;
using Xunit;

namespace Kickoffa.API.Application.UnitTests.Factories
{
	/// <summary>
	/// Testes unitários para SectionFactory
	/// </summary>
	public class SectionFactoryTests
	{
		private readonly IComponentFactory _componentFactory;
		private readonly IBriefingMediaFactory _briefingMediaFactory;
		private readonly SectionFactory _sectionFactory;

		public SectionFactoryTests()
		{
			_componentFactory = Substitute.For<IComponentFactory>();
			_briefingMediaFactory = Substitute.For<IBriefingMediaFactory>();
			_sectionFactory = new SectionFactory(_componentFactory, _briefingMediaFactory);
		}

		[Fact]
		public void CreateBriefingSection_WithoutContentJson_ShouldNotAddMedia()
		{
			// Arrange
			var sectionRequest = new BriefingSectionRequest
			{
				Id = 1,
				Title = "Test Section",
				Type = SectionTypeRequest.Briefing,
				Order = 1,
				ContentHtml = "<p>Test content</p>"
			};

			// Act
			var result = _sectionFactory.CreateBriefingSection(1, sectionRequest);

			// Assert
			Assert.NotNull(result);
			Assert.Equal("Test Section", result.Title);
			Assert.Equal(1, result.Order);
			Assert.Empty(result.Media);
			_briefingMediaFactory.DidNotReceive().CreateBriefingMedia(Arg.Any<long>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<long>(), Arg.Any<int?>(), Arg.Any<int?>(), Arg.Any<string?>());
		}

		[Fact]
		public void CreateBriefingSection_WithContentJsonContainingImages_ShouldAddMedia()
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
						""type"": ""paragraph"",
						""content"": [
							{
								""type"": ""text"",
								""text"": ""E outra imagem:""
							}
						]
					},
					{
						""type"": ""image"",
						""attrs"": {
							""src"": ""https://example.com/image2.png""
						}
					}
				]
			}";

			var sectionRequest = new BriefingSectionRequest
			{
				Id = 1,
				Title = "Test Section with Images",
				Type = SectionTypeRequest.Briefing,
				Order = 1,
				ContentJson = contentJson,
				ContentHtml = "<p>Test content with images</p>"
			};

			var mockMedia1 = Substitute.For<IBriefingMedia>();
			var mockMedia2 = Substitute.For<IBriefingMedia>();

			_briefingMediaFactory.CreateBriefingMedia(
				Arg.Any<long>(),
				"image1.jpg",
				"https://example.com/image1.jpg",
				"https://example.com/image1.jpg",
				"image/jpeg",
				0L,
				null,
				null,
				null
			).Returns(mockMedia1);

			_briefingMediaFactory.CreateBriefingMedia(
				Arg.Any<long>(),
				"image2.png",
				"https://example.com/image2.png",
				"https://example.com/image2.png",
				"image/jpeg",
				0L,
				null,
				null,
				null
			).Returns(mockMedia2);

			// Act
			var result = _sectionFactory.CreateBriefingSection(1, sectionRequest);

			// Assert
			Assert.NotNull(result);
			Assert.Equal("Test Section with Images", result.Title);
			Assert.Equal(2, result.Media.Count());

			// Verificar se o factory foi chamado para ambas as imagens
			_briefingMediaFactory.Received(1).CreateBriefingMedia(
				Arg.Any<long>(),
				"image1.jpg",
				"https://example.com/image1.jpg",
				"https://example.com/image1.jpg",
				"image/jpeg",
				0L,
				null,
				null,
				null
			);

			_briefingMediaFactory.Received(1).CreateBriefingMedia(
				Arg.Any<long>(),
				"image2.png",
				"https://example.com/image2.png",
				"https://example.com/image2.png",
				"image/jpeg",
				0L,
				null,
				null,
				null
			);
		}

		[Fact]
		public void CreateBriefingSection_WithInvalidContentJson_ShouldNotThrowException()
		{
			// Arrange
			var sectionRequest = new BriefingSectionRequest
			{
				Id = 1,
				Title = "Test Section",
				Type = SectionTypeRequest.Briefing,
				Order = 1,
				ContentJson = "invalid json content",
				ContentHtml = "<p>Test content</p>"
			};

			// Act & Assert
			var result = _sectionFactory.CreateBriefingSection(1, sectionRequest);

			Assert.NotNull(result);
			Assert.Equal("Test Section", result.Title);
			Assert.Empty(result.Media);
		}

		[Theory]
		[InlineData("https://example.com/image.jpg", "image.jpg")]
		[InlineData("https://example.com/folder/photo.png", "photo.png")]
		[InlineData("https://example.com/", "image.jpg")]
		[InlineData("invalid-url", "image.jpg")]
		public void ExtractFileNameFromUrl_WithVariousUrls_ShouldReturnCorrectFileName(string url, string expectedFileName)
		{
			// Arrange
			var contentJson = $@"{{
				""type"": ""doc"",
				""content"": [
					{{
						""type"": ""image"",
						""attrs"": {{
							""src"": ""{url}""
						}}
					}}
				]
			}}";

			var sectionRequest = new BriefingSectionRequest
			{
				Id = 1,
				Title = "Test Section",
				Type = SectionTypeRequest.Briefing,
				Order = 1,
				ContentJson = contentJson
			};

			var mockMedia = Substitute.For<IBriefingMedia>();
			_briefingMediaFactory.CreateBriefingMedia(
				Arg.Any<long>(),
				expectedFileName,
				Arg.Any<string>(),
				Arg.Any<string>(),
				Arg.Any<string>(),
				Arg.Any<long>(),
				Arg.Any<int?>(),
				Arg.Any<int?>(),
				Arg.Any<string?>()
			).Returns(mockMedia);

			// Act
			var result = _sectionFactory.CreateBriefingSection(1, sectionRequest);

			// Assert
			_briefingMediaFactory.Received(1).CreateBriefingMedia(
				Arg.Any<long>(),
				expectedFileName,
				Arg.Any<string>(),
				Arg.Any<string>(),
				Arg.Any<string>(),
				Arg.Any<long>(),
				Arg.Any<int?>(),
				Arg.Any<int?>(),
				Arg.Any<string?>()
			);
		}
	}
}
