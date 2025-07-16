using Kickoffa.API.Contracts.Checklist.Components.Request;
using Kickoffa.API.Contracts.Checklist.Sections;
using Kickoffa.API.TestUtils.JsonProperty;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Kickoffa.API.Contracts.UnitTests.Checklist.Sections;

public class SectionRequestTests
{
	[Fact]
	public void BriefingSectionRequest_WithValidData_ShouldPassValidation()
	{
		// Arrange
		var request = new BriefingSectionRequest
		{
			Id = 1,
			Title = "Test Briefing Section",
			Type = SectionTypeRequest.Briefing,
			Order = 1,
			ContentHtml = "<p>Test content</p>"
		};

		// Act
		var validationResults = ValidateModel(request);

		// Assert
		Assert.Empty(validationResults);
	}

	[Fact]
	public void ChecklistSectionRequest_WithValidData_ShouldPassValidation()
	{
		// Arrange
		var request = new ChecklistSectionRequest
		{
			Id = 1,
			Title = "Test Checklist Section",
			Type = SectionTypeRequest.Checklist,
			Order = 1,
			Components = new List<ComponentRequest>
			{
				new CheckboxComponentRequest
				{
					Id = 1,
					Title = "Test Checkbox",
					IsRequired = true,
					Order = 1
				}
			}
		};

		// Act
		var validationResults = ValidateModel(request);

		// Assert
		Assert.Empty(validationResults);
	}

	[Fact]
	public void SectionRequest_WithEmptyTitle_ShouldFailValidation()
	{
		// Arrange
		var request = new BriefingSectionRequest
		{
			Id = 1,
			Title = "",
			Type = SectionTypeRequest.Briefing,
			Order = 1,
			ContentHtml = "<p>Test content</p>"
		};

		// Act
		var validationResults = ValidateModel(request);

		// Assert
		Assert.Single(validationResults);
		Assert.Contains(validationResults, v => v.ErrorMessage == "O título da seção é obrigatório" && v.MemberNames.Contains("Title"));
	}

	[Fact]
	public void SectionRequest_WithTooShortTitle_ShouldFailValidation()
	{
		// Arrange
		var request = new BriefingSectionRequest
		{
			Id = 1,
			Title = "A", // 1 character
			Type = SectionTypeRequest.Briefing,
			Order = 1,
			ContentHtml = "<p>Test content</p>"
		};

		// Act
		var validationResults = ValidateModel(request);

		// Assert
		Assert.Single(validationResults);
		Assert.Contains(validationResults, v => v.ErrorMessage == "O título da seção deve ter entre 2 e 200 caracteres" && v.MemberNames.Contains("Title"));
	}

	[Fact]
	public void SectionRequest_WithTooLongTitle_ShouldFailValidation()
	{
		// Arrange
		var longTitle = new string('A', 201); // 201 characters
		var request = new BriefingSectionRequest
		{
			Id = 1,
			Title = longTitle,
			Type = SectionTypeRequest.Briefing,
			Order = 1,
			ContentHtml = "<p>Test content</p>"
		};

		// Act
		var validationResults = ValidateModel(request);

		// Assert
		Assert.Single(validationResults);
		Assert.Contains(validationResults, v => v.ErrorMessage == "O título da seção deve ter entre 2 e 200 caracteres" && v.MemberNames.Contains("Title"));
	}

	[Fact]
	public void SectionRequest_WithZeroOrder_ShouldFailValidation()
	{
		// Arrange
		var request = new BriefingSectionRequest
		{
			Id = 1,
			Title = "Test Section",
			Type = SectionTypeRequest.Briefing,
			Order = 0,
			ContentHtml = "<p>Test content</p>"
		};

		// Act
		var validationResults = ValidateModel(request);

		// Assert
		Assert.Single(validationResults);
		Assert.Contains(validationResults, v => v.ErrorMessage == "A ordem da seção deve ser maior que zero" && v.MemberNames.Contains("Order"));
	}

