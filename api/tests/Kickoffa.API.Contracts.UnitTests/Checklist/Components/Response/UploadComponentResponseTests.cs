using Kickoffa.API.Contracts.Checklist.Components.Response;
using Kickoffa.API.Contracts.FileType;
using Kickoffa.API.TestUtils.JsonProperty;
using Newtonsoft.Json;

namespace Kickoffa.API.Contracts.UnitTests.Checklist.Components.Response
{
	public class UploadComponentResponseTests
	{
		[Fact]
		public void UploadComponentResponse_WithValidData_ShouldCreateSuccessfully()
		{
			// Arrange
			var allowedFileTypes = new List<FileTypeResponse>
			{
				new FileTypeResponse
				{
					Id = 1L,
					MimeType = "image/jpeg",
					Extension = ".jpg",
					DisplayName = "JPEG Image"
				}
			};

			// Act
			var response = new UploadComponentResponse
			{
				Id = 1L,
				SectionId = 2L,
				Title = "Test Upload Component",
				Type = "upload",
				IsRequired = true,
				Order = 1,
				Placeholder = "Drop files here",
				AllowedFileTypes = allowedFileTypes,
				CreatedDateUtc = DateTime.UtcNow,
				LastUpdatedDateUtc = DateTime.UtcNow
			};

			// Assert
			Assert.Equal("upload", response.Type);
			Assert.Equal("Drop files here", response.Placeholder);
			Assert.Equal(allowedFileTypes, response.AllowedFileTypes);
		}

		[Fact]
		public void UploadComponentResponse_TypeShouldBeSetAutomatically()
		{
			// Arrange & Act
			var response = new UploadComponentResponse
			{
				Id = 1L,
				SectionId = 2L,
				Title = "Test Upload Component",
				IsRequired = true,
				Order = 1,
				CreatedDateUtc = DateTime.UtcNow,
				LastUpdatedDateUtc = DateTime.UtcNow
			};

			// Assert
			Assert.Equal("upload", response.Type);
		}

		[Fact]
		public void UploadComponentResponse_ShouldInheritFromComponentResponse()
		{
			// Arrange
			var response = new UploadComponentResponse
			{
				Id = 1L,
				SectionId = 2L,
				Title = "Test Upload Component",
				IsRequired = true,
				Order = 1,
				Type = "upload",
				CreatedDateUtc = DateTime.UtcNow,
				LastUpdatedDateUtc = DateTime.UtcNow
			};

			// Assert
			Assert.IsType<ComponentResponse>(response, exactMatch: false);
		}

		[Fact]
		public void UploadComponentResponse_ShouldHaveCorrectJsonPropertyAttributes()
		{
			// Arrange
			var type = typeof(UploadComponentResponse);

			// Act & Assert
			type.GetProperty(nameof(UploadComponentResponse.Id))!.AssertPropertyName("id").AssertRequired(Required.Always);
			type.GetProperty(nameof(UploadComponentResponse.SectionId))!.AssertPropertyName("sectionId").AssertRequired(Required.Always);
			type.GetProperty(nameof(UploadComponentResponse.Title))!.AssertPropertyName("title").AssertRequired(Required.Always);
			type.GetProperty(nameof(UploadComponentResponse.Description))!.AssertPropertyName("description").AssertRequired(Required.Default);
			type.GetProperty(nameof(UploadComponentResponse.Type))!.AssertPropertyName("type").AssertRequired(Required.Always);
			type.GetProperty(nameof(UploadComponentResponse.IsRequired))!.AssertPropertyName("isRequired").AssertRequired(Required.Always);
			type.GetProperty(nameof(UploadComponentResponse.Order))!.AssertPropertyName("order").AssertRequired(Required.Always);
			type.GetProperty(nameof(UploadComponentResponse.Status))!.AssertPropertyName("status").AssertRequired(Required.Default);
			type.GetProperty(nameof(UploadComponentResponse.CreatedDateUtc))!.AssertPropertyName("createdDateUtc").AssertRequired(Required.Always);
			type.GetProperty(nameof(UploadComponentResponse.LastUpdatedDateUtc))!.AssertPropertyName("lastUpdatedDateUtc").AssertRequired(Required.Always);
			type.GetProperty(nameof(UploadComponentResponse.Placeholder))!.AssertPropertyName("placeholder").AssertRequired(Required.Default);
			type.GetProperty(nameof(UploadComponentResponse.AllowedFileTypes))!.AssertPropertyName("allowedFileTypes").AssertRequired(Required.Always);
			type.GetProperty(nameof(UploadComponentResponse.FileTypeSizeConfigs))!.AssertPropertyName("fileTypeSizeConfigs").AssertRequired(Required.Always);
			type.GetProperty(nameof(UploadComponentResponse.ComponentFiles))!.AssertPropertyName("componentFiles").AssertRequired(Required.Default);
		}

		[Fact]
		public void UploadComponentFileResponse_ShouldHaveCorrectJsonPropertyAttributes()
		{
			// Arrange
			var type = typeof(UploadComponentFileResponse);

			// Act & Assert
			type.GetProperty(nameof(UploadComponentFileResponse.Id))!.AssertPropertyName("id").AssertRequired(Required.Always);
			type.GetProperty(nameof(UploadComponentFileResponse.ComponentId))!.AssertPropertyName("componentId").AssertRequired(Required.Always);
			type.GetProperty(nameof(UploadComponentFileResponse.FileName))!.AssertPropertyName("fileName").AssertRequired(Required.Always);
			type.GetProperty(nameof(UploadComponentFileResponse.OriginalName))!.AssertPropertyName("originalName").AssertRequired(Required.Always);
			type.GetProperty(nameof(UploadComponentFileResponse.MimeType))!.AssertPropertyName("mimeType").AssertRequired(Required.Always);
			type.GetProperty(nameof(UploadComponentFileResponse.Size))!.AssertPropertyName("size").AssertRequired(Required.Always);
			type.GetProperty(nameof(UploadComponentFileResponse.Url))!.AssertPropertyName("url").AssertRequired(Required.Always);
			type.GetProperty(nameof(UploadComponentFileResponse.CreatedDateUtc))!.AssertPropertyName("createdDateUtc").AssertRequired(Required.Always);
		}
	}
}