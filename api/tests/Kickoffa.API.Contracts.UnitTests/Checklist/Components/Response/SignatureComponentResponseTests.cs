using Kickoffa.API.Contracts.Checklist.Components.Response;
using Kickoffa.API.TestUtils.JsonProperty;
using Newtonsoft.Json;

namespace Kickoffa.API.Contracts.UnitTests.Checklist.Components.Response
{
	public class SignatureComponentResponseTests
	{
		[Fact]
		public void SignatureComponentResponse_WithValidData_ShouldCreateSuccessfully()
		{
			// Arrange & Act
			var response = new SignatureComponentResponse
			{
				Id = 1L,
				SectionId = 2L,
				Title = "Test Signature Component",
				IsRequired = true,
				Order = 1,
				CreatedDateUtc = DateTime.UtcNow,
				LastUpdatedDateUtc = DateTime.UtcNow
			};

			// Assert
			Assert.Equal("signature", response.Type);
			Assert.Equal(1L, response.Id);
			Assert.Equal("Test Signature Component", response.Title);
		}


		[Fact]
		public void UploadComponentResponse_TypeShouldBeSetAutomatically()
		{
			// Arrange & Act
			var response = new SignatureComponentResponse
			{
				Id = 1L,
				SectionId = 2L,
				Title = "Test Signature Component",
				IsRequired = true,
				Order = 1,
				CreatedDateUtc = DateTime.UtcNow,
				LastUpdatedDateUtc = DateTime.UtcNow
			};

			// Assert
			Assert.Equal("signature", response.Type);
		}

		[Fact]
		public void SignatureComponentResponse_ShouldInheritFromComponentResponse()
		{
			// Arrange
			var response = new SignatureComponentResponse
			{
				Id = 1L,
				SectionId = 2L,
				Title = "Test Signature Component",
				IsRequired = true,
				Order = 1,
				Type = "signature",
				CreatedDateUtc = DateTime.UtcNow,
				LastUpdatedDateUtc = DateTime.UtcNow
			};

			// Assert
			Assert.IsType<ComponentResponse>(response, exactMatch: false);
		}

		[Fact]
		public void SignatureComponentResponse_ShouldHaveCorrectJsonPropertyAttributes()
		{
			// Arrange
			var type = typeof(SignatureComponentResponse);

			// Act & Assert
			type.GetProperty(nameof(SignatureComponentResponse.Id))!.AssertPropertyName("id").AssertRequired(Required.Always);
			type.GetProperty(nameof(SignatureComponentResponse.SectionId))!.AssertPropertyName("sectionId").AssertRequired(Required.Always);
			type.GetProperty(nameof(SignatureComponentResponse.Title))!.AssertPropertyName("title").AssertRequired(Required.Always);
			type.GetProperty(nameof(SignatureComponentResponse.Description))!.AssertPropertyName("description").AssertRequired(Required.Default);
			type.GetProperty(nameof(SignatureComponentResponse.Type))!.AssertPropertyName("type").AssertRequired(Required.Always);
			type.GetProperty(nameof(SignatureComponentResponse.IsRequired))!.AssertPropertyName("isRequired").AssertRequired(Required.Always);
			type.GetProperty(nameof(SignatureComponentResponse.Order))!.AssertPropertyName("order").AssertRequired(Required.Always);
			type.GetProperty(nameof(SignatureComponentResponse.Status))!.AssertPropertyName("status").AssertRequired(Required.Default);
			type.GetProperty(nameof(SignatureComponentResponse.CreatedDateUtc))!.AssertPropertyName("createdDateUtc").AssertRequired(Required.Always);
			type.GetProperty(nameof(SignatureComponentResponse.LastUpdatedDateUtc))!.AssertPropertyName("lastUpdatedDateUtc").AssertRequired(Required.Always);
		}
	}
}