	[Theory]
	[InlineData("", "")]
	[InlineData(null, "")]
	[InlineData("", null)]
	[InlineData(null, null)]
	public void BriefingSectionRequest_WithBoth_ContentHtmlAndContentJsonEmptyOrNull_ShouldFailValidation(string? contentHtml, string? contentJson)
	{
		// Arrange
		var request = new BriefingSectionRequest
		{
			Id = 1,
			Title = "Test Briefing Section",
			Type = SectionTypeRequest.Briefing,
			Order = 1,
			ContentHtml = contentHtml,
			ContentJson = contentJson
		};

		// Act
		var validationResults = ValidateModel(request);

		// Assert
		Assert.Single(validationResults);
		Assert.Contains(validationResults, v => v.ErrorMessage == "Seções de briefing devem ter conteúdo JSON ou HTML" && v.MemberNames.Contains("ContentHtml"));
	}

	[Fact]
	public void ChecklistSectionRequest_WithEmptyComponents_ShouldPassValidation()
	{
		// Arrange
		var request = new ChecklistSectionRequest
		{
			Id = 1,
			Title = "Test Checklist Section",
			Type = SectionTypeRequest.Checklist,
			Order = 1,
			Components = []
		};

		// Act
		var validationResults = ValidateModel(request);

		// Assert
		Assert.Single(validationResults);
		Assert.Contains(validationResults, v => v.ErrorMessage == "Seções de checklist devem ter pelo menos um componente" && v.MemberNames.Contains("Components"));
	}

	[Fact]
	public void ChecklistSectionRequest_WithNullComponents_ShouldPassValidation()
	{
		// Arrange
		var request = new ChecklistSectionRequest
		{
			Id = 1,
			Title = "Test Checklist Section",
			Type = SectionTypeRequest.Checklist,
			Order = 1,
			Components = null!
		};

		// Act
		var validationResults = ValidateModel(request);

		// Assert
		Assert.Single(validationResults);
		Assert.Contains(validationResults, v => v.ErrorMessage == "Seções de checklist devem ter pelo menos um componente" && v.MemberNames.Contains("Components"));
	}

	[Fact]
	public void ChecklistSectionRequest_WithMultipleComponents_ShouldPassValidation()
	{
		// Arrange
		var request = new ChecklistSectionRequest
		{
			Id = 1,
			Title = "Test Checklist Section",
			Type = SectionTypeRequest.Checklist,
			Order = 1,
			Components =
			[
				new CheckboxComponentRequest
				{
					Id = 1,
					Title = "Checkbox 1",
					IsRequired = true,
					Order = 1
				},
				new TextComponentRequest
				{
					Id = 2,
					Title = "Text Input",
					IsRequired = false,
					Order = 2
				}
			]
		};

		// Act
		var validationResults = ValidateModel(request);

		// Assert
		Assert.Empty(validationResults);
	}

	[Theory]
	[InlineData(SectionTypeRequest.Briefing)]
	[InlineData(SectionTypeRequest.Checklist)]
	public void SectionRequest_WithValidSectionTypes_ShouldPassValidation(SectionTypeRequest sectionType)
	{
		// Arrange
		SectionRequest request = sectionType switch
		{
			SectionTypeRequest.Briefing => new BriefingSectionRequest
			{
				Id = 1,
				Title = "Test Section",
				Type = sectionType,
				Order = 1,
				ContentHtml = "<p>Test</p>"
			},
			SectionTypeRequest.Checklist => new ChecklistSectionRequest
			{
				Id = 1,
				Title = "Test Section",
				Type = sectionType,
				Order = 1,
				Components =
				[
					new CheckboxComponentRequest
					{
						Id = 1,
						Title = "Checkbox 1",
						IsRequired = true,
						Order = 1
					}
				]
			},
			_ => throw new ArgumentException("Invalid section type")
		};

		// Act
		var validationResults = ValidateModel(request);

		// Assert
		Assert.Empty(validationResults);
	}

	[Fact]
	public void BriefingSectionRequest_AsRecord_ShouldSupportEquality()
	{
		// Arrange
		var request1 = new BriefingSectionRequest
		{
			Id = 1,
			Title = "Test Section",
			Type = SectionTypeRequest.Briefing,
			Order = 1,
			ContentHtml = "<p>Test content</p>"
		};

		var request2 = new BriefingSectionRequest
		{
			Id = 1,
			Title = "Test Section",
			Type = SectionTypeRequest.Briefing,
			Order = 1,
			ContentHtml = "<p>Test content</p>"
		};

		// Act & Assert
		Assert.Equal(request1, request2);
		Assert.True(request1 == request2);
		Assert.False(request1 != request2);
	}

