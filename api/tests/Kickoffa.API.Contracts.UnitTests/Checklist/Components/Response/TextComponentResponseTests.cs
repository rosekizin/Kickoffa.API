using Kickoffa.API.Contracts.Checklist.Components.Response;
using Kickoffa.API.TestUtils.JsonProperty;
using Newtonsoft.Json;

namespace Kickoffa.API.Contracts.UnitTests.Checklist.Components.Response
{
	public class TextComponentResponseTests
	{
		[Fact]
		public void TextComponentResponse_WithValidData_ShouldCreateSuccessfully()
		{
			// Arrange & Act
			var response = new TextComponentResponse
			{
				Id = 1L,
				SectionId = 2L,
				Title = "Test Text Component",
				Type = "text",
				IsRequired = true,
				Order = 1,
				Placeholder = "Enter text here",
				MaxLength = 100,
				CreatedDateUtc = DateTime.UtcNow,
				LastUpdatedDateUtc = DateTime.UtcNow
			};

			// Assert
			Assert.Equal("text", response.Type);
			Assert.Equal("Enter text here", response.Placeholder);
			Assert.Equal(100, response.MaxLength);
		}

		[Fact]
		public void TextComponentResponse_WithNullOptionalProperties_ShouldSetCorrectly()
		{
			// Arrange & Act
			var response = new TextComponentResponse
			{
				Id = 1L,
				SectionId = 2L,
				Title = "Test Text Component",
				IsRequired = true,
				Order = 1,
				Type = "text",
				Placeholder = null,
				MaxLength = null,
				CreatedDateUtc = DateTime.UtcNow,
				LastUpdatedDateUtc = DateTime.UtcNow
			};

			// Assert
			Assert.Null(response.Placeholder);
			Assert.Null(response.MaxLength);
		}

		[Fact]
		public void TextComponentResponse_ShouldInheritFromComponentResponse()
		{
			// Arrange
			var response = new TextComponentResponse
			{
				Id = 1L,
				SectionId = 2L,
				Title = "Test Text Component",
				IsRequired = true,
				Order = 1,
				Type = "text",
				CreatedDateUtc = DateTime.UtcNow,
				LastUpdatedDateUtc = DateTime.UtcNow
			};

			// Assert
			Assert.IsType<ComponentResponse>(response, exactMatch: false);
		}

		[Fact]
		public void TextComponentResponse_ShouldHaveCorrectJsonPropertyAttributes()
		{
			// Arrange
			var type = typeof(TextComponentResponse);

			// Act & Assert
			type.GetProperty(nameof(TextComponentResponse.Id))!.AssertPropertyName("id").AssertRequired(Required.Always);
			type.GetProperty(nameof(TextComponentResponse.SectionId))!.AssertPropertyName("sectionId").AssertRequired(Required.Always);
			type.GetProperty(nameof(TextComponentResponse.Title))!.AssertPropertyName("title").AssertRequired(Required.Always);
			type.GetProperty(nameof(TextComponentResponse.Description))!.AssertPropertyName("description").AssertRequired(Required.Default);
			type.GetProperty(nameof(TextComponentResponse.Type))!.AssertPropertyName("type").AssertRequired(Required.Always);
			type.GetProperty(nameof(TextComponentResponse.IsRequired))!.AssertPropertyName("isRequired").AssertRequired(Required.Always);
			type.GetProperty(nameof(TextComponentResponse.Order))!.AssertPropertyName("order").AssertRequired(Required.Always);
			type.GetProperty(nameof(TextComponentResponse.Status))!.AssertPropertyName("status").AssertRequired(Required.Default);
			type.GetProperty(nameof(TextComponentResponse.CreatedDateUtc))!.AssertPropertyName("createdDateUtc").AssertRequired(Required.Always);
			type.GetProperty(nameof(TextComponentResponse.LastUpdatedDateUtc))!.AssertPropertyName("lastUpdatedDateUtc").AssertRequired(Required.Always);
			type.GetProperty(nameof(TextComponentResponse.Placeholder))!.AssertPropertyName("placeholder").AssertRequired(Required.Default);
			type.GetProperty(nameof(TextComponentResponse.MaxLength))!.AssertPropertyName("maxLength").AssertRequired(Required.Default);
		}
	}
}