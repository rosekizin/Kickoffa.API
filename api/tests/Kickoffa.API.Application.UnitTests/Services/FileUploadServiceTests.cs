using Amazon.S3;
using Amazon.S3.Model;
using Kickoffa.API.Application.Configuration;
using Kickoffa.API.Application.Services;
using Kickoffa.API.Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using System.Net;
using Xunit;

namespace Kickoffa.API.Application.UnitTests.Services
{
	/// <summary>
	/// Testes unitários para FileUploadService
	/// </summary>
	public class FileUploadServiceTests
	{
		private readonly IAmazonS3 _s3Client;
		private readonly ICurrentUserService _currentUserService;
		private readonly ILogger<FileUploadService> _logger;
		private readonly IOptions<AwsS3Configuration> _s3Config;
		private readonly FileUploadService _fileUploadService;

		public FileUploadServiceTests()
		{
			_s3Client = Substitute.For<IAmazonS3>();
			_currentUserService = Substitute.For<ICurrentUserService>();
			_logger = Substitute.For<ILogger<FileUploadService>>();
			
			var config = new AwsS3Configuration
			{
				BucketName = "test-bucket",
				BaseUrl = "https://test-bucket.s3.amazonaws.com",
				ApiBaseUrl = "http://localhost:5084"
			};
			_s3Config = Options.Create(config);

			_fileUploadService = new FileUploadService(_s3Client, _s3Config, _currentUserService, _logger);
		}

		[Fact]
		public void GenerateUserScopedFolder_WithAuthenticatedUser_ShouldIncludeUserId()
		{
			// Arrange
			var userId = 123L;
			var baseFolder = "briefing/images";
			
			_currentUserService.IsAuthenticated.Returns(true);
			_currentUserService.UserId.Returns(userId);

			// Act - usando reflexão para testar método privado
			var method = typeof(FileUploadService).GetMethod("GenerateUserScopedFolder", 
				System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			var result = (string)method!.Invoke(_fileUploadService, new object[] { baseFolder });

			// Assert
			Assert.Equal("briefing/images/userId/123", result);
		}

		[Fact]
		public void GenerateUserScopedFolder_WithoutAuthenticatedUser_ShouldUseAnonymous()
		{
			// Arrange
			var baseFolder = "briefing/images";
			
			_currentUserService.IsAuthenticated.Returns(false);
			_currentUserService.UserId.Returns((long?)null);

			// Act - usando reflexão para testar método privado
			var method = typeof(FileUploadService).GetMethod("GenerateUserScopedFolder", 
				System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			var result = (string)method!.Invoke(_fileUploadService, new object[] { baseFolder });

			// Assert
			Assert.Equal("briefing/images/anonymous", result);
		}

		[Fact]
		public void GenerateUserScopedFolder_WithNullUserId_ShouldUseAnonymous()
		{
			// Arrange
			var baseFolder = "briefing/images";
			
			_currentUserService.IsAuthenticated.Returns(true);
			_currentUserService.UserId.Returns((long?)null);

			// Act - usando reflexão para testar método privado
			var method = typeof(FileUploadService).GetMethod("GenerateUserScopedFolder", 
				System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			var result = (string)method!.Invoke(_fileUploadService, new object[] { baseFolder });

			// Assert
			Assert.Equal("briefing/images/anonymous", result);
		}

		[Theory]
		[InlineData("briefing/images", 456L, "briefing/images/userId/456")]
		[InlineData("uploads", 789L, "uploads/userId/789")]
		[InlineData("/briefing/images/", 123L, "briefing/images/userId/123")]
		[InlineData("briefing/images/", 999L, "briefing/images/userId/999")]
		public void GenerateUserScopedFolder_WithDifferentInputs_ShouldGenerateCorrectPath(
			string baseFolder, long userId, string expectedPath)
		{
			// Arrange
			_currentUserService.IsAuthenticated.Returns(true);
			_currentUserService.UserId.Returns(userId);

			// Act - usando reflexão para testar método privado
			var method = typeof(FileUploadService).GetMethod("GenerateUserScopedFolder", 
				System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			var result = (string)method!.Invoke(_fileUploadService, new object[] { baseFolder });

			// Assert
			Assert.Equal(expectedPath, result);
		}
	}
}