	[Fact]
	public void ChecklistSectionRequest_AsRecord_ShouldSupportEquality()
	{
		// Arrange
		var components = new List<ComponentRequest>();

		var request1 = new ChecklistSectionRequest
		{
			Id = 1,
			Title = "Test Section",
			Type = SectionTypeRequest.Checklist,
			Order = 1,
			Components = components
		};

		var request2 = new ChecklistSectionRequest
		{
			Id = 1,
			Title = "Test Section",
			Type = SectionTypeRequest.Checklist,
			Order = 1,
			Components = components
		};

		// Act & Assert
		Assert.Equal(request1, request2);
		Assert.True(request1 == request2);
		Assert.False(request1 != request2);
	}

	[Fact]
	public void SectionRequest_ShouldInheritCorrectly()
	{
		// Arrange
		var briefingSection = new BriefingSectionRequest
		{
			Id = 1,
			Title = "Test Section",
			Type = SectionTypeRequest.Briefing,
			Order = 1,
			ContentHtml = "<p>Test</p>"
		};

		var checklistSection = new ChecklistSectionRequest
		{
			Id = 1,
			Title = "Test Section",
			Type = SectionTypeRequest.Checklist,
			Order = 1,
			Components = new List<ComponentRequest>()
		};

		// Assert
		Assert.IsType<SectionRequest>(briefingSection, exactMatch: false);
		Assert.IsType<SectionRequest>(checklistSection, exactMatch: false);
	}

	[Fact]
	public void SectionRequest_WithMultipleValidationErrors_ShouldReturnAllErrors()
	{
		// Arrange
		var request = new BriefingSectionRequest
		{
			Id = 1,
			Title = "A", // Too short
			Type = SectionTypeRequest.Briefing,
			Order = 0, // Invalid
			ContentHtml = "", // Empty
			ContentJson = "" // Empty
		};

		// Act
		var validationResults = ValidateModel(request);

		// Assert
		Assert.Equal(3, validationResults.Count);
		Assert.Contains(validationResults, v => v.ErrorMessage == "O título da seção deve ter entre 2 e 200 caracteres");
		Assert.Contains(validationResults, v => v.ErrorMessage == "A ordem da seção deve ser maior que zero");
		Assert.Contains(validationResults, v => v.ErrorMessage == "Seções de briefing devem ter conteúdo JSON ou HTML");
	}

	[Theory]
	[InlineData("AB")] // 2 characters (minimum)
	[InlineData("Test Section Title")]
	[InlineData("A very long section title that is still within the 200 character limit and should pass validation")]
	public void SectionRequest_WithValidTitleLengths_ShouldPassValidation(string title)
	{
		// Arrange
		var request = new BriefingSectionRequest
		{
			Id = 1,
			Title = title,
			Type = SectionTypeRequest.Briefing,
			Order = 1,
			ContentHtml = "<p>Test content</p>"
		};

		// Act
		var validationResults = ValidateModel(request);

		// Assert
		Assert.Empty(validationResults);
	}

	[Fact]
	public void SectionRequest_ShouldHaveCorrectJsonPropertyAttributes()
	{
		// Arrange
		var type = typeof(SectionRequest);

		// Act & Assert
		type.GetProperty(nameof(SectionRequest.Id))!.AssertPropertyName("id").AssertRequired(Required.Default);
		type.GetProperty(nameof(SectionRequest.Title))!.AssertPropertyName("title").AssertRequired(Required.Always);
		type.GetProperty(nameof(SectionRequest.Type))!.AssertPropertyName("type").AssertRequired(Required.Always);
		type.GetProperty(nameof(SectionRequest.Order))!.AssertPropertyName("order").AssertRequired(Required.Always);
	}

