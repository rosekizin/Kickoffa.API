using Kickoffa.API.Application.Factories;
using Kickoffa.API.Application.Interfaces;
using Kickoffa.API.Application.Interfaces.Checkilists;
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
		private readonly IAddBriefingMediaService _addBriefingMediaService;
		private readonly ITipTapContentParserService _tipTapContentParserService;
		private readonly SectionFactory _sectionFactory;

		public SectionFactoryTests()
		{
			_componentFactory = Substitute.For<IComponentFactory>();
			_briefingMediaFactory = Substitute.For<IBriefingMediaFactory>();
			_addBriefingMediaService = Substitute.For<IAddBriefingMediaService>();
			_tipTapContentParserService = Substitute.For<ITipTapContentParserService>();
			_sectionFactory = new SectionFactory(_componentFactory, _briefingMediaFactory, _addBriefingMediaService, _tipTapContentParserService);
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
			_tipTapContentParserService.DidNotReceive().ExtractImageUrlsFromContentJson(Arg.Any<string>());
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

			_tipTapContentParserService.ExtractImageUrlsFromContentJson("invalid json content")
				.Returns(new List<string>());

			// Act & Assert
			var result = _sectionFactory.CreateBriefingSection(1, sectionRequest);

			Assert.NotNull(result);
			Assert.Equal("Test Section", result.Title);
			Assert.Empty(result.Media);
		}

		[Fact]
		public void CreateBriefingSection_WithContentJsonContainingImages_ShouldCallAddMediaService()
		{
			// Arrange
			var contentJson = @"{
				""type"": ""doc"",
				""content"": [
					{
						""type"": ""image"",
						""attrs"": {
							""src"": ""https://example.com/image1.jpg""
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

			var sectionRequest = new BriefingSectionRequest
			{
				Id = 1,
				Title = "Test Section with Images",
				Type = SectionTypeRequest.Briefing,
				Order = 1,
				ContentJson = contentJson,
				ContentHtml = "<p>Test content with images</p>"
			};

			var imageUrls = new List<string> { "https://example.com/image1.jpg", "https://example.com/image2.png" };

			_tipTapContentParserService.ExtractImageUrlsFromContentJson(contentJson)
				.Returns(imageUrls);

			// Act
			var result = _sectionFactory.CreateBriefingSection(1, sectionRequest);

			// Assert
			Assert.NotNull(result);
			Assert.Equal("Test Section with Images", result.Title);

			// Verificar se o parser foi chamado
			_tipTapContentParserService.Received(1).ExtractImageUrlsFromContentJson(contentJson);

			// Verificar se o serviço de adição de mídia foi chamado
			_addBriefingMediaService.Received(1).CreateAndAddBriefingMediaFromImageUrls(result, imageUrls);
		}

		[Fact]
		public void CreateBriefingSection_WithEmptyContentJson_ShouldNotCallAddMediaService()
		{
			// Arrange
			var sectionRequest = new BriefingSectionRequest
			{
				Id = 1,
				Title = "Test Section",
				Type = SectionTypeRequest.Briefing,
				Order = 1,
				ContentJson = "",
				ContentHtml = "<p>Test content</p>"
			};

			// Act
			var result = _sectionFactory.CreateBriefingSection(1, sectionRequest);

			// Assert
			Assert.NotNull(result);
			Assert.Equal("Test Section", result.Title);

			_tipTapContentParserService.DidNotReceive().ExtractImageUrlsFromContentJson(Arg.Any<string>());
			_addBriefingMediaService.DidNotReceive().CreateAndAddBriefingMediaFromImageUrls(Arg.Any<IBriefingSection>(), Arg.Any<List<string>>());
		}

		[Fact]
		public void CreateBriefingSection_WithNullContentJson_ShouldNotCallAddMediaService()
		{
			// Arrange
			var sectionRequest = new BriefingSectionRequest
			{
				Id = 1,
				Title = "Test Section",
				Type = SectionTypeRequest.Briefing,
				Order = 1,
				ContentJson = null,
				ContentHtml = "<p>Test content</p>"
			};

			// Act
			var result = _sectionFactory.CreateBriefingSection(1, sectionRequest);

			// Assert
			Assert.NotNull(result);
			Assert.Equal("Test Section", result.Title);

			_tipTapContentParserService.DidNotReceive().ExtractImageUrlsFromContentJson(Arg.Any<string>());
			_addBriefingMediaService.DidNotReceive().CreateAndAddBriefingMediaFromImageUrls(Arg.Any<IBriefingSection>(), Arg.Any<List<string>>());
		}
	}
}