using Kickoffa.API.Application.Interfaces;
using Kickoffa.API.Application.Interfaces.Checkilists;
using Kickoffa.API.Application.Interfaces.Factories;
using Kickoffa.API.Application.Services;
using Kickoffa.API.Application.Services.Checklists;
using Kickoffa.API.Domain.Interfaces.Models;
using Kickoffa.API.TestUtils.LoggerExtensions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Kickoffa.API.Application.UnitTests.Services
{
	/// <summary>
	/// Testes unitários para AddBriefingMediaService
	/// </summary>
	public class AddBriefingMediaServiceTests
    {
        private readonly ILogger<AddBriefingMediaService> _logger;
        private readonly IBriefingMediaFactory _briefingMediaFactory;
		private readonly ITipTapContentParserService _tipTapContentParserService;
		private readonly IAddBriefingMediaService _service;

		public AddBriefingMediaServiceTests()
        {
            _logger = Substitute.For<ILogger<AddBriefingMediaService>>();
            _briefingMediaFactory = Substitute.For<IBriefingMediaFactory>();
			_tipTapContentParserService = Substitute.For<ITipTapContentParserService>();
			_service = new AddBriefingMediaService(_logger, _briefingMediaFactory, _tipTapContentParserService);
		}

		[Fact]
		public void CreateAndAddBriefingMediaFromImageUrl_WithValidUrl_ShouldCreateAndAddMedia()
		{
			// Arrange
			var section = Substitute.For<IBriefingSection>();
			section.Id.Returns(123L);
			var imageUrl = "https://example.com/api/images/image.jpg";
			var mockMedia = Substitute.For<IBriefingMedia>();

			_tipTapContentParserService.ExtractFileNameFromUrl(imageUrl)
				.Returns("image");
			_briefingMediaFactory.CreateBriefingMedia(
				123L, "image", "/image.jpg", imageUrl, "image/jpeg", 0L, null, null, null)
				.Returns(mockMedia);

			// Act
			_service.CreateAndAddBriefingMediaFromImageUrl(section, imageUrl);

			// Assert
			section.Received(1).AddMedia(mockMedia);
            _briefingMediaFactory.Received(1).CreateBriefingMedia(
				123L, "image", "/image.jpg", imageUrl, "image/jpeg", 0L, null, null, null);
		}

		[Fact]
		public void CreateAndAddBriefingMediaFromImageUrl_WithEmptyUrl_ShouldNotAddMedia()
		{
			// Arrange
			var section = Substitute.For<IBriefingSection>();
			var imageUrl = "";

			// Act
			_service.CreateAndAddBriefingMediaFromImageUrl(section, imageUrl);

			// Assert
			section.DidNotReceive().AddMedia(Arg.Any<IBriefingMedia>());
			_briefingMediaFactory.DidNotReceive().CreateBriefingMedia(
				Arg.Any<long>(), Arg.Any<string>(), Arg.Any<string>(),
				Arg.Any<string>(), Arg.Any<string>(), Arg.Any<long>(),
				Arg.Any<int?>(), Arg.Any<int?>(), Arg.Any<string?>());
		}

		[Fact]
		public void CreateAndAddBriefingMediaFromImageUrl_WithWhitespaceUrl_ShouldNotAddMedia()
		{
			// Arrange
			var section = Substitute.For<IBriefingSection>();
			var imageUrl = "   ";

			// Act
			_service.CreateAndAddBriefingMediaFromImageUrl(section, imageUrl);

			// Assert
			section.DidNotReceive().AddMedia(Arg.Any<IBriefingMedia>());
			_briefingMediaFactory.DidNotReceive().CreateBriefingMedia(
				Arg.Any<long>(), Arg.Any<string>(), Arg.Any<string>(),
				Arg.Any<string>(), Arg.Any<string>(), Arg.Any<long>(),
				Arg.Any<int?>(), Arg.Any<int?>(), Arg.Any<string?>());
		}

		[Fact]
		public void CreateAndAddBriefingMediaFromImageUrl_WhenExceptionThrown_ShouldNotThrow()
		{
			// Arrange
			var section = Substitute.For<IBriefingSection>();
			var imageUrl = "https://example.com/image.jpg";

			_tipTapContentParserService.ExtractFileNameFromUrl(imageUrl)
				.Throws(new Exception("Invalid URL"));

			// Act & Assert
			// Não deve lançar exceção
			_service.CreateAndAddBriefingMediaFromImageUrl(section, imageUrl);

			section.DidNotReceive().AddMedia(Arg.Any<IBriefingMedia>());
		}

		[Fact]
		public void CreateAndAddBriefingMediaFromImageUrls_WithEmptyList_ShouldNotAddAnyMedia()
		{
			// Arrange
			var section = Substitute.For<IBriefingSection>();
			var imageUrls = new List<string>();

			// Act
			_service.CreateAndAddBriefingMediaFromImageUrls(section, imageUrls);

			// Assert
			section.DidNotReceive().AddMedia(Arg.Any<IBriefingMedia>());
			_briefingMediaFactory.DidNotReceive().CreateBriefingMedia(
				Arg.Any<long>(), Arg.Any<string>(), Arg.Any<string>(),
				Arg.Any<string>(), Arg.Any<string>(), Arg.Any<long>(),
				Arg.Any<int?>(), Arg.Any<int?>(), Arg.Any<string?>());
		}

		[Fact]
		public void CreateAndAddBriefingMediaFromImageUrls_WithValidUrls_ShouldAddAllMedia()
		{
			// Arrange
			var section = Substitute.For<IBriefingSection>();
			section.Id.Returns(123L);
			var imageUrls = new List<string>
			{
				"https://example.com/image1.jpg",
				"https://example.com/image2.png"
			};

			var mockMedia1 = Substitute.For<IBriefingMedia>();
			var mockMedia2 = Substitute.For<IBriefingMedia>();

			_tipTapContentParserService.ExtractFileNameFromUrl("https://example.com/image1.jpg")
				.Returns("image1.jpg");
			_tipTapContentParserService.ExtractFileNameFromUrl("https://example.com/image2.png")
				.Returns("image2.png");

			_briefingMediaFactory.CreateBriefingMedia(
				123L, "image1.jpg", "/image1.jpg",
				"https://example.com/image1.jpg", "image/jpeg", 0L, null, null, null)
				.Returns(mockMedia1);
			_briefingMediaFactory.CreateBriefingMedia(
				123L, "image2.png", "/image2.png",
				"https://example.com/image2.png", "image/png", 0L, null, null, null)
				.Returns(mockMedia2);

			// Act
			_service.CreateAndAddBriefingMediaFromImageUrls(section, imageUrls);

			// Assert
			section.Received(1).AddMedia(mockMedia1);
			section.Received(1).AddMedia(mockMedia2);
			section.Received(2).AddMedia(Arg.Any<IBriefingMedia>());
		}

		[Fact]
		public void CreateAndAddBriefingMediaFromImageUrls_WithMixedValidAndInvalidUrls_ShouldAddOnlyValidMedia()
		{
			// Arrange
			var section = Substitute.For<IBriefingSection>();
			section.Id.Returns(123L);
			var imageUrls = new List<string>
			{
				"https://example.com/valid-image.jpg",
				"", // URL vazia
				"   ", // URL com whitespace
				"https://example.com/another-valid-image.png"
			};

			var mockMedia1 = Substitute.For<IBriefingMedia>();
			var mockMedia2 = Substitute.For<IBriefingMedia>();

			_tipTapContentParserService.ExtractFileNameFromUrl("https://example.com/valid-image.jpg")
				.Returns("valid-image.jpg");
			_tipTapContentParserService.ExtractFileNameFromUrl("https://example.com/another-valid-image.png")
				.Returns("another-valid-image.png");

			_briefingMediaFactory.CreateBriefingMedia(
				123L, "valid-image.jpg", "/valid-image.jpg",
				"https://example.com/valid-image.jpg", "image/jpeg", 0L, null, null, null)
				.Returns(mockMedia1);
			_briefingMediaFactory.CreateBriefingMedia(
				123L, "another-valid-image.png", "/another-valid-image.png",
				"https://example.com/another-valid-image.png", "image/png", 0L, null, null, null)
				.Returns(mockMedia2);

			// Act
			_service.CreateAndAddBriefingMediaFromImageUrls(section, imageUrls);

			// Assert
			section.Received(1).AddMedia(mockMedia1);
			section.Received(1).AddMedia(mockMedia2);
			section.Received(2).AddMedia(Arg.Any<IBriefingMedia>());

			// Verificar que não tentou processar URLs vazias/whitespace
			_tipTapContentParserService.DidNotReceive().ExtractFileNameFromUrl("");
			_tipTapContentParserService.DidNotReceive().ExtractFileNameFromUrl("   ");
		}

		[Fact]
		public void CreateAndAddBriefingMediaFromImageUrls_WhenSomeUrlsThrowException_ShouldContinueWithOthers()
		{
			// Arrange
			var section = Substitute.For<IBriefingSection>();
			section.Id.Returns(123L);
			var imageUrls = new List<string>
			{
				"https://example.com/valid-image.jpg",
				"invalid-url-that-throws",
				"https://example.com/another-valid-image.png"
			};

			var mockMedia1 = Substitute.For<IBriefingMedia>();
			var mockMedia2 = Substitute.For<IBriefingMedia>();

			_tipTapContentParserService.ExtractFileNameFromUrl("https://example.com/valid-image.jpg")
				.Returns("valid-image.jpg");
			_tipTapContentParserService.ExtractFileNameFromUrl("invalid-url-that-throws")
				.Throws(new Exception("Invalid URL"));
			_tipTapContentParserService.ExtractFileNameFromUrl("https://example.com/another-valid-image.png")
				.Returns("another-valid-image.png");

			_briefingMediaFactory.CreateBriefingMedia(
				123L, "valid-image.jpg", "/valid-image.jpg",
				"https://example.com/valid-image.jpg", "image/jpeg", 0L, null, null, null)
				.Returns(mockMedia1);
			_briefingMediaFactory.CreateBriefingMedia(
				123L, "another-valid-image.png", "/another-valid-image.png",
				"https://example.com/another-valid-image.png", "image/png", 0L, null, null, null)
				.Returns(mockMedia2);

			// Act
			_service.CreateAndAddBriefingMediaFromImageUrls(section, imageUrls);

			// Assert
			section.Received(1).AddMedia(mockMedia1);
			section.Received(1).AddMedia(mockMedia2);
			section.Received(2).AddMedia(Arg.Any<IBriefingMedia>());
		}

		[Fact]
		public void CreateAndAddBriefingMediaFromImageUrls_WhenFactoryThrowsException_ShouldContinueWithOthers()
		{
			// Arrange
			var imageUrl1 = "https://example.com/image1.jpg";
			var imageUrl2 = "https://example.com/image2.png";

            var section = Substitute.For<IBriefingSection>();
			section.Id.Returns(123L);
			var imageUrls = new List<string>
			{
                imageUrl1,
                imageUrl2
            };

			var mockMedia2 = Substitute.For<IBriefingMedia>();

			_tipTapContentParserService.ExtractFileNameFromUrl(imageUrl1)
				.Returns("image1.jpg");
			_tipTapContentParserService.ExtractFileNameFromUrl(imageUrl2)
				.Returns("image2.png");

			_briefingMediaFactory.CreateBriefingMedia(
				123L, "image1.jpg", "/image1.jpg",
                imageUrl1, "image/jpeg", 0L, null, null, null)
				.Throws(new Exception("Factory error"));
			_briefingMediaFactory.CreateBriefingMedia(
				123L, "image2.png", "/image2.png",
                imageUrl2, "image/png", 0L, null, null, null)
				.Returns(mockMedia2);

			// Act
			_service.CreateAndAddBriefingMediaFromImageUrls(section, imageUrls);

			// Assert
			section.Received(1).AddMedia(mockMedia2);
			section.Received(1).AddMedia(Arg.Any<IBriefingMedia>());
            _logger.ShouldHaveLoggedException<AddBriefingMediaService, Exception>(
                LogLevel.Error,
                $"Ocorreu algum erro ao criar o BriefingMedia para imageUrl {imageUrl1}, na section {section.Id}",
                "Factory error");
        }

		[Fact]
		public void CreateAndAddBriefingMediaFromImageUrl_WithNullSection_ShouldTreatAndLogException()
		{
			// Arrange
			IBriefingSection section = null!;
			var imageUrl = "https://example.com/image.jpg";

			// Act
			_service.CreateAndAddBriefingMediaFromImageUrl(section, imageUrl);

            // Assert
            _logger.ShouldHaveLoggedException<AddBriefingMediaService, NullReferenceException>(
				LogLevel.Error,
				$"Ocorreu algum erro ao criar o BriefingMedia para imageUrl {imageUrl}, na section {null}",
				"Object reference not set to an instance of an object.");
        }
	}
}