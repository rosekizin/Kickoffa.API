using Kickoffa.API.Contracts.Checklist.Components.Response;
using Kickoffa.API.TestUtils.JsonProperty;
using Newtonsoft.Json;

namespace Kickoffa.API.Contracts.UnitTests.Checklist.Components.Response
{

	public class CheckboxComponentResponseTests
	{
		[Fact]
		public void CheckboxComponentResponse_WithValidData_ShouldCreateSuccessfully()
		{
			// Arrange & Act
			var response = new CheckboxComponentResponse
			{
				Id = 1L,
				SectionId = 2L,
				Title = "Test Checkbox",
				Type = "checkbox",
				IsRequired = true,
				Order = 1,
				CreatedDateUtc = DateTime.UtcNow,
				LastUpdatedDateUtc = DateTime.UtcNow
			};

			// Assert
			Assert.Equal("checkbox", response.Type);
			Assert.Equal(1L, response.Id);
			Assert.Equal("Test Checkbox", response.Title);
		}

		[Fact]
		public void CheckboxComponentResponse_ShouldInheritFromComponentResponse()
		{
			// Arrange
			var response = new CheckboxComponentResponse
			{
				Id = 1L,
				SectionId = 2L,
				Title = "Test Checkbox",
				IsRequired = true,
				Type = "checkbox",
				Order = 1,
				CreatedDateUtc = DateTime.UtcNow,
				LastUpdatedDateUtc = DateTime.UtcNow
			};

			// Assert
			Assert.IsType<ComponentResponse>(response, exactMatch: false);
		}

		[Fact]
		public void CheckboxComponentResponse_ShouldBeSealed()
		{
			// Assert
			var type = typeof(CheckboxComponentResponse);
			Assert.True(type.IsSealed);
		}

		[Fact]
		public void CheckboxComponentResponse_ShouldHaveCorrectJsonPropertyAttributes()
		{
			// Arrange
			var type = typeof(CheckboxComponentResponse);

			// Act & Assert
			type.GetProperty(nameof(CheckboxComponentResponse.Id))!.AssertPropertyName("id").AssertRequired(Required.Always);
			type.GetProperty(nameof(CheckboxComponentResponse.SectionId))!.AssertPropertyName("sectionId").AssertRequired(Required.Always);
			type.GetProperty(nameof(CheckboxComponentResponse.Title))!.AssertPropertyName("title").AssertRequired(Required.Always);
			type.GetProperty(nameof(CheckboxComponentResponse.Description))!.AssertPropertyName("description").AssertRequired(Required.Default);
			type.GetProperty(nameof(CheckboxComponentResponse.Type))!.AssertPropertyName("type").AssertRequired(Required.Always);
			type.GetProperty(nameof(CheckboxComponentResponse.IsRequired))!.AssertPropertyName("isRequired").AssertRequired(Required.Always);
			type.GetProperty(nameof(CheckboxComponentResponse.Order))!.AssertPropertyName("order").AssertRequired(Required.Always);
			type.GetProperty(nameof(CheckboxComponentResponse.Status))!.AssertPropertyName("status").AssertRequired(Required.Default);
			type.GetProperty(nameof(CheckboxComponentResponse.CreatedDateUtc))!.AssertPropertyName("createdDateUtc").AssertRequired(Required.Always);
			type.GetProperty(nameof(CheckboxComponentResponse.LastUpdatedDateUtc))!.AssertPropertyName("lastUpdatedDateUtc").AssertRequired(Required.Always);
		}
	}
}