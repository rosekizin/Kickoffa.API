using Kickoffa.API.Contracts.Checklist.Components.Response;
using Kickoffa.API.TestUtils.JsonProperty;
using Newtonsoft.Json;

namespace Kickoffa.API.Contracts.UnitTests.Checklist.Components.Response
{
	public class ConfirmationComponentResponseTests
	{
		[Fact]
		public void ConfirmationComponentResponse_WithValidData_ShouldCreateSuccessfully()
		{
			// Arrange & Act
			var response = new ConfirmationComponentResponse
			{
				Id = 1L,
				SectionId = 2L,
				Title = "Test Confirmation Component",
				Type = "confirmation",
				IsRequired = true,
				Order = 1,
				ConfirmationText = "I agree to the terms",
				CreatedDateUtc = DateTime.UtcNow,
				LastUpdatedDateUtc = DateTime.UtcNow
			};

			// Assert
			Assert.Equal("confirmation", response.Type);
			Assert.Equal("I agree to the terms", response.ConfirmationText);
		}

		[Fact]
		public void ConfirmationComponentResponse_WithNullConfirmationText_ShouldSetCorrectly()
		{
			// Arrange & Act
			var response = new ConfirmationComponentResponse
			{
				Id = 1L,
				SectionId = 2L,
				Title = "Test Confirmation Component",
				IsRequired = true,
				Order = 1,
				Type = "confirmation",
				ConfirmationText = null,
				CreatedDateUtc = DateTime.UtcNow,
				LastUpdatedDateUtc = DateTime.UtcNow
			};

			// Assert
			Assert.Null(response.ConfirmationText);
		}

		[Fact]
		public void ConfirmationComponentResponse_ShouldInheritFromComponentResponse()
		{
			// Arrange
			var response = new ConfirmationComponentResponse
			{
				Id = 1L,
				SectionId = 2L,
				Title = "Test Confirmation Component",
				IsRequired = true,
				Order = 1,
				Type = "confirmation",
				CreatedDateUtc = DateTime.UtcNow,
				LastUpdatedDateUtc = DateTime.UtcNow
			};

			// Assert
			Assert.IsType<ComponentResponse>(response, exactMatch: false);
		}

		[Fact]
		public void ConfirmationComponentResponse_ShouldHaveCorrectJsonPropertyAttributes()
		{
			// Arrange
			var type = typeof(ConfirmationComponentResponse);

			// Act & Assert
			type.GetProperty(nameof(ConfirmationComponentResponse.Id))!.AssertPropertyName("id").AssertRequired(Required.Always);
			type.GetProperty(nameof(ConfirmationComponentResponse.SectionId))!.AssertPropertyName("sectionId").AssertRequired(Required.Always);
			type.GetProperty(nameof(ConfirmationComponentResponse.Title))!.AssertPropertyName("title").AssertRequired(Required.Always);
			type.GetProperty(nameof(ConfirmationComponentResponse.Description))!.AssertPropertyName("description").AssertRequired(Required.Default);
			type.GetProperty(nameof(ConfirmationComponentResponse.Type))!.AssertPropertyName("type").AssertRequired(Required.Always);
			type.GetProperty(nameof(ConfirmationComponentResponse.IsRequired))!.AssertPropertyName("isRequired").AssertRequired(Required.Always);
			type.GetProperty(nameof(ConfirmationComponentResponse.Order))!.AssertPropertyName("order").AssertRequired(Required.Always);
			type.GetProperty(nameof(ConfirmationComponentResponse.Status))!.AssertPropertyName("status").AssertRequired(Required.Default);
			type.GetProperty(nameof(ConfirmationComponentResponse.CreatedDateUtc))!.AssertPropertyName("createdDateUtc").AssertRequired(Required.Always);
			type.GetProperty(nameof(ConfirmationComponentResponse.LastUpdatedDateUtc))!.AssertPropertyName("lastUpdatedDateUtc").AssertRequired(Required.Always);
			type.GetProperty(nameof(ConfirmationComponentResponse.ConfirmationText))!.AssertPropertyName("confirmationText").AssertRequired(Required.Default);
		}
	}
}