	[Fact]
	public void BriefingSectionRequest_ShouldHaveCorrectJsonPropertyAttributes()
	{
		// Arrange
		var type = typeof(BriefingSectionRequest);

		// Act & Assert
		type.GetProperty(nameof(BriefingSectionRequest.Id))!.AssertPropertyName("id").AssertRequired(Required.Default);
		type.GetProperty(nameof(BriefingSectionRequest.Title))!.AssertPropertyName("title").AssertRequired(Required.Always);
		type.GetProperty(nameof(BriefingSectionRequest.Type))!.AssertPropertyName("type").AssertRequired(Required.Always);
		type.GetProperty(nameof(BriefingSectionRequest.Order))!.AssertPropertyName("order").AssertRequired(Required.Always);
		type.GetProperty(nameof(BriefingSectionRequest.ContentJson))!.AssertPropertyName("contentJson").AssertRequired(Required.Default);
		type.GetProperty(nameof(BriefingSectionRequest.ContentHtml))!.AssertPropertyName("contentHtml").AssertRequired(Required.Default);
	}

	[Fact]
	public void SectionResponse_ShouldHaveCorrectJsonPropertyAttributes()
	{
		// Arrange
		var type = typeof(SectionResponse);

		// Act & Assert
		type.GetProperty(nameof(SectionResponse.Id))!.AssertPropertyName("id").AssertRequired(Required.Always);
		type.GetProperty(nameof(SectionResponse.ChecklistId))!.AssertPropertyName("checklistId").AssertRequired(Required.Always);
		type.GetProperty(nameof(SectionResponse.Title))!.AssertPropertyName("title").AssertRequired(Required.Always);
		type.GetProperty(nameof(SectionResponse.Type))!.AssertPropertyName("type").AssertRequired(Required.Always);
		type.GetProperty(nameof(SectionResponse.Order))!.AssertPropertyName("order").AssertRequired(Required.Always);
		type.GetProperty(nameof(SectionResponse.CreatedDateUtc))!.AssertPropertyName("createdDateUtc").AssertRequired(Required.Always);
		type.GetProperty(nameof(SectionResponse.LastUpdatedDateUtc))!.AssertPropertyName("lastUpdatedDateUtc").AssertRequired(Required.Always);
	}

	[Fact]
	public void BriefingSectionResponse_ShouldHaveCorrectJsonPropertyAttributes()
	{
		// Arrange
		var type = typeof(BriefingSectionResponse);

		// Act & Assert
		type.GetProperty(nameof(BriefingSectionResponse.Id))!.AssertPropertyName("id").AssertRequired(Required.Always);
		type.GetProperty(nameof(BriefingSectionResponse.ChecklistId))!.AssertPropertyName("checklistId").AssertRequired(Required.Always);
		type.GetProperty(nameof(BriefingSectionResponse.Title))!.AssertPropertyName("title").AssertRequired(Required.Always);
		type.GetProperty(nameof(BriefingSectionResponse.Type))!.AssertPropertyName("type").AssertRequired(Required.Always);
		type.GetProperty(nameof(BriefingSectionResponse.Order))!.AssertPropertyName("order").AssertRequired(Required.Always);
		type.GetProperty(nameof(BriefingSectionResponse.CreatedDateUtc))!.AssertPropertyName("createdDateUtc").AssertRequired(Required.Always);
		type.GetProperty(nameof(BriefingSectionResponse.LastUpdatedDateUtc))!.AssertPropertyName("lastUpdatedDateUtc").AssertRequired(Required.Always);
		type.GetProperty(nameof(BriefingSectionResponse.ContentJson))!.AssertPropertyName("contentJson").AssertRequired(Required.Default);
		type.GetProperty(nameof(BriefingSectionResponse.ContentHtml))!.AssertPropertyName("contentHtml").AssertRequired(Required.Default);
		type.GetProperty(nameof(BriefingSectionResponse.ContentLastUpdated))!.AssertPropertyName("contentLastUpdated").AssertRequired(Required.Default);
	}

	#region Helper Methods

	private static List<ValidationResult> ValidateModel(object model)
	{
		var validationResults = new List<ValidationResult>();
		var validationContext = new ValidationContext(model);
		Validator.TryValidateObject(model, validationContext, validationResults, true);
		return validationResults;
	}

	#endregion
}