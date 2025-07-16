using Kickoffa.API.Contracts.Checklist.Components.Request;
using Kickoffa.API.TestUtils.JsonProperty;
using Newtonsoft.Json;

namespace Kickoffa.API.Contracts.UnitTests.Checklist.Components.Request
{
	public class UploadComponentFileRequestTests
	{
		[Fact]
		public void UploadComponentFileRequest_WithValidData_ShouldCreateSuccessfully()
		{
			// Arrange
			var fileName = "document.pdf";
			var storagePath = "/uploads/2024/01/document.pdf";
			var fileSize = 1024L;
			var contentType = "application/pdf";
			var sha256Hash = "abc123def456";

			// Act
			var request = new UploadComponentFileRequest
			{
				FileName = fileName,
				StoragePath = storagePath,
				FileSize = fileSize,
				ContentType = contentType,
				Sha256Hash = sha256Hash
			};

			// Assert
			Assert.Equal(fileName, request.FileName);
			Assert.Equal(storagePath, request.StoragePath);
			Assert.Equal(fileSize, request.FileSize);
			Assert.Equal(contentType, request.ContentType);
			Assert.Equal(sha256Hash, request.Sha256Hash);
		}

		[Fact]
		public void UploadComponentFileRequest_AsRecord_ShouldSupportEquality()
		{
			// Arrange
			var request1 = new UploadComponentFileRequest
			{
				FileName = "test.jpg",
				StoragePath = "/uploads/test.jpg",
				FileSize = 2048L,
				ContentType = "image/jpeg",
				Sha256Hash = "hash123"
			};

			var request2 = new UploadComponentFileRequest
			{
				FileName = "test.jpg",
				StoragePath = "/uploads/test.jpg",
				FileSize = 2048L,
				ContentType = "image/jpeg",
				Sha256Hash = "hash123"
			};

			// Act & Assert
			Assert.Equal(request1, request2);
			Assert.True(request1 == request2);
			Assert.False(request1 != request2);
		}

		[Fact]
		public void UploadComponentFileRequest_DifferentValues_ShouldNotBeEqual()
		{
			// Arrange
			var request1 = new UploadComponentFileRequest
			{
				FileName = "test1.jpg",
				StoragePath = "/uploads/test1.jpg",
				FileSize = 1024L,
				ContentType = "image/jpeg",
				Sha256Hash = "hash1"
			};

			var request2 = new UploadComponentFileRequest
			{
				FileName = "test2.jpg", // Different filename
				StoragePath = "/uploads/test1.jpg",
				FileSize = 1024L,
				ContentType = "image/jpeg",
				Sha256Hash = "hash1"
			};

			// Act & Assert
			Assert.NotEqual(request1, request2);
			Assert.False(request1 == request2);
			Assert.True(request1 != request2);
		}

		[Theory]
		[InlineData("document.pdf", "application/pdf")]
		[InlineData("image.jpg", "image/jpeg")]
		[InlineData("video.mp4", "video/mp4")]
		[InlineData("text.txt", "text/plain")]
		public void UploadComponentFileRequest_WithDifferentFileTypes_ShouldSetCorrectly(string fileName, string contentType)
		{
			// Arrange & Act
			var request = new UploadComponentFileRequest
			{
				FileName = fileName,
				StoragePath = $"/uploads/{fileName}",
				FileSize = 1024L,
				ContentType = contentType,
				Sha256Hash = "hash123"
			};

			// Assert
			Assert.Equal(fileName, request.FileName);
			Assert.Equal(contentType, request.ContentType);
		}

		[Theory]
		[InlineData(0L)]
		[InlineData(1024L)]
		[InlineData(1048576L)] // 1MB
		[InlineData(long.MaxValue)]
		public void UploadComponentFileRequest_WithDifferentFileSizes_ShouldSetCorrectly(long fileSize)
		{
			// Arrange & Act
			var request = new UploadComponentFileRequest
			{
				FileName = "test.txt",
				StoragePath = "/uploads/test.txt",
				FileSize = fileSize,
				ContentType = "text/plain",
				Sha256Hash = "hash123"
			};

			// Assert
			Assert.Equal(fileSize, request.FileSize);
		}

		[Fact]
		public void UploadComponentFileRequest_ShouldBeSealed()
		{
			// Assert
			var type = typeof(UploadComponentFileRequest);
			Assert.True(type.IsSealed);
		}

		[Fact]
		public void UploadComponentFileRequest_ShouldHaveCorrectJsonPropertyAttributes()
		{
			// Arrange
			var type = typeof(UploadComponentFileRequest);

			// Act & Assert
			type.GetProperty(nameof(UploadComponentFileRequest.FileName))!.AssertPropertyName("fileName").AssertRequired(Required.Always);
			type.GetProperty(nameof(UploadComponentFileRequest.StoragePath))!.AssertPropertyName("storagePath").AssertRequired(Required.Always);
			type.GetProperty(nameof(UploadComponentFileRequest.FileSize))!.AssertPropertyName("fileSize").AssertRequired(Required.Always);
			type.GetProperty(nameof(UploadComponentFileRequest.ContentType))!.AssertPropertyName("contentType").AssertRequired(Required.Always);
			type.GetProperty(nameof(UploadComponentFileRequest.Sha256Hash))!.AssertPropertyName("sha256Hash").AssertRequired(Required.Default);
		}
	}
}