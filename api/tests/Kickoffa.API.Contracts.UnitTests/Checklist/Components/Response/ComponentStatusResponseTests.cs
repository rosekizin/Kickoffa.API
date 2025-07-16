using Kickoffa.API.Contracts.Checklist.Components.Response;
using Kickoffa.API.TestUtils.JsonProperty;
using Newtonsoft.Json;

namespace Kickoffa.API.Contracts.UnitTests.Checklist.Components.Response
{
	public class ComponentStatusResponseTests
	{
		[Fact]
		public void ComponentStatusResponse_WithRequiredProperties_ShouldCreateSuccessfully()
		{
			// Arrange
			var id = 1L;
			var componentId = 2L;
			var isCompleted = true;
			var createdDate = DateTime.UtcNow;
			var lastUpdatedDate = DateTime.UtcNow;

			// Act
			var response = new ComponentStatusResponse
			{
				Id = id,
				ComponentId = componentId,
				IsCompleted = isCompleted,
				CreatedDateUtc = createdDate,
				LastUpdatedDateUtc = lastUpdatedDate
			};

			// Assert
			Assert.Equal(id, response.Id);
			Assert.Equal(componentId, response.ComponentId);
			Assert.Equal(isCompleted, response.IsCompleted);
			Assert.Equal(createdDate, response.CreatedDateUtc);
			Assert.Equal(lastUpdatedDate, response.LastUpdatedDateUtc);
			Assert.Null(response.CompletedAt);
			Assert.Null(response.TextResponse);
			Assert.Null(response.SignatureData);
			Assert.Null(response.UploadedFiles);
		}

		[Fact]
		public void ComponentStatusResponse_WithAllProperties_ShouldSetCorrectly()
		{
			// Arrange
			var id = 1L;
			var componentId = 2L;
			var isCompleted = true;
			var completedAt = DateTime.UtcNow;
			var textResponse = "User response text";
			var signatureData = "signature_data_base64";
			var uploadedFiles = new List<UploadedFileResponse>
			{
				new UploadedFileResponse
				{
					Id = 1L,
					ComponentStatusId = 1L,
					FileName = "test.pdf",
					OriginalName = "original_test.pdf",
					MimeType = "application/pdf",
					Size = 1024L,
					Url = "/uploads/test.pdf",
					CreatedDateUtc = DateTime.UtcNow
				}
			};
			var createdDate = DateTime.UtcNow;
			var lastUpdatedDate = DateTime.UtcNow;

			// Act
			var response = new ComponentStatusResponse
			{
				Id = id,
				ComponentId = componentId,
				IsCompleted = isCompleted,
				CompletedAt = completedAt,
				TextResponse = textResponse,
				SignatureData = signatureData,
				UploadedFiles = uploadedFiles,
				CreatedDateUtc = createdDate,
				LastUpdatedDateUtc = lastUpdatedDate
			};

			// Assert
			Assert.Equal(id, response.Id);
			Assert.Equal(componentId, response.ComponentId);
			Assert.Equal(isCompleted, response.IsCompleted);
			Assert.Equal(completedAt, response.CompletedAt);
			Assert.Equal(textResponse, response.TextResponse);
			Assert.Equal(signatureData, response.SignatureData);
			Assert.Equal(uploadedFiles, response.UploadedFiles);
			Assert.Equal(createdDate, response.CreatedDateUtc);
			Assert.Equal(lastUpdatedDate, response.LastUpdatedDateUtc);
		}

		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public void ComponentStatusResponse_WithDifferentCompletionStates_ShouldSetCorrectly(bool isCompleted)
		{
			// Arrange & Act
			var response = new ComponentStatusResponse
			{
				Id = 1L,
				ComponentId = 2L,
				IsCompleted = isCompleted,
				CreatedDateUtc = DateTime.UtcNow,
				LastUpdatedDateUtc = DateTime.UtcNow
			};

			// Assert
			Assert.Equal(isCompleted, response.IsCompleted);
		}

		[Fact]
		public void ComponentStatusResponse_ShouldBeSealed()
		{
			// Assert
			var type = typeof(ComponentStatusResponse);
			Assert.True(type.IsSealed);
		}

		[Fact]
		public void ComponentStatusResponse_ShouldHaveCorrectJsonPropertyAttributes()
		{
			// Arrange
			var type = typeof(ComponentStatusResponse);

			// Act & Assert
			type.GetProperty(nameof(ComponentStatusResponse.Id))!.AssertPropertyName("id").AssertRequired(Required.Always);
			type.GetProperty(nameof(ComponentStatusResponse.ComponentId))!.AssertPropertyName("componentId").AssertRequired(Required.Always);
			type.GetProperty(nameof(ComponentStatusResponse.IsCompleted))!.AssertPropertyName("isCompleted").AssertRequired(Required.Always);
			type.GetProperty(nameof(ComponentStatusResponse.CompletedAt))!.AssertPropertyName("completedAt").AssertRequired(Required.Default);
			type.GetProperty(nameof(ComponentStatusResponse.TextResponse))!.AssertPropertyName("textResponse").AssertRequired(Required.Default);
			type.GetProperty(nameof(ComponentStatusResponse.SignatureData))!.AssertPropertyName("signatureData").AssertRequired(Required.Default);
			type.GetProperty(nameof(ComponentStatusResponse.UploadedFiles))!.AssertPropertyName("uploadedFiles").AssertRequired(Required.Default);
			type.GetProperty(nameof(ComponentStatusResponse.CreatedDateUtc))!.AssertPropertyName("createdDateUtc").AssertRequired(Required.Default);
			type.GetProperty(nameof(ComponentStatusResponse.LastUpdatedDateUtc))!.AssertPropertyName("lastUpdatedDateUtc").AssertRequired(Required.Default);
		}
	}
}